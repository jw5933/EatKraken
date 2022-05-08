using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    // ==============   variables   ==============
    private Sprite baseObjectSprite;
    private List <Ingredient> baseIngredientPrefabs;
    private List <Ingredient> carbIngredientPrefabs;
    private List <Ingredient> ingredientPrefabs;
    private List <Ingredient> proteinPrefabs;
    
    private List <CustomerList> customerPrefabs;
    private int customerPrefabIndex;

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
    private int currPhase;
    private int customersLeft;

    //times
    private float timeBeforeFirst = 2;
    private float timeAfterLast = 2;
    [SerializeField] private float rangeStart = 0.5f;
    [SerializeField] private float rangeEnd = 5f;
    
    GameManager gm;
    DayManager dm;
    CustomerManager cm;
    EventManager em;
    LevelDesignScript ld;
    Player player;

    // ==============   methods   ==============
    void Awake(){
        em = FindObjectOfType<EventManager>(true);
        //subscribe to events
        em.OnTimeChange += UpdateOnTimeChange;
        em.OnCustomerNeutral += UpdateOnCustomerNeutral;
        em.OnLocationChange += UpdateOnLocationChange;
        em.OnCustomerLeave += UpdateOnCustomerLeave;

        gm = FindObjectOfType<GameManager>();
        dm = FindObjectOfType<DayManager>();
        cm = FindObjectOfType<CustomerManager>();
        ld = FindObjectOfType<LevelDesignScript>();
        player = FindObjectOfType<Player>();

        timer = Instantiate(gm.timerPrefab, this.transform).GetComponent<Timer>();
    }

    private void UpdateOnLocationChange(Location next){
        Debug.Log("next: " + next.gameObject.name);
        //update the generator
        baseObjectSprite = next.baseSprite;
        baseIngredientPrefabs = next.baseIngredients;
        carbIngredientPrefabs = next.carbIngredients;
        ingredientPrefabs = next.ingredients;
        customerPrefabs = next.customers;
        customersPerStage = next.customersPerStage;
        proteinPrefabs = next.proteins;

        player.hasBaseIngredient = (baseIngredientPrefabs.Count > 0);

        dm.timeStage = next.timeStages;
        dm.loc = next;
        CreatePCList();
    }

    private void UpdateOnCustomerNeutral(){
        BeginCustomerTimer();
    }

    private void UpdateOnCustomerLeave(Customer c, int x, Customer.Mood mood, int nextPhase){
        if (nextPhase <= currPhase) return;
        customersLeft++;
        if (customersLeft >= maxCustomers){
            timer.Init(timeAfterLast, dm.SkipToNextPhase);
            timer.StartTimer();
            return;
        }
        if (mood == Customer.Mood.Happy) BeginCustomerTimer();
    }

    private void UpdateOnTimeChange(float time, int phase){
        currPhase = phase;
        maxCustomers = customersPerStage[phase];
        customersSpawned = 0;
        customersLeft = 0;

        //start timer
        //timeUntilCustomer = (time - timeBeforeFirst - timeAfterLast)/maxCustomers;
        timer.Init(timeBeforeFirst, SpawnCustomer);
        timer.StartTimer();
    }

    //spawning customers
    private void SpawnCustomer(){
        //Debug.Log("currphase: " + currPhase + " custspawned: " + customersSpawned);
        if (customersSpawned >= maxCustomers) return;
        //initiate customer
        Customer newCustomer = phaseCustomerL[currPhase][customersSpawned++];
        cm.LineupCustomer(newCustomer);
    }

    private void BeginCustomerTimer(){
        //start the timer until next player spawn
        if (customersSpawned >= maxCustomers) return;
        timeUntilCustomer = Random.Range(rangeStart, rangeEnd);
        timer.Init(timeUntilCustomer, SpawnCustomer);
        timer.StartTimer();
    }

    //creating the customer list (random gen)
    private void CreatePCList(){
        //Debug.Log("creating list");
        for (int phase = 0; phase < customersPerStage.Length; phase++){
            phaseCustomerL.Add(CreateCustomers(phase, customersPerStage[phase]));
        }
        if (ld !=null) ld.UpdateInfo(customersPerStage.Length);
        dm.ResetVars();
    }

    private void UpdateCustomerPrefabIndex(){
        while (customerPrefabs[customerPrefabIndex].customers.Count == 0) customerPrefabIndex++;
    }

    private List<Customer> CreateCustomers(int phase, int max){ //create a customer with random ingredients
        List <Customer> phaseList = new List<Customer>();
        
        for (int index = 0; index < max; index++){
            //get a customer profile
            UpdateCustomerPrefabIndex();
            int cs = Random.Range(0, customerPrefabs[customerPrefabIndex].customers.Count);
            Customer newCustomer = Instantiate(customerPrefabs[customerPrefabIndex].customers[cs], gm.orderParent).GetComponent<Customer>();
            customerPrefabs[customerPrefabIndex].customers.RemoveAt(cs);
            newCustomer.phase = phase;

            if (baseObjectSprite != null) newCustomer.AddToOrder(baseObjectSprite, null, null, 0, false);
            
            //adjust customer: base, carb, protein, ingredient
            int num = gm.maxIngredients;
            if (baseIngredientPrefabs.Count > 0){
                int b = Random.Range(0, baseIngredientPrefabs.Count);
                newCustomer.AddToOrder(baseIngredientPrefabs[b].orderSprite,
                null, baseIngredientPrefabs[b].name, 
                baseIngredientPrefabs[b].price, 
                false);
            }

            int c = Random.Range(0, carbIngredientPrefabs.Count);
            newCustomer.AddToOrder(carbIngredientPrefabs[c].orderSprite,
            carbIngredientPrefabs[c].initialSprite, carbIngredientPrefabs[c].name, 
            carbIngredientPrefabs[c].price,
            false);
            num--;

            while (num > 1){
                num--;
                int i = Random.Range(0, ingredientPrefabs.Count);
                newCustomer.AddToOrder(ingredientPrefabs[i].orderSprite,
                ingredientPrefabs[i].initialSprite, 
                ingredientPrefabs[i].name,
                ingredientPrefabs[i].price,
                ingredientPrefabs[i].hasCookStage);
            }
            
            int p = Random.Range(0, proteinPrefabs.Count);
            newCustomer.AddToOrder(proteinPrefabs[p].orderSprite,
            proteinPrefabs[p].initialSprite, 
            proteinPrefabs[p].name, 
            proteinPrefabs[p].price, 
            proteinPrefabs[p].hasCookStage);

            newCustomer.CalculateCoins();
            phaseList.Add(newCustomer);
        }
        return phaseList;
    }
}
