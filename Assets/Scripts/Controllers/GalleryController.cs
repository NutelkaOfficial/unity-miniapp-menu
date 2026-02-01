using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class GalleryController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public GridLayoutGroup grid;
    public GameObject cardPrefab;
    public Transform content;
    public int totalItems = 66;
    public int bufferRows = 2;
    public PopupController popupController;


    List<CardItem> cards = new List<CardItem>();
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    void Start()
    {
        if (scrollRect == null) Debug.LogError("ScrollRect not assigned");
        if (grid == null) Debug.LogError("GridLayoutGroup not assigned");
        if (cardPrefab == null) Debug.LogError("Card prefab not assigned");
        if (content == null) content = scrollRect.content;

        CreateCards();
        scrollRect.onValueChanged.AddListener(OnScrollChanged);
        UpdateVisibleRange();
    }

    void OnDestroy()
    {
        if (scrollRect != null) scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
    }

    void CreateCards()
    {
        for (int i = 0; i < totalItems; i++)
        {
            var go = Instantiate(cardPrefab, content);
            var ci = go.GetComponent<CardItem>();
            ci.Init(i);

            var btn = go.GetComponent<Button>();
            bool isPremium = ((i + 1) % 4) == 0;

            btn.onClick.AddListener(() =>
            {
                if (isPremium)
                {
                    popupController.OpenPremium();
                }
                else
                {
                    if (ci.cardImage != null && ci.cardImage.sprite != null)
                        popupController.OpenNormal(ci.cardImage.sprite);
                }
            });

            cards.Add(ci);

        }
    }

    void OnScrollChanged(Vector2 v)
    {
        UpdateVisibleRange();
    }

    void UpdateVisibleRange()
    {
        int columns = Mathf.Max(1, GetColumns());
        RectTransform contentRT = content.GetComponent<RectTransform>();
        RectTransform viewportRT = scrollRect.viewport;

        float scrollOffset = contentRT.anchoredPosition.y;
        if (contentRT.pivot.y < 0.5f) scrollOffset = -scrollOffset;

        float cellH = grid.cellSize.y;
        float spacingY = grid.spacing.y;
        float paddingTop = grid.padding.top;
        float rowHeight = cellH + spacingY;
        int totalRows = Mathf.CeilToInt((float)totalItems / columns);

        int firstVisibleRow = Mathf.FloorToInt((scrollOffset - paddingTop) / rowHeight);
        firstVisibleRow = Mathf.Clamp(firstVisibleRow, 0, totalRows - 1);

        float viewportHeight = viewportRT.rect.height;
        int visibleRows = Mathf.CeilToInt(viewportHeight / rowHeight);

        int startRow = Mathf.Max(0, firstVisibleRow - bufferRows);
        int endRow = Mathf.Min(totalRows - 1, firstVisibleRow + visibleRows - 1 + bufferRows);

        int startIndex = startRow * columns;
        int endIndex = Mathf.Min(totalItems - 1, (endRow + 1) * columns - 1);

        HashSet<int> want = new HashSet<int>();
        for (int i = startIndex; i <= endIndex; i++) want.Add(i);

        for (int i = startIndex; i <= endIndex && i < totalItems; i++)
        {
            int row = i / columns;
            bool isVisibleRow = row >= firstVisibleRow && row < firstVisibleRow + visibleRows;
            bool highPriority = isVisibleRow;
            cards[i].RequestImage(highPriority);
        }

        for (int i = 0; i < cards.Count; i++)
        {
            if (!want.Contains(i))
            {
                cards[i].CancelPending();
            }
        }
    }

    int GetColumns()
    {
        if (grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
            return Mathf.Max(1, grid.constraintCount);

        RectTransform contentRT = content.GetComponent<RectTransform>();
        float contentWidth = contentRT.rect.width - grid.padding.left - grid.padding.right;
        int cols = Mathf.FloorToInt((contentWidth + grid.spacing.x) / (grid.cellSize.x + grid.spacing.x));
        return Mathf.Max(1, cols);
    }
}
