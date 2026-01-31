using UnityEngine;
using UnityEngine.UI;

public class CardItemView : MonoBehaviour
{
    public GameObject popupPanel;
    public GameObject premiumPopup;
    public Image imagePopup;
    public Button closeButton;

    public GameObject cardPrefab;
    public Transform content;
    public Sprite[] sprites;

    void Start()
    {
        popupPanel.SetActive(false);
        premiumPopup.SetActive(false);

        closeButton.onClick.AddListener(ClosePopup);
        CreateCards();
    }

    void CreateCards()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            GameObject card = Instantiate(cardPrefab, content);

            Image img = card.transform.Find("CardImage")?.GetComponent<Image>();
            if (img == null) continue;

            img.sprite = sprites[i];

            bool isPremium = (i + 1) % 4 == 0;
            Transform badge = card.transform.Find("PremiumBadge");
            if (badge != null)
                badge.gameObject.SetActive(isPremium);

            card.GetComponent<Button>()?.onClick
                .AddListener(() => OnCardClicked(img.sprite, isPremium));
        }
    }

    void OnCardClicked(Sprite sprite, bool isPremium)
    {
        popupPanel.SetActive(true);
        premiumPopup.SetActive(isPremium);

        if (!isPremium)
            imagePopup.sprite = sprite;
    }

    void ClosePopup()
    {
        popupPanel.SetActive(false);
        premiumPopup.SetActive(false);
    }
}