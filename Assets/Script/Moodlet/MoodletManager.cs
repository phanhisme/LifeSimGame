using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoodletManager : MonoBehaviour
{
    public GameObject moodletItem;
    public Transform moodletHolder;

    public List<Moodlet> allMoodlet = new List<Moodlet>();
    public List<Moodlet> runningMoodlet = new List<Moodlet>();

    public List<GameObject> moodletUI = new List<GameObject>();

    private AdvancedAI AI;

    private void Start()
    {
        AI = FindObjectOfType<AdvancedAI>();
    }

    public void AddData()
    {
        //check for chances to get moodlet (depends on the item the player just picked)
        Moodlet dataToAdd = MoodletData();

        //reset item for the next pickup
        AI.lastPicked = null;

        if (dataToAdd != null && !CheckExist(dataToAdd)) //add to running moodlet if the target moodlet is not already existing
        {
            //add to list
            GameObject UIElement = Instantiate(moodletItem, moodletHolder);
            moodletUI.Add(UIElement);

            MoodletUI ui = UIElement.GetComponent<MoodletUI>(); 
            ui.thisMoodlet = dataToAdd;
            runningMoodlet.Add(dataToAdd);

            //if the moodlet is negative, increase decay rates with its relative path (AIStat)
            if (dataToAdd.effectType == Moodlet.EffectType.NEGATIVE)
            {
                switch (dataToAdd.statRelated)
                {
                    case AIStat.Energy:
                        AI.BaseEnergyDecayRate = AI.DecayCheck(dataToAdd);
                        break;

                    case AIStat.Fun:
                        AI.BaseFunDecayRate = AI.DecayCheck(dataToAdd);
                        break;

                    case AIStat.Hunger:
                        AI.BaseHungerDecayRate = AI.DecayCheck(dataToAdd);
                        break;
                }
            }
        }
    }

    public Moodlet MoodletData()
    {
        switch (AI.lastPicked.itemID)
        {
            case 1: //Microwave - choose a possible moodlet that is related to hunger
                float r = Random.value;
                if (r < 0.5f)
                {
                    int index = Random.Range(0, 2);
                    switch (index)
                    {
                        case 0:
                            //gain positive 8_stuffed
                            return CheckMoodlet(8);

                        case 1:
                            return CheckMoodlet(15);

                        case 2:
                            return CheckMoodlet(11);
                    }
                }
                return null;

            case 2: //TV - choose a possible moodlet that is related to fun
                float chances = Random.value;
                if (chances < 0.5f)
                {
                    //gain positive 9_boring!
                    return CheckMoodlet(9);
                }
                return null;

            case 3: //Musical instrument - choose a possible moodlet that is related to fun
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

            case 4: //Bed - choose a possible moodlet that is related to energy
                float rand = Random.value;

                if (rand < 0.7f)
                {
                    int index = Random.Range(0, 1);
                    switch (index)
                    {
                        case 0:
                            return CheckMoodlet(6);

                        case 1:
                            return CheckMoodlet(17);

                    }
                }
                return null;

            default:
                return null;
        }
    }

    public void MoodletDataNeeds() //check on updates for chances to get these moodlet
    {
        Moodlet dataToAdd = null;

        //FUN
        if (AI.CurrentFun > 0.8)
        {
            float randChance = Random.value; //random chance to get the moodlet
            if (randChance > 0.4f)
            {
                dataToAdd = CheckMoodlet(7); //fun and game - extreme happiness
            }
        }

        if (AI.CurrentFun < 0.4)
        {
            //chance to get moodlet
            float randChance = Random.value;
            if (randChance > 0.2f)
            {
                dataToAdd = CheckMoodlet(10); //low fun - bored
            }
        }
        else if (AI.CurrentFun < 0.2)
        {
            dataToAdd = CheckMoodlet(10); //if the condition is critical, always get influence moodlet - 
                                          //extreme low in fun
        }
        
        //HUNGER
        if (AI.CurrentHunger < 0.4)
        {
            float randChance = Random.value; 
            if (randChance > 0.6f)
            {
                dataToAdd = CheckMoodlet(2); //hangry - low in hunger
            }
        }
        else if (AI.CurrentFun < 0.2)
        {
            dataToAdd = CheckMoodlet(2); //exrtreme low in hunger
        }
        else if (AI.CurrentHunger > 0.8)
        {
            float randChance = Random.value;
            if (randChance < 0.2f)
            {
                dataToAdd = CheckMoodlet(16); //too full - but this room does not have a toilet for this sim
            }
        }

        float moodswingChance = Random.value; //random moodswing affects fun decay rate
        if (moodswingChance < 0.2f)
        {
            dataToAdd = CheckMoodlet(4);
        }

        if (dataToAdd != null && CheckExist(dataToAdd)) //if the moodlet is already exist in the current player, skip the moodlet
        {
            GameObject UIElement = Instantiate(moodletItem, moodletHolder);
            MoodletUI ui = UIElement.GetComponent<MoodletUI>(); //instantiate new UI item on screen
            ui.thisMoodlet = dataToAdd;
        }
    }
    
    public Moodlet CheckMoodlet(int id) //check which moodlet is correct in the whole list
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
