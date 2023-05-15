using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    void Start()
    {
        NetworkCore.EventConnect += OnConnect;
        NetworkCore.EventListener["HelloDomi"] = HelloServer;
    }

    void OnConnect() {
        print("오!!! 연결이 돼따!");
        NetworkCore.Send("HelloServer", "안녕하이~");
    }

    void HelloServer(LitJson.JsonData data) {
        print("오 서버가 인사 해줬다.");
        print("서버: "+(string)data);
    }
}
