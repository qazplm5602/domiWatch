using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class domiWeapon {
    // 설정 관련
    public GameObject Model;
    public GameObject BulletModel;
    public Sprite Image;
    public int MaxAmmo;
    public float FireDelay; // 총 빵빵 대기시간
    public float ReloadDelay; // 총 재장전 대기시간
    public KeyCode SlotKey;
    public AudioClip ShotSound;
    
    // 진짜 쓸꺼
    int Ammo;
    [HideInInspector] public float FireTime; // 총을 쏜 시간
    [HideInInspector] public Transform ShotCoords;

    [HideInInspector] // 혹시 모르니까 해야징
    public int ammo {
        get => Ammo;
        set {
            if (value < 0) value = 0;
            Ammo = value;
        }
    }
}

public class domiWeaponChangePacket {
    public string id;
    public int WeaponID;
}
public class domiWeaponPacket {
    public int WeaponID;
    public double[] StartPos;
    public double[] DirectionPos;

    public domiWeaponPacket(int _WeaponID, Vector3 _start, Vector3 direction) {
        WeaponID = _WeaponID;
        StartPos = new double[] {_start.x, _start.y, _start.z};
        DirectionPos = new double[] {direction.x, direction.y, direction.z};
    }
}

public class WeaponManager : MonoBehaviour
{
    int CurrentWeaponID; // 들고있는거
    [SerializeField] domiWeapon[] Weapons;

    private void Awake() {
        NetworkCore.EventListener["Room.PlayerWeaponChange"] = PlayerChangeWeapon;
    }

    private void OnDestroy() {
        NetworkCore.EventListener.Remove("Room.PlayerWeaponChange");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // 커서 정중앙에 있어야하지롱
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // 커서 숨김

        // 총알 꽉꽉 채울랭
        foreach (var weapon in Weapons)
            weapon.ammo = weapon.MaxAmmo;
        
        // 총 초기화
        ChangeMyWeapon(0);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Weapons.Length; i++)
        {
            var Weapon = Weapons[i];

            if (Input.GetKeyDown(Weapon.SlotKey)) {
                if (CurrentWeaponID != i) {
                    // 자기 자신
                    ChangeMyWeapon(i);
                    break;
                }
            }
        }

        var SelectWeapon = Weapons[CurrentWeaponID];
        // 총 쏘기
        if (Input.GetMouseButton(0) && (Time.time - SelectWeapon.FireTime) /* 총 쏜 시간으로부터 얼마나 지남 */ > SelectWeapon.FireDelay) {
            SelectWeapon.FireTime = Time.time;

            Vector3 Direcrtion = ShotDirectionPos();
            CreateBullet(SelectWeapon, null, SelectWeapon.ShotCoords.position, Direcrtion);

            // 서버한테 알리기
            NetworkCore.Send("Room.BulletCreate", new domiWeaponPacket(CurrentWeaponID, SelectWeapon.ShotCoords.position, Direcrtion));
        }
    }

    Vector3 ShotDirectionPos() {
        var Weapon = Weapons[CurrentWeaponID];

        var dir = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
        var ray = new Ray(Weapon.ShotCoords.position, dir);
        
        return ray.direction;
    }

    void CreateBullet(domiWeapon weapon, string AttackID, Vector3 startPos, Vector3 direction) {
        GameObject Bullet = Instantiate(weapon.BulletModel, startPos, Quaternion.identity);
        Bullet.tag = "Bullet"; // 태그 지정
        BulletSys Bullet_System = Bullet.AddComponent<BulletSys>();

        // 이 총알을 만든 사람
        Bullet_System.CreatePlayer = AttackID;
        // 어디로 날라가는지
        Bullet_System.Direction = direction;
    }

    void ChangeMyWeapon(int ID) {
        var Weapon = Weapons[ID];
        CurrentWeaponID = ID;
        // 자기 자신
        GameObject WeaponObject = UpdateWeapon(SpawnManager.instance.MyEntity.GetComponent<PlayerInfo>().HandHandler, Weapon);
        Weapon.ShotCoords = WeaponObject.transform.Find("ShotPos"); // 쏘는 좌표
        
        // 서버에서도 바꿔야징
        NetworkCore.Send("Room.WeaponChange", ID);
    }

    GameObject UpdateWeapon(GameObject Hand, domiWeapon Weapon) {
        print("update!");
        Transform GetWeapon = Hand.transform.GetChild(0);
        if (GetWeapon)
            Destroy(GetWeapon.gameObject); // 총 삭제
        
        // 총 소환
        GameObject WeaponEntity = Instantiate(Weapon.Model, Vector3.zero, Quaternion.identity, Hand.transform);
        WeaponEntity.transform.localPosition = Vector3.zero;
        WeaponEntity.transform.localEulerAngles = Vector3.zero;

        return WeaponEntity;
    }

    void PlayerChangeWeapon(LitJson.JsonData data) {
        var domi = LitJson.JsonMapper.ToObject<domiWeaponChangePacket>(data.ToJson());
        if (!SyncManager.PlayerEntity.TryGetValue(domi.id, out var PlayerEntity)) return;

        UpdateWeapon(PlayerEntity.GetComponent<PlayerInfo>().HandHandler, Weapons[domi.WeaponID]);
    }
}
