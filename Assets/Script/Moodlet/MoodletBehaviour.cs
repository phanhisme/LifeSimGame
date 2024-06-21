using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoodletBehaviour : MonoBehaviour
{
    //moodlet on the player
    //cannot find object of type i guess

    public AdvancedAI playerScript; //how can I handle 2 people?

    //public List<Moodlet> allMoodlet = new List<Moodlet>();
    //public List<Moodlet> runningMoodlet = new List<Moodlet>();

    //handle the moodlet on the player
    private void Start()
    {
        playerScript = this.gameObject.GetComponent<AdvancedAI>();
    }

    void MoodletFunction(float amount)
    {
       //When to add?
        
    }
}
