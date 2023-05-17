using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyHeaderButton : MonoBehaviour
{
    Image ButtonBackground;
    TextMeshProUGUI TextUI;

    private void Awake() {
        ButtonBackground = GetComponent<Image>();
        TextUI = GetComponentInChildren<TextMeshProUGUI>();
        Active(false);
    }

    private void Start() {
        // 첫번째 버튼이 이거다!!
        if (transform.parent.GetChild(0) == transform)
            Active(true);
    }

    void Active(bool isShow) {
        // 배경 색깔
        ButtonBackground.enabled = isShow;

        // 폰트 색깔
        TextUI.color = isShow ? Color.white : new Color(.1960784f, .2196079f, .282353f);
    }
}
