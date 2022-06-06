using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class EnemyCounter : MonoBehaviour
{
    public TextMeshProUGUI text;  //Add reference to UI Text here via the inspector
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject myTextgameObject = GameObject.Find("PCGSpawner");
        if(myTextgameObject!=null)
        {
            int counter = myTextgameObject.GetComponent<PCG>().EnemiesRequired;
            int counter2 = myTextgameObject.GetComponent<PCG>().BossesRequired;
            //if (counter <= 0) counter = 0;
            text.text = "Enemies Left: " + counter +
            "\n Bosses Left: " + counter2;
        }
        
    }
}
