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
    Vector2 endPos;
    float step;
    
    
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
    public void Init (float newTime, RectTransform newIndicator, float newStep, Vector2 newEndPos, UnityAction newMeterAction, UnityAction newAction){
        Init(newTime, newAction);
        indicatorTransform = newIndicator;
        step = newStep;
        endPos = newEndPos;
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
        return initialTime - time;
    }

    private IEnumerator DecrementTimer(){ //coroutine for timer
        while (time > 0){
            if (myText !=null) myText.text = "" + time.ToString("F0");
            time -= Time.deltaTime;
            yield return null;
        }
        time = 0;
        callerAction();
        //Destroy(this);
    }
}
