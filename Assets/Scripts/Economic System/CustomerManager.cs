using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    // ==============   variables   ==============
    [SerializeField] private Customer firstCustomer;
    private Customer selectedCustomer;

    private List<Customer> lineup = new List<Customer>();
    public bool lineUpIsEmpty{get{return lineup.Count == 0;}}

    private int lostCustomers;
    [SerializeField] private int maxLostCustomers;
    private int servedCustomers;
    private float coinsMade;

    private GameManager gm;
    private DayManager dm;
    
    // ==============   methods   ==============
    private void Awake(){
        FindObjectOfType<EventManager>().OnCoinChange += UpdateOnCoinChange;
        gm = FindObjectOfType<GameManager>();
        dm = FindObjectOfType<DayManager>();
    }

    //update the amount of money earned and customers served in current phase
    private void UpdateOnCoinChange(float made, float max, int phase){
        if (made <= 0) lostCustomers++;
        else servedCustomers++;
        if (lostCustomers>= maxLostCustomers) StartCoroutine(gm.HandleEndGame("Your customers were unhappy with your service, and complained to your boss. You got fired. Try again... \n Press <R> to retry"));
    }

    public void RemoveCustomer(Customer c){
        lineup.Remove(c);
        Destroy(c.gameObject);
        if (dm.overtime && lineUpIsEmpty) StartCoroutine(gm.HandleEndGame(string.Format("Congrats! You made it through day {0} in {4}. You have earned {1}, and served {2} customers, losing {3}.", dm.day, coinsMade, servedCustomers, lostCustomers, gm.currLocation)));
    }

    public void ServeCustomer(List<Ingredient> order){ //called by dropobject -> serve
        if (selectedCustomer == null) {
            if (firstCustomer !=null){
                firstCustomer.CheckOrder(order);
                if (lineup.Count > 0){
                    firstCustomer = lineup[0];
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
        }
    }
}
