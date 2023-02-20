using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class AnimatedTextWidget : MonoBehaviour
{
    [Header("Settings")]
    public float timePerCharacter = 0.1f;
    public float startDelay = 0f;
    public float endDelay = 0f;
    public bool loop = true;
    public bool startOnEnable = true;

    private TextMeshProUGUI textMesh;
    private string fullText;

    private int currentCharacterIndex;
    private float animationStartTime;

    public event Action OnAnimationFinished;

    private void Awake()
    {
        textMesh = this.GetComponent<TextMeshProUGUI>();
        fullText = textMesh.text;
        textMesh.text = "";
    }

    private void OnEnable()
    {
        if (startOnEnable)
            StartAnimation();
    }

    public void StartAnimation()
    {
        animationStartTime = Time.time;

        currentCharacterIndex = -1;
        textMesh.text = "";
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameState.Loading || animationStartTime < 0f)
            return;

        float timePassed = Time.time - animationStartTime;

        int characterIndex;
        if (timePassed < startDelay)
        {
            characterIndex = -1;
        }
        else if (timePassed > startDelay + endDelay + (fullText.Length + 1) * timePerCharacter)
        {
            characterIndex = fullText.Length;
        }
        else
        {
            float clampedTimePassed = timePassed - startDelay;
            characterIndex = Mathf.FloorToInt(clampedTimePassed / timePerCharacter) - 1;
            characterIndex = Mathf.Min(characterIndex, fullText.Length - 1);
        }

        if (characterIndex >= fullText.Length)
        {
            if (loop)
            {
                animationStartTime = Time.time;
                currentCharacterIndex = -1;
                textMesh.text = "";
            }
            else
            {
                animationStartTime = -1;
                currentCharacterIndex = -1;
            }

            OnAnimationFinished?.Invoke();
        }
        else if (characterIndex != currentCharacterIndex)
        {
            currentCharacterIndex = characterIndex;
            textMesh.text = fullText.Substring(0, currentCharacterIndex + 1);
        }
    }
}