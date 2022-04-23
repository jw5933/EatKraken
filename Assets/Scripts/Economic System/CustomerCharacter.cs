using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomerCharacter : MonoBehaviour
{
    Customer myCustomer;
    public Customer customer {set{myCustomer = value;}}

     private void OnMouseUp(){
        //Debug.Log("selected customer");
        myCustomer.OnMouseUpAsButton();
    }
}
