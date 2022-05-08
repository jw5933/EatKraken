using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Dialogue{
    [TextArea(3, 8)]
    public List<string> dialogue = new List<string>();
}
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
    [TextArea(3, 8)]
    [SerializeField] private string deathByHealth;
    [TextArea(3, 8)]
    [SerializeField] private string deathByCustomer;
    [SerializeField] private GameObject endImage;
    [SerializeField] private GameObject krakenImg;
    [SerializeField] private GameObject winProfiles;
    [SerializeField] Animator endAnim;
    [SerializeField] GameObject theEndButton;
    [SerializeField] private TextMeshProUGUI winTmp;
    [SerializeField] List<Dialogue> endDialogue = new List<Dialogue>();
    [SerializeField] private Sprite[] endImages;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    private int dIndex;
    bool endgame;
    bool won;

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
            RestartGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape)){
            if (pauseScreen.activeSelf) CheckPause();
        }
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)){
            ActivateDialogue();
        }
    }

    public void ActivateDialogue(){
        if (endgame && !dm.GoNextDialogue()){
            if (won){
                if (krakenImg.activeSelf){
                    krakenImg.SetActive(false);
                    winProfiles.SetActive(false);
                }
                endAnim.SetTrigger("GoNextAnim");
                if (dIndex < endDialogue.Count) dm.ChangeDialogue(endDialogue[dIndex++].dialogue);
            }
            else{
                dm.textmp.transform.parent.gameObject.SetActive(false);
                theEndButton.SetActive(true);
            }
        }   
    }

    public void CloseEndAnim(){
        dm.textmp.transform.parent.gameObject.SetActive(false);
        theEndButton.SetActive(true);
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

    public void RestartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    public void StartScene(){
        SceneManager.LoadScene("StartScene");
        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    public IEnumerator HandleEndGame(bool win, int death){ //check if the player has died (what conditions? if on no hearts?)
        // death: 0 -> win, 1 -> by health, 2 -> by customer
        endgame = true;
        won = win;
        if (am != null) am.StopAllConstantSFX();
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
        
        //Debug.Log(dm.textmp.transform.parent.name);
        
        if (win){
            dm.textmp = winTmp;
            krakenImg.SetActive(true);
            winProfiles.SetActive(true);
            dm.textmp.transform.parent.gameObject.SetActive(true);
            endAnim.SetTrigger("GoNextAnim");
            dm.ChangeDialogue(endDialogue[dIndex++].dialogue);
        }else{
            yield return new WaitForSecondsRealtime(win ? 0f: 1f);
            dm.textmp.transform.parent.gameObject.SetActive(true);
            dm.AddDialogue(death == 1? deathByHealth: deathByCustomer);
            dm.GoNextDialogue();
        }
    }
}
