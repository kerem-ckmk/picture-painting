using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Lofelt.NiceVibrations;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public GameplayController gameplayController;

    [Header("Config")]
    public GameConfigs gameConfigs;

    public GameState CurrentGameState { get; private set; }

    public bool IsVibrationEnabled
    {
        get { return PlayerPrefs.GetInt(PlayerPrefKeys.IsVibrationEnabled, 1) == 1; }
        set
        {
            if (value != IsVibrationEnabled)
            {
                PlayerPrefs.SetInt(PlayerPrefKeys.IsVibrationEnabled, value ? 1 : 0);
                VibrationSettingChanged(value);
            }
        }
    }
    public int LinearLevelIndex
    {
        get { return PlayerPrefs.GetInt(PlayerPrefKeys.LinearLevelIndex, 0); }
        set { PlayerPrefs.SetInt(PlayerPrefKeys.LinearLevelIndex, value); }
    }

    private float _lastHapticTime;

    public event Action<GameState /*Old*/, GameState /*New*/> OnGameStateChanged;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (GameConfigs.Instance == null)
        {
            gameConfigs = Instantiate(gameConfigs);
            gameConfigs.Initialize();
        }
        else
        {
            gameConfigs = GameConfigs.Instance;
        }

        gameplayController.OnGameplayFinished += GameplayController_OnGameplayFinished;
        DG.Tweening.DOTween.SetTweensCapacity(500, 500);

        VibrationSettingChanged(IsVibrationEnabled);

        ChangeCurrentGameState(GameState.Loading);
    }

    private void GameplayController_OnGameplayFinished()
    {
        ChangeCurrentGameState(GameState.Finish);
        DoHaptic(HapticPatterns.PresetType.Success, true);
    }

    public void FullyFinishGameplay()
    {
        LinearLevelIndex += 1;

        gameplayController.UnloadGameplay();
        PrepareGameplay();
    }

    private void VibrationSettingChanged(bool enabled)
    {
        HapticController.hapticsEnabled = enabled;
    }

    private void ChangeCurrentGameState(GameState newGameState)
    {
        var oldGameState = CurrentGameState;
        CurrentGameState = newGameState;
        OnGameStateChanged?.Invoke(oldGameState, CurrentGameState);
    }
    public void InitializeAfterLoading()
    {
#if UNITY_EDITOR
        Application.targetFrameRate = 9999;
#else
        Application.targetFrameRate = 60;
#endif

        gameplayController.Initialize();
    }

    public void ResetSaveData()
    {
        PlayerPrefs.DeleteAll();

        HardRestart();
    }

    public void PrepareGameplay()
    {
        gameplayController.PrepareGameplay(LinearLevelIndex);

        StartGameplay();
    }
    public void StartGameplay()
    {
        gameplayController.StartGameplay();

        ChangeCurrentGameState(GameState.Gameplay);
    }

    public void HardRestart()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public static void DoHaptic(HapticPatterns.PresetType hapticType, bool dominate = false)
    {
        if (Instance == null)
            return;

        if (dominate || Time.time - Instance._lastHapticTime >= Instance.gameConfigs.HapticIntervalLimit)
        {
            HapticPatterns.PlayPreset(hapticType);
            Instance._lastHapticTime = Time.time;
        }
    }
}

public enum GameState
{
    None,
    Loading,
    Gameplay,
    Finish
}
