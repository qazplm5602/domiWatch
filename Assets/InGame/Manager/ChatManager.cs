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
    [SerializeField] Transform ChatContent;
    [SerializeField] GameObject InputBox;
    [SerializeField] ScrollRect scrollRect;

    [SerializeField] GameObject MessageBox;

    bool isActive = false;
    bool isShow = false;
    float HideTime = 0;

    private void Awake() {
        NetworkCore.EventListener["Room.MessageAdd"] = NetMessageAdd;
    }
    private void OnDestroy() {
        NetworkCore.EventListener.Remove("Room.MessageAdd");
    }

    private void Update() {
        if (isShow) {
            if (isActive) HideTime = 0;
            else if (HideTime >= 5) ChatHide();
            else HideTime += Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Return) && !isActive) {
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
            return;
        }

        if (Input.GetKeyUp(KeyCode.Return) && isActive) SendChatMessage();

        if (Input.GetKeyDown(KeyCode.Escape) && isActive) InputDeSelected();
    }

    void ChatHide() {
        isShow = false;
        ChatMain.GetComponent<CanvasGroup>().DOFade(0, .2f).OnComplete(() => ChatMain.SetActive(false));
    }

    void ChatShow() {
        HideTime = 0;
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

    void SendChatMessage() {
        var InputField = InputBox.GetComponent<TMP_InputField>();
        if (InputField.text.Length <= 0) { // 아무것도 입력 안하면 닫아야징
            InputDeSelected();
            return;
        }

        // 서버한테 보냉
        // NetworkCore.Send("Room.SendMessage", InputField.text);
        AddChatMessage(InputField.text); // 테스트
        InputField.text = ""; // 입력 값 없어져람
        InputField.Select(); // 다시 입력창 포커수
        InputField.ActivateInputField();
    }

    void NetMessageAdd(LitJson.JsonData data) => AddChatMessage((string)data);
    public void AddChatMessage(string value) {
        ChatShow();
        print(scrollRect.verticalNormalizedPosition);
        var Message = Instantiate(MessageBox, Vector3.zero, Quaternion.identity, ChatContent);
        Message.GetComponent<TextMeshProUGUI>().text = value;
    }
}
