using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LobbyEnterGame : MonoBehaviour
{
    [SerializeField] GameObject loadUI;
    RectTransform loadUI_Trnasform;
    
    private void Awake() {
        loadUI_Trnasform = loadUI.GetComponent<RectTransform>();
        NetworkCore.EventListener["Room.ClientInit"] = (LitJson.JsonData _) => {
            SceneManager.LoadScene(1);
        };
    }

    private void OnDestroy() {
        NetworkCore.EventListener.Remove("Room.ClientInit");
    }

    public void GoGoPlay() {
        loadUI.SetActive(true);
        loadUI_Trnasform.DOAnchorPosY(0, .3f).SetEase(Ease.OutQuad);
        loadUI.GetComponent<CanvasGroup>().DOFade(1, .3f);

            // SceneManager.LoadScene(1);

        NetworkCore.Send("Room.Join", null);
    }
}
