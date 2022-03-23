using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
this is really customer profile; it is a *TYPE* of customer, but the sprites can vary
*/
public class Customer : MonoBehaviour
{
    // ==============   variables   ==============
    //customer appearance vars
    [Header("Appearance")]
    [SerializeField] private List<CustomerSprites> myPossibleSprites;
    private Sprite[] mySprites;
    private int currSpriteState;
    private Animator myCustomerAnim; 
    private SpriteRenderer myCustomer;

    //mood vars
    public enum Mood {Angry, Neutral, Happy}
    private Mood myMood = Mood.Happy;
    public Mood mood {get{return myMood;}}
    
    //order vars
    [SerializeField] private List<string> myOrder = new List<string>();
    private float myOrderPrice;
    private List<Image> orderUi = new List<Image>();
    private List<Sprite> orderUiSprites = new List<Sprite>();
    private Text orderText;
    private int myTimePhase;
    public int phase{set{myTimePhase = value;}}
    private GameObject order;

    //timer vars
    [Header("Wait Times")]
    [SerializeField] private float myHappyWaitTime;
    [SerializeField] private float myNeutralWaitTime;
    [SerializeField] private float myAngryWaitTime;
    private Timer timer;
    private Meter meter;

    //econ vars
    [Header("Tipping")]
    [SerializeField] private float myTipPercent;
    [SerializeField] private float myGenerousTipPercent;
    [SerializeField] private int myLeniency;
    private float coinHappy, coinNeutral, coinAngry;
    public float maxCoins {get{return coinHappy;}}

    //references
    private GameManager gm;
    private EventManager em;
    private CustomerManager cm;

    // ==============   methods   ==============
    public void Awake(){
        gm = FindObjectOfType<GameManager>();
        em = FindObjectOfType<EventManager>();

        CreateOrder();
        CreateMeter();
        CreateAppearance();


        this.gameObject.SetActive(false);
    }

    public void CalculateCoins(){
        coinAngry = myOrderPrice + (myOrderPrice*myTipPercent)/2;
        coinNeutral = myOrderPrice + myOrderPrice*myTipPercent;
        coinHappy = myOrderPrice + myOrderPrice*myGenerousTipPercent;
    }

    public void CreateOrder(){
        order = Instantiate(gm.orderPrefab, this.transform);
        foreach(Transform child in order.transform){
            Image i = child.gameObject.GetComponent<Image>();
            if (i !=null) orderUi.Add(i);
        }
        RectTransform r = order.GetComponent<RectTransform>();
        //GetComponent<RectTransform>().sizeDelta = new Vector2 (r.sizeDelta.x, r.sizeDelta.y);
    }
    private void CreateMeter(){
        meter = Instantiate(gm.meterPrefab, order.transform).GetComponent<Meter>();
        RectTransform meterTransform = meter.gameObject.GetComponent<RectTransform>();
        RectTransform orderTransform = order.gameObject.GetComponent<RectTransform>();
        float offset = 0f;

        meterTransform.sizeDelta = new Vector2 (meterTransform.sizeDelta.x, orderTransform.sizeDelta.x);
        meterTransform.anchoredPosition = new Vector2(0, -(orderTransform.rect.height + meterTransform.rect.width + offset));
        
        timer = meter.Init(myHappyWaitTime, myNeutralWaitTime, myAngryWaitTime, 0, EndTimerHandler);
    }

    public void CreateAppearance(){
        int a = Random.Range(0, myPossibleSprites.Count);
        mySprites = myPossibleSprites[a].sprites;

        myCustomer = Instantiate(gm.customerSkeleton, gm.customerView).GetComponent<SpriteRenderer>();
        myCustomer.gameObject.GetComponent<UIActivate>().action = Activate;
        myCustomer.sprite = mySprites[currSpriteState++];
        myCustomerAnim = myCustomer.gameObject.GetComponent<Animator>();
    }

    private void Activate(){
        this.gameObject.SetActive(true);
        meter.StartMeter();
    }

    public void Init(){
        UpdateOrderUI();
        //FIX: "spawn" character -> need a way to sort them smaller when they spawn
        //and move them up as customers leave (like theyre in a line)

        //IDEA: spawn along a line in 3D space & move forward until colliding with object in fromt; each step increase size
        myCustomerAnim.SetTrigger("MoveToFront");
    }
    //check state & ingrdient
    public void CheckOrder(List<Ingredient> given){
        int wrongIngredient = myOrder.Count;
        bool wrongState = false;
        List<string> order = new List<string>();
        //create a new list of strings to check against order, check if any ingredients are not in the right cooking state
        foreach(Ingredient i in given){
            if (!wrongState && i.cookedState != i.requiredCookedState){
                wrongState = true;
            }
            order.Add(i.name);
        }

        List<string> check = new List<string>(order);
        check.RemoveAt(0); //remove the base
        foreach(string i in order){
            if (myOrder.Remove(i)){
                wrongIngredient--;
                check.Remove(i);
            }
        }
        wrongIngredient = Mathf.Max(wrongIngredient, check.Count);
        Debug.Log("The order has " + wrongIngredient + " wrong or missing ingredients.");
        HandleAfterOrder(wrongIngredient);
    }

    private void HandleAfterOrder(int wrongIngredient){
        if (wrongIngredient > myLeniency){
            myMood = Mood.Angry;
            Debug.Log("Customer will leave without paying anything.");
            Leave();
        }
        else PayForOrder(); 
        //UpdateGenerator();
    }
    private void PayForOrder(){ //only pay for order if the number of wrong/missing ingredients is acceptable to customer
        Debug.Log("Customer will pay for something.");
        switch(myMood){
            case Mood.Angry:
                em.ChangeCoins(coinAngry, coinHappy, myTimePhase);
            break;
            case Mood.Neutral:
                em.ChangeCoins(coinNeutral, coinHappy, myTimePhase);
            break;
            case Mood.Happy:
                em.ChangeCoins(coinHappy, coinHappy, myTimePhase);
            break;
        }
        Leave();
    }

    public void AddToOrder(Sprite s, string i, float p){ //add an ingredient to this order and update the UI
        Debug.Log(string.Format("called add to order on {0}, orderUI Length is {1}", this.gameObject.name, orderUi.Count));
        //if (orderUiIndex >= orderUi.Count) return;
        //UpdateOrderUI(s);
        orderUiSprites.Add(s);
        myOrder.Add(i);
        myOrderPrice += p;
    }

    private void UpdateOrderUI(){
        //if (orderUiIndex < orderUi.Count) orderUi[orderUiIndex++].sprite = s;
        int index = 0;
        while (index < orderUi.Count){
            orderUi[index].sprite = orderUiSprites[index];
            index++;
        }
        while (index < myOrder.Count){
            myOrder.RemoveAt(index);
        }
    }

    public void EndTimerHandler(){
        switch(myMood){
            case Mood.Happy:
                myCustomer.sprite = mySprites[currSpriteState++];
                myMood = Mood.Neutral;
            break;
            case Mood.Neutral:
                myCustomer.sprite = mySprites[currSpriteState];
                myMood = Mood.Angry;
            break;
            case Mood.Angry:
                em.ChangeCoins(0, coinHappy, myTimePhase);
                Leave();
            return;
        }
    }

    private void Leave(){
        FindObjectOfType<CustomerManager>().RemoveCustomer(this);
        Destroy(myCustomerAnim.gameObject);
    }
}
