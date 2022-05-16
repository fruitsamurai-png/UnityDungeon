/*******************************************************************************
File:      EnemyChaseLogic.cs
Author:    Victor Cecci
DP Email:  victor.cecci@digipen.edu
Date:      12/6/2018
Course:    CS186
Section:   Z

Description:
    This component is responsible for the shoot behavior on some enemies.

*******************************************************************************/
using UnityEngine;

[RequireComponent(typeof(EnemyChaseLogic))]
public class EnemyShootLogic : MonoBehaviour
{
    public GameObject BulletPrefab;
    public float ShootCooldown = 1f;
    public float BulletSpeed = 8f;
    public float BulletRange = 20f;
    public int BulletsPerShot = 3;
    public float BulletSpreadAngle = 0.25f;

    private EnemyChaseLogic ChaseBehavior;
    private float Timer = -5.0f;

    // Start is called before the first frame update
    void Start()
    {
        ChaseBehavior = GetComponent<EnemyChaseLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;

        if (!ChaseBehavior.Aggroed)
            return;

        if (Timer >= ShootCooldown)
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

            Timer = 0;
        }
    }

    void FireBullet(float rotate)
    {
        var bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
        var fwd = RotateVector(transform.up, rotate);
        bullet.transform.up = fwd.normalized;
        bullet.GetComponent<Rigidbody2D>().velocity = fwd * BulletSpeed;
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
