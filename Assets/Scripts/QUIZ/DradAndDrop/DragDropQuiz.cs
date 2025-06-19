using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;
using I2.Loc;

public class DragDropQuiz : MonoBehaviour, IPointerClickHandler
{
    public GameObject[] actions;

    public GameObject QuestionToClose;
    public Button[] correctDropZones;
    private Vector3[] originalPositions;
    private bool[] isDropZoneOccupied;

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

    public Button tryAgainButton;
    public Button continueButton;

    public Button LinkButton;

    public GameObject quizTitle;
    public GameObject question;

    public GameObject actionTitle;
    public GameObject impactTitle;

    public float waitTime = 3f;

    void Start()
    {
        originalPositions = new Vector3[actions.Length];
        isDropZoneOccupied = new bool[correctDropZones.Length];

        for (int i = 0; i < actions.Length; i++)
        {
            originalPositions[i] = actions[i].transform.position;
        }

        UpdateScore();
        outcomeText.gameObject.SetActive(false);
        tryAgainButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        LinkButton.gameObject.SetActive(false);
    }

    public void OnItemDropped(GameObject draggedItem)
    {
        int actionIndex = System.Array.IndexOf(actions, draggedItem);
        if (actionIndex < 0 || actionIndex >= actions.Length)
        {
            return;
        }

        bool droppedCorrectly = false;
        Button correctDropZone = null;

        for (int i = 0; i < correctDropZones.Length; i++)
        {
            Button dropZone = correctDropZones[i];
            RectTransform dropZoneRect = dropZone.GetComponent<RectTransform>();
            RectTransform draggedItemRect = draggedItem.GetComponent<RectTransform>();

            if (IsRectOverlapping(dropZoneRect, draggedItemRect))
            {
                if (isDropZoneOccupied[i])
                {
                    draggedItem.transform.position = originalPositions[actionIndex];
                    return;
                }

                isDropZoneOccupied[i] = true;

                if (i == actionIndex)
                {
                    draggedItem.transform.position = dropZone.transform.position;
                    correctDropZone = dropZone;
                    droppedCorrectly = true;
                    SetButtonColor(correctDropZone, Color.green);
                }

                draggedItem.SetActive(false);
                break;
            }
        }

        if (droppedCorrectly)
        {
            ScoreManager.Instance.AddScore(1);
        }

        CheckCompletion();
    }

    private bool IsRectOverlapping(RectTransform rect1, RectTransform rect2)
    {
        Vector3[] corners1 = new Vector3[4];
        Vector3[] corners2 = new Vector3[4];

        rect1.GetWorldCorners(corners1);
        rect2.GetWorldCorners(corners2);

        Rect rect1Bounds = new Rect(corners1[0].x, corners1[0].y, corners1[2].x - corners1[0].x, corners1[2].y - corners1[0].y);
        Rect rect2Bounds = new Rect(corners2[0].x, corners2[0].y, corners2[2].x - corners2[0].x, corners2[2].y - corners2[0].y);

        return rect1Bounds.Overlaps(rect2Bounds);
    }

    private void SetButtonColor(Button button, Color color)
    {
        if (button == null) return;

        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = color;
        colorBlock.highlightedColor = color;
        colorBlock.pressedColor = color;
        button.colors = colorBlock;
    }

    void CheckCompletion()
    {
        bool allDropped = true;
        foreach (GameObject action in actions)
        {
            if (action.activeSelf)
            {
                allDropped = false;
                break;
            }
        }

        if (allDropped)
        {
            StartCoroutine(WaitAndCompleteQuiz());
        }

        UpdateScore();
    }

    IEnumerator WaitAndCompleteQuiz()
    {
        yield return new WaitForSeconds(waitTime);

        quizTitle.SetActive(false);
        question.SetActive(false);
        actionTitle.SetActive(false);
        impactTitle.SetActive(false);

        foreach (GameObject action in actions)
        {
            action.SetActive(false);
        }

        foreach (Button dropZone in correctDropZones)
        {
            dropZone.gameObject.SetActive(false);
        }

        DisplayOutcome();
    }

    void DisplayOutcome()
    {
        outcomeText.gameObject.SetActive(true);
        ShowOutcome(ScoreManager.Instance.GetScore() > 0);

        // Make text appear as normal text without link styling
        outcomeText.fontStyle = FontStyles.Normal;
        outcomeText.color = Color.white;

        tryAgainButton.gameObject.SetActive(ScoreManager.Instance.GetScore() >= 2);
        continueButton.gameObject.SetActive(true);
        LinkButton.gameObject.SetActive(true);
        QuestionToClose.SetActive(false);
    }

    // Implement IPointerClickHandler to detect clicks on outcomeText
    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if outcomeText was clicked
        if (eventData.pointerEnter == outcomeText.gameObject)
        {
            OpenOutcomeLink(ScoreManager.Instance.GetScore() > 0);
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
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].transform.position = originalPositions[i];
            actions[i].SetActive(true);
            SetButtonColor(correctDropZones[i], Color.white);
        }

        for (int i = 0; i < isDropZoneOccupied.Length; i++)
        {
            isDropZoneOccupied[i] = false;
        }

        actionTitle.SetActive(true);
        impactTitle.SetActive(true);
        quizTitle.SetActive(true);
        question.SetActive(true);
        QuestionToClose.SetActive(true);
        LinkButton.gameObject.SetActive(false);

        foreach (Button dropZone in correctDropZones)
        {
            dropZone.gameObject.SetActive(true);
        }

        ScoreManager.Instance.SubtractScore(2);
        UpdateScore();
    }

    void UpdateScore()
    {
        ScoreManager.Instance.UpdateScoreText();
    }

    public void ContinueQuiz()
    {
        quizTitle.SetActive(false);
        question.SetActive(false);
        outcomeText.gameObject.SetActive(false);
        tryAgainButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        LinkButton.gameObject.SetActive(false);
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