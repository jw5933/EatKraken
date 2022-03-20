using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Timer: MonoBehaviour
{
    // ==============   variables   ==============
    private float time;
    UnityAction callerAction;
    IEnumerator myCoroutine;
    Text myText;
    
    // ==============   methods   ==============
    public void Init (float newTime, UnityAction newAction){
        callerAction = newAction;
        time = newTime;
    }
    public void Init (float newTime, UnityAction newAction, Text text){
        callerAction = newAction;
        time = newTime;
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

    private IEnumerator DecrementTimer(){ //coroutine for timer
        while (time > 0){
            if (myText !=null) myText.text = "" + time;
            time -= 1;
            yield return new WaitForSeconds(1);
        }
        if (myText !=null) myText.text = "" + time;
        callerAction();
        //Destroy(this);
    }
}
