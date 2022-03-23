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
            if (myText !=null) myText.text = "" + time;
            time -= 1;
            yield return new WaitForSeconds(1);
        }
        time = 0;
        if (myText !=null) myText.text = "" + time;
        callerAction();
        //Destroy(this);
    }
}
