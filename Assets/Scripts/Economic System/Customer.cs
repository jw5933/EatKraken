using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    private Image myCustomer;
    private int positionInLine;

    //mood vars
    public enum Mood {Angry, Neutral, Happy}
    private Mood myMood = Mood.Happy;
    public Mood mood {get{return myMood;}}
    
    //order vars
    [SerializeField] private List<string> myOrder = new List<string>();
    private float myOrderPrice;

    private List<Image> orderUi = new List<Image>();
    [SerializeField] private List<Image> finalOrderUi = new List<Image>();
    private List<Sprite> orderUiSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> finalOrderUiSprites = new List<Sprite>();
    private GameObject detailedOrderUi;
    private Transform meterParent;

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

    //sounds
    [SerializeField] private AudioClip serveSfx;

    //references
    private GameManager gm;
    private EventManager em;
    private CustomerManager cm;
    private Player player;

    // ==============   methods   ==============
    public void Awake(){
        gm = FindObjectOfType<GameManager>();
        em = FindObjectOfType<EventManager>();
        cm = FindObjectOfType<CustomerManager>();
        player = FindObjectOfType<Player>();

        this.gameObject.SetActive(false);
    }

    public void OnMouseUpAsButton(){ //serve
        //cm.SelectCustomer(this);
        if (player.holdingBase && player.baseObject.order.Count > 0){
            List<Ingredient> order = new List<Ingredient>(player.baseObject.order);
            FindObjectOfType<AudioManager>().PlaySFX(serveSfx);
            Destroy(player.DropItem("base"));
            CheckOrder(order);
        }
    }

    public void OnMouseEnter(){
        detailedOrderUi.SetActive(!detailedOrderUi.activeSelf);
    }

    private void Activate(){
        this.gameObject.SetActive(true);
        order.gameObject.SetActive(true);
        meter.StartMeter();
    }

    public void Init(int posInLine){
        positionInLine = posInLine;
        transform.SetParent(gm.orderParent.GetChild(positionInLine));
        transform.localPosition = Vector3.zero;

        CreateOrder();
        CreateMeter();
        CreateAppearance();

        UpdateOrderUI();
        myCustomerAnim.SetTrigger("MoveToFront");
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
            if (i !=null) {
                switch (i.name){
                    case "initial group":
                        detailedOrderUi = i.gameObject;
                        foreach(Transform c in i.transform){
                            Image j = c.gameObject.GetComponent<Image>();
                            orderUi.Add(j);
                        }
                        detailedOrderUi.SetActive(false);
                    break;
                    case "final group":
                        foreach(Transform c in i.transform){
                            Image j = c.gameObject.GetComponent<Image>();
                            finalOrderUi.Add(j);
                        }
                    break;
                    case "meter":
                        meterParent = i.transform;
                    break;
                }
            }
        }
        RectTransform r = order.GetComponent<RectTransform>();
        GetComponent<RectTransform>().sizeDelta = new Vector2 (r.sizeDelta.x, r.sizeDelta.y);
    }
    private void CreateMeter(){
        meter = Instantiate(gm.meterPrefab, order.transform).GetComponent<Meter>();
        RectTransform meterTransform = meter.gameObject.GetComponent<RectTransform>();
        RectTransform orderTransform = order.gameObject.GetComponent<RectTransform>();
        //float offset = 0f;

        meterTransform.sizeDelta = new Vector2 (meterTransform.sizeDelta.x, orderTransform.sizeDelta.x);
        meter.transform.SetParent(meterParent);
        meterTransform.anchoredPosition = Vector2.zero;
        //meterTransform.anchoredPosition = new Vector2(0, -(orderTransform.rect.height + meterTransform.rect.width + offset));
        
        timer = meter.Init(myHappyWaitTime, myNeutralWaitTime, myAngryWaitTime, 0, EndTimerHandler);
    }

    public void CreateAppearance(){
        int a = Random.Range(0, myPossibleSprites.Count);
        mySprites = myPossibleSprites[a].sprites;

        myCustomer = Instantiate(gm.customerSkeleton, gm.customerParent.GetChild(positionInLine)).GetComponent<Image>();
        myCustomer.gameObject.GetComponent<UIActivate>().AddAction(Activate);
        myCustomer.sprite = mySprites[currSpriteState++];
        myCustomerAnim = myCustomer.gameObject.GetComponent<Animator>();
        myCustomer.GetComponent<CustomerCharacter>().customer = this;
    }
    
    //check state & ingrdient
    public void CheckOrder(List<Ingredient> given){
        int wrongIngredient = myOrder.Count;
        int wrongState = 0;
        List<string> order = new List<string>();
        //create a new list of strings to check against order, check if any ingredients are not in the right cooking state
        foreach(Ingredient i in given){
            if (i.cookedState != i.requiredCookedState){
                wrongState++;
            }
            order.Add(i.name);
        }

        List<string> check = new List<string>(order);
        foreach(string i in order){
            if (myOrder.Remove(i)){
                wrongIngredient--;
                check.Remove(i);
            }
        }
        wrongIngredient += wrongState;
        wrongIngredient = Mathf.Max(wrongIngredient, check.Count);
        Debug.Log("The order has " + wrongIngredient + " wrong or missing ingredients.");
        HandleAfterOrder(wrongIngredient);
    }

    private void HandleAfterOrder(int wrongIngredient){
        if (wrongIngredient > myLeniency){
            myMood = Mood.Angry;
            Debug.Log("Customer will leave without paying anything.");
            em.ChangeCoins(this, 0, coinHappy, myTimePhase);
            Leave();
        }
        else PayForOrder();
    }
    private void PayForOrder(){ //only pay for order if the number of wrong/missing ingredients is acceptable to customer
        Debug.Log("Customer will pay for something.");
        switch(myMood){
            case Mood.Angry:
                em.ChangeCoins(this, coinAngry, coinHappy, myTimePhase);
            break;
            case Mood.Neutral:
                em.ChangeCoins(this, coinNeutral, coinHappy, myTimePhase);
            break;
            case Mood.Happy:
                em.ChangeCoins(this, coinHappy, coinHappy, myTimePhase);
            break;
        }
        Leave();
    }

    public void AddToOrder(Sprite fs, Sprite s, string i, float p){ //add an ingredient to this order and update the UI
        if (fs != null) finalOrderUiSprites.Add(fs);
        if (s != null) orderUiSprites.Add(s);
        if (i != null) myOrder.Add(i);
        if (p != 0) myOrderPrice += p;
    }

    private void UpdateOrderUI(){
        int index = 0;
        while (index < orderUi.Count){
            orderUi[index].sprite = orderUiSprites[index];
            index++;
        }

        index = 0;
        while (index < finalOrderUiSprites.Count){
            finalOrderUi[index].sprite = finalOrderUiSprites[index];
            index++;
        }
    }

    public void EndTimerHandler(){
        switch(myMood){
            case Mood.Happy:
                myCustomer.sprite = mySprites[currSpriteState++];
                myMood = Mood.Neutral;
                em.ChangeCustomerMood();
                break;
            case Mood.Neutral:
                myCustomer.sprite = mySprites[currSpriteState];
                myMood = Mood.Angry;
                break;
            case Mood.Angry:
                em.ChangeCoins(this, 0, coinHappy, myTimePhase);
                Leave();
                break;
        }
    }

    private void Leave(){
        em.FreeCustomer(positionInLine, myMood);
        Destroy(myCustomerAnim.gameObject);
    }
}
