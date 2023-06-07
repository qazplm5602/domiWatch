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

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return) && !isActive) {
            isActive = true;

            // 커서 설정
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            ChatBox.GetComponent<Image>().DOFade(.3f, .2f);

            // 입력창 활성화
            InputBox.SetActive(true);
            InputBox.GetComponent<TMP_InputField>().text = ""; // 입력값 초기화 ㄱㄱ
            InputBox.GetComponent<TMP_InputField>().ActivateInputField();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isActive) {
            isActive = false;

            // 커서 설정
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            ChatBox.GetComponent<Image>().DOFade(0, .2f);

            // 입력창 비활
            InputBox.SetActive(false);
        }
    }
}
