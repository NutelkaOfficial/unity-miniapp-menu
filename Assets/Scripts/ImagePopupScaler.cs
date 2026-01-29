using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ImagePopupScaler : MonoBehaviour
{
    private RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (rt != null)
        {
            float screenWidth = rt.parent.GetComponent<RectTransform>().rect.width;
            rt.sizeDelta = new Vector2(screenWidth, screenWidth);
        }
    }
}
