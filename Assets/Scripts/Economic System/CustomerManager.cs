using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerManager : MonoBehaviour
{
    // ==============   variables   ==============
    private Customer selectedCustomer;

    [SerializeField] private List<Customer> lineup = new List<Customer>();
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

    [SerializeField] private Image[] xs = new Image[3];
    [SerializeField] Color xColor = Color.red;

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

    private void UpdateOnCustomerLeave(Customer customer, int position, Customer.Mood x, int nextPhase){
        RemoveCustomer(customer, nextPhase);
        freePositions.Enqueue(position);
        LineupCustomer(null);
    }

    //update the amount of money earned and customers served in current phase
    private void UpdateOnCoinChange(Customer customer, float made, float max, int phase){
        coinsMade += made;
        if (made <= 0){
            if (lostCustomers < xs.Length) xs[lostCustomers].color = xColor;
            lostCustomers++;
        }
        else servedCustomers++;
        if (lostCustomers>= maxLostCustomers) StartCoroutine(gm.HandleEndGame(false, 2));
    }

    private void RemoveCustomer(Customer c, int nextPhase){
        lineup.Remove(c);
        if (customerShownIndex > 0) customerShownIndex--;
        Destroy(c.gameObject);

        if (dm.endOfDay && lineUpIsEmpty) StartCoroutine(gm.HandleEndGame(true, 0));
    }

    /* public bool ServeCustomer(List<Ingredient> order){ //called by dropobject -> serve
        if (selectedCustomer != null) {
            selectedCustomer.CheckOrder(order);
            return true;
        }
        return false;
    } */

    public void LineupCustomer(Customer c){ //line up the customer behind the current end one -> called by generator
        if (c!= null) lineup.Add(c);
        while (freePositions.Count > 0 && customerShownIndex < lineup.Count){
            int pos = freePositions.Dequeue();
            Debug.Log(pos);
            lineup[customerShownIndex++].Init(pos);
        }
    }

    public bool TrySwapPosition(int given, int wanted){
        if (freePositions.Contains(wanted)){
            freePositions.Enqueue(given);
            int a = freePositions.Dequeue();
            while (a != wanted){
                freePositions.Enqueue(a);
                a = freePositions.Dequeue();
            }
            return true;
        }
        return false;
    }
}
