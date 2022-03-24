using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Meter : MonoBehaviour
{
    // ==============   variables   ==============
    private Timer timer;
    private float totalTime;
    [SerializeField] private float [] timePerStage;
    [SerializeField] private Color[] stageColours = new Color[3];
    private RectTransform[] stateImages = new RectTransform[3];

    private int currStage;
    UnityAction callerAction;
    private float myCallerTime;
    public float callerTime {get{return myCallerTime;}}
    float step;

    IEnumerator myCoroutine;
    [SerializeField] GameObject indicator;
    RectTransform myTransform;
    RectTransform indicatorTransform;

    GameManager gm;
    

    // ==============   methods   ==============
    private void Awake(){
        gm = FindObjectOfType<GameManager>();
        timer = Instantiate(gm.timerPrefab, this.transform).GetComponent<Timer>();
        myTransform = this.GetComponent<RectTransform>();
        indicatorTransform = indicator.GetComponent<RectTransform>();
    }

    public Timer Init(float t1, float t2, float t3, float tStart, UnityAction newAction){
        if (stateImages[0] == null) InitialSetUpVisuals();
        
        myCallerTime = tStart;
        totalTime = t1 + t2 + t3;
        
        callerAction = newAction;
        timePerStage = new float[] {t1, t2, t3};

        //get what stage meter should be in and the indicator position
        float indicatorTime = tStart/totalTime;
        float indicatorPos = myTransform.rect.height * indicatorTime;
        
        if ((tStart -= t1) < 0){ //in stage 1
            currStage = 0;
        }
        else if((tStart -= t2) < 0){ //stage 2
            currStage = 1;
        }
        else{ //stage 3
            currStage = 2;
        }
        SetupVisuals(indicatorPos);
        return timer;
    }

    private void InitialSetUpVisuals(){
        Debug.Log("initial setup of meter");
        Vector2 upperRightAnchor = new Vector2 (0, 1);
        step = (myTransform.rect.height/totalTime) * Time.deltaTime;

        for (int index = 0; index < 3; index++){
            //create each section of meter
            GameObject c = Instantiate(this.gameObject, this.transform);
            c.transform.SetSiblingIndex(0);
            c.transform.localScale = Vector3.one;
            Destroy(c.GetComponent<Meter>());
            foreach(Transform child in c.transform){
                Destroy(child.gameObject);
            }
            //adjust meter and add it to images
            RectTransform cTransform = c.GetComponent<RectTransform>();
            cTransform.anchorMax = upperRightAnchor;
            cTransform.anchorMin = upperRightAnchor;
            cTransform.pivot = upperRightAnchor;
            cTransform.Rotate(new Vector3(0, 0, -myTransform.rotation.eulerAngles.z));
            c.GetComponent<Image>().color = stageColours[index];

            stateImages[index] = cTransform;
        }
    }

    private void SetupVisuals(float indicatorPos){
        Vector2 startPos = Vector2.zero;

        //move indicator to position
        indicatorTransform.anchoredPosition = new Vector2(indicatorTransform.anchoredPosition.x, -indicatorPos);

        for (int index = 0; index < stateImages.Length; index++){
            float n = timePerStage[index]/totalTime;
            //move image
            stateImages[index].sizeDelta = new Vector2 (myTransform.sizeDelta.x, myTransform.sizeDelta.y * n);
            stateImages[index].anchoredPosition = startPos;
            startPos -= new Vector2(0, stateImages[index].rect.height);
        }
    }

    public void StartMeter(){
        myCoroutine = AdjustMeterVisual();
        StartCoroutine(myCoroutine);

        timer.Init(timePerStage[currStage], HandleEndStage);
        timer.StartTimer();
    }

     public void StopMeter(){
        if (myCoroutine != null) StopCoroutine(myCoroutine);
        myCallerTime += timer.GetTime();
        timer.StopTimer();
    }

    private IEnumerator AdjustMeterVisual(){
        float endTime = Time.time + (totalTime - myCallerTime);
        Vector2 endPos = new Vector2 (indicatorTransform.anchoredPosition.x, -myTransform.rect.height);
        float step = (myTransform.rect.height/totalTime) * Time.deltaTime;
        while(Time.time < endTime){
            // lerp meter indicator towards end goal
            indicatorTransform.localPosition = Vector2.MoveTowards(indicatorTransform.localPosition, endPos, step);
            yield return null;
        }
    }

    /* 
    indicatorTransform
    step
    endPos
     */
    private void HandleAddedTime(){

    }

    private void HandleEndStage(){
        myCallerTime += timePerStage[currStage];
        callerAction();
        
        if(currStage + 1 < timePerStage.Length){
            currStage++;
            Vector2 endPos = new Vector2 (indicatorTransform.anchoredPosition.x, - stateImages[currStage].rect.height);
            timer.Init(timePerStage[currStage], HandleEndStage);
            //timer.Init(timePerStage[currStage], indicatorTransform, step, endPos, HandleAddedTime, HandleEndStage);
            timer.StartTimer();
        }
    }
}
