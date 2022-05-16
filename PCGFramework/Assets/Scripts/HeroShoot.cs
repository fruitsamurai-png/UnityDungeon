/*******************************************************************************
File:      HeroShoot.cs
Author:    Victor Cecci
DP Email:  victor.cecci@digipen.edu
Date:      12/5/2018
Course:    CS186
Section:   Z

Description:
    This component is added to the player and is responsible for the player's
    shoot ability and rotating the player to face the mouse position.

*******************************************************************************/
using UnityEngine;

[RequireComponent(typeof(HeroStats))]
public class HeroShoot : MonoBehaviour
{
    public GameObject BulletPrefab;
    public float BulletSpeed = 5.0f;
    public float BulletRange = 20.0f;
    public float ShotCooldown = 1.0f;
    public float BulletSpreadAngle = 0.1f;
	[HideInInspector]
    public int BulletsPerShot = 1;

    private float Timer = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Rotate player towards mouse position
        var worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0f, 0f, 10f);
        transform.up = (worldMousePos - transform.position).normalized;

        Timer += Time.deltaTime;

        if (Timer >= ShotCooldown && Input.GetMouseButton(0))
        {
			int bulletsLeft = BulletsPerShot;
			float angleAdjust = 0.0f;
			//Odd number of bullets means fire the first one straight ahead
            if (bulletsLeft%2 == 1)
            {
                FireBullet(0.0f);
				bulletsLeft--;
            }
			else //Even number of bullets means we need to adjust the angle slightly
			{
				angleAdjust = 0.5f;
			}
			//The rest of the bullets are spread out evenly
            while (bulletsLeft > 0)
            {
                FireBullet(BulletSpreadAngle * (bulletsLeft/2) - (BulletSpreadAngle * angleAdjust));
                FireBullet(-BulletSpreadAngle * (bulletsLeft/2) + (BulletSpreadAngle * angleAdjust));
				bulletsLeft -= 2; //Must do this afterwards, otherwise the angle will be wrong
            }

            //Reset shoot timer
            Timer = 0f;
        }
    }

    void FireBullet(float rotate)
    {
		//Spawn Bullet
        var bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
		//Rotate bullet to match player direction
        var fwd = RotateVector(transform.up, rotate);
        bullet.transform.up = fwd.normalized;
		//Add bullet velocity
        bullet.GetComponent<Rigidbody2D>().velocity = fwd * (BulletSpeed + (GetComponent<HeroStats>().Speed - GetComponent<HeroStats>().StartingSpeed)*2);
		//Set bullet's range
        bullet.GetComponent<BulletLogic>().BulletRangeLeft = BulletRange;
	}

    Vector2 RotateVector(Vector2 vec, float Angle)
    {
        //x2 = cos(A) * x1 - sin(A) * y1
        var newX = Mathf.Cos(Angle) * vec.x - Mathf.Sin(Angle) * vec.y;

        //y2 = sin(A) * x1 + cos(B) * y1;
        var newY = Mathf.Sin(Angle) * vec.x + Mathf.Cos(Angle) * vec.y;
        
        return new Vector2(newX, newY);
    }
}
