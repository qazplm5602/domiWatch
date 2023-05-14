using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    void Start()
    {
        NetworkCore.EventConnect += OnConnect;
        NetworkCore.EventDisconnect += OnDisconnect;
        NetworkCore.EventListener["HelloDomi"] = HelloServer;
    }

    void OnConnect() {
        print("오!!! 연결이 돼따!");
        NetworkCore.Send("HelloServer", "안녕하이~");
    }

    void OnDisconnect() {
        print("잉, 연결이 끊였네.");
    }

    void HelloServer(LitJson.JsonData data) {
        print("오 서버가 인사 해줬다.");
        print("서버: "+(string)data);
    }
}
