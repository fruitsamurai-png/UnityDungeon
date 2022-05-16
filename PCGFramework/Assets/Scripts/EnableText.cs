using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class EnableText : MonoBehaviour
{
    public TextMeshProUGUI text;  //Add reference to UI Text here via the inspector
    public float timeToAppear = 2f;
    public float timeWhenDisappear;

    // Start is called before the first frame update
    public void onText()
    {
        text.enabled = true;
        timeWhenDisappear = Time.time + timeToAppear;
    }

    // Update is called once per frame
    void Update()
    {
        if (text.enabled && (Time.time >= timeWhenDisappear))
        {
            text.enabled = false;
        }
    }
}
