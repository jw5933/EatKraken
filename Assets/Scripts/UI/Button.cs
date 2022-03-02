using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // ==============   variables   ==============
    //indication settings
    private Vector3 myScale = new Vector3(1f,1f,1f);
    private Vector3 hoverScale = new Vector3(0.3f,0.3f,0.3f);

    // ==============   methods   ==============
    //indicate hovered location to player
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
