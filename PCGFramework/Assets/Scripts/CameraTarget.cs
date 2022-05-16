/*******************************************************************************
File:      CameraTarget.cs
Author:    Benjamin Ellinger
DP Email:  bellinge@digipen.edu
Date:      09/18/2020
Course:    DES214

Description:
    This component is added to an object that acts as the target a weighted dynamic camera
    will attempt to follow. This object will update its position based on the data fed to
	it by objects acting as camera anchors.

*******************************************************************************/

using UnityEngine;

public class CameraTarget : MonoBehaviour
{
	//Public Properties
    public float MinZoom = 10.0f;
    public float MaxZoom = 100.0f;
    public float Zoom = 10.0f;
	[HideInInspector]
    public float leftEdge = float.MaxValue;
	[HideInInspector]
    public float rightEdge = -float.MaxValue;
	[HideInInspector]
    public float bottomEdge = float.MaxValue;
	[HideInInspector]
    public float topEdge = -float.MaxValue;
	[HideInInspector]
    public float xAccumulator = 0.0f;
	[HideInInspector]
    public float yAccumulator = 0.0f;
	[HideInInspector]	
    public float weightsAccumulator = 0.0f;
	[HideInInspector]	
    public Vector2 playerTarget;

    // Update is called once per frame
    void Update()
    {
		//Determine the zoom level
		Zoom = Mathf.Max(rightEdge - leftEdge, topEdge - bottomEdge)/3.0f;
		Zoom = Mathf.Min(Zoom, MaxZoom);
		Zoom = Mathf.Max(Zoom, MinZoom);

		//Reset the edges
		leftEdge = float.MaxValue;
		rightEdge = -float.MaxValue;
		bottomEdge = float.MaxValue;
		topEdge = -float.MaxValue;

		if (weightsAccumulator <= 0)
		{
			transform.position = (Vector3)playerTarget;
			return;
		}
		
		//Get the average weighted position to move the camera target to.
		Vector3 newPosition;
		newPosition.x = xAccumulator / weightsAccumulator;
		newPosition.y = yAccumulator / weightsAccumulator;
		newPosition.z = 0.0f;
		//Average with the player's target position, unless weights are 10000+
		if (weightsAccumulator < 10000)
			transform.position = (newPosition + (Vector3)playerTarget) / 2.0f;
		else
			transform.position = newPosition;

		//Clear the accumulators
		//Note that the order objects update will not really make a difference in how this works.
		xAccumulator = 0.0f;
		yAccumulator = 0.0f; 
		weightsAccumulator = 0.0f;
    }
}
