/*******************************************************************************
File:      CameraAnchor.cs
Author:    Benjamin Ellinger
DP Email:  bellinge@digipen.edu
Date:      09/18/2020
Course:    DES214

Description:
    This component is tells a different object with a CameraTarget component that its
    weighted position should influence where the camera goes.

*******************************************************************************/

using UnityEngine;

public class CameraAnchor : MonoBehaviour
{
	public float Weight = 1.0f;
	public float Padding = 5.0f;
	public float Leading = 0.0f;
    private GameObject CameraTargetObject;
	private GameObject[] CameraTargetObjects;
	
    // Update is called once per frame
    void Update()
    {
		//Find a camera target if you don't already have one
		if (CameraTargetObject == null)
		{
            CameraTargetObjects = GameObject.FindGameObjectsWithTag("CameraTarget");
			if (CameraTargetObjects != null)
				CameraTargetObject = CameraTargetObjects[0];
			else
				return;
		}

		float activeWeight = Weight;
		
		//If the anchor is attached to an enemy with chase logic that is not aggroed,
		//make it inactive
		var ecl = GetComponent<EnemyChaseLogic>();
		if (ecl != null && ecl.Aggroed == false)
			activeWeight = 0.0f;

		//If this anchor is active, accumulate its x position, y position, and total weighted
		//on the camera target object, which will be averaged together later by that object.
		if (activeWeight > 0)
		{
			var cameraTarget = CameraTargetObject.GetComponent<CameraTarget>();
			
			//Adjust for leading anchors
			Vector2 adjustedPosition;
			adjustedPosition.x = transform.position.x;
			adjustedPosition.y = transform.position.y;
			if (Leading > 0.0f)
			{
				adjustedPosition.x += GetComponent<Rigidbody2D>().velocity.x * Leading;
				adjustedPosition.y += GetComponent<Rigidbody2D>().velocity.y * Leading;
			}

			//If this is the player, just give the camera target the player's position
			if (GetComponent<HeroStats>() != null)
			{
				cameraTarget.playerTarget = adjustedPosition;
			}
			else //If not the player, accumulated the weighted positions
			{
				cameraTarget.xAccumulator += adjustedPosition.x * activeWeight;
				cameraTarget.yAccumulator += adjustedPosition.y * activeWeight;
				cameraTarget.weightsAccumulator += activeWeight;
			}

			//Determine edges of the camera box
			cameraTarget.leftEdge = Mathf.Min(cameraTarget.leftEdge,transform.position.x-Padding);
			cameraTarget.rightEdge = Mathf.Max(cameraTarget.rightEdge,transform.position.x+Padding);
			cameraTarget.bottomEdge = Mathf.Min(cameraTarget.bottomEdge,transform.position.y-Padding);
			cameraTarget.topEdge = Mathf.Max(cameraTarget.topEdge,transform.position.y+Padding);
		}		        
    }
}
