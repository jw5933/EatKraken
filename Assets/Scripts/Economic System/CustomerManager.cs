using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
the customer manager object should also hold the generator
*/
public class CustomerManager : MonoBehaviour
{
    // ==============   variables   ==============
    [SerializeField] private Customer firstCustomer;
    private Customer selectedCustomer;
    //private Generator g;
    private List<Customer> lineup = new List<Customer>();
    public bool lineUpIsEmpty{get{return lineup.Count == 0;}}
    
    // ==============   methods   ==============
    void Awake()
    {
    }

    public void ServeCustomer(List<string> order){
        if (selectedCustomer == null) {
            if (firstCustomer !=null){
                firstCustomer.CheckOrder(order);
                if (lineup.Count > 0){
                    firstCustomer = lineup[0];
                    lineup.RemoveAt(0);
                }
                else firstCustomer = null;
            }
        }
        else selectedCustomer.CheckOrder(order);
    }

    public void LineupCustomer(Customer c){ //line up the customer behind the current end one
        lineup.Add(c);
        if (firstCustomer == null){
            firstCustomer = lineup[0];
            lineup.RemoveAt(0);
        }
    }
}
