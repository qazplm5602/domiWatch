using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using LitJson;

class domiLoginForm {
    public string name;
    public string HWID;

    public domiLoginForm(string _name, string _HWID) {
        name = _name;
        HWID = _HWID;
    }
}

public class LobbyManager : MonoBehaviour
{
    [SerializeField] RectTransform PlayButton;
    [SerializeField] GameObject Loading;
    [SerializeField] GameObject ErrorUI;
    [SerializeField] GameObject InputUI;

    TMP_InputField ID_Input;

    // 서버 연결 성공여부
    bool isConnect = false;
    public static string WhyDisconnect = null;

    private void Awake() {
        NetworkCore.EventConnect += SuccessConnect;
        NetworkCore.EventDisconnect += ErrorConnect; // 리스너 등록
        NetworkCore.EventListener["Lobby.Init"] = LobbyChange;

        ID_Input = InputUI.transform.Find("ID_Input").GetComponent<TMP_InputField>();
    }
    private void Start() {
        // 커서 설정
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayButton.GetComponent<LobbyButton>().SetLock();

        if (WhyDisconnect != null) {
            ErrorConnect(WhyDisconnect);
            WhyDisconnect = null;
        }
    }

    // 리스너 해제
    void OnDestroy() {
        NetworkCore.EventConnect -= SuccessConnect;
        NetworkCore.EventDisconnect -= ErrorConnect;
        NetworkCore.EventListener.Remove("disconnect.why");
        NetworkCore.EventListener.Remove("Lobby.Init");
    }

    public void InputChange() {
        if (ID_Input.text.Length <= 0)
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
            PlayButton.GetComponent<RectTransform>().DOAnchorPosY(280, 0.3f).SetEase(Ease.OutQuad);
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

        // 이름, HWID 보냄
        NetworkCore.Send("domiServer.Login", new domiLoginForm(ID_Input.text, SystemInfo.deviceUniqueIdentifier));
    }

    void ErrorConnect(string Why) {
        if (WhyDisconnect != null) {
            // 입력란 삭제
            InputUI.GetComponent<CanvasGroup>().DOFade(0, .2f).OnComplete(() => InputUI.SetActive(false));
        }

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
        ErrorUI.transform.Find("Why").GetComponent<TextMeshProUGUI>().text = Why == null ? "연결 도중 끊김" : Why;
    }
    
    void LobbyChange(JsonData data) {
        LobbyManager2.MyName = (string)data["name"];
        LobbyManager2.MyID = (string)data["id"];
        SceneManager.LoadScene(1);
    }
}