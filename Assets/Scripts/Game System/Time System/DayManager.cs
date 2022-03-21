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
    private int phaseIndex = -1;

    public int phase {get{return phaseIndex;}}
    public int numOfPhases {get{return timePerStage.Length;}}

    //FIX: delete
    [SerializeField] SpriteRenderer customerBackground;

    [SerializeField] Text timeText;

    GameManager gm;
    EventManager em;
    CustomerManager cm;
    LevelDesignScript ld;

    // ==============   methods   ==============
    public void Awake(){
        em = FindObjectOfType<EventManager>();
        gm = FindObjectOfType<GameManager>();
        cm = FindObjectOfType<CustomerManager>();
        ld = FindObjectOfType<LevelDesignScript>();

        dayTimer = Instantiate(gm.timerPrefab, this.transform).GetComponent<Timer>();
    }

    private void HandleDayChange(){
        if (phaseIndex+1 >= timePerStage.Length){
            if (!cm.lineUpIsEmpty) WorkOvertime();
            //FIX: show working overtime
            return;
        }
        phaseIndex++;
        //FIX: change the visuals
        if (phaseIndex < timeStageImages.Length) customerBackground.sprite = timeStageImages[phaseIndex];
        if (phaseIndex < timePerStage.Length) Init(timePerStage[phaseIndex]);
    }

    private void WorkOvertime(){ //FIX
        Debug.Log("working overtime");
    }

    private void Init(float time){
        //dayTimer = Instantiate(gm.timerPrefab, this.transform).GetComponent<Timer>();
        dayTimer.Init(time, HandleDayChange, timeText);
        dayTimer.StartTimer();
        em.ChangeTime(time, phaseIndex); //let subscribers know time has changed
    }

    public void ResetVars(){
        dayTimer.StopTimer();
        phaseIndex = 0;
        if (phaseIndex < timePerStage.Length) Init(timePerStage[phaseIndex]);
    }
}
