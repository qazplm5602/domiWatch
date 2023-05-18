using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject CharObject;

    private void Start() {
        // 자기자신 소환일때
        GameObject Player = Instantiate(CharObject, Vector3.zero + CharObject.transform.position, Quaternion.identity);

        PlayerInfo Player_Info = Player.GetComponent<PlayerInfo>();

        // 카메라를 캐릭터 안에 넣고
        Camera.main.transform.parent = Player.transform;
        // 카메라 위치 수정
        Camera.main.transform.localPosition = Player_Info.MaincameraCoords;
        
        // 카메라 안에 손 넣깅
        Player_Info.HandHandler.transform.parent = Camera.main.transform;
        // 손 좌표를 수정하자
        Player_Info.HandHandler.transform.localPosition = Player_Info.MyHandCoords;

        // 플레이어 움직이는 스크를 넣자
        Player.AddComponent<PlayerMovement>();
    }
}
