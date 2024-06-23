using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CycleDay : MonoBehaviour
{
    public GameObject panel;
    
    public float realTime = 10f; //1 realtime seconds
    public float inGameTime = 0.0f; //default in game time
    public int currentDay = 1; //track day in cycle
    public int yearNumber = 0;

    public TextMeshProUGUI dayDisplay;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI nextDayText;

    public enum Status { RUNNING, RATINGINPROCESS}
    public Status currentStatus;

    private enum Season { SPRING, SUMMER, FALL, WINTER }
    [SerializeField] private Season currentSeason;

    private BaseAI player;

    public int countdown;
    private bool counted = false;
    private bool callOnce = false;
    public bool getValue = false;

    public float test;

    void Start()
    {
        player = FindObjectOfType<BaseAI>();

        currentSeason = Season.SPRING;
        currentStatus = Status.RUNNING;
    }

    void Update()
    {
        if (currentStatus == Status.RUNNING)
        {
            CheckInGame();

            Time.timeScale = 1;
        }
        else if (currentStatus == Status.RATINGINPROCESS)
        {
            timerText.text = countdown.ToString() + " seconds";
            nextDayText.text = (currentDay + 1).ToString();

            if (!callOnce) //only work if the rating system is in process
            {
                if (getValue)
                {
                    StarRatingRunning();
                    callOnce = true;
                }
            }

            //COUNTER
            if (!counted)
            {
                counted = true;
                StartCoroutine(CountDown());
            }

            Time.timeScale = 0;
        }
    }

    void CheckInGame()
    {
        inGameTime += Time.deltaTime * realTime;

        if (inGameTime >= 24.0f)
        {
            currentStatus = Status.RATINGINPROCESS;
        }
    }

    public void StarRatingRunning()
    {
        panel.SetActive(true);
        inGameTime = 0; //reset time back to 0

        dayDisplay.text = FormattedDate();

        //calculate the current data of the needs and transform them into stars
        //Expressions expression = GetComponent<Expressions>();
        //expression.StarSystem(player.needValue);

        Expressions expression = GetComponent<Expressions>();
        Debug.Log(player.needValue);
        
        expression.express.text = expression.currentMood.ToString();
        StartCoroutine(DayCounter(countdown));
    }

    void ChangeSeason()
    {
        switch (currentSeason)
        {
            case Season.SPRING:
                currentSeason = Season.SUMMER;
                break;
            case Season.SUMMER:
                currentSeason = Season.FALL;
                break;
            case Season.FALL:
                currentSeason = Season.WINTER;
                break;
            case Season.WINTER:
                currentSeason = Season.SPRING;
                yearNumber++;
                break;
        }
    }

    public string FormattedDate()
    {
        //set string for day and year
        string dayOnDisplay = currentDay.ToString();
        string yearOnDisplay = yearNumber.ToString();
        
        //set string for season 
        string seasonOnDisplay = currentSeason.ToString();
        //keep the first letter of the season uppercase and lowercase the rest
        //substring returns a substring of the current StringView, starting at index start and until the end of the StringView - Unity API
        seasonOnDisplay = seasonOnDisplay.Substring(0, 1).ToUpper() + seasonOnDisplay.Substring(1).ToLower();
        
        //return a custom string for display
        return $"Day {dayOnDisplay:dddd}, {seasonOnDisplay}, Year {yearOnDisplay:yyyy}";
    }

    IEnumerator DayCounter(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        currentDay++;

        if (currentDay > 5)
        {
            currentDay = 1;
            ChangeSeason();
        }

        //deactivate report panel
        panel.SetActive(false);

        Expressions expression = GetComponent<Expressions>();
        foreach (GameObject star in expression.stars)
        {
            //destroy stars from last day
            Destroy(star);

            //expression.stars.Remove(star);
        }

        //reset countdown for next time
        countdown = 10;

        //middle ground 
        currentStatus = Status.RUNNING;
        callOnce = false;
        getValue = false;
    }

    IEnumerator CountDown()
    {
        if (countdown > 0)
        {
            yield return new WaitForSecondsRealtime(1f);
            countdown--;
        }

        if (countdown <= 5)
        {
            timerText.color = Color.red;
        }

        counted = false;
    }
}