using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DeckBuild
{
    public class DraggableCard : MonoBehaviour , IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        [Header("Settings")]
        [Tooltip("Sensitivity for scroll vs drag detection.")]
        public float scrollThreshold = 0.5f;

        [Header("References (Auto-Found if null)")]
        public ScrollRect parentScrollRect;
        public Canvas mainCanvas;

        // Internal State
        private bool _isScrolling = false;
        private GameObject _ghostObject;
        private GameObject _ghostCharObject;
        private RectTransform _ghostRect;
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            if (parentScrollRect == null)
                parentScrollRect = GetComponentInParent<ScrollRect>();

            
            if (mainCanvas == null)
                mainCanvas = GetComponentInParent<Canvas>();
        }
        public void OnBeginDrag(PointerEventData eventData)
    {
        // Direction Check
        if (parentScrollRect != null && Mathf.Abs(eventData.delta.y) < Mathf.Abs(eventData.delta.x))
        {
            _isScrolling = true;
            parentScrollRect.OnBeginDrag(eventData);
        }
        else
        {
            _isScrolling = false;

            CreateGhostIcon();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isScrolling && parentScrollRect != null)
        {
            // Pass the drag data to the ScrollRect
            parentScrollRect.OnDrag(eventData);
        }
        else
        {
            // Move ghost to mouse position (converted to canvas space)
            MoveRectToMouse(_ghostRect, eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isScrolling && parentScrollRect != null)
        {
            parentScrollRect.OnEndDrag(eventData);
            _isScrolling = false;
        }
        else
        {
                Destroy(_ghostObject);
        }
    }

    // --- Helper Methods ---

    private void CreateGhostIcon()
    {
        // Create a new GameObject
        _ghostObject = new GameObject("GhostCard");
        _ghostCharObject = new GameObject("GhostChar");
        
        // Parent it to the canvas
        _ghostObject.transform.SetParent(mainCanvas.transform, false);
        _ghostObject.transform.SetAsLastSibling(); // Ensure it renders on top
        _ghostCharObject.transform.SetParent(_ghostObject.transform, false);

        // Copy Image component
        Image originalBgImage = GetComponent<Image>();
        Image originalCharImage = transform.Find("CharacterSprite").GetComponent<Image>();
        
        //MakeGhostBG
        Image ghostBgImage = _ghostObject.AddComponent<Image>();
        ghostBgImage.sprite = originalBgImage.sprite;
        ghostBgImage.color = new Color(1, 1, 1, 1f);
        ghostBgImage.raycastTarget = false;
        
        //MakeGhostChar
        Image ghostCharImage = _ghostCharObject.AddComponent<Image>();
        ghostCharImage.sprite = originalCharImage.sprite;
        ghostCharImage.color = new Color(1, 1, 1, 1f);
        ghostCharImage.raycastTarget = false;
        
        _ghostRect = _ghostObject.GetComponent<RectTransform>();
        _ghostRect.sizeDelta = _rectTransform.sizeDelta;
        _ghostRect.position = _rectTransform.position;
        RectTransform charRect = _ghostCharObject.GetComponent<RectTransform>();
        charRect.sizeDelta = _rectTransform.sizeDelta;
        charRect.position = _rectTransform.position;
    }

    private void MoveRectToMouse(RectTransform target, PointerEventData eventData)
    {
        Vector2 localCursor;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mainCanvas.transform as RectTransform, 
            eventData.position, 
            mainCanvas.worldCamera, 
            out localCursor))
        {
            target.position = mainCanvas.transform.TransformPoint(localCursor);
        }
    }
    }
}