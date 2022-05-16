/*******************************************************************************
File:      TopDownController.cs
Author:    Victor Cecci
DP Email:  victor.cecci@digipen.edu
Date:      12/5/2016
Course:    CS186
Section:   Z

Description:
    This component is responsible for all the movement actions for a top down
    character.

*******************************************************************************/
using UnityEngine;

public class TopDownController : MonoBehaviour
{
    //Private References
    private Rigidbody2D RB;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Reset direction every frame
        Vector2 dir = Vector2.zero;
        //Determine movement direction based on input
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            dir += Vector2.up;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            dir += Vector2.left;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            dir += Vector2.down;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            dir += Vector2.right;

        //Apply velocity
        RB.velocity = dir.normalized * GetComponent<HeroStats>().Speed;
    }
}
