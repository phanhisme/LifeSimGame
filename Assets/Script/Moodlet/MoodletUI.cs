using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoodletUI : MonoBehaviour
{
    [SerializeField]
    private float time;

    public GameObject panel;
    public Moodlet thisMoodlet;

    public Image icon;
    public TextMeshProUGUI nameOfMoodlet;
    public TextMeshProUGUI moodletDes;
    public TextMeshProUGUI moodletTimer;

    private bool counted = false;

    public void Start()
    {
        panel.SetActive(false);
        time = thisMoodlet.secondsDuration;
    }

    public void Update()
    {
        if (!counted)
        {
            counted = true;
            StartCoroutine(CountDown());
        }

        if (time == 0) //if counter ends, remove this out of running moodlet
        {
            MoodletManager mm = FindObjectOfType<MoodletManager>();
            mm.runningMoodlet.Remove(thisMoodlet);
            Destroy(this.gameObject);
        }

        moodletTimer.text = time.ToString();
        icon.sprite = thisMoodlet.moodletSprite;
        nameOfMoodlet.text = thisMoodlet.moodletName;
        moodletDes.text = thisMoodlet.reason;
    }

    public void HoverOn()
    {
        panel.SetActive(true);
    }
    public void HoverOff()
    {
        panel.SetActive(false);
    }

    IEnumerator CountDown()
    {
        if (time > 0)
        {
            yield return new WaitForSeconds(1f);
            time--;
        }

        counted = false;
    }
}
