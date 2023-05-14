using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LobbyButton : MonoBehaviour
{
    bool Lock = false;
    Sequence sequence;
    new RectTransform transform;
    Button _button;
    private void Awake() {
        sequence = DOTween.Sequence();
        transform = GetComponent<RectTransform>();
        _button = GetComponent<Button>();
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
        _button.interactable = false;
    }

    public void UnLock() {
        Lock = false;
        _button.interactable = true;
    }
}
