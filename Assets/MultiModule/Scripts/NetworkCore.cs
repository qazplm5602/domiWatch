using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using LitJson;

// 데이터 형식
class domiPacket {
    public string type;
    public object data;
    
    public domiPacket(string _type, object _data) {
        type = _type;
        data = _data;
    }

    // Json으로 변환
    public string ToJson() => JsonMapper.ToJson(this);
}

public class NetworkCore : MonoBehaviour
{
    public static NetworkCore instance { get; private set; }
    [SerializeField, Header("서버 주소설정"), Tooltip("서버 아이피 주소")]
    string ServerIP = "127.0.0.1";
    [SerializeField, Tooltip("서버 연결 포트")]
    int ServerPort = 3000;

    [Header("데이터 처리 Config")]
    [SerializeField, Tooltip("(클수록 메모리 부담s..)")]
    int BufferSize = 1024;

    // 서버 관련
    TcpClient client;
    NetworkStream stream;
    byte[] buffer;
    StringBuilder PacketPlus = new(); // 패킷 재조립
    string WhyDisconnect;

    private readonly Queue<UnityAction> _actions = new();

    // 트리거
    public static Dictionary<string, UnityAction<JsonData>> EventListener = new();
    public static UnityAction EventConnect;
    public static UnityAction<string> EventDisconnect;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            throw new System.Exception("[domi Network] 네트워크 코어는 하나의 오브젝트 안에서만 구성해야 합니다.");

        // 연결 끊김 사유
        NetworkCore.EventListener["disconnect.why"] = (JsonData why) => {
            WhyDisconnect = (string)why;
            client.Close();
        };
    }
    
    // 엔티티가 없어지면 연결을 끊음 (에디터 playmode 가 아니여도 connection이 연결되어있음 ㅇㅅㅇ)
    private void OnDestroy() {
        client?.Close();
    }

    private void Update() {
        // 메인 스레드에서 실행해야하는 코드 (Destory 함수나 Unity에서 쓰는 함수등...)
        while(_actions.Count > 0)
        {
            if(_actions.TryDequeue(out var action))
            {
                action?.Invoke();
            }
        }
    }

    ////////////////// 메서드 //////////////////

    // 서버를 연결하자!
    public void ServerConnect() {
        if (client != null) {
            Debug.LogError("[domi Network] 이미 서버 세션이 있습니다.");
            return;
        }

        print("[domi Network] 아이피 주소 검사");
        System.Net.IPAddress ServerAdress;
        if (!System.Net.IPAddress.TryParse(ServerIP, out ServerAdress)) {
            Debug.LogError("[domi Network] 아이피 주소가 잘못되었습니다.");
            return;
        }

        print("[domi Network] 서버 연결 시도");
        client = new TcpClient();
        buffer = new byte[BufferSize];
        WhyDisconnect = null;

        client.BeginConnect(ServerIP, ServerPort, OnConnect, null);
        StartCoroutine(WaitServerConnect());
    }

    // 서버에게 전송 (Client -> Server)
    public static void Send(string Type, object Data) {
        byte[] data = Encoding.UTF8.GetBytes(new domiPacket(Type, Data).ToJson()+"|domi\\SLICE\\packet|");
        instance.stream.Write(data);
    }

    ////////////////// 이벤트 //////////////////
    // [이벤트 리스너] 연결 완료
    void OnConnect(System.IAsyncResult result) {
        client.EndConnect(result);
        print("[domi Network] 서버와 연결 성공");

        // 네트워크 스트림 생성
        stream = client.GetStream();
        stream.BeginRead(buffer, 0, buffer.Length, OnRead, null);

        // 메인 쓰레드에서 실행 (이 쓰레드에서 유니티 레퍼런스들은 작동안함)
        _actions.Enqueue(() => EventConnect?.Invoke()); // 이벤트 실행
    }

    // [이벤트 리스너] 데이터 읽음 (Server -> Client)
    void OnRead(System.IAsyncResult result) {
        int bytesRead = stream.EndRead(result);
        if (bytesRead <= 0) { // 받은 데이터가 없다!! (연결 종료 의미)
            client.Close();
            return;
        }

        string message = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
        if (PacketPlus.ToString().Length > 0) {
            message = PacketPlus.ToString()+message; // 누락된 부분 앞에다 붙임
            PacketPlus.Clear();
        }

        // 서버에서 마지막에 "|domi\SLICE\packet|" 붙여서 보냄 (어디에서 누락되는지 알기위해)
        string[] SlicePacket = message.Split("|domi\\SLICE\\packet|");

        Dictionary<string, JsonData> SaveUpdatePlayer = new();
        for (int i = 0; i < SlicePacket.Length; i++)
        {
            string MessageData = SlicePacket[i];

            if (MessageData.Length > 0)
                if (i == (SlicePacket.Length - 1)) { // 누락된 데이터 (재조립 예정)
                    PacketPlus.Append(MessageData);
                } else { // 데이터 정상!
                    JsonData DataDecode = JsonMapper.ToObject(MessageData);

                    // 메인 쓰레드에서 실행 (이 쓰레드에서 유니티 레퍼런스들은 작동안함)
                    UnityAction<JsonData> CallBack;
                    if (!EventListener.TryGetValue((string)DataDecode["type"], out CallBack)){
                        Debug.LogError($"[domi Network] {DataDecode["type"]} Trigger는 찾을 수 없습니다.");
                        return;
                    }

                    if ((string)DataDecode["type"] != "Room.PlayerUpdate")
                        _actions.Enqueue(() => CallBack.Invoke(DataDecode["data"]));
                    else
                        SaveUpdatePlayer[(string)DataDecode["data"]["id"]] = DataDecode["data"];
                }
        }

        foreach (var item in SaveUpdatePlayer) {
            print(item.Key);
            if (EventListener.TryGetValue("Room.PlayerUpdate", out var CallBack))
                _actions.Enqueue(() => CallBack.Invoke(item.Value));
        }

        // 다음 패킷..
        stream.BeginRead(buffer, 0, buffer.Length, OnRead, null);
    }
    
    // [이벤트 리스너] 서버와 연결끊김
    void OnDisconnect() {
        client = null; // connection 지움
        EventDisconnect?.Invoke(WhyDisconnect); // 이벤트 실행
    }

    ////////////////// 코루틴 //////////////////
    IEnumerator WaitServerConnect() {
        float TimeWait = 0;
        yield return new WaitUntil(() => { // 이미 연결되어있거나 5초 이상 넘어가면
            TimeWait += Time.deltaTime;

            if (client.Connected || TimeWait >= 5)
                return true;
            
            return false;
        });

        // 서버와 연결되있지 않은 경우
        if (!client.Connected) {
            Debug.LogError("[domi Network] 서버와 연결을 실패하였습니다. (타임아웃)");
            client.Close();
            WhyDisconnect = "TimeOut";
            OnDisconnect();
        } else /* 서버와 연결끊어짐을 감지함 */ StartCoroutine(DetectDisconnect());
    }

    IEnumerator DetectDisconnect() {
        yield return new WaitUntil(() => !client.Connected);
        Debug.LogError($"[domi Network] 서버와 연결이 끊어졌습니다. ({(WhyDisconnect != null ? WhyDisconnect : "연결 끊김")})");
        OnDisconnect();
    }
}
