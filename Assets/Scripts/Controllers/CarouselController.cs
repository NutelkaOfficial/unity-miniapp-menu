using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CarouselController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image bannerImage;
    public Sprite[] bannerSprites;

    [Header("Settings")]
    public float switchTime = 5f;
    public float swipeThreshold = 50f;

    private int currentIndex = 0;
    private float timer = 0f;

    private bool dragging = false;
    private Vector2 dragStart;
    private float dragDelta;

    void Start()
    {
        ShowBanner(currentIndex);
    }

    void Update()
    {
        if (!dragging)
        {
            timer += Time.deltaTime;
            if (timer >= switchTime)
            {
                timer = 0f;
                Next();
            }
        }
    }

    void ShowBanner(int index)
    {
        if (bannerSprites.Length == 0 || bannerImage == null)
            return;

        currentIndex = index;

        bannerImage.sprite = bannerSprites[index];
        bannerImage.SetNativeSize();
        RectTransform rt = bannerImage.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
    }

    public void Next()
    {
        ShowBanner((currentIndex + 1) % bannerSprites.Length);
    }

    public void Prev()
    {
        ShowBanner((currentIndex - 1 + bannerSprites.Length) % bannerSprites.Length);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        dragStart = eventData.position;
        dragDelta = 0f;
        timer = 0f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragDelta = eventData.position.x - dragStart.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;

        if (Mathf.Abs(dragDelta) < swipeThreshold)
            return;

        if (dragDelta < 0)
            Next();
        else
            Prev();
    }
}
