using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Meter : MonoBehaviour
{
    // ==============   variables   ==============
    private Timer timer;
    private float totalTime;
    [SerializeField] private float [] timePerStage;
    [SerializeField] private Color[] stageColours = new Color[3];

    private int currStage;
    UnityAction callerAction;
    GameManager gm;

    IEnumerator myCoroutine;
    bool changedStage;
    
    // ==============   methods   ==============
    private void Awake(){
        gm = FindObjectOfType<GameManager>();
        timer = Instantiate(gm.timerPrefab, this.transform).GetComponent<Timer>();
    }

    public Timer Init(float t1, float t2, float t3, UnityAction newAction){
        totalTime = t1 + t2 + t3;
        callerAction = newAction;
        timePerStage = new float[] {t1, t2, t3};
        timer.Init(timePerStage[currStage], HandleEndStage);
        SetupVisuals();
        return timer;
    }

    private void SetupVisuals(){
        /*
        colour in the meter
            calculate by degrees
        */
    }

    public void StartMeter(){
        myCoroutine = AdjustMeterVisual();
        StartCoroutine(myCoroutine);
    }

     public void StopMeter(){
        if (myCoroutine != null) StopCoroutine(myCoroutine);
    }

    private IEnumerator AdjustMeterVisual(){
       /*  while(currTime < totalTime){
            currTime -= Time.deltaTime;
            // lerp meter indicator towards end goal
            yield return null;
        }
        changedStage = false; */
        return null;
    }

    private void HandleEndStage(){
        /* 
        if not at end stage
        initiate next stage and start the timer
        let caller know that its stage has changed
        */
        if(currStage + 1 < timePerStage.Length){
            timer.Init(timePerStage[++currStage], HandleEndStage);
            timer.StartTimer();
        }
    }

}
