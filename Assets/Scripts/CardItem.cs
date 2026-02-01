using UnityEngine;
using UnityEngine.UI;

public class CardItem : MonoBehaviour
{
    public Image cardImage;
    public GameObject premiumBadge;

    [HideInInspector] public int index = -1;
    int pendingRequestId = -1;

    public void Init(int i)
    {
        index = i;
        UpdatePremiumBadge();
    }

    void UpdatePremiumBadge()
    {
        bool isPremium = ((index + 1) % 4) == 0;
        if (premiumBadge != null)
            premiumBadge.SetActive(isPremium);
    }


    public void RequestImage(bool highPriority = false)
    {
        Sprite cached = ImageLoader.Instance.GetFromCache(index);
        if (cached != null)
        {
            ApplySprite(cached);
            return;
        }

        pendingRequestId = ImageLoader.Instance.RequestImage(index, OnImageLoaded, highPriority);
    }

    public void CancelPending()
    {
        if (pendingRequestId != -1)
        {
            ImageLoader.Instance.CancelRequest(index, pendingRequestId);
            pendingRequestId = -1;
        }
    }

    void OnImageLoaded(int idx, Sprite sprite, int requestId)
    {
        if (requestId != 0 && requestId != pendingRequestId)
            return;

        pendingRequestId = -1;

        if (sprite != null)
            ApplySprite(sprite);
    }

    void ApplySprite(Sprite sp)
    {
        if (cardImage != null)
        {
            cardImage.sprite = sp;
            cardImage.enabled = true;
        }
    }
}
