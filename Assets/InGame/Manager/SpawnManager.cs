using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // 이미 리소스 파일에서 불러온것은 저장해야제 (계속 꺼내오면 렉걸리지롱)
    public static Dictionary<string, GameObject> CachePrefebs = new();
    public static SpawnManager instance;
    [SerializeField] GameObject CharObject;
    public GameObject MyEntity {get; private set;}
    
    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("SpawnManager 이미 등록되어 있습니다.");
            Destroy(gameObject);
        }
    }

    private void Start() {
        // 자기자신 소환일때
        GameObject Player = MyEntity = Instantiate(CharObject, Vector3.zero + CharObject.transform.position, Quaternion.identity);

        PlayerInfo Player_Info = Player.GetComponent<PlayerInfo>();

        // 카메라를 캐릭터 안에 넣고
        Camera.main.transform.parent = Player.transform;
        // 카메라 위치 수정
        Camera.main.transform.localPosition = Player_Info.MaincameraCoords;
        
        // // 카메라 안에 손 넣깅
        // Player_Info.HandHandler.transform.parent = Camera.main.transform;
        // // 손 좌표를 수정하자
        // Player_Info.HandHandler.transform.localPosition = Player_Info.MyHandCoords;

        // 플레이어 움직이는 스크를 넣자
        Player.AddComponent<PlayerMovement>();
    }

    public GameObject SpawnPlayer(string skin) {
        GameObject Prefebs;
        if (!CachePrefebs.TryGetValue(skin, out Prefebs)) {
            // Cache 에 저장과 동시에 프래핍 불러오장
            CachePrefebs[skin] = Prefebs = Resources.Load("Player/Characters/"+skin) as GameObject;
        }
        
        return Instantiate(Prefebs, Prefebs.transform.position, Quaternion.identity);
    } 
}
