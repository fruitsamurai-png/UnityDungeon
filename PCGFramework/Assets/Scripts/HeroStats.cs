/*******************************************************************************
File:      HeroStats.cs
Author:    Victor Cecci
DP Email:  victor.cecci@digipen.edu
Date:      12/5/2018
Course:    CS186
Section:   Z

Description:
    This component is keeps track of all relevant hero stats. It also handles
    collisions with objects that would modify any stat.

    - MaxHealth = 3
    - Power = 1

*******************************************************************************/
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroStats : MonoBehaviour
{
    //Hero Stats
    public GameObject MainCameraPrefab;
    public GameObject WeightedCameraTargetPrefab;
	public GameObject TimedAnchorPrefab;
    public GameObject UiCanvasPrefab;
    public int currentroom=0;
    private UiStatsDisplay HeroStatsDisplay;

    public int StartingHealth = 3;
    public int MaxHealth
    {
        get { return _MaxHealth; }

        set
        {
            HeroStatsDisplay.HealthBarDisplay.MaxHealth = value;
            _MaxHealth = value;
        }
    }
    private int _MaxHealth;

    public int Health
    {
        get { return _Health; }

        set
        {
            HeroStatsDisplay.HealthBarDisplay.Health = value;
            _Health = value;
        }

    }
    private int _Health;

    public int StartingSilverKeys = 0;
	[HideInInspector]
    public int SilverKeys;

    public int StartingGoldKeys = 0;
	[HideInInspector]
    public int GoldKeys;

    public int StartingSpeed = 5;
	[HideInInspector]
    public int Speed;

	bool FirstPan = true;

    // Start is called before the first frame update
    void Start()
    {
        //Spawn canvas
        var canvas = Instantiate(UiCanvasPrefab);
        HeroStatsDisplay = canvas.GetComponent<UiStatsDisplay>();

        //Spawn main camera
        var wct = Instantiate(WeightedCameraTargetPrefab);
        var cam = Instantiate(MainCameraPrefab);
        cam.GetComponent<CameraFollow>().ObjectToFollow = wct.transform;
		
        //Initialize stats
        MaxHealth = StartingHealth;
        Health = MaxHealth;
        SilverKeys = StartingSilverKeys;
        GoldKeys = StartingGoldKeys;
        Speed = StartingSpeed;
    }

    // Update is called once per frame
    void Update()
    {
		if (FirstPan == true)
		{
            var go = GameObject.Find("SilverKey(Clone)");
            if (go != null)
            {
                var ta = Instantiate(TimedAnchorPrefab);
                ta.transform.position = go.transform.position;
            }
			FirstPan = false;
		}
      
        if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
		}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Check collision against collectibles
        var collectible = collision.gameObject.GetComponent<CollectibleLogic>();
        if (collectible != null)
        {
			GameObject go;
            //Increment relevant stat baed on Collectible type
            switch (collectible.Type)
            {
                case CollectibleTypes.HealthBoost:
                    ++MaxHealth;
                    Health = MaxHealth;
                    break;
                case CollectibleTypes.SilverKey:
                    ++SilverKeys;
					//go = Instantiate(TimedAnchorPrefab);
					//go.transform.position = GameObject.Find("GoldKey(Clone)").transform.position;
					GameObject[] silverdoors = GameObject.FindGameObjectsWithTag("SilverDoor");
					foreach(GameObject silverdoor in silverdoors)
                        if(silverdoor.name==currentroom.ToString() && SilverKeys>0)
                        {
						    GameObject.Destroy(silverdoor);
                        }
                    break;
                case CollectibleTypes.GoldKey:
                    ++GoldKeys;
					go = Instantiate(TimedAnchorPrefab);
					go.transform.position = GameObject.Find("Portal(Clone)").transform.position;
					GameObject[] golddoors = GameObject.FindGameObjectsWithTag("GoldDoor");
					foreach(GameObject golddoor in golddoors)
						GameObject.Destroy(golddoor);
                    break;
                case CollectibleTypes.SpeedBoost:
					++Speed;
                    break;
                case CollectibleTypes.ShotBoost:
					++(GetComponent<HeroShoot>().BulletsPerShot);
                    break;
                case CollectibleTypes.Heart:
                    if (Health == MaxHealth)
                        return;
                    ++Health;
                    break;
            }

            //Destroy collectible
            Destroy(collectible.gameObject);

        }//Collectibles End

        //Check collsion against enemy bullets
        var bullet = collision.GetComponent<BulletLogic>();
        if (bullet != null && bullet.Team == Teams.Enemy)
        {
            Health -= 1;

            if (Health <= 0)
            {
                gameObject.SetActive(false);
                Invoke("ResetLevel", 1.5f);
            }
        }
        int count = GameObject.Find("PCGSpawner").GetComponent<PCG>().RoomList.Count;
        for (int i = 0; i < count; ++i)
        {
            PCG.RoomResults obj = GameObject.Find("PCGSpawner").GetComponent<PCG>().RoomList[i];
            if(obj.first_tile!=null)
            {
                if (collision.gameObject.name == obj.first_tile.name)
                {
                    //Debug.Log("currentroom is :"+ obj.first_tile.name);
                    currentroom = int.Parse(obj.first_tile.name);
                }
            }
            

        }
    }

    void ResetLevel()
    {
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
