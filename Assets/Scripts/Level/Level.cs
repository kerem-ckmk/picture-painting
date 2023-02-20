using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level : MonoBehaviour
{
    public bool IsInitialized { get; private set; }

    public List<GroupController> Groups { get; private set; }
    public event Action CompleteLevel;

    public void Initialize(TextMeshPro numberTMPPrefab)
    {
        Groups = new List<GroupController>();
        Groups.Clear();

        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).GetComponent<GroupController>())
                Groups.Add(transform.GetChild(i).GetComponent<GroupController>());

        for (int i = 0; i < Groups.Count; i++)
        {
            Groups[i].Initialize(i, numberTMPPrefab);
            Groups[i].PaintedGroup += Level_PaintedGroup;
        }
        

        IsInitialized = true;
    }

    private void Level_PaintedGroup()
    {
        if (GroupsPainted())
        {
            CompleteLevel?.Invoke();
            GameManager.DoHaptic(Lofelt.NiceVibrations.HapticPatterns.PresetType.Success);
        }
          
    }
    
    public void ShowGroup(int index)
    {
        foreach (var group in Groups)
            group.ShowParts(false);

        Groups[index].ShowParts(true);
    }



    private bool GroupsPainted()
    {
        foreach (var group in Groups)
            if (!group.CompletedGroup)
                return false;

        return true;
    }

}
