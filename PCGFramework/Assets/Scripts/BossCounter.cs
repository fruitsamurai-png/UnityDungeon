using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCounter : MonoBehaviour
{
    public bool bossbool = false;
    public bool bosshp = false;
    public static int numberofboss=0;
    private int hp;
    // Start is called before the first frame update
    void Start()
    {
        hp = this.GetComponent<EnemyStats>().StartingHealth;
        numberofboss++;
    }
    private void Update()
    {
        GameObject hero = GameObject.Find("Hero");
        if(hero!=null)
        {
            bool keycollect = hero.GetComponent<HeroStats>().keycollected;
            if (!keycollect && !bossbool && this.GetComponent<EnemyChaseLogic>().Aggroed == true)
            {
                //Debug.Log("here");
                this.GetComponent<EnemyShootLogic>().BulletsPerShot += 2;
                this.GetComponent<EnemyShootLogic>().BulletSpeed += 2;
                this.GetComponent<EnemyStats>().Health += 2;
                bossbool = true;
            }
            if (this.GetComponent<EnemyStats>().Health <= hp / 2 && !bosshp)
            {
                this.GetComponent<SpriteRenderer>().color = Color.red;
                this.GetComponent<EnemyShootLogic>().BulletsPerShot += 3;
                this.GetComponent<EnemyShootLogic>().BulletSpeed += 3;
                this.GetComponent<EnemyChaseLogic>().MoveSpeed += 2;
                bosshp = true;
            }
        }
        
    }
    // Update is called once per frame
    void OnDestroy()
    {
        numberofboss--;
    }
}
