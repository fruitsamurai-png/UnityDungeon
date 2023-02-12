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
    public float dashspeed;
    public bool dashavailable;
    public float dashlength = 0.5f;
    public float dashcooldown = 1.0f;
    private float dashcounter;
    public float dashcoolcounter;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        speed = GetComponent<HeroStats>().Speed;
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
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (!GetComponent<HeroStats>().isDisabled)
            {
                Debug.Log("Cheat enabled");
                speed += 20;
                GetComponent<HeroStats>().isDisabled = true;
            }
            else
            {
                Debug.Log("Cheat disabled");
                speed -= 20;
                GetComponent<HeroStats>().isDisabled = false;
            }
        }
        //Apply velocity
        RB.velocity = dir.normalized * (speed);
        if(!GetComponent<HeroStats>().isDisabled)
            Dash();
    }
    private void Dash()
    {
        if (dashcoolcounter <= 0 && dashcounter <= 0)
        {
            dashavailable = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(dashcoolcounter<=0 &&dashcounter<=0)
            {
                speed += dashspeed;
                dashcounter = dashlength;
            }
        }
        if(dashcounter>0)
        {
            dashcounter -= Time.deltaTime;
            if(dashcounter <= 0)
            {
                dashavailable = false;
                speed = GetComponent<HeroStats>().Speed;
                dashcoolcounter = dashcooldown;
            }
        }
        if(dashcoolcounter>0)
        {
            dashcoolcounter -= Time.deltaTime;
        }
    }
}
