using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class AllPlayerPacket {
    public string id;
    public double[] coords = new double[3];
    public double[] rotate = new double[2];
}

public class SyncManager : MonoBehaviour
{
    public static Dictionary<string, GameObject> PlayerEntity {get; private set;}

    private void Awake() {
        PlayerEntity = new();
        NetworkCore.EventListener["Room.ResultAllPlayer"] = AllPlayerSync;
        NetworkCore.EventListener["Room.PlayerUpdate"] = ResultPlayerUpdate;
    }

    private void OnDestroy() {
        PlayerEntity = null;
        NetworkCore.EventListener.Remove("Room.ResultAllPlayer");
        NetworkCore.EventListener.Remove("Room.PlayerUpdate");
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
            GameObject PlayerObj = SpawnManager.instance.SpawnPlayer("MyChar");
            PlayerObj.name = "Player-"+PlayerData.id;
            PlayerEntity[PlayerData.id] = PlayerObj;
        }
    }

    // 서버에서 플레이어 좌표를 업뎃 해달라고 요청했지롱
    void ResultPlayerUpdate(JsonData data) {
        AllPlayerPacket PlayerData = JsonMapper.ToObject<AllPlayerPacket>(data.ToJson());
        if (!PlayerEntity.TryGetValue(PlayerData.id, out var Entity)) // 없음ㄴ 나가
            return;

        PlayerInfo EntityInfo = Entity.GetComponent<PlayerInfo>();
        Entity.transform.position = new Vector3((float)PlayerData.coords[0], (float)PlayerData.coords[1], (float)PlayerData.coords[2]) + Vector3.left * 2.5f;

        Vector3 CacheEuler = Entity.transform.localEulerAngles;
        CacheEuler.y = (float)PlayerData.rotate[1];
        Entity.transform.localEulerAngles = CacheEuler;

        CacheEuler = EntityInfo.HandHandler.transform.localEulerAngles;
        CacheEuler.x = (float)PlayerData.rotate[0];
        EntityInfo.HandHandler.transform.localEulerAngles = CacheEuler;
    }
}
