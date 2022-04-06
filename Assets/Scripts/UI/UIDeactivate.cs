using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class UIDeactivate : MonoBehaviour

{
    private UnityAction action;

    public void AddAction(UnityAction a){
        action = a;
    }

    public void StopAnim(){
        gameObject.SetActive(false);
        action();
    }
}
