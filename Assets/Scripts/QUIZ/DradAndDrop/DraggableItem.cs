using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 startPosition;
    private CanvasGroup canvasGroup;
    private DragDropQuiz dragDropQuiz;

    void Awake()
    {
        // Try to get the CanvasGroup component (it's optional now)
        canvasGroup = GetComponent<CanvasGroup>();
        dragDropQuiz = FindObjectOfType<DragDropQuiz>(); // Find the DragDropQuiz in the scene
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position; // Store the original position

        // Check if canvasGroup exists before modifying it
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false; // Disable raycasting so the item can be dragged freely
        }
        
        Debug.Log("Dragging Started: " + gameObject.name);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; // Move the item with the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Re-enable raycasting after dragging ends (if canvasGroup exists)
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }

        // Inform the manager that this item has been dropped
        if (dragDropQuiz != null)
        {
            dragDropQuiz.OnItemDropped(gameObject);
        }

        Debug.Log("Dragging Ended: " + gameObject.name);
    }
}
