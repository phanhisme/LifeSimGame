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
        }
        else if (currentStatus == Status.RATINGINPROCESS)
        {
            Expressions expression = GetComponent<Expressions>();

            timerText.text = countdown.ToString() + " seconds";
            nextDayText.text = (currentDay + 1).ToString();

            if (!callOnce)
            {
                callOnce = true;
                expression.UpdateExpression();
                expression.express.text = expression.currentMood.ToString();
                StarRatingRunning();
            }

            //COUNTER
            if (!counted)
            {
                counted = true;
                StartCoroutine(CountDown());
            }
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

    void StarRatingRunning()
    {
        player.toggleDecay = false;
        player.starRatingInProcess = true;

        panel.SetActive(true);
        inGameTime = 0; //reset time back to 0

        dayDisplay.text = FormattedDate();

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
        
        yield return new WaitForSeconds(time);
        currentDay++;

        if (currentDay > 5)
        {
            currentDay = 1;
            ChangeSeason();
        }

        //deactivate report panel
        panel.SetActive(false);

        //start decay needs again
        player.toggleDecay = true;
        player.starRatingInProcess = false;

        //reset countdown for next time
        countdown = 10;

        //middle ground 
        currentStatus = Status.RUNNING;
        callOnce = false;
    }

    IEnumerator CountDown()
    {
        if (countdown > 0)
        {
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        if (countdown <= 5)
        {
            timerText.color = Color.red;
        }

        counted = false;
    }
}
