using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayManager : MonoBehaviour
{
    // ==============   variables   ==============
    //time stages
    [HideInInspector][SerializeField] Timer dayTimer;
    [SerializeField] GameObject meterPrefab;

    private Meter meter;
    private RectTransform meterController;
    private Vector2 shrunkSize;
    private Vector2 normSize;
    RectTransform meterTransform;
    RectTransform myTransform;

    private float[] timePerStage;
    public float[] timeStage {set{timePerStage = value;}}
    [SerializeField] Sprite[] timeStageImages;
    bool isOvertime;
    public bool overtime {get{return isOvertime;}}
    public bool endOfDay {get{return (phaseIndex+1 >= timePerStage.Length);}}

    private int phaseIndex = -1;
    public int phase {get{return phaseIndex;}}
    public int numOfPhases {get{return timePerStage.Length;}}
    private GameObject phaseButton;

    [SerializeField] SpriteRenderer customerBackground;

    //FIX: save to location
    private int locationDay = 1;
    public int day{get{return locationDay;}}

    GameManager gm;
    EventManager em;
    CustomerManager cm;
    LevelDesignScript ld;
    Location location;
    public Location loc {set{location = value;}}

    // ==============   methods   ==============
    public void Awake(){
        em = FindObjectOfType<EventManager>();
        //em.OnLocationChange += UpdateOnLocationChange;

        gm = FindObjectOfType<GameManager>();
        cm = FindObjectOfType<CustomerManager>();
        ld = FindObjectOfType<LevelDesignScript>();
        phaseButton = FindObjectOfType<PhaseSkipButton>(true).gameObject;

        myTransform = this.gameObject.GetComponent<RectTransform>();

        meterController = transform.GetChild(0).GetComponent<RectTransform>();
        shrunkSize = new Vector2 (meterController.sizeDelta.y, meterController.sizeDelta.x);
        normSize = new Vector2 (myTransform.sizeDelta.y, myTransform.sizeDelta.x);
        meterController.sizeDelta = myTransform.sizeDelta;

        meter = Instantiate(meterPrefab, meterController.transform).GetComponent<Meter>();
        meter.transform.SetSiblingIndex(0);
        meterTransform = meter.gameObject.GetComponent<RectTransform>();
        meterTransform.sizeDelta = normSize;
        meter.gameObject.SetActive(false);
    }

    public void SkipToNextPhase(){
        Debug.Log("skip to next phase");
        if (meter.gameObject.activeSelf) meter.StopMeter();
        HandleTimeChange();
    }

    private void HandleTimeChange(){
        if (phaseIndex+1 >= timePerStage.Length){
            if (!cm.lineUpIsEmpty) isOvertime = true;
            else{
                StartCoroutine(gm.HandleEndGame(true, 0, string.Format("Congrats! You made it through day {0} in {4}. You have earned {1}, and served {2} customers, losing {3}.", locationDay, cm.coins, cm.served, cm.lost, gm.currLocation)));
            }
            //FIX: show working overtime
            return;
        }
        Debug.Log("handle day change");
        phaseIndex++;
        //FIX: change the visuals
        if (phaseIndex < timeStageImages.Length) customerBackground.sprite = timeStageImages[phaseIndex];
        if (phaseIndex < timePerStage.Length) Init(timePerStage[phaseIndex]);
    }

    private void HandleMeterChange(){
        if (meter.stage >= 2){ 
            HandleTimeChange();
        }
    }

    public void ShrinkMeter(){
        meter.StopMeter();
        float currTime = meter.callerTime;
        meterController.sizeDelta = new Vector2(shrunkSize.y, shrunkSize.x);
        meterTransform.sizeDelta = shrunkSize;
        meter.SetupVisuals();
        meter.Init(timePerStage[phaseIndex], 0, 0, currTime, HandleMeterChange);
        meter.StartMeter();
    }

    public void ResizeMeter(){
        meter.StopMeter();
        float currTime = meter.callerTime;
        meterController.sizeDelta = new Vector2(normSize.y, normSize.x);
        meterTransform.sizeDelta = normSize;
        meter.SetupVisuals();
        meter.Init(timePerStage[phaseIndex], 0, 0, currTime, HandleMeterChange);
        meter.StartMeter();
    }

    private void Init(float time){
        //Debug.Log("location: " + (location != null) + ", daytimer: " +  (meter != null) + ", phaseButton: " +(phaseButton != null) + ", em: " + (em != null));
        if (location.customersPerStage[phaseIndex] <= 1){
            //Debug.Log("timer is a break");
            meter.gameObject.SetActive(true);
            meter.Init(time, 0, 0, 0, HandleMeterChange);
            meter.StartMeter();
            phaseButton.SetActive(true);
        }
        else{
            meter.gameObject.SetActive(false);
            phaseButton.SetActive(false);
        }
        em.ChangeTime(time, phaseIndex); //let subscribers know time has changed
    }

    public void ResetVars(){
        meter.StopMeter();
        phaseIndex = 0;
        if (phaseIndex < timePerStage.Length) Init(timePerStage[phaseIndex]);
    }

    private void GoNextDay(){
        locationDay++;
    }
}
