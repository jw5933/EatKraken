using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayManager : MonoBehaviour
{
    // ==============   variables   ==============
    //time stages
    [HideInInspector][SerializeField] Timer dayTimer;
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
    [SerializeField] Text timeText;

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

        dayTimer = Instantiate(gm.timerPrefab, this.transform).GetComponent<Timer>();
    }

    public void SkipToNextPhase(){
        Debug.Log("skip to next phase");
        dayTimer.StopTimer();
        HandleTimeChange();
    }

    private void HandleTimeChange(){
        if (phaseIndex+1 >= timePerStage.Length){
            if (!cm.lineUpIsEmpty) isOvertime = true;
            else{
                StartCoroutine(gm.HandleEndGame(string.Format("Congrats! You made it through day {0} in {4}. You have earned {1}, and served {2} customers, losing {3}.", locationDay, cm.coins, cm.served, cm.lost, gm.currLocation)));
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

    private void Init(float time){
        //Debug.Log("location: " + (location != null) + ", daytimer: " +  (dayTimer != null) + ", phaseButton: " +(phaseButton != null) + ", em: " + (em != null));
        if (location.customersPerStage[phaseIndex] <= 0){
            //Debug.Log("timer is a break");
            dayTimer.Init(time, HandleTimeChange);
            dayTimer.StartTimer();
            phaseButton.SetActive(true);
        }
        else phaseButton.SetActive(false);
        em.ChangeTime(time, phaseIndex); //let subscribers know time has changed
    }

    public void ResetVars(){
        dayTimer.StopTimer();
        phaseIndex = 0;
        if (phaseIndex < timePerStage.Length) Init(timePerStage[phaseIndex]);
    }

    private void GoNextDay(){
        locationDay++;
    }
}
