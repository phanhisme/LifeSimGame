using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Expressions : MonoBehaviour
{
    public List<Image> starsCounter = new List<Image>();

    public int[] starLevel = { 0, 1, 2, 3, 4, 5 };
    public int currentStar;

    public enum Expression { SAD, HAPPY, BORED, EXCITED, ANGRY, DISAPPOINTMENT, TIRED, PLAYFUL, CONFIDENT}
    public Expression currentMood;

    public TextMeshProUGUI commentText;
    public TextMeshProUGUI express;

    public Transform starHolder;
    public Image starImage;

    private BaseAI Player;
    
    void Start()
    {
        Player = FindObjectOfType<BaseAI>(); // in case there are 2 players, this needs update
    }

    private void StarSystem()
    {
        float value = Player.GetTotalValue();
        Debug.Log(value);

        if (value < 1) //total of 3 skills are 3
        {
            currentStar = starLevel[1];
            Debug.Log("Total number is " + value + ". The star level is 1");
        }
        else if (value < 1.5) //larger than 1 and less than 1.5
        {
            currentStar = starLevel[2];
            Debug.Log("Total number is " + value + ". The star level is 2");
        }
        else if (value < 2) //larger than 1.5 and less than 2
        {
            currentStar = starLevel[3];
            Debug.Log("Total number is " + value + ". The star level is 3");
        }
        else if (value < 2.5) //larger than 2 and less than 2.5
        {
            currentStar = starLevel[4];
            Debug.Log("Total number is " + value + ". The star level is 4");
            
        }
        else if (value < 3) //larger than 2.5 and less than 3
        {
            currentStar = starLevel[5];
            Debug.Log("Total number is " + value + ". The star level is 5");
        }

        GetStar(starLevel[currentStar]);
    }

    void GetStar(int star)
    {
        for (int i = 0; i < star ; i++)
        {
            Image thisStar = Instantiate(starImage, starHolder);
            Destroy(thisStar.gameObject, 10f);
        }
    }

    public void UpdateExpression()
    {
        StarSystem();

        switch (currentStar)
        {
            case 1:
            case 2:

                //random expression (angry, sad, tired)
                int i = Random.Range(0, 2);

                if (i == 0)
                {
                    //set expression
                    currentMood = Expression.ANGRY;

                    //random comment - there is only 10% for special comment (randomReaction 2)
                    int randomReaction = GetRandomValue();

                    if (randomReaction == 0)
                    {
                        commentText.text = "Hey! Can you take care of me better?";
                    }

                    else if (randomReaction == 1)
                    {
                        commentText.text = "I hate being here!";
                    }

                    else if (randomReaction == 2)
                    {
                        commentText.text = "This is so frustrating! I want to quit everything and work as a farmer in Stardew Valley!"; //Stardew Valley reference

                        //chang size of the text to fit with the text box
                        commentText.GetComponent<TextMeshProUGUI>().fontSize = 18;
                    }

                }

                else if (i == 1)
                {
                    currentMood = Expression.SAD;

                    int randomReaction = GetRandomValue();

                    if (randomReaction == 0)
                    {
                        commentText.text = "I am sad, can you take care of me?";
                    }

                    else if (randomReaction == 1)
                    {
                        commentText.text = "Is there anything you can do?";
                    }

                    else if (randomReaction == 2)
                    {
                        commentText.text = "It feels so sad even City of Tears looks happier..."; //Hollow Knight reference
                    }
                }

                else if (i == 2)
                {
                    currentMood = Expression.TIRED;

                    int randomReaction = GetRandomValue();

                    if (randomReaction == 0)
                    {
                        commentText.text = "Eye... shutting, I can't help it.";
                    }

                    else if (randomReaction == 1)
                    {
                        commentText.text = "zZzzZZzz";
                    }

                    else if (randomReaction == 2)
                    {
                        commentText.text = "This must be The Dream Realm Dabria alaways speak of..."; //The Dream Catcher reference

                        //chang size of the text to fit with the text box
                        commentText.GetComponent<TextMeshProUGUI>().fontSize = 18;
                    }
                }

                break;

            case 3:
                //random expression (disappointment, bored)
                int a = Random.Range(0, 1);

                if (a == 0)
                {
                    currentMood = Expression.DISAPPOINTMENT;
                    Debug.Log(currentMood);

                    int randomReaction = GetRandomValue();

                    if (randomReaction == 0)
                    {
                        commentText.text = "Ah, It did not work out well I guess...";
                    }

                    else if (randomReaction == 1)
                    {
                        commentText.text = "Come on, are you even trying?";
                    }

                    else if (randomReaction == 2)
                    {
                        commentText.text = "No, no, no! Must you destroy my Wonder?"; //Age of Empire reference
                    }

                }

                else if (a == 1)
                {
                    currentMood = Expression.BORED;
                    Debug.Log(currentMood);

                    int randomReaction = GetRandomValue();

                    if (randomReaction == 0)
                    {
                        commentText.text = "Urg! There is nothing to do here!";
                    }

                    else if (randomReaction == 1)
                    {
                        commentText.text = "Is it the time for something exciting to happen yet?";
                    }

                    else if (randomReaction == 2)
                    {
                        commentText.text = "Why are you blocking me inside these walls... Is this the Room of Requirement?"; //Harry Potter reference

                        //chang size of the text to fit with the text box
                        commentText.GetComponent<TextMeshProUGUI>().fontSize = 18;
                    }
                }
                break;

            case 4:
            case 5:
                //random expression (happy, playful, confident, excited)
                int b = Random.Range(0, 2);

                if (b == 0)
                {
                    //set expression
                    currentMood = Expression.HAPPY;

                    //random comment - there is only 10% for special comment (randomReaction 2)
                    int randomReaction = GetRandomValue();

                    if (randomReaction == 0)
                    {
                        commentText.text = "You took care of me, did you not? Thank you!";
                    }

                    else if (randomReaction == 1)
                    {
                        commentText.text = "Today is such a beautiful day since you are here with me!";
                    }

                    else if (randomReaction == 2)
                    {
                        commentText.text = ""; //Stardew Valley reference

                        //chang size of the text to fit with the text box
                        commentText.GetComponent<TextMeshProUGUI>().fontSize = 18;
                    }

                }

                else if (b == 1)
                {
                    currentMood = Expression.PLAYFUL;

                    int randomReaction = GetRandomValue();

                    if (randomReaction == 0)
                    {
                        commentText.text = "";
                    }

                    else if (randomReaction == 1)
                    {
                        commentText.text = "";
                    }

                    else if (randomReaction == 2)
                    {
                        commentText.text = ""; //Hollow Knight reference
                    }
                }

                else if (b == 2)
                {
                    currentMood = Expression.CONFIDENT;

                    int randomReaction = GetRandomValue();

                    if (randomReaction == 0)
                    {
                        commentText.text = "";
                    }

                    else if (randomReaction == 1)
                    {
                        commentText.text = "";
                    }

                    else if (randomReaction == 2)
                    {
                        commentText.text = "";

                        //chang size of the text to fit with the text box
                        commentText.GetComponent<TextMeshProUGUI>().fontSize = 18;
                    }
                }

                else if (b == 3)
                {
                    currentMood = Expression.EXCITED;

                    int randomReaction = GetRandomValue();

                    if (randomReaction == 0)
                    {
                        commentText.text = "";
                    }

                    else if (randomReaction == 1)
                    {
                        commentText.text = "";
                    }

                    else if (randomReaction == 2)
                    {
                        commentText.text = ""; //The Dream Catcher reference

                        //chang size of the text to fit with the text box
                        commentText.GetComponent<TextMeshProUGUI>().fontSize = 18;
                    }
                }

                break;
        }
    }

    public int GetRandomValue()
    {
        float rand = Random.value;
        if (rand <= .9f)
        {
            return Random.Range(0, 1);
        }
        else
        {
            return 2;
        }
    }
}
