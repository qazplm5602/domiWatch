using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

class PlayerDiePacket {
    public string DiePlayer;
    public string DiePlayerName;
    public string Attacker;
    public bool YouKill;
    public bool YouDead;
}

public class PlayerHealth : MonoBehaviour
{
    [Header("자신 플레이어")]
    [SerializeField] int DefualtHealth = 200;
    [SerializeField] Image HealthBar;
    
    [Header("킬로그 설정")]
    [SerializeField] Transform KillLog_List;
    [SerializeField] GameObject KillLog_Form;

    [Header("아이유")]
    [SerializeField] RectTransform WaitHeader;
    
    public static PlayerHealth instance;
    
    int _Health;
    public int health {
        get => _Health;
        set {
            if (value < 0) value = 0;
            if (value > DefualtHealth) value = DefualtHealth;
            int CacheHealth = health;
            _Health = value;
            UpdateHealthBar();

            // 죽었따.
            if (CacheHealth > 0 && value <= 0)
                OnDie();
        }
    }

    // 짱짱 편함
    public bool isDie {
        get => health <= 0;
    }

    private void Awake() {
        if (instance == null) instance = this;

        health = DefualtHealth;
        HealthBar.fillAmount = 1;

        // 이벤트 리스너 등록
        NetworkCore.EventListener["Room.BroadcastPlayerDie"] = OtherPlayerDie;
    }

    private void OnDestroy() {
        // 이벤트 리스너 해제    
        NetworkCore.EventListener.Remove("Room.BroadcastPlayerDie");
    }

    void UpdateHealthBar() {
        float Percent = HealthBar.fillAmount = (float)health / DefualtHealth;

        // 체력에 따라 색상 변경
        if (Percent <= .2f) {
            HealthBar.color = new Color(1, 0.3742138f, 0.3742138f);
        } else if (Percent <= .5f) {
            HealthBar.color = new Color(0.9528302f, 0.734655f, 0.2936395f);
        } else {
            HealthBar.color = Color.white;
        }
    }

    void OnDie() {
        GameObject MyEntity = SpawnManager.instance.MyEntity;

        // 죽는 애니메이션
        MyEntity.GetComponent<Animator>().SetBool("isDie", true);

        // 카메라 뒤로
        Camera.main.transform.DOLocalMove(Camera.main.transform.localPosition + (Vector3.up * 5) + (Vector3.back * 5), 1f).SetEase(Ease.OutQuad);
        Camera.main.transform.DOLocalRotate(new Vector3(45.65f, 0, 0), 1f).SetEase(Ease.OutQuad);

        // 리스폰 대기...
        WaitHeader.gameObject.SetActive(true);
    }

    void OtherPlayerDie(LitJson.JsonData data) {
        var DieInfo = LitJson.JsonMapper.ToObject<PlayerDiePacket>(data.ToJson());

        // 플레이어 쓰러지게 해야징 (내가 죽은거면 안함)
        if (!DieInfo.YouDead && SyncManager.PlayerEntity.TryGetValue(DieInfo.DiePlayer, out var DieEntity)) {
            DieEntity.GetComponent<Animator>().SetBool("isDie", true); // 죽은 행동
            DieEntity.GetComponent<CharacterController>().enabled = false; // 콜라이더 비활..            
        }

        // 킬로그 소환
        GameObject KillLogEntity = Instantiate(KillLog_Form, Vector3.zero, Quaternion.identity, KillLog_List);
        KillLogEntity.GetComponent<KillLogSys>().Init(DieInfo.Attacker, DieInfo.DiePlayerName);
        
        print($"서버가 {DieInfo.DiePlayerName} 이(가) 죽었고 {DieInfo.Attacker} 이(가) 사살했다고 말했음");
    }
}
