using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderButton : MonoBehaviour
{
    //indicate hovered location to player
    private UnityAction action;
    private Vector3 myScale = new Vector3(1f,1f,1f);
    private Vector3 hoverScale = new Vector3(0.3f,0.3f,0.3f);

    private void OnMouseDown(){
        Invoke();
    }

    public void AddAction(UnityAction a){
        action = a;
    }
    
    private void Invoke(){
        if (action != null) action();
    }

/*     private void OnMouseEnter(){
        ChangeScale(hoverScale);
    }
    private void OnMouseExit(){
        ResetScale();
    } */

    private void ChangeScale(Vector3 deltaScale){
        //scale up/down by scale change
        this.transform.localScale += deltaScale;
    }
    private void ResetScale(){
        this.transform.localScale = myScale;
    }
}
