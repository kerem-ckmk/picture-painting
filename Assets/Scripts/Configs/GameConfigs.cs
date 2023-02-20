using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameConfigs", menuName = "Scriptable Objects/Game Configs", order = 1)]
public class GameConfigs : ScriptableObject
{
    public static GameConfigs Instance;

    [Header("Level")]
    public int LevelSkipCountAtRepeat = 0;

    [Header("Haptic")]
    public float HapticIntervalLimit = 0.15f;
    public void Initialize()
    {
        Debug.Assert(Instance == null);

        Instance = this;
    }
}
