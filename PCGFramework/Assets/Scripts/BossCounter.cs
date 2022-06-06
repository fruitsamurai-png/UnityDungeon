using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCounter : MonoBehaviour
{
    public static int numberofboss;
    // Start is called before the first frame update
    void Start()
    {
        numberofboss++;
    }

    // Update is called once per frame
    void OnDestroy()
    {
        numberofboss--;
    }
}
