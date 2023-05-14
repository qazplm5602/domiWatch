using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] RectTransform PlayButton;
    [SerializeField] GameObject Loading;
    [SerializeField] GameObject ErrorUI;

    // 서버 연결 성공여부
    bool isConnect = false;

    private void Start() {
        NetworkCore.EventConnect += SuccessConnect;
        NetworkCore.EventDisconnect += ErrorConnect; // 리스너 등록
    }

    public void PlayClick() {
        isConnect = false;

        // 로딩..
        Loading.SetActive(true);
        Loading.GetComponent<CanvasGroup>().DOFade(1, .2f);

        // 에러 삭제
        ErrorUI.GetComponent<CanvasGroup>().DOFade(0, .2f).OnComplete(() => ErrorUI.SetActive(false));

        // 버튼
        PlayButton.GetComponent<RectTransform>().DOAnchorPosY(200, 0.3f).SetEase(Ease.OutQuad);
        PlayButton.GetComponent<Button>().interactable = false;
        PlayButton.GetComponent<LobbyButton>().SetLock();

        // 서버와 연결 시도
        NetworkCore.instance.ServerConnect();
    }

    void SuccessConnect() {
        isConnect = true;
        Loading.GetComponentInChildren<TextMeshProUGUI>().text = "연결 성공! 서버 응답을 기다리는중...";
    }

    void ErrorConnect() {
        // 에러 표시
        ErrorUI.SetActive(true);
        ErrorUI.GetComponent<CanvasGroup>().DOFade(1, .2f);

        // 로딩 제거
        Loading.GetComponent<CanvasGroup>().DOFade(0, .2f).OnComplete(() => Loading.SetActive(false));

        // 버튼 활성화
        PlayButton.GetComponent<RectTransform>().DOAnchorPosY(260, 0.3f).SetEase(Ease.OutQuad);
        PlayButton.GetComponent<Button>().interactable = true;
        PlayButton.GetComponent<LobbyButton>().UnLock();

        if (isConnect) {
            Loading.GetComponentInChildren<TextMeshProUGUI>().text = "접속 중...";
        }
        ErrorUI.transform.Find("Why").GetComponent<TextMeshProUGUI>().text = isConnect ? "연결 도중 끊김" : "TimeOut";
    }
}
