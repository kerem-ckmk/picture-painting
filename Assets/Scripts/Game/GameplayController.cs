using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [Header("References")]
    public CameraController gameCamera;
    public LevelManager levelManager;
    public bool IsInitialized { get; private set; }
    public bool IsActive { get; private set; }
    public int SelectedGroupIndex { get; private set; }

    private RaycastHit2D[] _hits;

    public event Action OnGameplayFinished;
    public event Action PrepareLevel;
    public event Action<int> ColoredPart;

    public void Initialize()
    {
        _hits = new RaycastHit2D[8];

        levelManager.Initialize();
        gameCamera.Initialize();

        IsInitialized = true;
    }

    public void PrepareGameplay(int linearLevelIndex)
    {
        levelManager.CreateLevel(linearLevelIndex);
        levelManager.CurrentLevelInstance.CompleteLevel += CurrentLevelInstance_CompleteLevel;

        gameCamera.SetTarget(levelManager.CurrentLevelInstance.transform);

        PrepareLevel?.Invoke();
    }

    public void UnloadGameplay()
    {
        levelManager.UnloadLevel();

        PrepareLevel = null;
    }

    public void StartGameplay()
    {
        IsActive = true;
    }

    private void FinishGameplay()
    {
        IsActive = false;

        OnGameplayFinished?.Invoke();
    }

    private void CurrentLevelInstance_CompleteLevel()
    {
        FinishGameplay();
    }

    public void UpdateSelectedGroupIndex(int index)
    {
        SelectedGroupIndex = index;
    }

    private void Update()
    {
        if (!IsActive)
            return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            FinishGameplay();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            int hitCount = Physics2D.GetRayIntersectionNonAlloc(inputRay, _hits);

            for (int i = 0; i < hitCount; i++)
            {
                if (_hits[i].collider.gameObject.GetComponent<PartController>())
                {
                    var partController = _hits[i].collider.gameObject.GetComponent<PartController>();
                    if (partController.GroupIndex == SelectedGroupIndex)
                    {
                        GameManager.DoHaptic(Lofelt.NiceVibrations.HapticPatterns.PresetType.MediumImpact);
                        partController.Collider.enabled = false;
                        partController.SetColorPart();
                        ColoredPart?.Invoke(partController.GroupIndex);
                    }
                }
            }

        }
    }
}
