/*******************************************************************************
File:      DestroyOnTime.cs
Author:    Victor Cecci
DP Email:  victor.cecci@digipen.edu
Date:      12/5/2016
Course:    CS186
Section:   Z

Description:
    This component is responsible for destroying the game object it is attached
    to when a given amount of time has passed;

*******************************************************************************/
using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
    public float TimeToDestroy = 2f;

    private float Timer = 0;

    // Update is called once per frame
    void Update()
    {
        //Increment timer
        Timer += Time.deltaTime;

        //Once the timer reaches the limit, destroy the game object
        if (Timer >= TimeToDestroy)
        {
            Destroy(gameObject);
        }
    }
}
