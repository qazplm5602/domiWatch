using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using LitJson;

class domiLoginForm {
    public string id;
    public string password;

    public domiLoginForm(string _id, string _password) {
        id = _id;
        password = _password;
    }
}

public class LobbyManager : MonoBehaviour
{
    [SerializeField] RectTransform PlayButton;
    [SerializeField] GameObject Loading;
    [SerializeField] GameObject ErrorUI;
    [SerializeField] GameObject InputUI;

    TMP_InputField ID_Input;
    TMP_InputField Password_Input;

    // 서버 연결 성공여부
    bool isConnect = false;

    private void Start() {
        NetworkCore.EventConnect += SuccessConnect;
        NetworkCore.EventDisconnect += ErrorConnect; // 리스너 등록
        NetworkCore.EventListener["domiServer.Hello"] = HelloDomi;
        PlayButton.GetComponent<LobbyButton>().SetLock();

        ID_Input = InputUI.transform.Find("ID_Input").GetComponent<TMP_InputField>();
        Password_Input = InputUI.transform.Find("Password_Input").GetComponent<TMP_InputField>();
    }

    void HelloDomi(JsonData data) {
        print($"나는 {data["name"]}이고 아이디는 {data["id"]} 이다!!!!");
    }

    public void InputChange() {
        if (ID_Input.text.Length <= 0 || Password_Input.text.Length <= 0)
            PlayButton.GetComponent<LobbyButton>().SetLock();
        else
            PlayButton.GetComponent<LobbyButton>().UnLock();
    }

    public void PlayClick() {
        // 에러 삭제
        if (ErrorUI.activeSelf)
            ErrorUI.GetComponent<CanvasGroup>().DOFade(0, .2f).OnComplete(() => ErrorUI.SetActive(false));

        // 확인 버튼 이면
        if (PlayButton.GetComponentInChildren<TextMeshProUGUI>().text == "확인") {
            PlayButton.GetComponentInChildren<TextMeshProUGUI>().text = "접속";
            
            // 입력란 보이게
            InputUI.SetActive(true);
            InputUI.GetComponent<CanvasGroup>().DOFade(1, .2f);

            // 버튼 위치
            PlayButton.GetComponent<RectTransform>().DOAnchorPosY(200, 0.3f).SetEase(Ease.OutQuad);
            return;
        }

        isConnect = false;

        // 로딩..
        Loading.SetActive(true);
        Loading.GetComponent<CanvasGroup>().DOFade(1, .2f);

        // 입력란 삭제
        InputUI.GetComponent<CanvasGroup>().DOFade(0, .2f).OnComplete(() => InputUI.SetActive(false));

        // 버튼
        PlayButton.GetComponent<RectTransform>().DOAnchorPosY(255, 0.3f).SetEase(Ease.OutQuad);
        PlayButton.GetComponent<LobbyButton>().SetLock();

        // 서버와 연결 시도
        NetworkCore.instance.ServerConnect();
    }

    void SuccessConnect() {
        isConnect = true;
        Loading.GetComponentInChildren<TextMeshProUGUI>().text = "연결 성공! 서버 응답을 기다리는중...";

        // 아이디랑 패스워드 보냄
        NetworkCore.Send("domiServer.Login", new domiLoginForm(ID_Input.text, Password_Input.text));
    }

    void ErrorConnect() {
        // 에러 표시
        ErrorUI.SetActive(true);
        ErrorUI.GetComponent<CanvasGroup>().DOFade(1, .2f);

        // 로딩 제거
        Loading.GetComponent<CanvasGroup>().DOFade(0, .2f).OnComplete(() => Loading.SetActive(false));

        // 버튼 활성화
        PlayButton.GetComponent<RectTransform>().DOAnchorPosY(260, 0.3f).SetEase(Ease.OutQuad);
        PlayButton.GetComponentInChildren<TextMeshProUGUI>().text = "확인";
        PlayButton.GetComponent<LobbyButton>().UnLock();

        if (isConnect) {
            Loading.GetComponentInChildren<TextMeshProUGUI>().text = "접속 중...";
        }
        ErrorUI.transform.Find("Why").GetComponent<TextMeshProUGUI>().text = isConnect ? "연결 도중 끊김" : "TimeOut";
    }
}
