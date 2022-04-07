using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIActivate : MonoBehaviour
{
    private UnityAction action;
    //FIX: should add this script and set object upon my object spawn
    [SerializeField] GameObject myObject;
    public GameObject obj {set{myObject = value;}}

    public void AddAction(UnityAction a){
        action = a;
    }
    
    public void Activate(){
        this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        action();
    }
}
