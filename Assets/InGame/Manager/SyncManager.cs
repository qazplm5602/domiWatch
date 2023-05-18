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
    }

    private void OnDestroy() {
        PlayerEntity = null;
        NetworkCore.EventListener.Remove("Room.ResultAllPlayer");
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
}
