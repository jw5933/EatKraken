using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Appliance : MonoBehaviour
{
    protected enum Type { //the types of drag + drop
        Timed,
        Meter
    };
    [SerializeField] protected Type myType;

    protected Timer timer;
    [SerializeField] protected Animator myAnimator;
    [SerializeField] protected int maxState;

    //carb cooker
    [SerializeField] protected float myCookingTime;
    protected Ingredient myIngredient;
    [SerializeField] protected Text timedCarbText;

    protected Player player;
    protected GameManager gm;
    
    // ==============   Unity functions   ==============
    protected virtual void Awake(){
        player = FindObjectOfType<Player>();
        gm = FindObjectOfType<GameManager>();
    }

    //if the player presses this object, what should happen?
    public void OnMouseDown(){
        switch (myType){
            case Type.Timed: //start its associated function
                if (!player.handFree) StartTimer();
            break;

            case Type.Meter:
                if (!player.handFree) StartMeter();
            break;
        }
    }

    //indicate hovered tool to player
    public void OnMouseOver(){
        if (myAnimator == null) return;
        //set start state and play animation
    }
    public void OnMouseExit(){
        if (myAnimator == null) return;
        //set end state and play animation
    }

    // ==============   functions   ==============
    // start the carbohydrates (timed) cooker
    protected bool DropIngredient(){
        //see if the player is holding an ingredient
        GameObject i = player.DropItem("ingredient");
        if (i == null) return false;
        myIngredient = i.GetComponent<Ingredient>();
        return true;
    }

    protected virtual void StartTimer(){}

    protected virtual void StartMeter(){}

    //what to do when the timed carb tool is finished
    protected virtual void HandleEndTimer(){}

    protected virtual void HandlePickUp(){}
}
