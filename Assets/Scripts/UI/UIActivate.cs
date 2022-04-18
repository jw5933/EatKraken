using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIActivate : MonoBehaviour
{
    private UnityAction action;
    [SerializeField] UnityEvent myEvent;

    public void AddAction(UnityAction a){
        action = a;
    }
    
    public void AddListener(UnityAction a){
        myEvent.AddListener(a);
    }
    public void Activate(){
        //this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        if (action != null) action();
        if (myEvent != null){
            myEvent.Invoke();
            myEvent.RemoveAllListeners();
        }
    }
}
