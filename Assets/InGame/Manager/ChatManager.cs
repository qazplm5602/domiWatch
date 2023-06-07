using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ChatManager : MonoBehaviour
{
    [SerializeField] GameObject ChatMain;
    [SerializeField] GameObject ChatBox;
    [SerializeField] GameObject InputBox;

    bool isActive = false;
    bool isShow = false;
    float HideTime = 0;

    private void Update() {
        if (isShow) {
            if (isActive) HideTime = 0;
            else if (HideTime >= 5) ChatHide();
            else HideTime += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Return) && !isActive) {
            isActive = true;
            ChatShow();

            // 커서 설정
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            ChatBox.GetComponent<Image>().DOFade(.3f, .2f);

            // 입력창 활성화
            InputBox.SetActive(true);
            InputBox.GetComponent<TMP_InputField>().text = ""; // 입력값 초기화 ㄱㄱ
            InputBox.GetComponent<TMP_InputField>().ActivateInputField();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isActive) InputDeSelected();
    }

    void ChatHide() {
        isShow = false;
        ChatMain.GetComponent<CanvasGroup>().DOFade(0, .2f).OnComplete(() => ChatMain.SetActive(false));
    }

    void ChatShow() {
        if (isShow) return;

        isShow = true;
        ChatMain.SetActive(true);
        ChatMain.GetComponent<CanvasGroup>().DOFade(1, .2f);
    }

    public void InputDeSelected() {
        isActive = false;

        // 커서 설정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ChatBox.GetComponent<Image>().DOFade(0, .2f);

        // 입력창 비활
        InputBox.SetActive(false);
    }
}
