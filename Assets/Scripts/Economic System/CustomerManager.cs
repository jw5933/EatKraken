using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    // ==============   variables   ==============
    [SerializeField] private Customer firstCustomer;
    [SerializeField] private Customer selectedCustomer;

    private List<Customer> lineup = new List<Customer>();
    public bool lineUpIsEmpty{get{return lineup.Count == 0;}}

    private int lostCustomers;
    [SerializeField] private int maxLostCustomers;
    private int servedCustomers;
    private float coinsMade;
    public float coins {get{return coinsMade;}}
    public int served {get{return servedCustomers;}}
    public int lost {get{return lostCustomers;}}

    private GameManager gm;
    private DayManager dm;
    
    // ==============   methods   ==============
    private void Awake(){
        FindObjectOfType<EventManager>().OnCoinChange += UpdateOnCoinChange;
        gm = FindObjectOfType<GameManager>();
        dm = FindObjectOfType<DayManager>();
    }

    //update the amount of money earned and customers served in current phase
    private void UpdateOnCoinChange(Customer customer, float made, float max, int phase){
        RemoveCustomer(customer);
        coinsMade += made;
        if (made <= 0) lostCustomers++;
        else servedCustomers++;
        if (lostCustomers>= maxLostCustomers) StartCoroutine(gm.HandleEndGame("Your customers were unhappy with your service, and complained to your boss. You got fired. Try again... \n Press <R> to retry"));
    }

    public void SelectCustomer(Customer c){
        if (selectedCustomer !=null) selectedCustomer.Deselect();
        selectedCustomer = c;
        selectedCustomer.Select();
    }

    private void RemoveCustomer(Customer c){
        lineup.Remove(c);
        Destroy(c.gameObject);
        if (dm.endOfDay && lineUpIsEmpty) StartCoroutine(gm.HandleEndGame(string.Format("Congrats! You made it through day {0} in {4}. You have earned {1}, and served {2} customers, losing {3}.", dm.day, coinsMade, servedCustomers, lostCustomers, gm.currLocation)));
    }

    public bool ServeCustomer(List<Ingredient> order){ //called by dropobject -> serve
        if (selectedCustomer != null) {
            selectedCustomer.CheckOrder(order);
            return true;
        }
        return false;
    }

    public void LineupCustomer(Customer c){ //line up the customer behind the current end one
        lineup.Add(c);
        if (firstCustomer == null){
            firstCustomer = lineup[0];
        }
    }
}
