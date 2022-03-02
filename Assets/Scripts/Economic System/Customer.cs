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
    private List<string> myOrder = new List<string>();
    private float myOrderPrice;
    private List<Image> orderUi = new List<Image>();
    private int orderUiIndex;

    //timer vars
    [Header("Wait Times")]
    [SerializeField] private float myHappyWaitTime;
    [SerializeField] private float myNeutralWaitTime;
    [SerializeField] private float myAngryWaitTime;
    private Timer waitTimer;
    GameManager gm;

    //econ vars
    [Header("Tipping")]
    [SerializeField] private float myTipPercent;
    [SerializeField] private float myGenerousTipPercent;
    [SerializeField] private int myLeniency;

    //references
    private Economy econ;

    // ==============   methods   ==============
    public void Awake(){
        econ = FindObjectOfType<Economy>();
        gm = FindObjectOfType<GameManager>();
        
        waitTimer = Instantiate(gm.timerPrefab, this.transform).GetComponent<Timer>();
        waitTimer.Init(myHappyWaitTime, EndTimerHandler);

        CreateOrder();
        CreateAppearance();
        //ensure inactive
        this.gameObject.SetActive(false);
    }

    public void CreateOrder(){
        orderUiIndex = 0;
        GameObject order = Instantiate(gm.orderPrefab, this.transform);
        foreach(Transform child in order.transform){
            orderUi.Add(child.gameObject.GetComponent<Image>());
        }
        RectTransform r = order.GetComponent<RectTransform>();
        GetComponent<RectTransform>().sizeDelta = new Vector2 (r.sizeDelta.x, r.sizeDelta.y);
    }

    public void CreateAppearance(){
        int a = Random.Range(0, myPossibleSprites.Count);
        mySprites = myPossibleSprites[a].sprites;

        myCustomer = Instantiate(gm.customerSkeleton, gm.customerView).GetComponent<SpriteRenderer>();
        myCustomer.gameObject.GetComponent<UIActivate>().obj = this.gameObject;
        myCustomer.sprite = mySprites[currSpriteState];
        myCustomerAnim = myCustomer.gameObject.GetComponent<Animator>();
    }

    public void Init(){
        //FIX: "spawn" character -> need a way to sort them smaller when they spawn
        //and move them up as customers leave (like theyre in a line)

        //IDEA: spawn along a line in 3D space & move forward until colliding with object in fromt; each step increase size
        myCustomerAnim.SetTrigger("MoveToFront");
    }

    public void CheckOrder(List<string> given){
        int wrongIngredient = myOrder.Count;
        List<string> check = new List<string>(given);
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
        }
        else PayForOrder(); 
        //UpdateGenerator();
    }
    private void PayForOrder(){ //only pay for order if the number of wrong/missing ingredients is acceptable to customer
        Debug.Log("Customer will pay for something.");
        switch(myMood){
            case Mood.Angry:
                econ.AddPlayerCoins(myOrderPrice + (myOrderPrice*myTipPercent)/2);
            break;
            case Mood.Neutral:
                econ.AddPlayerCoins(myOrderPrice + myOrderPrice*myTipPercent);
            break;
            case Mood.Happy:
                econ.AddPlayerCoins(myOrderPrice + myOrderPrice*myGenerousTipPercent);
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
                waitTimer.Init(myNeutralWaitTime, EndTimerHandler);
            break;
            case Mood.Neutral:
                waitTimer.Init(myAngryWaitTime, EndTimerHandler);
            break;
            case Mood.Angry:
                Destroy(this.gameObject);
            break;
        }
        waitTimer.StartTimer();
    }

    private void Leave(){
        Destroy(myCustomerAnim.gameObject);
        Destroy(this.gameObject);
    }
}
