using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoodletManager : MonoBehaviour
{
    //handle the UI for moodlet
    //return the number??

    public GameObject moodletItem;
    public Transform moodletHolder;

    public List<Moodlet> allMoodlet = new List<Moodlet>();
    public List<Moodlet> runningMoodlet = new List<Moodlet>();

    private AdvancedAI AI;

    private void Start()
    {
        AI = FindObjectOfType<AdvancedAI>();
    }

    private void Update()
    {
        
    }

    public void AddData()
    {
        Moodlet dataToAdd = MoodletData();
        AI.lastPicked = null;

        if (dataToAdd != null && !CheckExist(dataToAdd))
        {
            GameObject UIElement = Instantiate(moodletItem, moodletHolder);
            MoodletUI ui = UIElement.GetComponent<MoodletUI>();
            ui.thisMoodlet = dataToAdd;
            runningMoodlet.Add(dataToAdd);
        }
    }

    public Moodlet MoodletData()
    {
        switch (AI.lastPicked.itemID)
        {
            case 1: //Microwave
                if (AI.CurrentHunger > 0.8)
                {
                    float chances = Random.value;
                    if (chances < 0.5f)
                    {
                        //gain positive 8_stuffed
                        return CheckMoodlet(8);
                    }
                }
                return null;

            case 2: //TV
                if (AI.CurrentFun > 0.5)
                {
                    float chances = Random.value;
                    if (chances < 0.5f)
                    {
                        //gain positive 9_boring!
                        return CheckMoodlet(9);
                    }
                }

                return CheckMoodlet(17);

            case 3: //Musical instrument
                float chance = Random.value;
                
                if (chance < 0.5f)
                {
                    int index = Random.Range(0, 1);
                    switch (index)
                    {
                        case 0:
                            return CheckMoodlet(1);

                        case 1:
                            return CheckMoodlet(12);

                    }
                }
                return null;

            case 4: //Bed
                if (AI.CurrentEnergy < 0.5)
                {
                    //chance to obtain one of the following negative moodlet
                    float chances = Random.value;
                    if (chances < 0.4f)
                    {
                        //get 17_sleepless
                        return CheckMoodlet(17);
                    }
                }
                else if (AI.CurrentEnergy > 0.8)
                {
                    float chances = Random.value;
                    if (chances < 0.6f)
                    {
                        //get 6_nightnight
                        return CheckMoodlet(6);
                    }
                }
                return null;

            default:
                return null;
        }
    }

    public void MoodletDataNeeds()
    {
        Moodlet dataToAdd = null;

        //if (AI.CurrentEnergy < 0.4)
        //{
        //    if (!CheckExist(dataToAdd))
        //    {
        //        runningMoodlet.Add(dataToAdd);
        //    }
        //}

        //FUN
        if (AI.CurrentFun > 0.8)
        {
            float randChance = Random.value;
            if (randChance > 0.4f)
            {
                dataToAdd = CheckMoodlet(7);
            }
        }

        if (AI.CurrentFun < 0.4)
        {
            //chance to get moodlet
            float randChance = Random.value;
            if (randChance > 0.2f)
            {
                dataToAdd = CheckMoodlet(10);
            }
        }
        else if (AI.CurrentFun < 0.2)
        {
            dataToAdd = CheckMoodlet(10);
        }
        
        //HUNGER
        if (AI.CurrentHunger < 0.4)
        {
            float randChance = Random.value;
            if (randChance > 0.6f)
            {
                dataToAdd = CheckMoodlet(2);
            }
        }
        else if (AI.CurrentFun < 0.2)
        {
            dataToAdd = CheckMoodlet(2);
        }

        float moodswingChance = Random.value;
        if (moodswingChance < 0.2f)
        {
            dataToAdd = CheckMoodlet(4);
        }

        if (dataToAdd != null && CheckExist(dataToAdd))
        {
            GameObject UIElement = Instantiate(moodletItem, moodletHolder);
            MoodletUI ui = UIElement.GetComponent<MoodletUI>();
            ui.thisMoodlet = dataToAdd;
        }
    }
    
    public Moodlet CheckMoodlet(int id)
    {
        foreach (Moodlet moodlet in allMoodlet)
        {
            if (moodlet.moodletID == id)
            {
                return moodlet;
            }
        }

        return null;
    }

    public bool CheckExist(Moodlet thisMoodlet) //check if the running moodlets have this yet
    {
        if (runningMoodlet.Contains(thisMoodlet))
        {
            return true; //if running moodlet contains this moodlet => return true => do not add more of this moodlet
        }

        return false; //able to add this moodlet to the running list
    }

}
