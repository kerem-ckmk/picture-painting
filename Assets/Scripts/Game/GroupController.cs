using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GroupController : MonoBehaviour
{
    [Header("References")]
    public Color groupColor;
    public int GroupIndex { get; private set; } = -1;
    public List<PartController> PartControllerList { get; private set; }
    public bool CompletedGroup { get; private set; }
    public bool IsInitialized { get; private set; }

    private TextMeshPro _numberTMPPrefab;

    public event Action PaintedGroup;
    public void Initialize(int index, TextMeshPro numberTMPPrefab)
    {
        GroupIndex = index;
        _numberTMPPrefab = numberTMPPrefab;
        PartControllerList = new List<PartController>();
        PartControllerList.Clear();

        PartControllerCheck();

        IsInitialized = true;
    }

    public void PartControllerCheck()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var partControllerObject = transform.GetChild(i).GetComponent<PartController>();
            partControllerObject.Paint += PartControllerObject_Paint;
            partControllerObject.Initialize(GroupIndex, groupColor, _numberTMPPrefab);

            PartControllerList.Add(partControllerObject);
        }
    }

    public void ShowParts(bool show)
    {
        foreach (var partController in PartControllerList)
            partController.SetAlpha(show);
    }

    private void PartControllerObject_Paint()
    {
        if (PartsPainted())
        {
            CompletedGroup = true;
            PaintedGroup?.Invoke();
        }       
    }

    private bool PartsPainted()
    {
        foreach (var partController in PartControllerList)
            if (!partController.Painted)
                return false;

        return true;
    }

}
