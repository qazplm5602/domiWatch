using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// content size fitter이 이상해서 그냥 직접 만듬 ㅅㄱ
public class ScoreboardAutioSize : MonoBehaviour
{
    [SerializeField] RectTransform thead;
    [SerializeField] RectTransform Playerlist;

    RectTransform ThisTransform;

    private void Awake() {
        ThisTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        ThisTransform.sizeDelta = new(ThisTransform.sizeDelta.x, thead.rect.height + Playerlist.rect.height);
    }
}
