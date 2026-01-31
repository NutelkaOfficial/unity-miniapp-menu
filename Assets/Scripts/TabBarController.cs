using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabBarController : MonoBehaviour
{
    public enum Tab { All, Odd, Even }

    [Header("UI Buttons")]
    public Button allButton;
    public Button oddButton;
    public Button evenButton;

    [Header("Underlines")]
    public GameObject allUnderline;
    public GameObject oddUnderline;
    public GameObject evenUnderline;

    [Header("Content")]
    public Transform content;

    [Header("Colors")]
    public Color activeTextColor = Color.green;
    public Color normalTextColor = Color.black;

    private Tab selectedTab = Tab.All;

    void Start()
    {
        allButton.onClick.AddListener(() => SelectTab(Tab.All));
        oddButton.onClick.AddListener(() => SelectTab(Tab.Odd));
        evenButton.onClick.AddListener(() => SelectTab(Tab.Even));

        SelectTab(Tab.All);
    }

    void SelectTab(Tab tab)
    {
        selectedTab = tab;

        for (int i = 0; i < content.childCount; i++)
        {
            Transform card = content.GetChild(i);

            bool isOdd = (i + 1) % 2 != 0;
            bool isEven = (i + 1) % 2 == 0;

            switch (selectedTab)
            {
                case Tab.All:
                    card.gameObject.SetActive(true);
                    break;
                case Tab.Odd:
                    card.gameObject.SetActive(isOdd);
                    break;
                case Tab.Even:
                    card.gameObject.SetActive(isEven);
                    break;
            }
        }

        UpdateTabVisuals();
    }

    void UpdateTabVisuals()
    {
        allButton.GetComponentInChildren<TMP_Text>().color = (selectedTab == Tab.All) ? activeTextColor : normalTextColor;
        oddButton.GetComponentInChildren<TMP_Text>().color = (selectedTab == Tab.Odd) ? activeTextColor : normalTextColor;
        evenButton.GetComponentInChildren<TMP_Text>().color = (selectedTab == Tab.Even) ? activeTextColor : normalTextColor;

        allUnderline.SetActive(selectedTab == Tab.All);
        oddUnderline.SetActive(selectedTab == Tab.Odd);
        evenUnderline.SetActive(selectedTab == Tab.Even);
    }
}
