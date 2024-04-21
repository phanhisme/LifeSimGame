using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
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
}
