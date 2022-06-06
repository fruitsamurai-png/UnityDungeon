using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PCG : MonoBehaviour
{
	public float GridSize = 5.0f; //Size of floor and wall tiles in units
	public int MaxMapSize = 100; //Maximum height and width of tile map
    public int EnemiesRequired = 1;
    public int BossesRequired = 0;
    public int RoomsToSpawn = 20;
    public int sideroomchance = 3;
    public int sideroomlength = 8;
	private	Dictionary<string, GameObject> Prefabs; //Dictionary of all PCG prefabs
	private	GameObject[,] TileMap; //Tilemap array to make sure we don't put walls over floors
	private	int TileMapMidPoint; //The 0,0 point of the tile map array
	private System.Random RNG;
    public bool portalenabled = false;
    private int totalenemies=0;
    [System.Serializable]
    public class RoomResults
    {
        public Vector2Int ExitTile;
        public Vector2Int SideExitTile;
        public Vector2Int StartTile;
        public int Width;
        public int Height;
        public bool IsVertical;
        public int numberofenemies;
        public int Direction;
        public bool keycollected;
        public GameObject first_tile;
        public RoomResults(Vector2Int tile, Vector2Int s, int w,int h, bool isVertical,int enemycount,GameObject firsttile,Vector2Int sideexit)
        {
            keycollected = false;
            first_tile = firsttile;
            StartTile = s;
            Width = w;
            Height = h;
            ExitTile = tile;
            IsVertical = isVertical;
            numberofenemies = enemycount ;
            SideExitTile = sideexit;
        }

    }
    public List<RoomResults> RoomList;
    public List<String> PowerList;
    public GameObject firstenemy;
    private enum Direction:int
    {
        UP=0,
        DOWN,
        RIGHT
    }
	
    // Start is called before the first frame update
    void Start()
    {
		//Load all the prefabs we need for map generation (note that these must be in a "Resources" folder)
		Prefabs = new Dictionary<string, GameObject>();
		Prefabs.Add("floor", Resources.Load<GameObject>("Prefabs/Floor"));
		Prefabs["floor"].transform.localScale = new Vector3(GridSize, GridSize, 1.0f); //Scale the floor properly
		Prefabs.Add("special", Resources.Load<GameObject>("Prefabs/FloorSpecial"));
		Prefabs["special"].transform.localScale = new Vector3(GridSize, GridSize, 1.0f); //Scale the floor properly
		Prefabs.Add("wall", Resources.Load<GameObject>("Prefabs/Wall"));
		Prefabs["wall"].transform.localScale = new Vector3(GridSize, GridSize, 1.0f); //Scale the wall properly
		Prefabs.Add("portal", Resources.Load<GameObject>("Prefabs/Portal"));
		Prefabs.Add("enemy", Resources.Load<GameObject>("Prefabs/BaseEnemy"));
		Prefabs.Add("fast", Resources.Load<GameObject>("Prefabs/FastEnemy"));
        Prefabs.Add("fasttank", Resources.Load<GameObject>("Prefabs/FastTankEnemy"));
        Prefabs.Add("spread", Resources.Load<GameObject>("Prefabs/SpreadEnemy"));
		Prefabs.Add("tank", Resources.Load<GameObject>("Prefabs/TankEnemy"));
		Prefabs.Add("ultra", Resources.Load<GameObject>("Prefabs/UltraEnemy"));
		Prefabs.Add("boss", Resources.Load<GameObject>("Prefabs/BossEnemy"));
		Prefabs.Add("heart", Resources.Load<GameObject>("Prefabs/HeartPickup"));
		Prefabs.Add("healthboost", Resources.Load<GameObject>("Prefabs/HealthBoost"));
		Prefabs.Add("shotboost", Resources.Load<GameObject>("Prefabs/ShotBoost"));
		Prefabs.Add("speedboost", Resources.Load<GameObject>("Prefabs/SpeedBoost"));
		Prefabs.Add("silverkey", Resources.Load<GameObject>("Prefabs/SilverKey"));
		Prefabs.Add("goldkey", Resources.Load<GameObject>("Prefabs/GoldKey"));
        Prefabs.Add("timeslow", Resources.Load<GameObject>("Prefabs/TimeSlow"));
        Prefabs.Add("silverdoor", Resources.Load<GameObject>("Prefabs/SilverDoor"));
		Prefabs["silverdoor"].transform.localScale = new Vector3(GridSize/2.0f, 1.0f, 1.0f); //Scale the door properly
		Prefabs.Add("golddoor", Resources.Load<GameObject>("Prefabs/GoldDoor"));
		Prefabs["golddoor"].transform.localScale = new Vector3(GridSize/2.0f, 1.0f, 1.0f); //Scale the door properly
        Prefabs.Add("obstacle", Resources.Load<GameObject>("Prefabs/Obstacle"));
        Prefabs["obstacle"].transform.localScale = new Vector3(GridSize/4.0f, 1.0f, 1.0f); //Scale the Obstacle properly
        //Delete everything visible except the hero when reloading       
        var objsToDelete = FindObjectsOfType<SpriteRenderer>();
        RoomList = new List<RoomResults>();
        PowerList = new List<String>();
        PowerList.Add("healthboost");
        PowerList.Add("shotboost");
        PowerList.Add("speedboost");
        PowerList.Add("timeslow");
        totalenemies = 0;
        int totalObjs = objsToDelete.Length;
		for (int i = 0; i < totalObjs; i++)
		{
			if (objsToDelete[i].gameObject.ToString().StartsWith("Hero") == false && objsToDelete[i].gameObject.ToString().StartsWith("Portalpointer")==false)
				UnityEngine.Object.DestroyImmediate(objsToDelete[i].gameObject);
		}
			
		//Create the tile map
		TileMap = new GameObject[MaxMapSize,MaxMapSize];
		TileMapMidPoint = (MaxMapSize*MaxMapSize)/2;
		RNG = new System.Random();

        //Spawn Edges

        Vector2Int StartingTile = new Vector2Int(1, 1);
        for (int i = 0; i < RoomsToSpawn; i++)
        {
            RoomResults result = SpawnRoom(StartingTile.x, StartingTile.y,i);
            StartingTile = result.ExitTile;
            RoomList.Add(result);
            if (result.first_tile != null)
            {
                result.first_tile.name = i.ToString();
            }
            for (int k = 0; k < result.Width; k++)
            {
                for (int j = 0; j < result.Height; j++)
                {
                    if (GetTile(k, j) != null)
                    {
                        SpawnPowerup(result.StartTile.x + k, result.StartTile.y + j, RNG.Next(0, i));
                        SpawnEnemies(result.StartTile.x + k, result.StartTile.y + j, ref result.numberofenemies, i);
                    }
                }
            }
            if (i < RoomsToSpawn - 1)
            {
                //if (i == RoomsToSpawn - 2)
                //{
                //    Debug.Log(i);
                //    SpawnDoor(StartingTile.x, StartingTile.y, result.IsVertical,i);
                //}
                StartingTile = SpawnCorridor(result.IsVertical, StartingTile.x, StartingTile.y);
            }       
        }
        for (int i = 0; i < RoomsToSpawn; i++)
        {
            if (i>3 &&i<=RoomsToSpawn-3)
            {
                bool Addsideroom = RNG.Next(0, sideroomchance) <= 4;
                if (Addsideroom)
                {
                    RoomResults result = RoomList[i];
                    StartingTile = result.SideExitTile;
                    //SpawnDoor(StartingTile.x, StartingTile.y, !result.IsVertical, i);
                    StartingTile = SpawnCorridor(!result.IsVertical, StartingTile.x, StartingTile.y);
                    //SpriteRenderer sprite=TileMap[StartingTile.x, StartingTile.y].GetComponent<SpriteRenderer>();
                    //sprite.color = Color.red;
                    int length = RNG.Next(1, sideroomlength); // 1 to 7
                    for (int j = 1; j < length+1; ++j)
                    {
                        RoomResults sideresult = SpawnRoom(StartingTile.x, StartingTile.y,i);
                        StartingTile = sideresult.ExitTile;
                        RoomList.Add(sideresult);
                        if (sideresult.first_tile != null)
                        {
                            int newindex = i + j;
                            sideresult.first_tile.name = newindex.ToString();
                        }
                        for (int k = 0; k < sideresult.Width; k++)
                        {
                            for (int x = 0 ;x < sideresult.Height; x++)
                            {
                                SpawnObstacle(sideresult.StartTile.x + k, sideresult.StartTile.y + x, RNG.Next(0, i));
                                if (GetTile(k, x)!=null)
                                {
                                    SpawnPowerup(sideresult.StartTile.x + k, sideresult.StartTile.y + x, RNG.Next(0, i));
                                    SpawnEnemies(sideresult.StartTile.x + k, sideresult.StartTile.y + x, ref sideresult.numberofenemies, RNG.Next(0,i)+j);
                                }
                            }
                        }
                        if (j < length )
                        {
                            //SpawnDoor(StartingTile.x, StartingTile.y, sideresult.IsVertical, i);
                            StartingTile = SpawnCorridor(sideresult.IsVertical, StartingTile.x, StartingTile.y);
                        }
                    }
                }
                else continue;
                
            }
        }
        EnemiesRequired = RNG.Next(totalenemies/2, totalenemies);

        //Fill In Walls
        FillInWalls();
        SpawnEdgeWalls();

    }
    private void Update()
    {
        GameObject hero = GameObject.Find("Hero");
        int index;
        if (hero!=null)
        {
            index = hero.GetComponent<HeroStats>().currentroom;
        }
        else
        {
            index = 0;
        }

        if(RoomList!=null)
        {
            var obj = RoomList[index];
            if (obj != null && totalenemies >= 0)
            {
                GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
                BossesRequired = BossCounter.numberofboss;
                obj.numberofenemies=array.Length;
                //Debug.Log(array.Length + " and total enemies is " + totalenemies);
                if(array.Length<totalenemies)
                {
                    EnemiesRequired--;
                    if (EnemiesRequired < 0) EnemiesRequired = 0;
                    totalenemies = array.Length;
                }
                
                //Debug.Log(obj.numberofenemies+" and total enemies is"+ totalenemies +" is detroyed "+EnemyStats.isdestroyed);
            }
            if (Input.GetKey(KeyCode.F) && obj != null)
            {
                //Debug.Log(obj.numberofenemies+"and key is "+obj.keycollected);
            }
            if (obj != null && obj.numberofenemies <= 0 && !obj.keycollected && GameObject.Find("Hero").GetComponent<HeroStats>().currentroom != RoomsToSpawn - 1)
            {
                //SpawnKey(obj.StartTile.x+obj.Width/2, obj.StartTile.y + obj.Height / 2);
                obj.keycollected = true;
            }
            //SPAWN PORTAL BELOW
            if (RoomList != null)
            {
                RoomResults Lastroom = RoomList[RoomsToSpawn - 1];
                if (!portalenabled && EnemiesRequired <= 0 && BossesRequired<=0)
                {
                    Spawn("portal", Lastroom.StartTile.x + Lastroom.Width / 2, Lastroom.StartTile.y + Lastroom.Height / 2);
                    portalenabled = true;
                }
            }
        }
        
    }
    private bool CheckBoundary(int x,int y)
    {
        if ((x == 0 || y == 0 || x == MaxMapSize-1 || y == MaxMapSize-1))
            return true;
        else return false;
    }
    private void SpawnObstacle(int x,int y,int roomlevel)
    {
        if(roomlevel>=3 && roomlevel<=8)
        {
            int numberenem = RNG.Next(1, roomlevel);
            if (numberenem <= 2)
            {
                Spawn("obstacle", x, y);
            }
        }
        
    }
    private void SpawnPowerup(int x, int y,int roomlevel)
    {
        switch (roomlevel)
        {
            //case 0:
            //    {
            //        int numberenem = RNG.Next(1, 10);
            //        if (numberenem <= 2)
            //        {
            //            int typepowerup = RNG.Next(0, PowerList.Count);
            //            Spawn(PowerList[PowerList.Count-1], x, y);
            //            Debug.Log(typepowerup);
            //        }
            //        break;
            //    }
            case 1:
                {
                    int numberenem = RNG.Next(1, 10);
                    if (numberenem <= 3)
                    {
                        int typepowerup = RNG.Next(0, PowerList.Count - 1);
                        Spawn(PowerList[typepowerup], x, y);
                    }
                    break;
                }
            case 2:
                {
                    int numberenem = RNG.Next(1, 6);
                    if (numberenem <= 2)
                    {
                        int typepowerup = RNG.Next(0, PowerList.Count - 1);
                        Spawn(PowerList[typepowerup], x, y);
                    }
                    break;
                }
            case 3:
                {
                    int numberenem = RNG.Next(1, 6);
                    if (numberenem <= 2)
                    {
                        int typepowerup = RNG.Next(0, PowerList.Count - 1);
                        Spawn(PowerList[typepowerup], x, y);
                    }
                    else if (numberenem <= 4)
                    {
                        int typepowerup = RNG.Next(0, PowerList.Count - 1);
                        Spawn(PowerList[typepowerup], x, y);
                    }
                    break;
                }
            case 4:
                {
                    int numberenem = RNG.Next(1, 6);
                    if (numberenem <= 3)
                    {
                        int typepowerup = RNG.Next(0, PowerList.Count - 1);
                        Spawn(PowerList[typepowerup], x, y);
                    }
                    else if (numberenem <= 4)
                    {
                        int typepowerup = RNG.Next(0, PowerList.Count - 1);
                        Spawn(PowerList[typepowerup], x, y);
                    }
                     else if (numberenem <= 4)
                    {
                        int typepowerup = RNG.Next(0, PowerList.Count - 1);
                        Spawn(PowerList[typepowerup], x, y);
                    }
                    break;
                }
            case 5:
                {
                    int numberenem = RNG.Next(1, 15);
                    if (numberenem <= 7)
                    {
                        int typepowerup = RNG.Next(0, PowerList.Count - 1);
                        Spawn(PowerList[typepowerup], x, y);
                    }
                    break;
                }
            case 6:
                {
                    int numberenem = RNG.Next(1, 6);
                    if (numberenem <= 2)
                    {
                        int typepowerup = RNG.Next(0, PowerList.Count);
                        Spawn(PowerList[typepowerup], x, y);
                    }
                    break;
                }
            case 7:
                {
                    int numberenem = RNG.Next(1, 8);
                    if (numberenem <= 2)
                    {
                        int typepowerup = RNG.Next(0, PowerList.Count);
                        Spawn(PowerList[typepowerup], x, y);
                    }
                    else if (numberenem <= 6)
                    {
                        Debug.Log("wtf");
                        Spawn(PowerList[PowerList.Count-1], x, y);
                    }
                    break;
                }
            case 8:
                {
                    int numberenem = RNG.Next(1, 8);
                    if (numberenem <= 3)
                    {
                        int typepowerup = RNG.Next(0, PowerList.Count);
                        Spawn(PowerList[typepowerup], x, y);
                    }
                    else if (numberenem <= 5)
                    {
                        Debug.Log("wtf");
                        Spawn(PowerList[PowerList.Count - 1], x, y);
                    }
                    break;
                }
            default:
                break;
        }
    }
    private void SpawnKey(int x,int y)
    {
        Spawn("silverkey", x, y);
    }
    void SpawnDoor(int x,int y,bool Vertical,int index)
    {
        if(Vertical)
        {

            TileMap[x, y]= Spawn("silverdoor", x, y);
            TileMap[x, y].name = index.ToString();
        }
        else
        {
            TileMap[x, y] = SpawnRotateLeft("silverdoor", x, y);
            TileMap[x, y].name = index.ToString();
        }
    }
    void SpawnEdgeWalls()
    {
        //for (int x = 0; x < MaxMapSize / 2; x++)
        //{
        //    for (int y = 0; y < MaxMapSize / 2; y++)
        //    {
        //        if(x == 0 || y == 0 || x == MaxMapSize - 1 || y == MaxMapSize - 1)
        //            TileMap[x, y] = Spawn("wall", x, y);
        //    }
        //}
        for(int x=0;x<MaxMapSize/2;++x)
        {
            TileMap[0,x]= Spawn("wall", 0, x);
            TileMap[MaxMapSize / 2, x] = Spawn("wall", MaxMapSize / 2, x);
        }
        for (int x = 0; x < MaxMapSize / 2; ++x)
        {
            TileMap[x, 0] = Spawn("wall", x, 0);
            TileMap[ x, MaxMapSize / 2] = Spawn("wall", x, MaxMapSize / 2);
        }
    }

    void FillInWalls()
    {
        for (int x = 0; x < MaxMapSize; x++)
        {
            for (int y = 0; y < MaxMapSize; y++)
            {
                GameObject obj = GetTile(x, y);

                if (obj == null)
                    TileMap[x, y] = Spawn("wall", x, y);
                    
            }
        }   
    }

    Vector2Int SpawnCorridor(bool IsVertical, int startX, int startY)
    {
        int length = RNG.Next(1, 3); // 1 to 3

        for (int i = 0; i < length; i++)
        {
            if (IsVertical)
                TileMap[startX, startY + i] = Spawn("special", startX, startY + i);
            else
                TileMap[startX + i, startY] = Spawn("special", startX + i, startY);
        }
        //TileMap[startX, startY].name = index.ToString();
        //RoomList[index].first_corridor = TileMap[startX, startY];
        //Return the corridor's exit tile
        if (IsVertical)
            return new Vector2Int(startX, startY + length);
        else
            return new Vector2Int(startX + length, startY);

    }
    void SpawnEnemies(int x,int y,ref int numberofenemies,int roomlevel)
    {
        switch (roomlevel)
        {
            //case 0:
            //{
            //   int numberenem = RNG.Next(1, 10);
            //   if (numberenem <= 2)
            //   {
            //      if (totalenemies <= 0)
            //      {
            //         //Debug.Log(totalenemies);
            //         firstenemy=Spawn("enemy", x, y);
            //      }
            //      else
            //      Spawn("enemy", x, y);
            //      numberofenemies++;
            //      totalenemies++;
            //   }
            //   break;
            //}
            case 1:
            {
               if (totalenemies <= 0)
               {
                   //Debug.Log(totalenemies);
                   firstenemy = Spawn("enemy", x, y);
               }
               int numberenem = RNG.Next(1, 10);
               if (numberenem <= 3)
               {
                   Spawn("enemy", x, y);
                   numberofenemies++;
                   totalenemies++;
               }
               break;
            }
            case 2:
            {
               int numberenem = RNG.Next(1, 6);
               if (numberenem <= 2)
               {
                   Spawn("enemy", x, y);
                        totalenemies++;
                        numberofenemies++;
               }
               else if(numberenem <= 4)
               {
                   Spawn("fast", x, y);
                   numberofenemies++;
                        totalenemies++;
               }
               break;
            }
            case 3:
            {
               int numberenem = RNG.Next(1, 6);
               if (numberenem <= 2)
               {
                   Spawn("enemy", x, y);
                   numberofenemies++;
                   totalenemies++;
               }
               else if (numberenem <= 4)
               {
                   Spawn("fast", x, y);
                   numberofenemies++;
                   totalenemies++;
               }
                    break;
            }
            case 4:
            {
               int numberenem = RNG.Next(1, 6);
               if (numberenem <= 3)
               {
                   Spawn("enemy", x, y);
                   numberofenemies++;
                        totalenemies++;
               }
               else if (numberenem <= 4)
               {
                   Spawn("fast", x, y);
                   numberofenemies++;
                        totalenemies++;
                }
                    break;
            }
            case 5:
            {
                int numberenem = RNG.Next(1, 15);
                if (numberenem <= 7 &&numberofenemies==0)
                {
                    Spawn("boss", x, y);
                    numberofenemies++;
                    totalenemies++;
                    BossesRequired++;
                }
                break;
            }
            case 6:
            {
                    int numberenem = RNG.Next(1, 15);
                    if (numberenem <= 7 && numberofenemies <= 2)
                    {
                        Spawn("fasttank", x, y);
                        numberofenemies++;
                        totalenemies++;
                        BossesRequired++;
                    }
                    break;
            }
            case 7:
            {
                    int numberenem = RNG.Next(1, 6);
                    if (numberenem <= 3)
                    {
                        Spawn("tank", x, y);
                        numberofenemies++;
                        totalenemies++;
                    }
                    else if (numberenem <= 4)
                    {
                        Spawn("spread", x, y);
                        numberofenemies++;
                        totalenemies++;
                    }
                    break;
            }
            case 8:
                {
                    int numberenem = RNG.Next(1, 6);
                    if (numberenem <= 3)
                    {
                        Spawn("fasttank", x, y);
                        numberofenemies++;
                        totalenemies++;
                    }
                    else if (numberenem <= 5)
                    {
                        Spawn("ultra", x, y);
                        numberofenemies++;
                        totalenemies++;
                    }
                    break;
                }
            default:
                break;
        }
        
    }
    RoomResults SpawnRoom(int startX, int startY,int level)
    {
        //print("test");

        int width = RNG.Next(2, 5); // 3 to 5
        int height = RNG.Next(2, 5); // 3 to 5
        int numberofenemies = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                SpawnTile(startX + i, startY + j);
            }
        }

       // Debug.Log("number of enemies is " + numberofenemies);

        bool isVertical = RNG.Next(0, 2) == 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(CheckBoundary(startX + i, startY + j))
                {
                    Debug.Log("true");
                    isVertical = false;
                }
            }
        }
                //Return exit tile
        if (isVertical)
            return new RoomResults(new Vector2Int(RNG.Next(startX, startX + width), startY + height),new Vector2Int(startX,startY),width,height, true, numberofenemies,TileMap[startX, startY]
                , new Vector2Int(startX + width, RNG.Next(startY, startY + height)));
        else
            return new RoomResults(new Vector2Int(startX + width, RNG.Next(startY, startY + height)), new Vector2Int(startX, startY), width, height, false, numberofenemies, TileMap[startX, startY]
                , new Vector2Int(RNG.Next(startX, startX + width), startY + height));   

    }


	//Get a tile object (only walls and floors, currently)
	GameObject GetTile(int x, int y) 
	{
		if (Math.Abs(x) > MaxMapSize/2 || Math.Abs(y) > MaxMapSize/2)
			return Prefabs["wall"];
		return TileMap[x, y];
	}

	//Spawn a tile object if one isn't already there
	void SpawnTile(int x, int y)
	{
		if (GetTile(x,y) != null)
			return;
		TileMap[x, y] = Spawn("floor", x, y);
	}

	//Spawn any object
	GameObject Spawn(string obj, float x, float y)
	{
		return Instantiate(Prefabs[obj], new Vector3(x * GridSize, y * GridSize, 0.0f), Quaternion.identity);
	}

	//Spawn any object rotated 90 degrees left
	GameObject SpawnRotateLeft(string obj, float x, float y)
	{
		return Instantiate(Prefabs[obj], new Vector3(x * GridSize, y * GridSize, 0.0f), Quaternion.AngleAxis(-90, Vector3.forward));
	}

	//Spawn any object rotated 90 degrees right
	GameObject SpawnRotateRight(string obj, float x, float y)
	{
		return Instantiate(Prefabs[obj], new Vector3(x * GridSize, y * GridSize, 0.0f), Quaternion.AngleAxis(90, Vector3.forward));
	}

}
