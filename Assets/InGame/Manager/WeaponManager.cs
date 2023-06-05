using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class domiWeapon {
    // 설정 관련
    public GameObject Model;
    public GameObject BulletModel;
    public GameObject FireEffect;
    public Sprite Image;
    public int MaxAmmo;
    public int Damage;
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
public class domiWeaponPacketResult {
    public string AttackID;
    public int WeaponID;
    public double[] StartPos;
    public double[] DirectionPos;
}

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;
    
    int CurrentWeaponID; // 들고있는거
    [SerializeField, Tooltip("피 튀기는 이펙")] GameObject BloodEffect;
    [Header("UI")]
    [SerializeField] TextMeshProUGUI MaxWeaponAmmo;
    [SerializeField] TextMeshProUGUI CurrnetWeaponAmmo;
    
    [SerializeField, Header("무기 설정")] domiWeapon[] Weapons;

    // 재장전
    IEnumerator ReloadThread;

    private void Awake() {
        NetworkCore.EventListener["Room.PlayerWeaponChange"] = PlayerChangeWeapon;
        NetworkCore.EventListener["Room.PlayerFireWeapon"] = PlayerFireWeapon;
        if (instance == null) instance = this;
    }

    private void OnDestroy() {
        NetworkCore.EventListener.Remove("Room.PlayerWeaponChange");
        NetworkCore.EventListener.Remove("Room.PlayerFireWeapon");
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
        if (Input.GetMouseButton(0) && (Time.time - SelectWeapon.FireTime) /* 총 쏜 시간으로부터 얼마나 지남 */ > SelectWeapon.FireDelay && ReloadThread == null /* 재장전중이 아님 */) {
            if (SelectWeapon.ammo <= 0) { // 총알이 없넹
                ReloadThread = WeaponReload(SelectWeapon);
                StartCoroutine(ReloadThread);
                return;
            }
            SelectWeapon.FireTime = Time.time;

            Vector3 Direcrtion = ShotDirectionPos();
            CreateBullet(SelectWeapon, null, SelectWeapon.ShotCoords.position, Direcrtion);

            // 총알 소모
            SelectWeapon.ammo --;
            CurrnetWeaponAmmo.text = SelectWeapon.ammo.ToString();

            // 발사 이펙트트
            GameObject FireEffect = Instantiate(SelectWeapon.FireEffect, Vector3.zero, Quaternion.identity, SelectWeapon.ShotCoords);
            FireEffect.transform.localPosition = SelectWeapon.FireEffect.transform.localPosition;
            FireEffect.transform.localEulerAngles = SelectWeapon.FireEffect.transform.localEulerAngles;
            FireEffect.AddComponent<AutoRemoveEntity>().DelayRemove = .2f; // 자동 삭제ㅔㅔㅔ

            // 서버한테 알리기
            NetworkCore.Send("Room.BulletCreate", new domiWeaponPacket(CurrentWeaponID, SelectWeapon.ShotCoords.position, Direcrtion));
        }
        if (Input.GetKeyDown(KeyCode.R) && ReloadThread == null) {
            ReloadThread = WeaponReload(SelectWeapon);
            StartCoroutine(ReloadThread);
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
        // 피 튀기는 옵젝
        Bullet_System.BloodPrefeb = BloodEffect;
        // 어디로 날라가는지
        Bullet_System.Direction = direction;
        // 맞으면 피 얼마나 깎는지ㅣㅣ
        Bullet_System.Damage = weapon.Damage;

        // 소리도 추가!!
        GameObject SoundObj = new("WeaponAidio-"+(AttackID != null ? AttackID : "my"));
        SoundObj.transform.position = startPos;
        
        var AudioComponent = SoundObj.AddComponent<AudioSource>();
        AudioComponent.playOnAwake = false;
        AudioComponent.loop = false;
        AudioComponent.clip = weapon.ShotSound; // 소리 파일
        AudioComponent.rolloffMode = AudioRolloffMode.Linear; // 멀리 갈수록 안들리게
        if (AttackID != null) // 3d 오디오는 자신이 아닌 다른 사람이 쏜경우에만 적용함!!!
            AudioComponent.spatialBlend = 1; // 3d 오디오오오
        AudioComponent.maxDistance = 50; // 최대 거리
        AudioComponent.Play();

        // 자동 삭제
        StartCoroutine(AudioAutoRemove(AudioComponent, SoundObj));
    }

    void ChangeMyWeapon(int ID) {
        var Weapon = Weapons[ID];
        CurrentWeaponID = ID;
        // 자기 자신
        GameObject WeaponObject = UpdateWeapon(SpawnManager.instance.MyEntity.GetComponent<PlayerInfo>().HandHandler, Weapon);
        Weapon.ShotCoords = WeaponObject.transform.Find("ShotPos"); // 쏘는 좌표

        if (ReloadThread != null) {
            StopCoroutine(ReloadThread);
            ReloadThread = null;
        }

        // UI 변경
        MaxWeaponAmmo.text = Weapon.MaxAmmo.ToString();
        CurrnetWeaponAmmo.text = Weapon.ammo.ToString();
        
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
        // WeaponEntity.transform.localPosition = Vector3.zero;
        // WeaponEntity.transform.localEulerAngles = Vector3.zero;
        WeaponEntity.transform.localPosition = Weapon.Model.transform.localPosition;
        WeaponEntity.transform.localEulerAngles = Weapon.Model.transform.localEulerAngles;

        return WeaponEntity;
    }

    void PlayerChangeWeapon(LitJson.JsonData data) {
        var domi = LitJson.JsonMapper.ToObject<domiWeaponChangePacket>(data.ToJson());
        if (!SyncManager.PlayerEntity.TryGetValue(domi.id, out var PlayerEntity)) return;

        UpdateWeapon(PlayerEntity.GetComponent<PlayerInfo>().HandHandler, Weapons[domi.WeaponID]);
    }
    // 구냥
    public void PlayerChangeWeapon(string id, int WeaponID) {
        if (!SyncManager.PlayerEntity.TryGetValue(id, out var PlayerEntity)) return;
        UpdateWeapon(PlayerEntity.GetComponent<PlayerInfo>().HandHandler, Weapons[WeaponID]);
    }

    // 총쏨
    void PlayerFireWeapon(LitJson.JsonData data) {
        var domi = LitJson.JsonMapper.ToObject<domiWeaponPacketResult>(data.ToJson());

        CreateBullet(
            Weapons[domi.WeaponID],
            domi.AttackID,
            new Vector3((float)domi.StartPos[0], (float)domi.StartPos[1], (float)domi.StartPos[2]),
            new Vector3((float)domi.DirectionPos[0], (float)domi.DirectionPos[1], (float)domi.DirectionPos[2])
        );
    }

    // 총 재장전
    IEnumerator WeaponReload(domiWeapon Weapon) {
        CurrnetWeaponAmmo.text = "재장전...";
        yield return new WaitForSeconds(Weapon.ReloadDelay);

        Weapon.ammo = Weapon.MaxAmmo; // 다시 채우기
        CurrnetWeaponAmmo.text = Weapon.ammo.ToString();
        ReloadThread = null;
    }

    // 오디오 자동 삭제
    IEnumerator AudioAutoRemove(AudioSource audioSource, GameObject entity) {
        yield return new WaitUntil(() => !audioSource.isPlaying);
        Destroy(entity);
    }
}
