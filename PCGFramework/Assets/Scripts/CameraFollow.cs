/*******************************************************************************
File:      CameraFollow.cs
Author:    Benjamin Ellinger
DP Email:  bellinge@digipen.edu
Date:      09/18/2020
Course:    DES214

Description:
    This component is added to a camera to have it follow a specified target.
    It follows the target using an adjusted 2D linear interpolation on FixedUpdate.

*******************************************************************************/

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //Public Properties
    public Transform ObjectToFollow;
    public float MaxSpeed = 11.0f;
    public float MinSpeed = 5.0f; 
    public float Interpolant = 0.25f;
	private float MoveSpeed = 7.0f;
	private float MaxAccel = 3.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ObjectToFollow == null)
            return;
        //Find the offset to the target
        Vector3 targetPos = ObjectToFollow.position;
		Vector3 offset = targetPos - transform.position;

		//Convert to 2D
		Vector2 adjust;
		adjust.x = offset.x;
		adjust.y = offset.y;
		float distance2D = adjust.magnitude; //Use later to detect overshooting

		//Determine amount to interpolate
		adjust.x = offset.x * Interpolant;
		adjust.y = offset.y * Interpolant;

		//Adjust if it is going too fast
		if (adjust.magnitude > MoveSpeed + MaxAccel)
        {
			adjust = adjust.normalized * (MoveSpeed + MaxAccel);
        }
		if (adjust.magnitude > MaxSpeed)
        {
			adjust = adjust.normalized * MaxSpeed;
        }

		//Adjust if it is going too slow
		if (adjust.magnitude < MinSpeed)
			adjust = adjust.normalized * MinSpeed;
		
		var oldPosition = transform.position;
		
		//Move towards the target, but not along the Z axis
		transform.Translate(adjust.x, adjust.y, 0.0f);

		//Don't overshoot the target
		if (adjust.magnitude > distance2D)
        {
			transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);
        }
		
		MoveSpeed = (transform.position - oldPosition).magnitude * Time.deltaTime;
        //Find the target zoom level
        float targetZoom = ObjectToFollow.GetComponent<CameraTarget>().Zoom;
		float zoomOffset = targetZoom - GetComponent<Camera>().orthographicSize;

		//Convert to 2D
		float zoomAdjust;
		zoomAdjust = zoomOffset;
		float zoomDistance = Mathf.Abs(zoomAdjust); //Use later to detect overshooting

		//Determine amount to interpolate
		zoomAdjust = zoomOffset * Interpolant;

		//Adjust if it is going too fast
		if (zoomAdjust > MaxSpeed)
			zoomAdjust = MaxSpeed;
		else if (zoomAdjust < -MaxSpeed / 15.0f) //Zoom in slower than zoom out
			zoomAdjust = -MaxSpeed / 15.0f;

		//Adjust if it is going too slow
		if (zoomAdjust < MinSpeed && zoomAdjust > 0)
			zoomAdjust = MinSpeed;
		else if (zoomAdjust > -MinSpeed && zoomAdjust < 0)
			zoomAdjust = -MinSpeed;
		
		//Move towards the target zoom level
		GetComponent<Camera>().orthographicSize += zoomAdjust;

		//Don't overshoot the target
		if (Mathf.Abs(zoomAdjust) > zoomDistance)
			GetComponent<Camera>().orthographicSize = targetZoom;
    }
}
