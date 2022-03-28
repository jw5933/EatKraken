using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Timer: MonoBehaviour
{
    // ==============   variables   ==============
    private float initialTime;
    private float time;
    UnityAction callerAction;
    
    IEnumerator myCoroutine;
    Text myText;

    //for meter
    UnityAction meterAction;
    RectTransform indicatorTransform;
    Image indicatorImage;
    RectTransform meterTransform;
    float endPosY;
    float totalTime;
    
    
    // ==============   methods   ==============
    public void Init (float newTime, UnityAction newAction){
        callerAction = newAction;
        initialTime = newTime;
        time = newTime;
    }
    public void Init (float newTime, UnityAction newAction, Text text){
        Init(newTime, newAction);
        myText = text;
    }

    public void AddMeter(Image newIndicator, float tTime, UnityAction newMeterAction){
        indicatorImage = newIndicator;
        totalTime = tTime;
        meterAction = newMeterAction;
    }

    public void AddMeter(RectTransform newIndicator, float pos, float tTime, UnityAction newMeterAction){
        indicatorTransform = newIndicator;
        endPosY = pos;
        totalTime = tTime;
        meterAction = newMeterAction;
    }

    public void AddToTimer(float t){
        time += t;
    }

    public void StartTimer(){
        myCoroutine = DecrementTimer();
        StartCoroutine(myCoroutine);
    }

    public void StopTimer(){
        if (myCoroutine != null) StopCoroutine(myCoroutine);
    }

    public float GetTime(){
        return (initialTime - time);
    }

    private IEnumerator DecrementTimer(){ //coroutine for timer
        while (time > 0){
            if (myText !=null) 
                myText.text = "" + time.ToString("F0");

            if (indicatorImage != null)
                indicatorImage.fillAmount += 1.0f/totalTime * Time.deltaTime;

            if (indicatorTransform !=null){
                Vector2 endPos = new Vector2 (indicatorTransform.anchoredPosition.x, -endPosY);
                float step = (endPosY/totalTime) * Time.deltaTime;
                indicatorTransform.anchoredPosition = Vector2.MoveTowards(indicatorTransform.anchoredPosition, endPos, step);
            }

            time -= Time.deltaTime;
            yield return null;
        }
        time = 0;
        callerAction();
    }
}
