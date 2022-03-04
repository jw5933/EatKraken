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
    private int orderUiIndex;
    private Text orderText;
    private int myTimePhase;
    public int phase{set{myTimePhase = value;}}

    //timer vars
    [Header("Wait Times")]
    [SerializeField] private float myHappyWaitTime;
    [SerializeField] private float myNeutralWaitTime;
    [SerializeField] private float myAngryWaitTime;
    private Timer waitTimer;

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

    // ==============   methods   ==============
    public void Awake(){
        gm = FindObjectOfType<GameManager>();
        em = FindObjectOfType<EventManager>();

        CreateOrder();
        CreateAppearance();
        //ensure inactive
        waitTimer = Instantiate(gm.timerPrefab, this.transform).GetComponent<Timer>();
        waitTimer.Init(myHappyWaitTime, EndTimerHandler, orderText);
        this.gameObject.SetActive(false);
    }

    public void CalculateCoins(){
        coinAngry = myOrderPrice + (myOrderPrice*myTipPercent)/2;
        coinNeutral = myOrderPrice + myOrderPrice*myTipPercent;
        coinHappy = myOrderPrice + myOrderPrice*myGenerousTipPercent;
    }

    public void CreateOrder(){
        orderUiIndex = 0;
        GameObject order = Instantiate(gm.orderPrefab, this.transform);
        orderText = order.transform.GetChild(1).GetComponent<Text>(); //FIX: DELETE
        foreach(Transform child in order.transform){
            Image i = child.gameObject.GetComponent<Image>();
            if (i !=null) orderUi.Add(i);
        }
        RectTransform r = order.GetComponent<RectTransform>();
        GetComponent<RectTransform>().sizeDelta = new Vector2 (r.sizeDelta.x, r.sizeDelta.y);
    }

    public void CreateAppearance(){
        int a = Random.Range(0, myPossibleSprites.Count);
        mySprites = myPossibleSprites[a].sprites;

        myCustomer = Instantiate(gm.customerSkeleton, gm.customerView).GetComponent<SpriteRenderer>();
        myCustomer.gameObject.GetComponent<UIActivate>().obj = this.gameObject;
        myCustomer.sprite = mySprites[currSpriteState++];
        myCustomerAnim = myCustomer.gameObject.GetComponent<Animator>();
    }

    public void Init(){
        //FIX: "spawn" character -> need a way to sort them smaller when they spawn
        //and move them up as customers leave (like theyre in a line)

        //IDEA: spawn along a line in 3D space & move forward until colliding with object in fromt; each step increase size
        myCustomerAnim.SetTrigger("MoveToFront");
        waitTimer.StartTimer();
    }

    public void CheckOrder(List<string> given){
        int wrongIngredient = myOrder.Count;
        List<string> check = new List<string>(given);
        check.RemoveAt(0); //remove the base
        foreach(string i in given){
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
        //Debug.Log(string.Format("called add to order on {0}, orderUI Length is {1}", this.gameObject.name, orderUi.Count));
        if (orderUiIndex >= orderUi.Count) return;
        UpdateOrderUI(s);
        myOrder.Add(i);
        myOrderPrice += p;
    }

    private void UpdateOrderUI(Sprite s){
        if (orderUiIndex < orderUi.Count) orderUi[orderUiIndex++].sprite = s;
    }

    public void EndTimerHandler(){
        //waitTimer = Instantiate(gm.timerPrefab, this.transform).GetComponent<Timer>();
        switch(myMood){
            case Mood.Happy:
                waitTimer.Init(myNeutralWaitTime, EndTimerHandler, orderText);
                myCustomer.sprite = mySprites[currSpriteState++];
                myMood = Mood.Neutral;
            break;
            case Mood.Neutral:
                waitTimer.Init(myAngryWaitTime, EndTimerHandler, orderText);
                myCustomer.sprite = mySprites[currSpriteState];
                myMood = Mood.Angry;
            break;
            case Mood.Angry:
                em.ChangeCoins(0, coinHappy, myTimePhase);
                Leave();
            return;
        }
        waitTimer.StartTimer();
    }

    private void Leave(){
        Destroy(myCustomerAnim.gameObject);
        Destroy(this.gameObject);
    }
}
