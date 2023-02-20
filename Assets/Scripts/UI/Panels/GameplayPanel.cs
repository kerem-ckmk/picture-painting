using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameplayPanel : UIPanel
{
    [Header("References")]
    public ColorButtonController colorButtonControllerPrefab;
    public Transform colorButtonParent;
    public List<ColorButtonController> ColorButtons { get; private set; }

    private GameplayController _gameplayController;
    protected override void OnInitialize()
    {
        base.OnInitialize();
        _gameplayController = GameManager.Instance.gameplayController;
        _gameplayController.ColoredPart += GameplayController_ColoredPart;
        ColorButtons = new List<ColorButtonController>();
    }

    protected override void OnShowPanel()
    {
        base.OnShowPanel();
        StartCoroutine(DelayedShowPanel());
    }
    protected override void OnHidePanel()
    {
        base.OnHidePanel();

        if (ColorButtons == null)
            return;

        foreach (var colorButton in ColorButtons)
            if (colorButton != null)
                Destroy(colorButton.gameObject);

        ColorButtons.Clear();
    }

    private IEnumerator DelayedShowPanel()
    {
        yield return null;

        HorizontalLayoutGroup colorGroup = colorButtonParent.GetComponent<HorizontalLayoutGroup>();
        RectTransform parentRect = colorButtonParent.GetComponent<RectTransform>();
        RectTransform prefabRect = colorButtonControllerPrefab.GetComponent<RectTransform>();

        float prefabWidth = prefabRect.sizeDelta.x;
        float spacing = colorGroup.spacing;
        float lastSpacing = colorGroup.padding.left;
        var groups = _gameplayController.levelManager.CurrentLevelInstance.Groups;
        int colorButtonControllerCount = groups.Count;
        float parentWidth = ((prefabWidth + spacing - 1) * colorButtonControllerCount) + lastSpacing;

        parentRect.sizeDelta = new Vector2(parentWidth, parentRect.sizeDelta.y);

        for (int i = 0; i < colorButtonControllerCount; i++)
        {
            var colorButtonObject = CreateColorButtonController();
            colorButtonObject.Initialize(i, groups[i].groupColor, groups[i].PartControllerList.Count);

            colorButtonObject.ClickColorButton += ColorButtonObject_ClickColorButton;
            colorButtonObject.NextButton += ColorButtonObject_NextButton;

            if (i == 0)
                ColorButtonObject_ClickColorButton(colorButtonObject);
        }
    }

    private void ColorButtonObject_NextButton()
    {
        for (int i = 0; i < ColorButtons.Count; i++)
        {
            if (ColorButtons[i].gameObject.activeSelf)
            {
                ColorButtonObject_ClickColorButton(ColorButtons[i]);
                break;
            }
        }
    }

    private void ColorButtonObject_ClickColorButton(ColorButtonController colorButtonObject)
    {
        foreach (var colorButton in ColorButtons)
            colorButton.AnimationButton(false);

        colorButtonObject.AnimationButton(true);
        int buttonIndex = colorButtonObject.ColorButtonIndex;

        _gameplayController.UpdateSelectedGroupIndex(buttonIndex);
        _gameplayController.levelManager.CurrentLevelInstance.ShowGroup(buttonIndex);
    }

    public ColorButtonController CreateColorButtonController()
    {
        var colorButtonControllerObject = Instantiate(colorButtonControllerPrefab, colorButtonParent);
        ColorButtons.Add(colorButtonControllerObject);

        return colorButtonControllerObject;
    }

    private void GameplayController_ColoredPart(int groupIndex)
    {
        if (ColorButtons.Count > 0)
            ColorButtons[groupIndex]?.UpdateFill();
    }
}
