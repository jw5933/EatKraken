using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appliance : MonoBehaviour
{
    protected enum Type { //the types of drag + drop
        Timed,
        Metered
    };
    protected Type myType;

    protected Timer timer;
    [SerializeField] protected Animator myAnimator;

    protected Collider2D myCollider;

    //carb cooker
    protected Ingredient myIngredient;
    public Ingredient ingredient {set{myIngredient = value;}}
    [SerializeField] protected Ingredient.Type myIngredientType;

    [SerializeField] protected AudioClip cookingSound;
    protected int audioSourceIndex;

    protected Player player;
    protected GameManager gm;
    protected AudioManager am;
    
    // ==============   Unity functions   ==============
    protected virtual void Awake(){
        am = FindObjectOfType<AudioManager>();
        player = FindObjectOfType<Player>();
        gm = FindObjectOfType<GameManager>();
        myCollider = GetComponent<Collider2D>();
    }

    //if the player presses this object, what should happen?
    public void OnMouseDown(){
        Debug.Log("appliance on mouse down");
        switch (myType){
            case Type.Timed: //start its associated function
                Debug.Log("clicked timer");
                StartTimer();
            break;

            case Type.Metered:
                Debug.Log("clicked metered");
                StartMeter(false);
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
    protected virtual void StartTimer(){}

    protected virtual void StartMeter(bool swapped){}

    //what to do when the timed carb tool is finished
    protected virtual void HandleEndTimer(){}

    protected virtual void HandlePickUp(){}
}
