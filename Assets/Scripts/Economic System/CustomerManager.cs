using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    // ==============   variables   ==============
    private Customer selectedCustomer;

    private List<Customer> lineup = new List<Customer>();
    public bool lineUpIsEmpty{get{return lineup.Count == 0;}}
    private int customerShownIndex;

    private Queue<int> freePositions = new Queue<int>();
    private Vector3[] orderPositions = new Vector3[3];
    private Vector3[] customerPositions = new Vector3[3];

    private int lostCustomers;
    [SerializeField] private int maxLostCustomers;
    private int servedCustomers;
    private float coinsMade;
    public float coins {get{return coinsMade;}}
    public int served {get{return servedCustomers;}}
    public int lost {get{return lostCustomers;}}

    private GameManager gm;
    private DayManager dm;
    private EventManager em;
    
    // ==============   methods   ==============
    private void Awake(){
        gm = FindObjectOfType<GameManager>();
        dm = FindObjectOfType<DayManager>();
        GetPositions();

        em = FindObjectOfType<EventManager>();
        em.OnCustomerLeave += UpdateOnCustomerLeave;
        em.OnCoinChange += UpdateOnCoinChange;
    }

    private void GetPositions(){
        int index = 0;
        foreach(Transform c in gm.orderParent.transform){
            freePositions.Enqueue(index);
            orderPositions[index++] = c.position;
        }
        index = 0;
        foreach(Transform c in gm.customerParent.transform){
            customerPositions[index++] = c.position;
        }
        
    }

    private void UpdateOnCustomerLeave(int position, Customer.Mood x){
        freePositions.Enqueue(position);
    }

    //update the amount of money earned and customers served in current phase
    private void UpdateOnCoinChange(Customer customer, float made, float max, int phase){
        RemoveCustomer(customer);
        coinsMade += made;
        if (made <= 0) lostCustomers++;
        else servedCustomers++;
        if (lostCustomers>= maxLostCustomers) StartCoroutine(gm.HandleEndGame("Your customers were unhappy with your service, and complained to your boss. You got fired. Try again... \n Press <R> to retry"));
    }

    private void RemoveCustomer(Customer c){
        lineup.Remove(c);
        customerShownIndex--;
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
        while (freePositions.Count > 0 && customerShownIndex < lineup.Count){
            int pos = freePositions.Dequeue();
            Debug.Log(pos);
            lineup[customerShownIndex++].Init(pos);
        }
    }
}
