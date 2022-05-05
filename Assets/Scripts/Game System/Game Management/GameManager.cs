using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // ==============   variables   ==============
    [Header("General")]
    [SerializeField] bool gameIn3d;
    public bool in3d {get{return gameIn3d;}}
    [SerializeField] private GameObject pauseScreen;
    public bool paused {get{return pauseScreen.activeSelf;}}

    //parents
    [Header("Parents")]
    [SerializeField] private GameObject theOrderParent;
    public Transform orderParent {get{return theOrderParent.transform;}}

    [SerializeField] private GameObject theProteinParent;
    public Transform proteinParent {get{return theProteinParent.transform;}}
    
    [SerializeField] private GameObject theCustomerView;
    public Transform customerView {get{return theCustomerView.transform;}}

    [SerializeField] private GameObject theCustomerParent;
    public Transform customerParent {get{return theCustomerParent.transform;}}

    [SerializeField] private GameObject theMeterParent;
    public Transform meterParent {get{return theMeterParent.transform;}}
    
    //prefabs
    [Header("Prefabs")]
    [SerializeField] private GameObject theTimerPrefab;
    public GameObject timerPrefab{get{return theTimerPrefab;}}
    
    [SerializeField] private GameObject theMeterPrefab;
    public GameObject meterPrefab{get{return theMeterPrefab;}}

    [SerializeField] private GameObject theStationaryMeterPrefab;
    public GameObject stationaryMeterPrefab{get{return theStationaryMeterPrefab;}}

    [SerializeField] private GameObject thePopUpMessagePrefab;
    public GameObject popUpMessagePrefab{get{return thePopUpMessagePrefab;}}

    [SerializeField] private GameObject theOrderPrefab;
    public GameObject orderPrefab {get{return theOrderPrefab;}}

    [SerializeField] private GameObject theCustomerSkeleton;
    public GameObject customerSkeleton {get{return theCustomerSkeleton;}}

    //canvas
    [Header("Canvas")]
    [SerializeField] private GameObject theGameCanvas;
    public GameObject gameCanvas{get{return theGameCanvas;}}
    [SerializeField] private GameObject theStationaryCanvas;
    public GameObject stationaryCanvas{get{return theStationaryCanvas;}}

    //other vars
    [Header("Game Variables")]
    [SerializeField] Location firstLocation;
    //FIX: should update onlocationchange
    public string currLocation {get{return firstLocation.name;}}
    [SerializeField] private int theMaxIngredients;
    public int maxIngredients {get{return theMaxIngredients;}}

    //end state
    [Header("End State")]
    bool endgame;
    [SerializeField] private GameObject endImage;
    [SerializeField] private Sprite[] endImages;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    DialogueManager dm;
    AudioManager am;

    // ==============   methods   ==============
    
    void Awake()
    {
        FindObjectOfType<Map>().selectedLocation = firstLocation;
        am = FindObjectOfType<AudioManager>();
        dm = GetComponent<DialogueManager>();
        if (am != null) am.Activate();
    }

    void Update(){
        CheckInput();
    }

    private void CheckInput(){
        if (Input.GetKeyDown(KeyCode.R)){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Time.timeScale = 1;
            AudioListener.pause = false;
        }
        else if (Input.GetKeyDown(KeyCode.Escape)){
            if (pauseScreen.activeSelf) CheckPause();
        }
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)){
            ActivateDialogue();
        }
    }

    public void ActivateDialogue(){
        if (endgame){
            if (!dm.GoNextDialogue()){
                dm.textmp.SetActive(false);
            }
        }   
    }

    public void CheckPause(){
        if (pauseScreen.activeSelf) UnPauseGame();
        else PauseGame();
    }

    private void PauseGame(){
        pauseScreen.SetActive(true);
        AudioListener.pause = true;
        Time.timeScale = 0;
    }

    private void UnPauseGame(){
        pauseScreen.SetActive(false);
        AudioListener.pause = false;
        Time.timeScale = 1;
    }

    public void QuitGame(){
        //FIX: for testing
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void StartScene(){
        SceneManager.LoadScene("StartScene");
        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    public IEnumerator HandleEndGame(bool win, int death, string endString){ //check if the player has died (what conditions? if on no hearts?)
        endgame = true;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.5f);
        if (win){
            if (am != null) am.PlaySFX(winSound);
        }
        else{
           if (am != null) am.PlaySFX(loseSound);
        }

        endImage.GetComponent<Image>().sprite = endImages[death];
        endImage.SetActive(true);

        yield return new WaitForSecondsRealtime(win ? 0f: 1f);
        //Debug.Log(dm.textmp.transform.parent.name);
        dm.textmp.transform.parent.GetChild(0).gameObject.SetActive(true);

        switch(death){
            case 0: //win -> will be changed to an animation -> daymanager
                dm.AddDialogue(endString);
                dm.GoNextDialogue();
            break;
            case 1: //death -> healthmanager
                dm.AddDialogue(endString);
                dm.GoNextDialogue();

            break;
            case 2: //replaced -> customer manager
                dm.AddDialogue(endString);
                dm.GoNextDialogue();
            break;
        }
    }
}
