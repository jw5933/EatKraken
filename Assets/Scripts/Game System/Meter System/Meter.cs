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
    [SerializeField] GameObject indicator;

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
        //colour in the meter
        Vector3 pos = new Vector3(this.transform.position.x, this.GetComponent<Renderer>().bounds.max.y, this.transform.position.z);
        for (int index = 0; index < stageColours.Length; index++){
            float n = timePerStage[index]/totalTime;
            
            GameObject c = Instantiate(this.gameObject, this.transform);
            Destroy(c.GetComponent<Meter>());
            
            c.transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y * n, transform.localScale.z);
            c.transform.position = pos;
            
            Bounds b = GetComponent<Renderer>().bounds;
            c.transform.position = pos - new Vector3 (0, b.size.y, 0);
            pos += new Vector3(0, b.size.y, 0);

            c.GetComponent<SpriteRenderer>().color = stageColours[index];
        }
    }

    public void StartMeter(){
        myCoroutine = AdjustMeterVisual();
        StartCoroutine(myCoroutine);
        timer.StartTimer();
    }

     public void StopMeter(){
        if (myCoroutine != null) StopCoroutine(myCoroutine);
        timer.StopTimer();
    }

    private IEnumerator AdjustMeterVisual(){
        float currTime = 0;
        Vector3 endPos = new Vector3 (indicator.transform.position.x, this.GetComponent<Renderer>().bounds.min.y, indicator.transform.position.z);
         while(currTime < totalTime){
            currTime += Time.deltaTime;
            // lerp meter indicator towards end goal
            indicator.transform.position = Vector3.Lerp(indicator.transform.position, endPos, (currTime / totalTime));
            yield return null;
        }
    }

    private void HandleEndStage(){
        callerAction();
        if(currStage + 1 < timePerStage.Length){
            timer.Init(timePerStage[++currStage], HandleEndStage);
            timer.StartTimer();
        }
    }

}
