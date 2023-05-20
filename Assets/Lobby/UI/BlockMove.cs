using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlockMove : MonoBehaviour
{
    RectTransform _trnasform;
    RawImage _image;
    Sequence[] sequence = new Sequence[3];

    private void Awake() {
        _trnasform = GetComponent<RectTransform>();
        _image = GetComponent<RawImage>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Move();
        SizeBig();
        Fade();
    }

    private void OnDestroy() {
        foreach (var _sequence in sequence)
            _sequence?.Kill();
    }

    void Fade() {
        sequence[0] = DOTween.Sequence();
        sequence[0].Append(_image.DOFade(Random.Range(0.1f, 0.8f), Random.Range(3, 8)).OnComplete(Fade));
    }

    void SizeSmall() {
        sequence[1] = DOTween.Sequence();
        float Small = Random.Range(0.2f, _trnasform.localScale.x);
        sequence[1].Append(_trnasform.DOScale(new Vector2(Small,Small), Random.Range(1, 5)).OnComplete(SizeBig));
    }

    void SizeBig() {
        sequence[1] = DOTween.Sequence();
        float Biiig = Random.Range(_trnasform.localScale.x, 2f);
        sequence[1].Append(_trnasform.DOScale(new Vector2(Biiig,Biiig), Random.Range(1, 5)).OnComplete(SizeSmall));
    }

    void Move() {
        sequence[2] = DOTween.Sequence();
        sequence[2].Append(_trnasform.DOAnchorPosX(1070, Random.Range(5, 30)).SetEase(Ease.OutQuad).OnComplete(MoveBack));
    }
    void MoveBack() {
        sequence[2] = DOTween.Sequence();
        sequence[2].Append(_trnasform.DOAnchorPosX(-1550, Random.Range(5, 30)).SetEase(Ease.InQuad).OnComplete(Move));
    }
}
