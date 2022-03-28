using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimedAppliance : Appliance
{
    [SerializeField] private float myCookingTime;
    [SerializeField] private Text timedCarbText;
    [SerializeField] private int maxState;

    protected override void Awake(){
        base.Awake();
        myType = Appliance.Type.Timed;
    }
    
    protected override void StartTimer(){
        if (myIngredient != null) return;
        Debug.Log("starting timer");
        if (timer == null) timer = Instantiate(gm.timerPrefab, this.transform).GetComponent<Timer>();
        //don't cook the wrong ingredient, or ingredients that are already cooked
        
        //see if the player is holding an ingredient
        GameObject i = player.DropItem("ingredient");
        if (i == null) return;
        myIngredient = i.GetComponent<Ingredient>();
        if (myIngredient.type != myIngredientType || myIngredient.imgState >= myIngredient.maxImageState){
            player.PickUpItem(myIngredient.gameObject);
            myIngredient = null;
            return;
        }
            

        //start timer
        Debug.Log("starting timer");
        myIngredient.gameObject.SetActive(false);
        timer.Init(myCookingTime, HandleEndTimer, timedCarbText);
        timer.StartTimer();
        //TODO: move ingredient into place
    }

    protected override void HandleEndTimer(){
        if (myIngredient != null) myIngredient.ChangeCookedState();
        myIngredient.transform.position = this.transform.position + new Vector3(0,0,- 0.2f);
        myIngredient.gameObject.SetActive(true);
        myIngredient = null;
        
    }
}
