using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleVisualController : MonoBehaviour
{
    [Header("Toggles")]
    [SerializeField] private Toggle[] toggles;

    [Header("Sprites")]
    [SerializeField] private Sprite normalBackground;
    [SerializeField] private Sprite selectedBackground;

    [Header("Text Colors")]
    [SerializeField] private Color normalTextColor = Color.green;
    [SerializeField] private Color selectedTextColor = Color.magenta;


    private void Start()
    {
        foreach (var toggle in toggles)
        {
            toggle.onValueChanged.AddListener(_ => Refresh());
        }

        Refresh();
    }

    private void Refresh()
    {
        foreach (var toggle in toggles)
        {
            bool isOn = toggle.isOn;

            Image bg = toggle.targetGraphic as Image;
            if (bg != null)
                bg.sprite = isOn ? selectedBackground : normalBackground;

            Text label = toggle.GetComponentInChildren<Text>();
            if (label != null)
                label.color = isOn ? selectedTextColor : normalTextColor;

            TMP_Text savePerDay = toggle.GetComponentInChildren<TMP_Text>();
            if(savePerDay != null)
                savePerDay.color = isOn ? selectedTextColor : normalTextColor;
        }
    }
}
