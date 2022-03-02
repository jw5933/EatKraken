using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActivate : MonoBehaviour
{
    //FIX: should add this script and set object upon my object spawn
    [SerializeField] GameObject myObject;
    public GameObject obj {set{myObject = value;}}

    public void Activate(){
        myObject.SetActive(true);
        this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }
}
