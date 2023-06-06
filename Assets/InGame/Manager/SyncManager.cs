using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class AllPlayerPacket {
    public string id;
    public string name;
    public double[] coords = new double[3];
    public double[] rotate = new double[2];
    public int weapon;
    public bool dead;
    public int score_kill;
    public int score_death;
}
public class PlayerAddPacket {
    public string id;
    public string name;
}

public class SyncManager : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI NameTag; // 뜬금없지만 그냥 넣음
    public static Dictionary<string, GameObject> PlayerEntity {get; private set;}

    private void Awake() {
        PlayerEntity = new();
        NetworkCore.EventListener["Room.ResultAllPlayer"] = AllPlayerSync;
        NetworkCore.EventListener["Room.PlayerAdd"] = PlayerAdd;
        NetworkCore.EventListener["Room.PlayerUpdate"] = ResultPlayerUpdate;
        NetworkCore.EventListener["Room.PlayerSetCoords"] = MyCoordSet;
        NetworkCore.EventListener["Room.ScoreAddMY"] = MyScoreAdd;
    }

    private void OnDestroy() {
        PlayerEntity = null;
        NetworkCore.EventListener.Remove("Room.ResultAllPlayer");
        NetworkCore.EventListener.Remove("Room.PlayerAdd");
        NetworkCore.EventListener.Remove("Room.PlayerUpdate");
        NetworkCore.EventListener.Remove("Room.PlayerSetCoords");
        NetworkCore.EventListener.Remove("Room.ScoreAddMY");
    }

    void Start()
    {
        NetworkCore.Send("Room.GetAllPlayer", null);
    }

    // 모든 플레이어 동기화 시켜버리기
    void AllPlayerSync(JsonData data) {
        AllPlayerPacket[] PlayersData = JsonMapper.ToObject<AllPlayerPacket[]>(data.ToJson());

        foreach (var PlayerData in PlayersData)
        {
            // 플레이어 생성
            GameObject PlayerObj = SpawnManager.instance.SpawnPlayer("VanguardChoonyung");
            PlayerObj.tag = "Player";
            PlayerObj.name = "Player-"+PlayerData.id;
            PlayerEntity[PlayerData.id] = PlayerObj;

            // 플레이어 위치 동기화
            PlayerSyncMove EntityMove = PlayerObj.GetComponent<PlayerSyncMove>();
            EntityMove.LastCoords = new Vector3((float)PlayerData.coords[0], (float)PlayerData.coords[1], (float)PlayerData.coords[2]);
            EntityMove.LastMouseX = (float)PlayerData.rotate[1];
            // EntityMove.LastMouseY = (float)PlayerData.rotate[0];

            // 총 동기화
            WeaponManager.instance.PlayerChangeWeapon(PlayerData.id, PlayerData.weapon);

            // 이미 죽었낭?
            if (PlayerData.dead) {
                PlayerObj.GetComponent<Animator>().SetBool("isDie", true);
                PlayerObj.GetComponent<CharacterController>().enabled = false;
            }

            // 스코어보드 추가
            ScoreboardManager.Create(PlayerData.id, PlayerData.name, false);
            ScoreboardManager.EditText(ScoreboardManager.TextMode.Kill, PlayerData.id, PlayerData.score_kill.ToString());
            ScoreboardManager.EditText(ScoreboardManager.TextMode.Death, PlayerData.id, PlayerData.score_death.ToString());
        }
    }

    // 서버에서 플레이어 추가해달라고 함
    void PlayerAdd(JsonData data) {
        var PlayerInfo = JsonMapper.ToObject<PlayerAddPacket>(data.ToJson());
        GameObject PlayerObj = SpawnManager.instance.SpawnPlayer("VanguardChoonyung");
        PlayerObj.tag = "Player";
        PlayerObj.name = "Player-"+PlayerInfo.id;
        PlayerEntity[PlayerInfo.id] = PlayerObj;

        // 스코어보드 추가
        ScoreboardManager.Create(PlayerInfo.id, PlayerInfo.name, false);
    }

    // 서버에서 플레이어 좌표를 업뎃 해달라고 요청했지롱
    void ResultPlayerUpdate(JsonData data) {
        AllPlayerPacket PlayerData = JsonMapper.ToObject<AllPlayerPacket>(data.ToJson());
        if (!PlayerEntity.TryGetValue(PlayerData.id, out var Entity)) // 없음ㄴ 나가
            return;

        // 얘가 부드럽게 해준다고 시킬꺼
        PlayerSyncMove EntityMove = Entity.GetComponent<PlayerSyncMove>();
        Vector3 UpdateCoords = new Vector3((float)PlayerData.coords[0], (float)PlayerData.coords[1], (float)PlayerData.coords[2]);

        // 텔포다!!!
        if (Vector3.Distance(UpdateCoords, Entity.transform.position) > 10)
            Entity.transform.position = UpdateCoords;

        EntityMove.LastCoords = UpdateCoords;
        EntityMove.LastMouseX = (float)PlayerData.rotate[1];
        // EntityMove.LastMouseY = (float)PlayerData.rotate[0];
    }

    // 서버에서 내 좌표 바꾸래
    void MyCoordSet(JsonData data) {
        Vector3 Coords = JsonUtility.FromJson<Vector3>(data.ToJson());

        var MyEntity = SpawnManager.instance.MyEntity;
        if (MyEntity != null) 
            MyEntity.GetComponent<PlayerMovement>().SetCoords(Coords);
    }

    // 서버에서 내꺼 스코어 추가하래
    void MyScoreAdd(JsonData data) {
        NameTag.text = (string)data["name"];
        ScoreboardManager.Create((string)data["id"], (string)data["name"], true);
    }
}
