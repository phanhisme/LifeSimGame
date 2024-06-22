using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoodletUI : MonoBehaviour
{
    public GameObject panel;
    public Moodlet thisMoodlet;

    public Image icon;
    public TextMeshProUGUI nameOfMoodlet;
    public TextMeshProUGUI moodletDes;
    public TextMeshProUGUI moodletTimer;

    public void Start()
    {
        panel.SetActive(false);
    }

    public void Update()
    {
        if (thisMoodlet != null)
        {
            float time = thisMoodlet.secondsDuration;
            time -= Time.deltaTime;
            moodletTimer.text = Mathf.CeilToInt(time).ToString();

            icon.sprite = thisMoodlet.moodletSprite;
            nameOfMoodlet.text = thisMoodlet.moodletName;
            moodletDes.text = thisMoodlet.reason;
        }
    }

    public void HoverOn()
    {
        panel.SetActive(true);
    }
    public void HoverOff()
    {
        panel.SetActive(false);
    }
}
