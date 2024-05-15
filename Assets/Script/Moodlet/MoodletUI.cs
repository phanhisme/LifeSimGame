using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoodletUI : MonoBehaviour
{
    public GameObject panel;
    public Moodlet thisMoodlet;

    public TextMeshProUGUI nameOfMoodlet;
    public TextMeshProUGUI moodletDes;
    public TextMeshProUGUI moodletTimer;

    public void Start()
    {
        panel.SetActive(false);

        nameOfMoodlet.text = thisMoodlet.moodletName;
        moodletDes.text = thisMoodlet.reason;
    }

    public void Update()
    {
        moodletTimer.text = thisMoodlet.secondsDuration.ToString();
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
