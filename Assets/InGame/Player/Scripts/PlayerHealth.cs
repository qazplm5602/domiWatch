using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int DefualtHealth = 200;
    [SerializeField] Image HealthBar;
    
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

    private void Start() {
        Invoke(nameof(asdsadas), 1);
    }
    void asdsadas() => health = 0;

    void OnDie() {
        GameObject MyEntity = SpawnManager.instance.MyEntity;

        // 죽는 애니메이션
        MyEntity.GetComponent<Animator>().SetBool("isDie", true);

        // 카메라 뒤로
        Camera.main.transform.localPosition += (Vector3.up * 5) + (Vector3.back * 5);
        // Camera.main.transform.DOLocalMove(Camera.main.transform.localPosition + (Camera.main.transform.forward * -5) + (Camera.main.transform.up * 3), 3f).SetEase(Ease.OutQuad);
        // Camera.main.transform.DOLocalRotate(new Vector3(35.4f, 0, 0), 3f).SetEase(Ease.OutQuad);
    }
}
