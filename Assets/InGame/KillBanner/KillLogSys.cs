using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class KillLogSys : MonoBehaviour
{
    [SerializeField,Tooltip("죽인 사람이다")] TextMeshProUGUI AttackerUI; 
    [SerializeField, Tooltip("죽은 사람이다.")] TextMeshProUGUI DiePlayerUI; 
    new RectTransform transform;
    CanvasGroup canvasG;

    Sequence domiSequence;
    
    private void Awake() {
        canvasG = GetComponent<CanvasGroup>();
        transform = GetComponent<RectTransform>();
    }

    public void Init(string Attacker, string Dier) {
        AttackerUI.text = Attacker;
        DiePlayerUI.text = Dier;
        
        // 나타나는 효과
        domiSequence = DOTween.Sequence();
        domiSequence.Append(transform.DOSizeDelta(new Vector2(450, transform.sizeDelta.y), 0.3f).SetEase(Ease.OutQuad));
        domiSequence.Append(canvasG.DOFade(1, 0.3f).SetEase(Ease.OutQuad));

        // 선한쌤이 제일 싫어하시는 Invoke!!
        Invoke(nameof(RemoveKillLog), 8);
    }

    void RemoveKillLog() {
        domiSequence = DOTween.Sequence();
        domiSequence.Append(transform.DOSizeDelta(new Vector2(550, transform.sizeDelta.y), 0.3f).SetEase(Ease.OutQuad));
        domiSequence.Append(canvasG.DOFade(0, 0.3f).SetEase(Ease.OutQuad).OnComplete(() => Destroy(gameObject)));
    }
}