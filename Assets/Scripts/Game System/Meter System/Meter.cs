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

    private int currStage;
    UnityAction callerAction;
    GameManager gm;

    IEnumerator myCoroutine;
    [SerializeField] GameObject indicator;
    RectTransform myTransform;
    RectTransform indicatorTransform;

    // ==============   methods   ==============
    private void Awake(){
        gm = FindObjectOfType<GameManager>();
        timer = Instantiate(gm.timerPrefab, this.transform).GetComponent<Timer>();
        myTransform = this.GetComponent<RectTransform>();
        indicatorTransform = indicator.GetComponent<RectTransform>();
    }

    public Timer Init(float t1, float t2, float t3, UnityAction newAction){
        totalTime = t1 + t2 + t3;
        Debug.Log("total time: " + totalTime);
        callerAction = newAction;
        timePerStage = new float[] {t1, t2, t3};
        timer.Init(timePerStage[currStage], HandleEndStage);
        SetupVisuals();
        return timer;
    }

    private void SetupVisuals(){
        Rect b = myTransform.rect;
        Vector2 startPos = Vector2.zero;
        Vector2 upperRightAnchor = new Vector2 (0, 1);
        //move indicator to start
        /* indicatorTransform.anchorMax = upperRightAnchor;
        indicatorTransform.anchorMin = upperRightAnchor;
        indicatorTransform.pivot = upperRightAnchor;
        indicatorTransform.anchoredPosition = new Vector2(indicatorTransform.anchoredPosition.x, 0); */
        //colour in the meter
        for (int index = 0; index < stageColours.Length; index++){
            float n = timePerStage[index]/totalTime;

            GameObject c = Instantiate(this.gameObject, this.transform);
            c.transform.SetSiblingIndex(0);
            Destroy(c.GetComponent<Meter>());
            foreach(Transform child in c.transform){
                Destroy(child.gameObject);
            }

            RectTransform cTransform = c.GetComponent<RectTransform>();
            cTransform.anchorMax = upperRightAnchor;
            cTransform.anchorMin = upperRightAnchor;
            cTransform.pivot = upperRightAnchor;

            cTransform.sizeDelta = new Vector2 (cTransform.sizeDelta.x, cTransform.sizeDelta.y * n);
            cTransform.Rotate(new Vector3(0, 0, -myTransform.rotation.eulerAngles.z));

            cTransform.anchoredPosition = startPos;
            startPos -= new Vector2(0, cTransform.rect.height);
            c.GetComponent<Image>().color = stageColours[index];
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
        callerAction();
        if(currStage + 1 < timePerStage.Length){
            timer.Init(timePerStage[++currStage], HandleEndStage);
            timer.StartTimer();
        }
    }

}
