using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CycleDay : MonoBehaviour
{
    public int countdown;
    private bool counted = false;
    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        text.text = countdown.ToString() + " seconds";

        if (!counted)
        {
            StartCoroutine(CountDown());
            counted = true;
        }
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
            text.color = Color.red;
        }

        counted = false;
    }
}
