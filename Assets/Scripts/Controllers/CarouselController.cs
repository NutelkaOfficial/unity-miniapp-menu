using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarouselController : MonoBehaviour
{
    public Image[] banners;
    private int currentIndex = 0;
    private float timer = 0f;
    public float switchTime = 5f;

    void Start()
    {
        ShowBanner(currentIndex);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= switchTime)
        {
            timer = 0f;
            currentIndex = (currentIndex + 1) % banners.Length;
            ShowBanner(currentIndex);
        }
    }

    void ShowBanner(int index)
    {
        for (int i = 0; i < banners.Length; i++)
            banners[i].gameObject.SetActive(i == index);
    }
}
