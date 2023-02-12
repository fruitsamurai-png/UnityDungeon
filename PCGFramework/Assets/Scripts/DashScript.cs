using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
public class DashScript : MonoBehaviour
{
    public TextMeshProUGUI text;  //Add reference to UI Text here via the inspector
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject myTextgameObject = GameObject.Find("Hero");
        if (myTextgameObject != null)
        {
            bool counter = myTextgameObject.GetComponent<TopDownController>().dashavailable;
            int dashcounter = (int)myTextgameObject.GetComponent<TopDownController>().dashcoolcounter;
            string temp;
            if(counter)
            {
                temp = "Available";
            }
            else
            {
                temp = "Available in " + dashcounter.ToString();
            }
            text.text = "Dash: "+ temp;
        }
    }
}
