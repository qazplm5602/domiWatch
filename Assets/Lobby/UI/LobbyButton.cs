using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LobbyButton : MonoBehaviour
{
    bool Lock = false;
    Sequence sequence;
    new RectTransform transform;
    private void Awake() {
        sequence = DOTween.Sequence();
        transform = GetComponent<RectTransform>();
    }
    public void Hover() {
        if (Lock) return;
        sequence.Append(transform.DOScale(1.1f, 0.1f));
        sequence.Kill(true);
    }

    public void UnHover() {
        if (Lock) return;
        sequence.Append(transform.DOScale(1, 0.1f));
        sequence.Kill(true);
    }

    public void SetLock() {
        UnHover();
        Lock = true;
    }

    public void UnLock() => Lock = false;
}
