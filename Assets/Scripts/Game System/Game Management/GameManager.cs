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
    //parents
    [Header("Parents")]
    [SerializeField] private GameObject theOrderParent;
    public Transform orderParent {get{return theOrderParent.transform;}}

    [SerializeField] private GameObject theIngredientParent;
    public Transform ingredientParent {get{return theIngredientParent.transform;}}
    
    [SerializeField] private GameObject theCustomerView;
    public Transform customerView {get{return theCustomerView.transform;}}
    
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
    [SerializeField] private Text endText;
    [SerializeField] private GameObject endImage;
    

    // ==============   methods   ==============
    
    void Awake()
    {
        FindObjectOfType<Map>().selectedLocation = firstLocation;
    }

    void Update(){
        CheckRestart();
    }

    private void CheckRestart(){
        if (Input.GetKeyDown(KeyCode.R)){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Time.timeScale = 1;
        }
    }

    public IEnumerator HandleEndGame(string endString){ //check if the player has died (what conditions? if on no hearts?)
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0;
        endImage.SetActive(true);
        endText.text = endString;
    }
}
