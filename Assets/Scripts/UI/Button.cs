using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // ==============   variables   ==============
    //indication settings
    private Vector3 myScale = new Vector3(1f,1f,1f);
    private Vector3 hoverScale = new Vector3(0.3f,0.3f,0.3f);

    // ==============   methods   ==============
    //indicate hovered location to player
    private UnityAction action;

    public virtual void OnPointerClick(PointerEventData eventData){
        Invoke();
    }

    public void AddAction(UnityAction a){
        action = a;
    }
    
    private void Invoke(){
        if (action != null) action();
    }

    public virtual void OnPointerEnter(PointerEventData eventData){
        ChangeScale(hoverScale);
    }
    public virtual void OnPointerExit(PointerEventData eventData){
        ResetScale();
    }

    public void ChangeScale(Vector3 deltaScale){
        //scale up/down by scale change
        this.transform.localScale += deltaScale;
    }
    public void ResetScale(){
        this.transform.localScale = myScale;
    }
}
