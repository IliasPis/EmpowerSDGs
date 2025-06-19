using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using I2.Loc;

public class DragDropCartQuiz : MonoBehaviour, IPointerClickHandler
{
    public GameObject[] items;

    public GameObject QuestionToClose;
    public Button cart;
    public GameObject[] sustainableItems;

    private Vector3[] originalPositions;
    private bool[] isItemInCart;
    private int totalSustainableItems;
    private int droppedItems = 0;
    private int droppedSustainableItems = 0;

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI outcomeText;
    public string rightOutcome;
    public string wrongOutcome;

    // Fields for English
    public string rightOutcomeEnglish;
    public string wrongOutcomeEnglish;

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

    public Button tryAgainButton;
    public Button continueButton;
    public Button LinkButton;
    public GameObject quizTitle;
    public GameObject question;

    public float waitTime = 3f;

    void Start()
    {
        originalPositions = new Vector3[items.Length];
        isItemInCart = new bool[items.Length];
        totalSustainableItems = sustainableItems.Length;

        for (int i = 0; i < items.Length; i++)
        {
            originalPositions[i] = items[i].transform.position;
        }

        UpdateScore();

        outcomeText.gameObject.SetActive(false);
        tryAgainButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        LinkButton.gameObject.SetActive(false);
    }

    public void OnItemDropped(GameObject draggedItem)
    {
        int itemIndex = System.Array.IndexOf(items, draggedItem);
        if (itemIndex < 0 || itemIndex >= items.Length)
        {
            return;
        }

        RectTransform cartRect = cart.GetComponent<RectTransform>();
        RectTransform draggedItemRect = draggedItem.GetComponent<RectTransform>();

        if (IsRectOverlapping(cartRect, draggedItemRect))
        {
            if (isItemInCart[itemIndex])
            {
                draggedItem.transform.position = originalPositions[itemIndex];
                return;
            }

            isItemInCart[itemIndex] = true;
            draggedItem.transform.position = cart.transform.position;
            draggedItem.SetActive(false);
            droppedItems++;

            if (IsSustainable(draggedItem))
            {
                droppedSustainableItems++;
                StartCoroutine(HighlightCart(Color.green));
                ScoreManager.Instance.AddScore(1);
            }
            else
            {
                StartCoroutine(HighlightCart(Color.red));
            }

            CheckCompletion();
        }
    }

    private bool IsSustainable(GameObject item)
    {
        foreach (GameObject sustainableItem in sustainableItems)
        {
            if (item == sustainableItem)
            {
                return true;
            }
        }
        return false;
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

    private IEnumerator HighlightCart(Color color)
    {
        SetButtonColor(cart, color);
        yield return new WaitForSeconds(2);
        SetButtonColor(cart, Color.white);
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
        if (droppedItems >= totalSustainableItems)
        {
            StartCoroutine(WaitAndCompleteQuiz());
        }
    }

    IEnumerator WaitAndCompleteQuiz()
    {
        yield return new WaitForSeconds(waitTime);

        quizTitle.SetActive(false);
        question.SetActive(false);
        cart.gameObject.SetActive(false);

        foreach (GameObject item in items)
        {
            item.SetActive(false);
        }

        DisplayOutcome();
    }

    void DisplayOutcome()
    {
        outcomeText.gameObject.SetActive(true);

        string currentLanguage = LocalizationManager.CurrentLanguage;

        if (droppedSustainableItems == totalSustainableItems)
        {
            switch (currentLanguage)
            {
                case "English":
                    outcomeText.text = rightOutcomeEnglish;
                    break;
                case "Greek":
                    outcomeText.text = rightOutcomeGreek;
                    break;
                case "Serbian":
                    outcomeText.text = rightOutcomeSerbian;
                    break;
                case "Turkish":
                    outcomeText.text = rightOutcomeTurkish;
                    break;
                case "Spanish":
                    outcomeText.text = rightOutcomeSpanish;
                    break;
                default:
                    outcomeText.text = rightOutcomeEnglish;
                    break;
            }
            ScoreManager.Instance.AddScore(3);
            tryAgainButton.gameObject.SetActive(false);
        }
        else
        {
            switch (currentLanguage)
            {
                case "English":
                    outcomeText.text = wrongOutcomeEnglish;
                    break;
                case "Greek":
                    outcomeText.text = wrongOutcomeGreek;
                    break;
                case "Serbian":
                    outcomeText.text = wrongOutcomeSerbian;
                    break;
                case "Turkish":
                    outcomeText.text = wrongOutcomeTurkish;
                    break;
                case "Spanish":
                    outcomeText.text = wrongOutcomeSpanish;
                    break;
                default:
                    outcomeText.text = wrongOutcomeEnglish;
                    break;
            }
            tryAgainButton.gameObject.SetActive(ScoreManager.Instance.GetScore() >= 2);
        }

        // Ensure text appears normal without link style
        outcomeText.fontStyle = FontStyles.Normal;
        outcomeText.color = Color.white; // Set to desired color for regular text

        continueButton.gameObject.SetActive(true);
        LinkButton.gameObject.SetActive(true);
        QuestionToClose.SetActive(false);
    }

    // Implement IPointerClickHandler to detect clicks on outcomeText
    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if the outcomeText was clicked
        if (eventData.pointerEnter == outcomeText.gameObject)
        {
            OpenOutcomeLink(droppedSustainableItems == totalSustainableItems);
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
        for (int i = 0; i < items.Length; i++)
        {
            items[i].transform.position = originalPositions[i];
            items[i].SetActive(true);
            isItemInCart[i] = false;
        }

        droppedItems = 0;
        droppedSustainableItems = 0;
        outcomeText.gameObject.SetActive(false);
        tryAgainButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        LinkButton.gameObject.SetActive(false);
        quizTitle.SetActive(true);
        question.SetActive(true);
        cart.gameObject.SetActive(true);
        QuestionToClose.SetActive(true);

        ScoreManager.Instance.SubtractScore(2);
        UpdateScore();
    }

    private void UpdateScore()
    {
        ScoreText.text = ScoreManager.Instance.GetScore().ToString();
    }
}