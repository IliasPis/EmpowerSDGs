using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItemSust : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;

    void Start()
    {
        originalPosition = transform.position;
        canvasGroup = GetComponent<CanvasGroup>();

        // Add CanvasGroup if it's missing
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false; // Make the item draggable and not block raycast for drop detection
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; // Move the item with the cursor
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // Restore raycast blocking after the drag
        FindObjectOfType<DragDropCartQuiz>().OnItemDropped(gameObject); // Notify the main script that the item was dropped
    }

    public void ResetPosition()
    {
        transform.position = originalPosition; // Return the item to its original position if needed
    }
}
