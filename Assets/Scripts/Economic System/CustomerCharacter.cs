using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerCharacter : MonoBehaviour
{
    Customer myCustomer;
    public Customer customer {set{myCustomer = value;}}

    public void OnMouseUp(){ //serve
        myCustomer.OnMouseUp();
    }
}
