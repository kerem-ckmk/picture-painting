using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class PartController : MonoBehaviour
{
    public Collider2D Collider { get; private set; }
    public int GroupIndex { get; private set; } = -1;
    public bool Painted { get; private set; }
    public bool IsInitialized { get; private set; }

    public TextMeshPro NumberTMP { get; private set; }

    private SpriteRenderer _spriteRenderer;
    private Color _color;
    private TextMeshPro _numberTMPPrefab;
    private TextMeshPro _activeTMP;
    public event Action Paint;

    public void Initialize(int index, Color color, TextMeshPro numberTMPPrefab)
    {
        Collider = GetComponent<PolygonCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        GroupIndex = index;
        _color = color;
        _numberTMPPrefab = numberTMPPrefab;

        _activeTMP = CreateTMP();
        _activeTMP.text = (GroupIndex + 1).ToString();

        IsInitialized = true;
    }

    public TextMeshPro CreateTMP()
    {
        //nonAlloc Test

        var numberTMP = Instantiate(_numberTMPPrefab, this.transform);
        Vector3 transformTMP = Collider.bounds.center;
        transformTMP.z = -0.05f;
        numberTMP.transform.position = transformTMP;

        float maxBoundSize = Mathf.Max(Collider.bounds.size.x, Collider.bounds.size.y);
        numberTMP.GetComponent<RectTransform>().sizeDelta = Vector2.one * maxBoundSize * 0.25f;

        return numberTMP;
    }

    public void SetColorPart()
    {
        _activeTMP.enabled = false;

        Painted = true;

        _spriteRenderer.color = _color;
        Paint?.Invoke();
    }

    public void SetAlpha(bool active)
    {
        if (Painted)
            return;

        Color spriteRendererColor = _spriteRenderer.color;
        spriteRendererColor.a = active ? 0 : 1;
        _spriteRenderer.color = spriteRendererColor;
    }

    private void OnDestroy()
    {
        Paint = null;
    }


}
