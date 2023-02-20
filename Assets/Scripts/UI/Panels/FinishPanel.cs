using UnityEngine;
using UnityEngine.UI;

public class FinishPanel : UIPanel
{
    [Header("References - UI")]
    public Button continueButton;
    public Image chineseSun;
    private bool _isClosing = false;

    private void Awake()
    {
        continueButton.onClick.AddListener(ContinueButtonClicked);
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
    }

    protected override void OnShowPanel()
    {
        base.OnShowPanel();

        _isClosing = false;
    }

    protected override void OnHidePanel()
    {
        base.OnHidePanel();
    }

    private void ContinueButtonClicked()
    {
        if (_isClosing)
            return;

        _isClosing = true;

        GameManager.FullyFinishGameplay();
    }

    private void LateUpdate()
    {
        if (!IsShown)
            return;

        chineseSun.transform.rotation *= Quaternion.AngleAxis(-15f * Time.deltaTime, Vector3.forward);
    }
}
