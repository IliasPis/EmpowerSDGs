using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using I2.Loc;

public class TrueFalseScript : MonoBehaviour, IPointerClickHandler
{
    public GameObject TrueAnswerGameObject;
    public GameObject FalseAnswerGameObject;

    private Button TrueAnswerButton;
    private Button FalseAnswerButton;

    private bool buttonPressed = false;

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI outcomeText;
    public string rightOutcome;
    public string wrongOutcome;

    // Fields for Greek
    public string rightOutcomeGreek;
    public string wrongOutcomeGreek;

    // Fields for Serbian
    public string rightOutcomeSerbian;
    public string wrongOutcomeSerbian;

    // Fields for Turkish
    public string rightOutcomeTurkish;
    public string wrongOutcomeTurkish;

    // Fields for Spanish
    public string rightOutcomeSpanish;
    public string wrongOutcomeSpanish;

    // Fields for link URLs
    public string rightOutcomeLink;
    public string wrongOutcomeLink;

    public GameObject present;
    public GameObject question;

    public Button tryAgainButton;
    public Button continueButton;
    public Button LinkButton;
    public float waitTime = 3f;

    private bool isCorrect;

    void Start()
    {
        TrueAnswerButton = TrueAnswerGameObject.GetComponent<Button>();
        FalseAnswerButton = FalseAnswerGameObject.GetComponent<Button>();

        ResetButtonColors(TrueAnswerButton);
        ResetButtonColors(FalseAnswerButton);

        UpdateScore();

        outcomeText.gameObject.SetActive(false);
        tryAgainButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        LinkButton.gameObject.SetActive(false);
    }

    void Update()
    {
        CheckScoreLimits();
    }

    void SetButtonColor(Button button, Color color)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = color;
        colors.pressedColor = color;
        colors.selectedColor = color;
        button.colors = colors;
    }

    void ResetButtonColors(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        colors.pressedColor = Color.white;
        colors.selectedColor = Color.white;
        button.colors = colors;
    }

    void CheckScoreLimits()
    {
        if (ScoreManager.Instance.GetScore() < 0)
        {
            ScoreManager.Instance.ResetScore();
            UpdateScore();
        }
    }

    public void TrueChoice()
    {
        if (buttonPressed) return;

        SetButtonColor(TrueAnswerButton, Color.green);
        ScoreManager.Instance.AddScore(3);
        buttonPressed = true;
        isCorrect = true;
        UpdateScore();
        StartCoroutine(WaitAndDisableButtons());
    }

    public void FalseChoice(GameObject wrongAnswerGameObject)
    {
        if (buttonPressed) return;

        Button wrongAnswerButton = wrongAnswerGameObject.GetComponent<Button>();
        SetButtonColor(wrongAnswerButton, Color.red);
        buttonPressed = true;
        isCorrect = false;
        UpdateScore();
        StartCoroutine(WaitAndDisableButtons());
    }

    IEnumerator WaitAndDisableButtons()
    {
        yield return new WaitForSeconds(waitTime);

        TrueAnswerGameObject.SetActive(false);
        FalseAnswerGameObject.SetActive(false);
        question.SetActive(false);

        DisplayOutcome(isCorrect);
        continueButton.gameObject.SetActive(true);
        LinkButton.gameObject.SetActive(true);

        if (!isCorrect && ScoreManager.Instance.GetScore() >= 2)
        {
            tryAgainButton.gameObject.SetActive(true);
        }
    }

    void DisplayOutcome(bool isCorrect)
    {
        outcomeText.gameObject.SetActive(true);
        ShowOutcome(isCorrect);

        // Ensure text appears normal without link style
        outcomeText.fontStyle = FontStyles.Normal;
        outcomeText.color = Color.white; // Set to desired color for regular text
    }

    // Implement IPointerClickHandler to detect clicks on outcomeText
    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if the outcomeText was clicked
        if (eventData.pointerEnter == outcomeText.gameObject)
        {
            OpenOutcomeLink(isCorrect);
        }
    }

    void OpenOutcomeLink(bool isCorrect)
    {
        string linkToOpen = isCorrect ? rightOutcomeLink : wrongOutcomeLink;
        if (!string.IsNullOrEmpty(linkToOpen))
        {
            Application.OpenURL(linkToOpen);
        }
    }

    public void TryAgain()
    {
        buttonPressed = false;
        ScoreManager.Instance.SubtractScore(2);
        ResetButtonColors(TrueAnswerButton);
        ResetButtonColors(FalseAnswerButton);

        outcomeText.gameObject.SetActive(false);
        tryAgainButton.gameObject.SetActive(false);
        LinkButton.gameObject.SetActive(false);
        TrueAnswerGameObject.SetActive(true);
        FalseAnswerGameObject.SetActive(true);
        question.SetActive(true);

        UpdateScore();
    }

    private void UpdateScore()
    {
        ScoreText.text = ScoreManager.Instance.GetScore().ToString();
    }

    public void FinishClose()
    {
        TrueAnswerGameObject.SetActive(false);
        FalseAnswerGameObject.SetActive(false);
        question.SetActive(false);
        outcomeText.gameObject.SetActive(false);
        tryAgainButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        LinkButton.gameObject.SetActive(false);
        present.SetActive(false);
    }

    private void ShowOutcome(bool isCorrect)
    {
        string currentLanguage = LocalizationManager.CurrentLanguage;

        switch (currentLanguage)
        {
            case "English":
                outcomeText.text = isCorrect ? rightOutcome : wrongOutcome;
                break;
            case "Greek":
                outcomeText.text = isCorrect ? rightOutcomeGreek : wrongOutcomeGreek;
                break;
            case "Serbian":
                outcomeText.text = isCorrect ? rightOutcomeSerbian : wrongOutcomeSerbian;
                break;
            case "Turkish":
                outcomeText.text = isCorrect ? rightOutcomeTurkish : wrongOutcomeTurkish;
                break;
            case "Spanish":
                outcomeText.text = isCorrect ? rightOutcomeSpanish : wrongOutcomeSpanish;
                break;
            default:
                outcomeText.text = isCorrect ? rightOutcome : wrongOutcome;
                break;
        }

        outcomeText.gameObject.SetActive(true);
    }
}