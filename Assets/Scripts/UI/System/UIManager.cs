using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lofelt.NiceVibrations;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;

    [Header("References - Panels")]
    public LoadingPanel loadingPanel;
    public GameplayPanel gameplayPanel;
    public FinishPanel finishPanel;

    [Header("References - Common HUD")]
    public TextMeshProUGUI levelText;

    private List<UIPanel> allPanels = new List<UIPanel>();

    private void Awake()
    {
        allPanels.Add(loadingPanel);
        allPanels.Add(gameplayPanel);
        allPanels.Add(finishPanel);

        HideAllPanels(true);

        loadingPanel.OnLoadingFinished += LoadingPanel_OnLoadingFinished;
        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void LoadingPanel_OnLoadingFinished(bool extended)
    {
        if (!extended)
        {
            loadingPanel.ExtendLoading();
        }
        else if (extended)
        {
            gameManager.InitializeAfterLoading();

            foreach (var panel in allPanels)
            {
                panel.Initialize(gameManager);
            }

            gameManager.PrepareGameplay();
        }
    }

    private void GameManager_OnGameStateChanged(GameState oldGameState, GameState newGameState)
    {
        if (newGameState == GameState.Loading)
        {
            ShowPanel(loadingPanel);
        }
        else if (newGameState == GameState.Gameplay)
        {
            OnMenuState();
            ShowPanel(gameplayPanel);
        }
        else if (newGameState == GameState.Finish)
        {
            ShowPanel(finishPanel);
        }
        else
        {
            HideAllPanels();
        }
    }

    private void HideAllPanels(bool forceHide = false)
    {
        foreach (var panel in allPanels)
        {
            if (panel.IsShown || forceHide)
                panel.HidePanel();
        }
    }

    private void ShowPanel(UIPanel panel)
    {
        HideAllPanels();

        panel.ShowPanel();
    }

    private void OnMenuState()
    {
        levelText.text = $"Level {gameManager.LinearLevelIndex + 1}";
    }
}
