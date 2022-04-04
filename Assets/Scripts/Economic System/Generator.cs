using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    // ==============   variables   ==============
    private List <Ingredient> baseIngredientPrefabs;
    private List <Ingredient> carbIngredientPrefabs;
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
        carbIngredientPrefabs = next.carbIngredients;
        ingredientPrefabs = next.ingredients;
        customerPrefabs = next.customers;
        customersPerStage = next.customersPerStage;
        proteinPrefabs = next.proteins;

        dm.timeStage = next.timeStages;
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
        //Debug.Log("creating list");
        for (int phase = 0; phase < customersPerStage.Length; phase++){
            phaseCustomerL.Add(CreateCustomer(phase, customersPerStage[phase]));
        }
        if (ld !=null) ld.UpdateInfo(customersPerStage.Length);
        dm.ResetVars();
    }

    private List<Customer> CreateCustomer(int phase, int max){ //create a customer with random ingredients
        List <Customer> phaseList = new List<Customer>();
        
        for (int index = 0; index < max; index++){
            //get a customer profile
            int cs = Random.Range(0, customerPrefabs.Count);
            Customer newCustomer = Instantiate(customerPrefabs[cs], gm.orderParent).GetComponent<Customer>();
            customerPrefabs.RemoveAt(cs);
            newCustomer.phase = phase;
            
            //adjust customer: base, carb, protein, ingredient
            int num = gm.maxIngredients;
            int b = Random.Range(0, baseIngredientPrefabs.Count);
            newCustomer.AddToOrder(baseIngredientPrefabs[b].orderSprite,
            baseIngredientPrefabs[b].initialSprite, baseIngredientPrefabs[b].name, 
            baseIngredientPrefabs[b].price);

            int c = Random.Range(0, carbIngredientPrefabs.Count);
            newCustomer.AddToOrder(carbIngredientPrefabs[c].orderSprite,
            carbIngredientPrefabs[c].initialSprite, carbIngredientPrefabs[c].name, 
            carbIngredientPrefabs[c].price);
            num--;

            int p = Random.Range(0, proteinPrefabs.Count);
            newCustomer.AddToOrder(proteinPrefabs[p].orderSprite,
            proteinPrefabs[p].initialSprite, proteinPrefabs[p].name, proteinPrefabs[p].price);
            num--;

            while (num > 0){
                num--;
                Debug.Log(num);
                int i = Random.Range(0, ingredientPrefabs.Count);
                newCustomer.AddToOrder(ingredientPrefabs[i].orderSprite,
                    ingredientPrefabs[i].initialSprite, 
                ingredientPrefabs[i].name, ingredientPrefabs[i].price);
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
        //newCustomer.gameObject.SetActive(true);
        newCustomer.Init(); 
    }

    private void BeginCustomerTimer(){
        if (customersSpawned >= maxCustomers) return;
        SpawnCustomer();
        //start the timer until next player spawn
        timer.Init(timeUntilCustomer, BeginCustomerTimer);
        timer.StartTimer();
    }
}
