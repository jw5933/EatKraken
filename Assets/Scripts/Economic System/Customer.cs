using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    protected Animator myCustomerAnim; 
    private Image myCustomer;
    private TextMeshProUGUI tipText;
    protected int positionInLine;

    //mood vars
    public enum Mood {Angry, Neutral, Happy}
    protected Mood myMood = Mood.Happy;
    public Mood mood {get{return myMood;}}
    
    //order vars
    private List<string> myOrder = new List<string>();
    private float myOrderPrice;
    public float orderPrice{get{return myOrderPrice;}}
    private Text orderText;

    private List<Image> orderUi = new List<Image>();
    private List<Image> finalOrderUi = new List<Image>();
    private List<Image> orderFlameUi = new List<Image>();
    private List<Sprite> orderUiSprites = new List<Sprite>();
    private List<Sprite> finalOrderUiSprites = new List<Sprite>();
    private List<bool> orderUiFlameBools = new List<bool>();
    private GameObject detailedOrderUi;
    private Transform meterParent;

    protected int myTimePhase;
    public int phase{set{myTimePhase = value;}}

    protected GameObject order;
    
    //timer vars
    [Header("Wait Times")]
    [SerializeField] private float myHappyWaitTime;
    [SerializeField] private float myNeutralWaitTime;
    [SerializeField] private float myAngryWaitTime;
    private Timer timer;
    protected Meter meter;

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
    protected EventManager em;
    protected CustomerManager cm;
    private Player player;

    // ==============   methods   ==============
    public virtual void Awake(){
        gm = FindObjectOfType<GameManager>();
        em = FindObjectOfType<EventManager>();
        cm = FindObjectOfType<CustomerManager>();
        player = FindObjectOfType<Player>();

        this.gameObject.SetActive(false);
    }

    public virtual void OnMouseUpAsButton(){ //serve
        //cm.SelectCustomer(this);
        if (player.holdingBase && player.baseObject.order.Count > 0){
            List<Ingredient> order = new List<Ingredient>(player.baseObject.order);
            AudioManager am = FindObjectOfType<AudioManager>();
            if (am != null) am.PlaySFX(serveSfx);
            Destroy(player.DropItem("base"));
            CheckOrder(order);
        }
    }

    public void OnMouseEnter(){
        detailedOrderUi.SetActive(!detailedOrderUi.activeSelf);
    }

    protected virtual void Activate(){
        this.gameObject.SetActive(true);
        order.gameObject.SetActive(true);
        meter.StartMeter();
    }

    public virtual void Init(int posInLine){
        positionInLine = posInLine;
        transform.SetParent(gm.orderParent.GetChild(positionInLine));
        transform.localPosition = Vector3.zero;

        CreateOrder();
        CreateMeter();
        CreateAppearance();

        UpdateOrderUI();
        myCustomerAnim.SetTrigger("MoveForward");
    }

    public void CalculateCoins(){
        coinAngry = myOrderPrice + (myOrderPrice*myTipPercent)/2;
        coinNeutral = myOrderPrice + myOrderPrice*myTipPercent;
        coinHappy = myOrderPrice + myOrderPrice*myGenerousTipPercent;
    }

    private void CreateOrder(){
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
                            j = c.transform.GetChild(0).GetComponent<Image>();
                            orderFlameUi.Add(j);
                        }
                        detailedOrderUi.SetActive(false);
                    break;
                    case "final group":
                        foreach(Transform c in i.transform){
                            Image j = c.gameObject.GetComponent<Image>();
                            if (j != null) finalOrderUi.Add(j);
                            else orderText = c.gameObject.GetComponent<Text>();
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

    public void AddToOrder(Sprite fs, Sprite s, string i, float p, bool cooked){ //add an ingredient to this order and update the UI
        if (fs != null) finalOrderUiSprites.Add(fs);
        if (s != null) orderUiSprites.Add(s);
        if (i != null) myOrder.Add(i);
        if (s != null) orderUiFlameBools.Add(cooked);
        if (p != 0) myOrderPrice += p;
    }

    private void UpdateOrderUI(){
        orderText.text = "" + myOrderPrice;
        
        int index = 0;
        while (index < orderUi.Count){
            orderUi[index].sprite = orderUiSprites[index];
            orderFlameUi[index].enabled = orderUiFlameBools[index];
            index++;
        }

        index = 0;
        while (index < finalOrderUiSprites.Count){
            finalOrderUi[index].sprite = finalOrderUiSprites[index];
            index++;
        }
    }

    private void CreateMeter(){
        meter = Instantiate(gm.meterPrefab, order.transform).GetComponent<Meter>();
        RectTransform meterTransform = meter.gameObject.GetComponent<RectTransform>();
        RectTransform orderTransform = order.gameObject.GetComponent<RectTransform>();
        RectTransform meterPTransform = meterParent.gameObject.GetComponent<RectTransform>();
        //float offset = 0f;

        meterTransform.sizeDelta = new Vector2 (meterPTransform.sizeDelta.y, meterPTransform.sizeDelta.x);
        meter.transform.SetParent(meterParent);
        meter.transform.SetAsFirstSibling();
        meterTransform.anchoredPosition = Vector2.zero;
        //meterTransform.anchoredPosition = new Vector2(0, -(orderTransform.rect.height + meterTransform.rect.width + offset));
        
        timer = meter.Init(myHappyWaitTime, myNeutralWaitTime, myAngryWaitTime, 0, EndTimerHandler);
    }

    public virtual void CreateAppearance(){
        int a = Random.Range(0, myPossibleSprites.Count);
        mySprites = myPossibleSprites[a].sprites;

        myCustomer = Instantiate(gm.customerSkeleton, gm.customerParent.GetChild(positionInLine)).GetComponent<Image>();
        myCustomer.gameObject.GetComponent<UIActivate>().AddAction(Activate);
        myCustomer.gameObject.GetComponent<UIDeactivate>().AddAction(Leave);
        tipText = myCustomer.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        myCustomer.sprite = mySprites[currSpriteState++];
        myCustomerAnim = myCustomer.gameObject.GetComponent<Animator>();
        myCustomer.GetComponent<CustomerCharacter>().customer = this;
    }
    
    //check state & ingrdient
    private void CheckOrder(List<Ingredient> given){
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
            myCustomer.sprite = mySprites[mySprites.Length-1];
            Debug.Log("Customer will leave without paying anything.");
            InitialLeave(0);
        }
        else PayForOrder();
    }

    private void PayForOrder(){ //only pay for order if the number of wrong/missing ingredients is acceptable to customer
        Debug.Log("Customer will pay for something.");
        switch(myMood){
            case Mood.Angry:
                InitialLeave(coinAngry);
            break;
            case Mood.Neutral:
                InitialLeave(coinNeutral);
            break;
            case Mood.Happy:
                InitialLeave(coinHappy);
            break;
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
                InitialLeave(0);
                break;
        }
    }

    protected void InitialLeave(float coins){
        em.ChangeCoins(this, coins, coinHappy, myTimePhase);
        float tip = coins - orderPrice;
        tipText.text = (tip > 0 ? tip : 0).ToString("0.00");
        order.SetActive(false);
        myCustomerAnim.SetTrigger("Leave");
    }

    protected virtual void Leave(){
        em.FreeCustomer(this, positionInLine, myMood, myTimePhase + 1); //em OnCustomerLeave
        Destroy(myCustomerAnim.gameObject);
    }
}
