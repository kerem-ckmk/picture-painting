using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LevelData", menuName = "Scriptable Objects/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public int ID;
    public Level LevelPrefab;
}