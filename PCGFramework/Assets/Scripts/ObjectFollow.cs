/*******************************************************************************
File:      ObjectFollow.cs
Author:    Victor Cecci
DP Email:  victor.cecci@digipen.edu
Date:      12/5/2016
Course:    CS186
Section:   Z

Description:
    This component is added to any object to have it follow a specified target.
    It follows the target using linear interpolation on FixedUpdate.

*******************************************************************************/
using UnityEngine;

public class ObjectFollow : MonoBehaviour
{
    //Public Properties
    public Transform ObjectToFollow;
    public Vector3 Offset;
    public float Interpolant = 0.15f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ObjectToFollow == null)
            return;

        //Lerp towards object every frame
        Vector3 targetPos = ObjectToFollow.position + Offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, Interpolant);
    }
}
