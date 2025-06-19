using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using I2.Loc;

public class MultChoices : MonoBehaviour, IPointerClickHandler
{
    public GameObject RightAnswerGameObject;
    public GameObject WrongAnswerGameObject1;
    public GameObject WrongAnswerGameObject2;

    public GameObject QuestionToClose;

    private Button RightAnswerButton;
    private Button WrongAnswerButton1;
    private Button WrongAnswerButton2;

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

    // New fields for link URLs
    public string rightOutcomeLink;
    public string wrongOutcomeLink;

    public GameObject present;
    public GameObject next;
    public GameObject question;

    public bool haveNext;

    public Button nextButton;
    public Button tryAgainButton;
    public Button LinkButton;
    public float waitTime = 3f;

    private bool isCorrect;

    void Start()
    {
        RightAnswerButton = RightAnswerGameObject.GetComponent<Button>();
        WrongAnswerButton1 = WrongAnswerGameObject1.GetComponent<Button>();
        WrongAnswerButton2 = WrongAnswerGameObject2.GetComponent<Button>();

        ResetButtonColors(RightAnswerButton);
        ResetButtonColors(WrongAnswerButton1);
        ResetButtonColors(WrongAnswerButton2);

        UpdateScore();

        outcomeText.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        tryAgainButton.gameObject.SetActive(false);
        LinkButton.gameObject.SetActive(false);
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

    public void TrueChoice()
    {
        if (buttonPressed) return;

        SetButtonColor(RightAnswerButton, Color.green);
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
        // SetButtonColor(wrongAnswerButton, Color.red);
        buttonPressed = true;
        isCorrect = false;
        UpdateScore();
        StartCoroutine(WaitAndDisableButtons());
    }

    IEnumerator WaitAndDisableButtons()
    {
        yield return new WaitForSeconds(waitTime);

        RightAnswerGameObject.SetActive(false);
        WrongAnswerGameObject1.SetActive(false);
        WrongAnswerGameObject2.SetActive(false);
        question.SetActive(false);

        DisplayOutcome(isCorrect);
        nextButton.gameObject.SetActive(true);
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

        // Set the text to appear as normal without link color or underline
        outcomeText.fontStyle = FontStyles.Normal;
        outcomeText.color = Color.white;  // Set to desired color, such as black
        QuestionToClose.SetActive(false);
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

    public void GoToNext()
    {
        present.SetActive(false);
        if (haveNext && next != null)
        {
            next.SetActive(true);
        }
    }

    public void TryAgain()
    {
        buttonPressed = false;
        ScoreManager.Instance.SubtractScore(2);
        ResetButtonColors(RightAnswerButton);
        ResetButtonColors(WrongAnswerButton1);
        ResetButtonColors(WrongAnswerButton2);

        outcomeText.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        tryAgainButton.gameObject.SetActive(false);
        LinkButton.gameObject.SetActive(false);
        RightAnswerGameObject.SetActive(true);
        WrongAnswerGameObject1.SetActive(true);
        WrongAnswerGameObject2.SetActive(true);
        QuestionToClose.SetActive(true);

        question.SetActive(true);

        UpdateScore();
    }

    private void UpdateScore()
    {
        ScoreText.text = ScoreManager.Instance.GetScore().ToString();
    }

    public void SetScoreText(TextMeshProUGUI scoreText)
    {
        scoreText.text = ScoreManager.Instance.GetScore().ToString();
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