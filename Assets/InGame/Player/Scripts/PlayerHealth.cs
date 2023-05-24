using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            _Health = value;
            UpdateHealthBar();
        }
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
}
