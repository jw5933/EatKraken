using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedAppliance : Appliance
{
    [SerializeField] Ingredient.Type myIngredientType;
    
    protected override void StartTimer(){
        if (timer == null) timer = Instantiate(gm.timerPrefab, this.transform).GetComponent<Timer>();
        //don't cook the wrong ingredient, or ingredients that are already cooked
        if (!DropIngredient() || myIngredient.type != myIngredientType || myIngredient.imgState >= maxState)
            return;

        //start timer
        myIngredient.gameObject.SetActive(false);
        timer.Init(myCookingTime, HandleEndTimer, timedCarbText);
        timer.StartTimer();
        //TODO: move ingredient into place
    }

    protected override void HandleEndTimer(){
        if (myIngredient != null) myIngredient.ChangeImageState();
        myIngredient.transform.position = this.transform.position + new Vector3(0,0,0.2f);
        myIngredient.gameObject.SetActive(true);
    }
}
