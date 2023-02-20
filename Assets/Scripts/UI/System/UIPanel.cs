using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanel : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup == null)
                _canvasGroup = this.GetComponent<CanvasGroup>();

            return _canvasGroup;
        }
    }

    public bool IsInitialized { get; private set; }
    public bool IsShown { get; private set; }

    protected GameManager GameManager { get; private set; }

    public event Action OnPanelShown;
    public event Action OnPanelHidden;

    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;

        IsInitialized = true;
        OnInitialize();
    }

    public void HidePanel()
    {
        IsShown = false;
        OnHidePanel();
        OnPanelHidden?.Invoke();
    }

    public void ShowPanel()
    {
        IsShown = true;
        OnShowPanel();
        OnPanelShown?.Invoke();
    }

    protected virtual void OnInitialize()
    {

    }

    protected virtual void OnHidePanel()
    {
        CanvasGroup.alpha = 0f;
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
    }

    protected virtual void OnShowPanel()
    {
        CanvasGroup.alpha = 1f;
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;
    }
}
