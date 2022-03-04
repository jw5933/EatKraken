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

    //other vars
    [Header("Game Variables")]
    [SerializeField] Location firstLocation;
    [SerializeField] private int theMaxIngredients;
    public int maxIngredients {get{return theMaxIngredients;}}
    

    // ==============   methods   ==============
    
    void Awake()
    {
        //set variables before start is called
        FindObjectOfType<Map>().selectedLocation = firstLocation;
    }
    
    void Update()
    {
        CheckRestart();
    }

    private void CheckRestart(){
        if (Input.GetKeyDown(KeyCode.R)){
            string n = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(n);
        }
    }

    public void HandlePlayerDeath(){ //check if the player has died (what conditions? if on no hearts?)

    }
}
