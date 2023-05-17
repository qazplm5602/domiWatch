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
    RectTransform PlayPanel_Transform;

    private void Awake() {
        MenuCallback.Add(MainMenu);
        MenuCallback.Add(PlayMenu);

        PlayPanel_Transform = PlayPanel.GetComponent<RectTransform>();
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
        if (PlayPanel.activeSelf)
            PlayPanel_Transform.offsetMin = new Vector2(PlayPanel_Transform.offsetMin.x, -200);
            PlayPanel_Transform.offsetMax = new Vector2(PlayPanel_Transform.offsetMax.x, 0);
            PlayPanel.GetComponent<CanvasGroup>().alpha = 0;
            PlayPanel.SetActive(false);
    }

    void PlayMenu() {
        PlayPanel.SetActive(true);
        PlayPanel_Transform.DOAnchorPosY(0, 0.3f).SetEase(Ease.OutQuad);
        PlayPanel.GetComponent<CanvasGroup>().DOFade(1, 0.3f).SetEase(Ease.OutQuad);
    }

    void MainMenu() {
        
    }
}
