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
using System.Collections;

public class HeroStats : MonoBehaviour
{
    //Hero Stats
    public GameObject MainCameraPrefab;
    public GameObject WeightedCameraTargetPrefab;
	public GameObject TimedAnchorPrefab;
    public GameObject UiCanvasPrefab;
    public int currentroom=0;
    public static bool isdead = false;
    private UiStatsDisplay HeroStatsDisplay;
    public static bool gameIsPaused;
    private bool timeslow = false;
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
    private GameObject cam;
    // Start is called before the first frame update
    void Start()
    {
        isdead = false;
        //Spawn canvas
        var canvas = Instantiate(UiCanvasPrefab);
        HeroStatsDisplay = canvas.GetComponent<UiStatsDisplay>();
        timeslow = false;
        //Spawn main camera
        var wct = Instantiate(WeightedCameraTargetPrefab);
        cam = Instantiate(MainCameraPrefab);
        cam.GetComponent<CameraFollow>().ObjectToFollow = wct.transform;
        currentroom = 0;

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
        var old = cam.GetComponent<Camera>().orthographicSize;
        cam.GetComponent<Camera>().orthographicSize = old;
        if (Input.GetKeyDown(KeyCode.P))
        {
            gameIsPaused = !gameIsPaused;
            PauseGame();
        }
        if(timeslow)
        {
            StartCoroutine(bullettime());
        }
    }
    public IEnumerator bullettime()
    {
        float duration = 2.0f;
        float magnitude = 0.3f;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            Time.timeScale = magnitude;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            elapsed += Time.deltaTime;
            yield return null;
        }
        Time.timeScale = 1.0f;
        timeslow = false;
    }
    public IEnumerator CameraShake()
    {
        Vector3 originalposition = cam.GetComponent<CameraFollow>().ObjectToFollow.position;
        float duration = 1.0f;
        float magnitude = 1.0f;
        float elapsed = 0.0f;
        while (elapsed<duration)
        {
            float xoffset = originalposition.x + Random.Range(-5f, 5f) * magnitude;
            float yoffset = originalposition.y+ Random.Range(-5f, 5f) * magnitude;
            cam.GetComponent<CameraFollow>().ObjectToFollow.position = new Vector3(xoffset, yoffset, 0.0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.GetComponent<CameraFollow>().ObjectToFollow.position = originalposition;
    }
    public void setfalse()
    {
        gameObject.SetActive(false);
    }
    void PauseGame()
    {
        if (gameIsPaused)
        {
            Time.timeScale = 0f;
            cam.GetComponent<Camera>().orthographicSize = 100.0f;

        }
        else
        {
            Time.timeScale = 1;
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
					GameObject[] Silverdoors = GameObject.FindGameObjectsWithTag("SilverDoor");
					foreach(GameObject silverdoor in Silverdoors)
                        if(silverdoor.name==currentroom.ToString() && SilverKeys>0)
                        {
                            SilverKeys = 0;
                            currentroom = int.Parse(silverdoor.name) + 1;
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
                case CollectibleTypes.SlowTime:
                {
                 timeslow = true;
                 break;
                }
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
                StartCoroutine(CameraShake());
                Invoke("setfalse", 2.0f);
                isdead = true;
                //GameObject.Find("PCGSpawner").GetComponent<PCG>().RoomList.Clear();
                Invoke("ResetLevel", 6.5f);
            }
        }
       
        int count = GameObject.Find("PCGSpawner").GetComponent<PCG>().RoomList.Count;
        for (int i = 0; i < count; ++i)
        {
            PCG.RoomResults obj = GameObject.Find("PCGSpawner").GetComponent<PCG>().RoomList[i];
            if(obj.first_tile!=null)
            {
                if (collision.gameObject.name == obj.first_tile.name )
                {
                    //Debug.Log("currentroom is :"+ obj.first_tile.name);
                    currentroom = int.Parse(obj.first_tile.name);
                }
            }
            //if(obj.first_corridor!=null)
            //{
            //    if(collision.gameObject.name == obj.first_corridor.name)
            //    {
            //        currentroom = int.Parse(obj.first_corridor.name);

            //    }
            //}
            

        }
    }

    void ResetLevel()
    {
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        currentroom = 0;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
