using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class domiWeapon {
    // 설정 관련
    public GameObject Model;
    public Sprite Image;
    public int MaxAmmo;
    public float FireDelay; // 총 빵빵 대기시간
    public float ReloadDelay; // 총 재장전 대기시간
    public KeyCode SlotKey;
    public AudioClip ShotSound;
    
    // 진짜 쓸꺼
    int Ammo;
    [HideInInspector] public float FireTime; // 총을 쏜 시간

    [HideInInspector] // 혹시 모르니까 해야징
    public int ammo {
        get => Ammo;
        set {
            if (value < 0) value = 0;
            Ammo = value;
        }
    }
}

public class WeaponManager : MonoBehaviour
{
    int CurrentWeaponID; // 들고있는거
    [SerializeField] domiWeapon[] Weapons;

    private void Awake() {

    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Weapons.Length; i++)
        {
            var Weapon = Weapons[i];

            if (Input.GetKeyDown(Weapon.SlotKey)) {
                if (CurrentWeaponID != i) {
                    CurrentWeaponID = i;
                    // 자기 자신
                    UpdateWeapon(SpawnManager.instance.MyEntity.GetComponent<PlayerInfo>().HandHandler, Weapon);
                    break;
                }
            }
        }

        var SelectWeapon = Weapons[CurrentWeaponID];
        // 총 쏘기
        if (Input.GetMouseButton(0) && (Time.time - SelectWeapon.FireTime) /* 총 쏜 시간으로부터 얼마나 지남 */ > SelectWeapon.FireDelay) {
            SelectWeapon.FireTime = Time.time;

            print("fire!!!");
        }
    }

    void UpdateWeapon(GameObject Hand, domiWeapon Weapon) {
        print("update!");
        Destroy(Hand.transform.GetChild(0).gameObject); // 총 삭제
        
        // 총 소환
        GameObject WeaponEntity = Instantiate(Weapon.Model, Vector3.zero, Quaternion.identity, Hand.transform);
        WeaponEntity.transform.localPosition = Vector3.zero;
        WeaponEntity.transform.localEulerAngles = Vector3.zero;
    }
}
