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
    private List<List<Customer>> phaseCustomerL = new List<List<Customer>>();
    public List<Customer> GetCustomerListForPhase(int i){ return phaseCustomerL[i]; }

    //variable by time phase
    private int[] customersPerStage;
    public int[] cps {get{return customersPerStage;}}
    private int customersSpawned;
    private int maxCustomers;
    private float timeUntilCustomer;

    //constant
    private float timeBeforeFirst = 2;
    private float timeAfterLast = 2;
    
    GameManager gm;
    DayManager dm;
    CustomerManager cm;
    EventManager em;
    LevelDesignScript ld;

    // ==============   methods   ==============
    void Awake(){
        em = FindObjectOfType<EventManager>(true);
        //subscribe to events
        em.OnTimeChange += UpdateOnTimeChange;

        gm = FindObjectOfType<GameManager>();
        dm = FindObjectOfType<DayManager>();
        cm = FindObjectOfType<CustomerManager>();
        ld = FindObjectOfType<LevelDesignScript>();

        timer = Instantiate(gm.timerPrefab, this.transform).GetComponent<Timer>();
        em.OnLocationChange += UpdateOnLocationChange;
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
        CreatePCList();
    }

    private void UpdateOnTimeChange(float time, int phase){
        maxCustomers = customersPerStage[phase];
        customersSpawned = 0;

        //start timer
        timeUntilCustomer = (time - timeBeforeFirst - timeAfterLast)/maxCustomers;
        timer.Init(timeBeforeFirst, BeginCustomerTimer);
        timer.StartTimer();
    }

    private void CreatePCList(){
        Debug.Log("creating list");
        for (int phase = 0; phase < customersPerStage.Length; phase++){
            phaseCustomerL.Add(CreateCustomer(phase, customersPerStage[phase]));
        }
        ld.UpdateInfo(customersPerStage.Length);
    }

    private List<Customer> CreateCustomer(int phase, int max){ //create a customer with random ingredients
        List <Customer> phaseList = new List<Customer>();
        
        for (int index = 0; index < max; index++){
            //get a customer profile
            int c = Random.Range(0, customerPrefabs.Count);
            Customer newCustomer = Instantiate(customerPrefabs[c], gm.orderParent).GetComponent<Customer>();
            customerPrefabs.RemoveAt(c);
            newCustomer.phase = phase;
            
            //adjust customer: base, protein, ingredient
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
            newCustomer.CalculateCoins();
            phaseList.Add(newCustomer);
        }
        return phaseList;
    }

    private void SpawnCustomer(){
        //initiate customer
        Customer newCustomer = phaseCustomerL[dm.phase][customersSpawned++];
        cm.LineupCustomer(newCustomer);
        //FIX: delete below; only the cm needs to lineup customer
        newCustomer.gameObject.SetActive(true);
        newCustomer.Init(); 
    }

    public void BeginCustomerTimer(){
        if (customersSpawned >= maxCustomers) return;
        SpawnCustomer();
        //start the timer until next player spawn
        timer.Init(timeUntilCustomer, BeginCustomerTimer);
        timer.StartTimer();
    }
}
