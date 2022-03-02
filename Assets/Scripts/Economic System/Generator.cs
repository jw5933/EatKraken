using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    // ==============   variables   ==============
    private List <Ingredient> baseIngredientPrefabs;
    private List <Ingredient> ingredientPrefabs;
    private List <Ingredient> proteinPrefabs;
    
    private List <Customer> customerPrefabs;

    //timer vars
    private Timer timer;
    private int[] customersPerStage;
    private int customersSpawned;
    private int maxCustomers;
    private float timeUntilCustomer;
    [SerializeField] private float timeBeforeFirst;
    [SerializeField] private float timeAfterLast;
    
    GameManager gm;
    DayManager dm;
    CustomerManager cm;

    // ==============   methods   ==============

    void Awake(){
        EventManager ev = FindObjectOfType<EventManager>(true);
        //subscribe to events
        ev.OnLocationChange += UpdateOnLocationChange;
        ev.OnTimeChange += UpdateOnTimeChange;

        gm = FindObjectOfType<GameManager>();
        dm = FindObjectOfType<DayManager>();
        cm = FindObjectOfType<CustomerManager>();
        timer = Instantiate(gm.timerPrefab, this.transform).GetComponent<Timer>();
    }

    private void UpdateOnLocationChange(Location next){
        Debug.Log("next: " + next.gameObject.name);
        //update the generator
        baseIngredientPrefabs = next.baseIngredients;
        ingredientPrefabs = next.ingredients;
        customerPrefabs = next.customers;
        customersPerStage = next.customersPerStage;
        proteinPrefabs = next.proteins;

        dm.ResetVars();
    }

    private void UpdateOnTimeChange(float time, int phase){
        maxCustomers = customersPerStage[phase];
        timeUntilCustomer = (time - timeBeforeFirst - timeAfterLast)/maxCustomers;
        customersSpawned = 0;
        timer.Init(timeBeforeFirst, BeginTimer);
        timer.StartTimer();
    }

    private void CreateCustomer(){ //create a customer with random ingredients
        if (customersSpawned >= maxCustomers)return;
        Customer newCustomer;

        //get a new customer
        int c = Random.Range(0, customerPrefabs.Count);
        newCustomer = Instantiate(customerPrefabs[c], gm.orderParent).GetComponent<Customer>();
        customerPrefabs.RemoveAt(c);

        //adjust customer
        int num = gm.maxIngredients;
        int b = Random.Range(0, baseIngredientPrefabs.Count);
        newCustomer.AddToOrder(baseIngredientPrefabs[b].initialSprite, baseIngredientPrefabs[b].name, baseIngredientPrefabs[b].price);
        num--;

        int p = Random.Range(0, proteinPrefabs.Count);
        newCustomer.AddToOrder(proteinPrefabs[p].initialSprite, proteinPrefabs[p].name, proteinPrefabs[p].price);
        num--;

        for (int n = num; n >= 0; n--){
            int i = Random.Range(0, ingredientPrefabs.Count);
            newCustomer.AddToOrder(ingredientPrefabs[i].initialSprite, ingredientPrefabs[i].name, ingredientPrefabs[i].price);
        }

        //initiate customer
        cm.LineupCustomer(newCustomer);
        newCustomer.Init(); //FIX: delete; only the cm needs to lineup customer
        customersSpawned++;
    }

    public void BeginTimer(){
        if (customersSpawned >= maxCustomers) return;
        CreateCustomer();
        //start the timer until next player spawn
        timer.Init(timeUntilCustomer, BeginTimer);
        timer.StartTimer();
    }
}
