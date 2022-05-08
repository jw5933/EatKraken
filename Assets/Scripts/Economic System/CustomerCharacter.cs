using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CustomerCharacter : MonoBehaviour
{
    Customer myCustomer;
    public Customer customer {set{myCustomer = value;}}
    public TextMeshProUGUI tipText;
    public TextMeshProUGUI orderText;

     private void OnMouseUp(){
        //Debug.Log("selected customer");
        myCustomer.OnMouseUpAsButton();
    }
}
