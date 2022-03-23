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
        InitialSetUpVisuals();
    }

    public Timer Init(float t1, float t2, float t3, float tStart, UnityAction newAction){
        myCallerTime = tStart;
        totalTime = t1 + t2 + t3;
        Debug.Log("total time: " + totalTime);
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
        timer.Init(timePerStage[currStage], HandleEndStage);
        SetupVisuals(indicatorPos);
        return timer;
    }

    private void InitialSetUpVisuals(){
        Vector2 upperRightAnchor = new Vector2 (0, 1);
        //move indicator to start
        indicatorTransform.anchoredPosition = new Vector2(indicatorTransform.anchoredPosition.x, 0);

        for (int index = 0; index < stateImages.Length; index++){
            //create each section of meter
            GameObject c = Instantiate(this.gameObject, this.transform);
            c.transform.SetSiblingIndex(0);
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
        Rect b = myTransform.rect;
        Vector2 startPos = Vector2.zero;

        //move indicator to position
        indicatorTransform.anchoredPosition = new Vector2(indicatorTransform.anchoredPosition.x, indicatorPos);

        for (int index = 0; index < stateImages.Length; index++){
            float n = timePerStage[index]/totalTime;
            //move image
            stateImages[index].sizeDelta = new Vector2 (stateImages[index].sizeDelta.x, stateImages[index].sizeDelta.y * n);
            stateImages[index].anchoredPosition = startPos;
            startPos -= new Vector2(0, stateImages[index].rect.height);
        }
    }

    public void StartMeter(){
        myCoroutine = AdjustMeterVisual();
        StartCoroutine(myCoroutine);
        timer.StartTimer();
    }

     public void StopMeter(){
        if (myCoroutine != null) StopCoroutine(myCoroutine);
        myCallerTime += timer.GetTime();
        timer.StopTimer();
    }

    private IEnumerator AdjustMeterVisual(){
        float endTime = Time.time + totalTime;
        Vector2 endPos = new Vector2 (indicatorTransform.anchoredPosition.x, -myTransform.rect.height);
        float step = (myTransform.rect.height/totalTime) * Time.deltaTime;
         while(Time.time < endTime){
            // lerp meter indicator towards end goal
            indicatorTransform.anchoredPosition = Vector2.MoveTowards(indicatorTransform.anchoredPosition, endPos, step);
            yield return null;
        }
    }

    private void HandleEndStage(){
        myCallerTime += timer.GetTime();
        callerAction();
        if(currStage + 1 < timePerStage.Length){
            timer.Init(timePerStage[++currStage], HandleEndStage);
            timer.StartTimer();
        }
    }
}
