using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

    //--------------------------------------------------------
    // Game variables

    public static int Level = 0;
    public static int lives = 3;

	public enum GameState { Init, Game, Dead, Scores }
	public static GameState gameState;

    private GameObject pacman;

    
    private GameObject blinky = null;

    private GameObject covidr;

    private GameObject covidr_1 = null;
    private GameObject covidr_2 = null;
    private GameObject covidr_3 = null;

    private GameObject covidb = null;
    private GameObject covidb_1 = null;
    private GameObject covidb_2 = null;
    private GameObject covidb_3 = null;

    private GameObject covidc = null;
    private GameObject covidc_1 = null;
    private GameObject covidc_2 = null;
    private GameObject covidc_3 = null;

    private GameGUINavigation gui;

	public static bool scared;
    static public int score;

	public float scareLength;
	private float _timeToCalm;

    public float SpeedPerLevel;
    
    //jze
    private TileManager tileManager;



    //-------------------------------------------------------------------
    // singleton implementation
    private static GameManager _instance;

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    //-------------------------------------------------------------------
    // function definitions

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if(this != _instance)   
                Destroy(this.gameObject);
        }

        tileManager = GameObject.FindObjectOfType<TileManager>();

        AssignGhosts(Level);
    }

	void Start () 
	{
		gameState = GameState.Init;
        tileManager.ChangeMap(Level);
	}

    void OnLevelWasLoaded()
    {
        if (Level == 0) lives = 3;

        Debug.Log("Level " + Level + " Loaded!");
        AssignGhosts(Level);
        ResetVariables();

        // Adjust Ghost variables!

        switch(Level){
            case 0:
                blinky.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
 
                covidr_1.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
                covidr_2.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
                covidr_3.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
            break;
            case 1:
                covidb.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
                covidb_1.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
                covidb_2.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
                covidb_3.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
            break;
            case 2:
                covidc.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
                covidc_1.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
                covidc_2.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
                covidc_3.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
            break;
        }

        // for (int i = 1; i < 4; i++)
        // {
            // blinky_1.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
            // inky_1.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
            // pinky_1.GetComponent<GhostMove>().speed += Level * SpeedPerLevel;
        // }


        pacman.GetComponent<PlayerController>().speed += Level*SpeedPerLevel/2;

    }

    private void ResetVariables()
    {
        _timeToCalm = 0.0f;
        scared = false;
        PlayerController.killstreak = 0;
    }

    // Update is called once per frame
	void Update () 
	{
		if(scared && _timeToCalm <= Time.time)
			CalmGhosts();
	}

	public void ResetScene()
	{
        CalmGhosts();

        switch(Level){
            case 0:
                pacman.transform.position = new Vector3(15f, 11f, 0f);
                pacman.GetComponent<PlayerController>().ResetDestination();

                blinky.transform.position = new Vector3(15f, 20f, 0f);
                blinky.GetComponent<GhostMove>().InitializeGhost();

                covidr_1.transform.position = new Vector3(12f, 20f, 0f);
                covidr_1.GetComponent<GhostMove>().InitializeGhost();

                covidr_2.transform.position = new Vector3(14f, 18f, 0f);
                covidr_2.GetComponent<GhostMove>().InitializeGhost();

                covidr_3.transform.position = new Vector3(16f, 16.5f, 0f);
                covidr_3.GetComponent<GhostMove>().InitializeGhost();
            break;
            case 1:
                pacman.transform.position = new Vector3(15f, 14f, 0f);
                pacman.GetComponent<PlayerController>().ResetDestination();

                covidb.transform.position = new Vector3(15f, 20f, 0f);
                covidb.GetComponent<GhostMove>().InitializeGhost();

                covidb_1.transform.position = new Vector3(10f, 20f, 0f);
                covidb_1.GetComponent<GhostMove>().InitializeGhost();

                covidb_2.transform.position = new Vector3(16f, 18f, 0f);
                covidb_2.GetComponent<GhostMove>().InitializeGhost();

                covidb_3.transform.position = new Vector3(12.5f, 16.5f, 0f);
                covidb_3.GetComponent<GhostMove>().InitializeGhost();
            break;
            case 2:
                pacman.transform.position = new Vector3(15f, 14f, 0f);
                pacman.GetComponent<PlayerController>().ResetDestination();

                covidc.transform.position = new Vector3(15f, 20f, 0f);
                covidc.GetComponent<GhostMove>().InitializeGhost();

                covidc_1.transform.position = new Vector3(10f, 20f, 0f);
                covidc_1.GetComponent<GhostMove>().InitializeGhost();

                covidc_2.transform.position = new Vector3(16f, 16.5f, 0f);
                covidc_2.GetComponent<GhostMove>().InitializeGhost();

                covidc_3.transform.position = new Vector3(12.5f, 17f, 0f);
                covidc_3.GetComponent<GhostMove>().InitializeGhost();
            break;
        }


        gameState = GameState.Init;  
        gui.H_ShowReadyScreen();

	}

	public void ToggleScare()
	{
		if(!scared)	ScareGhosts();
		else 		CalmGhosts();
	}

	public void ScareGhosts()
	{
		scared = true;

        switch(Level){
            case 0:
                blinky.GetComponent<GhostMove>().Frighten();
                covidr_1.GetComponent<GhostMove>().Frighten();
                covidr_2.GetComponent<GhostMove>().Frighten();
                covidr_3.GetComponent<GhostMove>().Frighten();
            break;
            case 1:
                covidb.GetComponent<GhostMove>().Frighten();
                covidb_1.GetComponent<GhostMove>().Frighten();
                covidb_2.GetComponent<GhostMove>().Frighten();
                covidb_3.GetComponent<GhostMove>().Frighten();
            break;
            case 2:
                covidc.GetComponent<GhostMove>().Frighten();
                covidc_1.GetComponent<GhostMove>().Frighten();
                covidc_2.GetComponent<GhostMove>().Frighten();
                covidc_3.GetComponent<GhostMove>().Frighten();
            break;
        }
		_timeToCalm = Time.time + scareLength;

        Debug.Log("Ghosts Scared");
	}

	public void CalmGhosts()
	{
		scared = false;
		
        //covidr.GetComponent<GhostMove>().Calm();
        if(Level == 0){

            Debug.Log("calmghost = entro level 1");
            blinky.GetComponent<GhostMove>().Calm();
            covidr_1.GetComponent<GhostMove>().Calm();
            covidr_2.GetComponent<GhostMove>().Calm();
            covidr_3.GetComponent<GhostMove>().Calm();
        }
		
        if(Level == 1){
             Debug.Log("calmghost = entro level 2");
            covidb.GetComponent<GhostMove>().Calm();


            covidb_1.GetComponent<GhostMove>().Calm();
            covidb_2.GetComponent<GhostMove>().Calm();
            covidb_3.GetComponent<GhostMove>().Calm();
        }
            
        if(Level == 2){
            Debug.Log("calmghost = entro level 3");
            

            covidc.GetComponent<GhostMove>().Calm();

            covidc_1.GetComponent<GhostMove>().Calm();
            covidc_2.GetComponent<GhostMove>().Calm();
            covidc_3.GetComponent<GhostMove>().Calm();
        }
            
	    PlayerController.killstreak = 0;
    }

    void AssignGhosts(int level)
    {

        Debug.Log(level);
        // find and assign ghosts
        blinky = GameObject.Find("blinky");

        pacman = GameObject.Find("pacman");

        //covidr = GameObject.Find("covidr");
        covidr_1 = GameObject.Find("covidr_1");
        covidr_2 = GameObject.Find("covidr_2");
        covidr_3 = GameObject.Find("covidr_3");


        covidb = GameObject.Find("covidb");
        covidb_1 = GameObject.Find("covidb_1");
        covidb_2 = GameObject.Find("covidb_2");
        covidb_3 = GameObject.Find("covidb_3");

        covidc = GameObject.Find("covidc");
        covidc_1 = GameObject.Find("covidc_1");
        covidc_2 = GameObject.Find("covidc_2");
        covidc_3 = GameObject.Find("covidc_3");
        

        if (blinky == null || covidb == null || covidc == null) Debug.Log("One of ghosts are NULL");
        if (pacman == null) Debug.Log("Pacman is NULL");

        gui = GameObject.FindObjectOfType<GameGUINavigation>();

        if(gui == null) Debug.Log("GUI Handle Null!");

    }

    public void LoseLife()
    {
        lives--;
        gameState = GameState.Dead;
    
        // update UI too
        UIScript ui = GameObject.FindObjectOfType<UIScript>();
        Destroy(ui.lives[ui.lives.Count - 1]);
        ui.lives.RemoveAt(ui.lives.Count - 1);
    }

    public static void DestroySelf()
    {

        score = 0;
        Level = 0;
        lives = 3;
        Destroy(GameObject.Find("Game Manager"));
    }

    public void LoadLevel (string name)
    {
         SceneManager.LoadScene(name);
    }
}
