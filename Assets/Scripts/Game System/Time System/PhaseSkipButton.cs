using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhaseSkipButton : MonoBehaviour, IPointerClickHandler
{
    private DayManager dm;

    private void Awake(){
        dm = FindObjectOfType<DayManager>();
    }

    public void OnPointerClick(PointerEventData eventData){
        dm.SkipToNextPhase();
    }
}
