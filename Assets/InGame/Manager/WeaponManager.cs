using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class domiWeapon {
    // 설정 관련
    public GameObject Model;
    public Sprite Image;
    public int MaxAmmo;
    public int ReloadDelay;
    public KeyCode SlotKey;
    public AudioClip ShotSound;
    
    // 진짜 쓸꺼
    int Ammo;

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
    }

    void UpdateWeapon(GameObject Hand, domiWeapon Weapon) {
        print("update!");
        Destroy(Hand.transform.GetChild(0).gameObject); // 총 삭제
        
        GameObject WeaponEntity = Instantiate(Weapon.Model, Vector3.zero, Quaternion.identity, Hand.transform);
        WeaponEntity.transform.localPosition = Vector3.zero;
        WeaponEntity.transform.localEulerAngles = Vector3.zero;
    }
}
