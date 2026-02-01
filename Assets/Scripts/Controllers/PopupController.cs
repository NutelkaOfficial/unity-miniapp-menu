using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    [Header("Popups")]
    public GameObject popupPanel;
    public GameObject premiumPopup;
    public Image imagePopup;
    public Button closeButton;

    void Start()
    {
        popupPanel.SetActive(false);
        premiumPopup.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePopup);
    }

    public void OpenNormal(Sprite sprite)
    {
        popupPanel.SetActive(true);
        premiumPopup.SetActive(false);
        imagePopup.sprite = sprite;
    }

    public void OpenPremium()
    {
        popupPanel.SetActive(true);
        premiumPopup.SetActive(true);
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
        premiumPopup.SetActive(false);
    }
}
