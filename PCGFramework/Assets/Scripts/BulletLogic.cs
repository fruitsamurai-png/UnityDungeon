/*******************************************************************************
File:      BulletLogic.cs
Author:    Victor Cecci
DP Email:  victor.cecci@digipen.edu
Date:      12/5/2018
Course:    CS186
Section:   Z

Description:
    This component is added to the bullet and controls all of its behavior,
    including how to handle when different objects are hit.

*******************************************************************************/
using UnityEngine;

public enum Teams { Player, Enemy }

public class BulletLogic : MonoBehaviour
{
    public Teams Team = Teams.Player;
	[HideInInspector]
    public float BulletRangeLeft;

    private Rigidbody2D RB;

    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }
	
    void Update()
    {
		//Destroy the bullet after it has travelled far enough
		BulletRangeLeft -= (Time.deltaTime * RB.velocity.magnitude);
		if (BulletRangeLeft < 0)
			Destroy(gameObject);
	}

    private void OnTriggerEnter2D(Collider2D col)
    {
		//No friendly fire
        if (col.isTrigger || col.tag == Team.ToString())
            return;
        Destroy(gameObject);
    }
}
