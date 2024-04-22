using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CycleDay : MonoBehaviour
{
    public float realTime = 10f; //1 realtime seconds
    public float inGameTime = 0.0f; //default in game time
    public int currentDay = 1; //track day in cycle
    public int yearNumber = 0;

    public TextMeshProUGUI dayDisplay;

    private enum Season { SPRING, SUMMER, FALL, WINTER }
    [SerializeField] private Season currentSeason;

    void Start()
    {
        currentSeason = Season.SPRING;

        dayDisplay.text = FormattedDate();
    }

    void Update()
    {
        CheckInGame();
    }

    void CheckInGame()
    {
        inGameTime += Time.deltaTime * realTime;

        if (inGameTime >= 24.0f)
        {
            inGameTime = 0; //reset time back to 0
            currentDay++;

            if (currentDay > 5)
            {
                currentDay = 1;
                ChangeSeason();
            }
        }
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
}
