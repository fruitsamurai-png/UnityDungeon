/*******************************************************************************
File:      EnemyStats.cs
Author:    Victor Cecci
DP Email:  victor.cecci@digipen.edu
Date:      12/5/2018
Course:    CS186
Section:   Z

Description:
    This component controls all behaviors for enemies in the game.

*******************************************************************************/
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public GameObject EnemyHealthBarPrefab;
    private GameObject HealthBar;
    private HealthBar HealthBarComp;
    public static bool isdestroyed=false;
    public int StartingHealth = 3;
    public int Health
    {
        get { return _Health; }

        set
        {
            HealthBarComp.Health = value;
            _Health = value;
        }

    }
    private int _Health;


    // Start is called before the first frame update
    void Start()
    {
        //Initialize enemy health bar
        HealthBar = Instantiate(EnemyHealthBarPrefab);
        HealthBar.GetComponent<ObjectFollow>().ObjectToFollow = transform;
        HealthBarComp = HealthBar.GetComponent<HealthBar>();
        HealthBarComp.MaxHealth = StartingHealth;
        HealthBarComp.Health = StartingHealth;
        Health = StartingHealth;
        isdestroyed = false;
    }

    // Update is called once per frame
    void Update()
    {
        isdestroyed = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var bullet = col.GetComponent<BulletLogic>();
        if (bullet != null && bullet.Team == Teams.Player)
        {
            Health -= 1;
			GetComponent<EnemyChaseLogic>().Aggroed = true;
            
            if (Health <= 0)
            {
                Destroy(gameObject);
            }
        }
		
		//Aggro on friendly fire
        if (bullet != null && bullet.Team != Teams.Player)
			GetComponent<EnemyChaseLogic>().Aggroed = true;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
		//Aggro on friendly collision
        var enemy = col.GetComponent<EnemyChaseLogic>();
        if (enemy != null && enemy.Aggroed == true)
			GetComponent<EnemyChaseLogic>().Aggroed = true;
    }

    private void OnDestroy()
    {
        Destroy(HealthBar);
        isdestroyed = true;
    }
}
