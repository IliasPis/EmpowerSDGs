using UnityEngine;
using UnityEngine.UI;

public class QuizOutcomeLinkHandler : MonoBehaviour
{
    public enum QuizType
    {
        MultiChoice,
        DragDropQuiz,
        DragDropCartQuiz,
        TrueFalse
    }

    [Header("Quiz Configuration")]
    public QuizType selectedQuizType;
    public GameObject selectedQuizObject;

    [Header("Button Configuration")]
    public Button linkButton;

    private string outcomeLink;

    void Start()
    {
        if (linkButton != null)
        {
            linkButton.onClick.AddListener(OpenOutcomeLink);
        }
    }

    void Update()
    {
        UpdateOutcomeLink();
    }

    private void UpdateOutcomeLink()
    {
        outcomeLink = null; // Reset outcomeLink to avoid stale data

        if (selectedQuizObject == null)
        {
            Debug.LogWarning("No quiz object assigned.");
            return;
        }

        switch (selectedQuizType)
        {
            case QuizType.MultiChoice:
                MultChoices multiChoiceScript = selectedQuizObject.GetComponent<MultChoices>();
                if (multiChoiceScript != null)
                {
                    outcomeLink = GetOutcomeLink(multiChoiceScript, multiChoiceScript.ScoreText.text == "Correct");
                }
                break;

            case QuizType.DragDropQuiz:
                DragDropQuiz dragDropQuizScript = selectedQuizObject.GetComponent<DragDropQuiz>();
                if (dragDropQuizScript != null)
                {
                    bool isCorrect = ScoreManager.Instance.GetScore() > 0; // Assuming positive score indicates success
                    outcomeLink = GetOutcomeLink(dragDropQuizScript, isCorrect);
                }
                break;

            case QuizType.DragDropCartQuiz:
                DragDropCartQuiz dragDropCartQuizScript = selectedQuizObject.GetComponent<DragDropCartQuiz>();
                if (dragDropCartQuizScript != null)
                {
                    bool isCorrect = ScoreManager.Instance.GetScore() >= 3; // Assuming correct threshold for sustainable items
                    outcomeLink = GetOutcomeLink(dragDropCartQuizScript, isCorrect);
                }
                break;

            case QuizType.TrueFalse:
                TrueFalseScript trueFalseScript = selectedQuizObject.GetComponent<TrueFalseScript>();
                if (trueFalseScript != null)
                {
                    outcomeLink = GetOutcomeLink(trueFalseScript, trueFalseScript.ScoreText.text == "Correct");
                }
                break;

            default:
                Debug.LogWarning("Invalid quiz type selected.");
                break;
        }
    }

    private string GetOutcomeLink<T>(T script, bool isCorrect) where T : MonoBehaviour
    {
        if (script == null) return null;

        var rightLinkField = script.GetType().GetField("rightOutcomeLink");
        var wrongLinkField = script.GetType().GetField("wrongOutcomeLink");

        if (rightLinkField != null && wrongLinkField != null)
        {
            return isCorrect ? (string)rightLinkField.GetValue(script) : (string)wrongLinkField.GetValue(script);
        }

        Debug.LogWarning("Outcome link fields not found.");
        return null;
    }

    private void OpenOutcomeLink()
    {
        if (!string.IsNullOrEmpty(outcomeLink))
        {
            Application.OpenURL(outcomeLink);
        }
        else
        {
            Debug.LogWarning("Outcome link is empty or not assigned.");
        }
    }
}
