using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlockMove : MonoBehaviour
{
    RectTransform _trnasform;
    RawImage _image;
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

    void Fade() {
        _image.DOFade(Random.Range(0.1f, 0.8f), Random.Range(3, 8)).OnComplete(Fade);
    }

    void SizeSmall() {
        float Small = Random.Range(0.2f, _trnasform.localScale.x);
        _trnasform.DOScale(new Vector2(Small,Small), Random.Range(1, 5)).OnComplete(SizeBig);
    }

    void SizeBig() {
        float Biiig = Random.Range(_trnasform.localScale.x, 2f);
        _trnasform.DOScale(new Vector2(Biiig,Biiig), Random.Range(1, 5)).OnComplete(SizeSmall);
    }

    void Move() {
        _trnasform.DOAnchorPosX(1070, Random.Range(5, 30)).SetEase(Ease.OutQuad).OnComplete(MoveBack);
    }
    void MoveBack() {
        _trnasform.DOAnchorPosX(-1550, Random.Range(5, 30)).SetEase(Ease.InQuad).OnComplete(Move);
    }
}
