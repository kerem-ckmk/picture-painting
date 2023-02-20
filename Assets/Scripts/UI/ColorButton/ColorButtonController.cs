using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorButtonController : MonoBehaviour
{
    [Header("References")]
    public Button colorButton;
    public TextMeshProUGUI numberTMP;
    public Image fillImage;
    public bool IsInitialized { get; private set; }
    public int ColorButtonIndex { get; private set; }

    private float _addFillValue;
    private int _totalNumberParts;
    private Sequence _buttonScaleSequence;
    private Sequence _fillSequence;
    private Sequence _destroySequence;
    private float _fillValue;

    public event Action<ColorButtonController> ClickColorButton;
    public event Action NextButton;

    public void Initialize(int colorNumber, Color buttonColor, int totalNumberOfParts)
    {
        ColorButtonIndex = colorNumber;
        _totalNumberParts = totalNumberOfParts;
        _addFillValue = 1f / _totalNumberParts;

        numberTMP.text = (ColorButtonIndex + 1).ToString();
        colorButton.image.color = buttonColor;

        colorButton.onClick.AddListener(ClickButton);

        fillImage.fillAmount = 0f;

        IsInitialized = true;
    }

    private void OnDestroy()
    {
        _buttonScaleSequence?.Kill();
        _fillSequence?.Kill();
        _destroySequence?.Kill();

        ClickColorButton = null;
        NextButton = null;
    }

    public void ClickButton()
    {
        ClickColorButton?.Invoke(this);
    }

    public void UpdateFill()
    {
        _fillValue += _addFillValue;

        if (_fillValue >= 1)
            DestroyAnimation();

        float firstFillValue = fillImage.fillAmount;

        _fillSequence?.Kill();
        _fillSequence = DOTween.Sequence();
        _fillSequence.Append(DOTween.To(() => firstFillValue, x => firstFillValue = x, _fillValue, 0.15f).SetEase(Ease.Linear).OnUpdate(() =>
        {
            fillImage.fillAmount = firstFillValue;
        }));
        _fillSequence.Play();
    }

    public void AnimationButton(bool open)
    {
        float scale = open ? 1.2f : 1f;

        _buttonScaleSequence?.Kill();
        _buttonScaleSequence = DOTween.Sequence();
        _buttonScaleSequence.Append(transform.DOScale(scale, 0.2f).SetEase(Ease.OutBack));
        _buttonScaleSequence.Play();
    }

    public void DestroyAnimation()
    {
        _destroySequence?.Kill();
        _destroySequence = DOTween.Sequence();
        _destroySequence.Append(transform.DOScale(Vector2.one * 0.01f, 0.45f).SetEase(Ease.InBack));
        _destroySequence.AppendCallback(() =>
        {
            gameObject.SetActive(false);
            NextButton?.Invoke();
        });
        _destroySequence.Play();
    }


}
