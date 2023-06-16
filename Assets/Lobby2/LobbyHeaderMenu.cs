using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class LobbyHeaderMenu : MonoBehaviour
{
    [SerializeField] Transform ButtonList;
    List<UnityAction> MenuCallback = new();
    int ActiveMenu = 0;
    [SerializeField] GameObject PlayPanel;
    [SerializeField] GameObject SettingPanel;
    RectTransform PlayPanel_Transform;
    RectTransform SettingPanel_Transform;

    private void Awake() {
        MenuCallback.Add(MainMenu);
        MenuCallback.Add(PlayMenu);
        MenuCallback.Add(SettingScreen);

        PlayPanel_Transform = PlayPanel.GetComponent<RectTransform>();
        SettingPanel_Transform = SettingPanel.GetComponent<RectTransform>();
    }

    public void MenuClick(int index) {
        if (index == ActiveMenu) return; // 이미 선택되어 있음

        ActiveMenu = index;
        Transform ButtonTrans = ButtonList.GetChild(index);

        for (int i = 0; i < ButtonList.childCount; i++)
            ButtonList.GetChild(i).GetComponent<LobbyHeaderButton>().Active(index == i);

        AllReset();
        MenuCallback[index].Invoke();
    }

    void AllReset() {
        // 플레이 창 끄기
        if (PlayPanel.activeSelf) {
            PlayPanel_Transform.offsetMin = new Vector2(PlayPanel_Transform.offsetMin.x, -200);
            PlayPanel_Transform.offsetMax = new Vector2(PlayPanel_Transform.offsetMax.x, 0);
            PlayPanel.GetComponent<CanvasGroup>().alpha = 0;
            PlayPanel.SetActive(false);
        }
        // 설정 창 끄기
        if (SettingPanel.activeSelf) {
            SettingPanel_Transform.offsetMin = new Vector2(SettingPanel_Transform.offsetMin.x, 0);
            SettingPanel_Transform.offsetMax = new Vector2(SettingPanel_Transform.offsetMax.x, -145);
            SettingPanel.GetComponent<CanvasGroup>().alpha = 0;
            SettingPanel.SetActive(false);
        }
    }

    void PlayMenu() {
        PlayPanel.SetActive(true);
        PlayPanel_Transform.DOAnchorPosY(0, 0.3f).SetEase(Ease.OutQuad);
        PlayPanel.GetComponent<CanvasGroup>().DOFade(1, 0.3f).SetEase(Ease.OutQuad);
    }

    void MainMenu() {
        
    }

    void SettingScreen() {
        SettingPanel.SetActive(true);
        SettingPanel_Transform.DOAnchorPosY(-55, 0.3f).SetEase(Ease.OutQuad);
        SettingPanel_Transform.GetComponent<CanvasGroup>().DOFade(1, 0.3f).SetEase(Ease.OutQuad);
    }
}
