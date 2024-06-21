using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoodletManager : MonoBehaviour
{
    //handle the UI for moodlet
    //return the number??

    public Transform moodletHolder;

    public List<Moodlet> allMoodlet = new List<Moodlet>();
    public List<Moodlet> runningMoodlet = new List<Moodlet>();

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void MoodletData(Moodlet moodlet)
    {
        switch (moodlet.moodletID)
        {
            //ANGRY
            case 1: //angry by bad music
                break;

            case 2: //low in anger
                break;

            case 3: //mean social
                break;

            case 4: //moodswing
                break;

            //HAPPY
            case 5: //great conversation
                break;

            case 6: //good sleep
                break;

            case 7: //good fun
                break;

            case 8: //good food
                break;

            //BORED
            case 9: //watch a boring show
                break;

            case 10: //low fun
                break;

            case 11:
                break;

            case 12:
                break;

            case 13:
                break;

            case 14:
                break;

            case 15:
                break;

            case 16:
                break;

            default:
                Debug.Log("Cannot find this ID for moodlet");
                break;
        }
    }
}
