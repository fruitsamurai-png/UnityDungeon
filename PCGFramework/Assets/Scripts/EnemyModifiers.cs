using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModifiers : MonoBehaviour
{
    [HideInInspector]
    public EnemyChaseLogic ECL;
    [HideInInspector]
    public EnemyShootLogic ESL;
    [HideInInspector]
    public EnemyStats ES;

    public int EnemyDifficulty = 0;

    private List<Modifier> Modifiers = new List<Modifier>();


    // Start is called before the first frame update
    void Awake()
    {
        //Get references to external components holding each stat
        ECL = this.GetComponent<EnemyChaseLogic>();
        ESL = this.GetComponent<EnemyShootLogic>();
        ES = this.GetComponent<EnemyStats>();

        //Create Modifiers
        HealthModifier HM = new HealthModifier();
        ProjectileCountModifier PCM = new ProjectileCountModifier();
        EnemyChaseModifier ECM = new EnemyChaseModifier();
        //Add modifiers to the list
        Modifiers.Add(HM);
        Modifiers.Add(PCM);
        Modifiers.Add(ECM);

        //Increment modifier tiers
        for (int i = 0; i < EnemyDifficulty; i++)
        {
            int mod = Random.Range(0, Modifiers.Count);
            Modifiers[mod].Tier++;
        }
        
        //Apply all the modifiers in our list
        for (int i = 0; i < Modifiers.Count; i++)
        {
            Modifiers[i].ApplyModifier(this);
        }
     
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

abstract public class Modifier
{
    public int Tier = 0;

    public virtual void ApplyModifier(EnemyModifiers script)
    {

    }
}
public class EnemyChaseModifier : Modifier
{
    public override void ApplyModifier(EnemyModifiers script)
    {
        switch (Tier)
        {
            case 0:
            {
              script.ECL.AggroRange += 2; 
              break;
            }
            case 1:
            {
              script.ECL.AggroRange += 1;
              script.ECL.MoveSpeed += 2;
              break;
            }
            case 2:
            {
               script.ECL.AggroRange += 2;
               script.ECL.MoveSpeed += 3;
               break;
            }
            case 3:
            {
              script.ECL.AggroRange += 1;
              script.ECL.MoveSpeed += 3;
              script.ECL.WanderInterval += 3.0f;
              break;
            }
            case 4:
            {

              script.ECL.AggroRange += 1;
              script.ECL.MoveSpeed += 4;
              script.ECL.WanderInterval += 2.0f;
              break;
            }
            case 5:
            {
              script.ECL.AggroRange += 2;
              script.ECL.MoveSpeed += 4;
              script.ECL.WanderInterval += 3.0f;
              break;
            }
            case 6:
            {
              script.ECL.AggroRange += 3;
              script.ECL.MoveSpeed += 5;
              script.ECL.WanderInterval += 5.0f;
              break;
            }
        }
    }
}
public class HealthModifier : Modifier
{
    public override void ApplyModifier(EnemyModifiers script)
    {
        switch (Tier)
        {
            case 0:
                script.ES.StartingHealth += 1;
                break;
            case 1:
                script.ES.StartingHealth += 2;
                break;
            case 2:
                script.ES.StartingHealth += 3;
                break;
            case 3:
                script.ES.StartingHealth += 4;
                break;
            case 4:
                script.ES.StartingHealth += 4;
                break;
            case 5:
                script.ES.StartingHealth += 5;
                break;
            case 6:
                script.ES.StartingHealth += 6  ;
                break;
        }
    }
}

public class ProjectileCountModifier : Modifier
{
    public override void ApplyModifier(EnemyModifiers script)
    {
        switch (Tier)
        {
            //1, 2, 3, 5, 8, 12

            case 0:
                script.ESL.BulletRange += 2;
                break;
            case 1:
                script.ESL.BulletsPerShot += 2;
                script.ESL.BulletRange += 1;
                break;
            case 2:
                script.ESL.BulletsPerShot += 2;
                script.ESL.BulletRange += 2;
                break;
            case 3:
                script.ESL.BulletsPerShot += 3;
                script.ESL.BulletSpeed += 5;
                script.ESL.BulletRange += 2;
                break;
            case 4:
                script.ESL.BulletsPerShot += 3;
                script.ESL.BulletSpeed += 5;
                script.ESL.BulletRange += 4;
                break;
            case 5:
                script.ESL.BulletsPerShot += 4;
                script.ESL.BulletSpeed += 6;
                script.ESL.BulletRange += 4;
                break;
        }
    }
}
