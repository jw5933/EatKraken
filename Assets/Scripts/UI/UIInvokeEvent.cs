using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIInvokeEvent : MonoBehaviour
{
    [SerializeField] UnityEvent myEvent;
    
    public void OnMouseUpAsButton(){
        Invoke();
    }

    /* public void AddListener(UnityAction a){
        myEvent.AddListener(a);
    } */

    public void Invoke(){
        if (myEvent != null){
            myEvent.Invoke();
        }
    }
}
