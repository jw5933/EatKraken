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
    private float stageTime;
    [SerializeField] private bool filler;
    private float [] timePerStage;
    [SerializeField] private Color[] stageColours = new Color[3];
    [SerializeField] private Sprite[] stageIndicatorSprites = new Sprite[3];
    private RectTransform[] stateImages = new RectTransform[3];

    private int currStage;
    public int stage {get{return currStage;}}
    UnityAction callerAction;
    private float myCallerTime;
    public float callerTime {get{return myCallerTime;}}
    float step;

    IEnumerator myCoroutine;
    [SerializeField] GameObject indicator;
    RectTransform myTransform;
    RectTransform indicatorTransform;
    Image indicatorFiller;
    [SerializeField] Image indicatorImage;

    GameManager gm;
    

    // ==============   methods   ==============
    private void WakeUp(){
        Debug.Log("called meter start");
        gm = FindObjectOfType<GameManager>();
        timer = Instantiate(gm.timerPrefab, this.transform).GetComponent<Timer>();
        myTransform = this.GetComponent<RectTransform>();
        indicatorTransform = indicator.GetComponent<RectTransform>();
        indicatorFiller = indicator.GetComponent<Image>();
    }

    public Timer Init(float t1, float t2, float t3, float tStart, UnityAction newAction){
        if (gm == null) WakeUp();
        if (stateImages[0] == null) InitialSetUpVisuals();
        
        myCallerTime = tStart;
        totalTime = t1 + t2 + t3;
        
        callerAction = newAction;
        timePerStage = new float[] {t1, t2, t3};

        SetupVisuals();
        
        float indicatorTime = tStart/totalTime;
        float indicatorPos = 0 - (myTransform.rect.height * indicatorTime);
        
        if (filler) {
            indicatorFiller.fillAmount = 1.0f/totalTime * tStart;
            timer.AddMeter(indicatorFiller, totalTime, HandleAddedTime);
        }
        else{
            //move indicator to position
            indicatorTransform.anchoredPosition = new Vector2(indicatorTransform.anchoredPosition.x, indicatorPos);
            timer.AddMeter(indicatorTransform, myTransform.rect.height, totalTime, HandleAddedTime);
        }

        //get what stage meter should be in
        if ((tStart -= t1) < 0)     currStage = 0; //in stage 1
        else if((tStart -= t2) < 0) currStage = 1; //stage 2
        else if((tStart -= t3) < 0) currStage = 2;

        stageTime = -(tStart);
        indicatorImage.sprite = stageIndicatorSprites[currStage];

        return timer;
    }

    private void InitialSetUpVisuals(){ //called once
        Debug.Log("initial setup of meter");
        Vector2 upperRightAnchor = new Vector2 (0, 1);

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
            Vector3 newPos = cTransform.localPosition;
            newPos.z = 0;
            cTransform.localPosition = newPos;
            cTransform.Rotate(new Vector3(0, 0, -myTransform.rotation.eulerAngles.z));
            c.GetComponent<Image>().color = stageColours[index];

            stateImages[index] = cTransform;
        }
    }

    public void SetupVisuals(){ //setup for changes
        Vector2 startPos = Vector2.zero;

        for (int index = 0; index < stateImages.Length; index++){
            float n = timePerStage[index]/totalTime;
            //move image
            stateImages[index].sizeDelta = new Vector2 (myTransform.sizeDelta.x, myTransform.sizeDelta.y * n);
            stateImages[index].anchoredPosition = startPos;
            startPos -= new Vector2(0, stateImages[index].rect.height);
        }
    }

    public void StartMeter(){
        timer.Init(stageTime, HandleEndStage);
        timer.StartTimer();
    }

    public void StopMeter(){
        if (timer == null) return;
        timer.StopTimer();
        myCallerTime += timer.GetTime();
    }

    private IEnumerator AdjustMeterVisual(){
        float endTime = Time.time + (totalTime - myCallerTime);
        Vector2 endPos = new Vector2 (indicatorTransform.anchoredPosition.x, -myTransform.rect.height);
        float step = (myTransform.rect.height/totalTime) * Time.deltaTime;
        while(Time.time < endTime){
            // lerp meter indicator towards end goal
            indicatorTransform.anchoredPosition = Vector2.MoveTowards(indicatorTransform.anchoredPosition, endPos, step);
            yield return null;
        }
    }

    private void HandleAddedTime(){
        //not using because of scope
    }

    private void HandleEndStage(){
        myCallerTime += timer.GetTime();
        callerAction();
        
        if(currStage + 1 < timePerStage.Length){
            currStage++;
            indicatorImage.sprite = stageIndicatorSprites[currStage];
            timer.Init(timePerStage[currStage], HandleEndStage);
            timer.StartTimer();
        }
    }

    public void ResetVars(){
        indicatorFiller.fillAmount = 0;
    }
}
