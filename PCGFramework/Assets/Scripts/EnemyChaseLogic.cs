/*******************************************************************************
File:      EnemyChaseLogic.cs
Author:    Victor Cecci
DP Email:  victor.cecci@digipen.edu
Date:      12/6/2018
Course:    CS186
Section:   Z

Description:
    This component is responsible for the chase behavior on some enemies.

*******************************************************************************/
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChaseLogic : MonoBehaviour
{
    public float AggroRange = 8f;
    public float MoveSpeed = 5f;
    public float WanderInterval = 2f;
	[HideInInspector]
    public bool Aggroed = false;

	private float MinDeaggroRange = 0.0f;
    private Transform Player;
    private Rigidbody2D RB;
	private float Timer = 0f;
	private bool Wander = false;
	private float MoveVerticalTimer = 0.0f;
	private float MoveHorizontalTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
		var hero = GameObject.Find("Hero");
        Player = hero.transform;
		MinDeaggroRange = hero.GetComponent<HeroShoot>().BulletRange + 4.0f;
        RB = GetComponent<Rigidbody2D>();
    }

	private void OnCollisionStay2D(Collision2D col)
    {
		if (col.gameObject.ToString().StartsWith("Wall") == false)
			return;
		var wallTransform = col.collider.transform;
		var xdist = Math.Abs(transform.position.x - wallTransform.position.x);
		var ydist = Math.Abs(transform.position.y - wallTransform.position.y);
		if (xdist < ydist &&
			xdist <= wallTransform.localScale.x/2.0f + transform.localScale.x/2.0f &&
			MoveHorizontalTimer < -0.25f)
			MoveHorizontalTimer = 0.5f;
		if (ydist < xdist &&
			ydist <= wallTransform.localScale.y/2.0f + transform.localScale.x/2.0f &&
			MoveVerticalTimer < -0.25f)
			MoveVerticalTimer = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
		
        Timer += Time.deltaTime;
		MoveVerticalTimer -= Time.deltaTime;
		MoveHorizontalTimer -= Time.deltaTime;

        if (Wander == false && Timer >= WanderInterval)
        {
			if (UnityEngine.Random.Range(0.0f,1.0f) <= 0.3)
			{
				Wander = true;
				transform.up = SnapVectorToGrid(UnityEngine.Random.insideUnitCircle, MoveVerticalTimer > 0, MoveHorizontalTimer > 0);
			}
            Timer = 0;
        }

        if (Wander == true && Timer >= WanderInterval/4)
        {
			if (Aggroed == true || Timer >= WanderInterval/2)
			{
				Wander = false;
				Timer = 0;
			}
        }
		
        //No reference to player, Nothing to chase
        if (Player == null || !Player.gameObject.activeInHierarchy)
        {
            RB.velocity = Vector2.zero;
            Aggroed = false;
            return;
        }

        //If player is within aggro range, chase it!
        var dir = (Player.position - transform.position);
        if (dir.magnitude <= AggroRange)
		{
			if (Aggroed == false)
			{
				Wander = false;
				Timer = 0;
			}
            Aggroed = true;
		}
        else if (dir.magnitude >= Math.Max(AggroRange * 1.5f, MinDeaggroRange))
            Aggroed = false;
		
        //Rotate to face the player
		if (Aggroed == true && Wander == false)
			transform.up = SnapVectorToGrid(dir, MoveVerticalTimer > 0, MoveHorizontalTimer > 0);

        //Move at designated velocity
		if (Aggroed == true || Wander == true)
			RB.velocity = transform.up * MoveSpeed;
		else
			RB.velocity = Vector2.zero;

    }
	
	//Snap this vector to only going vertical and/or horizontal
	private Vector3 SnapVectorToGrid(Vector3 v, bool vert, bool horiz)
	{
		var snappedVector = v;
		if (vert == true && horiz != true)
			snappedVector.x = 0;
		if (horiz == true && vert != true)
			snappedVector.y = 0;
		if (snappedVector.magnitude <= 0.05f)
			return v.normalized;
		return snappedVector.normalized;
	}
}
