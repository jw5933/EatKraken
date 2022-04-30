using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteredAppliance : Appliance
{
    private Meter meter;
    private Vector3 meterPosition;
    private Vector3 ingredientPosition;
    private Material material;

    protected override void Awake(){
        base.Awake();
        base.myType = Appliance.Type.Metered;
        foreach (Transform child in transform){
            switch(child.gameObject.name){
                case "meter pos":
                    meterPosition = child.position;
                break;
                case "ingredient pos":
                    ingredientPosition = child.position;
                break;
            }
        }
        material = GetComponent<SpriteRenderer>().material;
    }

    //indicate whether or not appliance can be used
    /* public void OnMouseEnter(){
        if (player.holdingIngredient){
            if (player.ingredient.hasCookStage){
                material.SetColor("_Color", Color.green);
            }
            else{
                material.SetColor("_Color", Color.red);
            }
            material.SetFloat("_Outline", 1);
        }
    }

    public void OnMouseExit(){
        material.SetFloat("_Outline", 0);
        material.SetColor("_Color", Color.white);
    } */

    protected override void StartMeter(bool swapped){
        if (!swapped){ //if ingredients were just swapped then there is an ingredient on appliance still
            if (myIngredient != null){
                HandlePickUp();
                return;
            }
            //see if the player is holding an ingredient
            GameObject i = player.DropItem("ingredient");
            if (i == null) return;
            myIngredient = i.GetComponent<Ingredient>();
        }

        if (myIngredient.type != myIngredientType || !myIngredient.finishedCutStage || myIngredient.imgState >= myIngredient.maxImageState){
                player.PickUpItem(myIngredient.gameObject);
                myIngredient = null;
                return;
            }

        Collider c = myIngredient.GetComponent<Collider>();
        if (c !=null) c.enabled = false;
        Collider2D c2 = myIngredient.GetComponent<Collider2D>();
        if (c2 !=null) c2.enabled = false;

        myIngredient.transform.position = ingredientPosition;

        //start meter
        if (meter == null){
            meter = Instantiate(gm.stationaryMeterPrefab, gm.meterParent).GetComponent<Meter>();
            meter.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            meter.transform.position = meterPosition;
        }

        timer = meter.Init(myIngredient.raw, myIngredient.cooked, myIngredient.burnt, myIngredient.cookedTime, HandleEndTimer);
        meter.gameObject.SetActive(true);
        meter.StartMeter();
        if (am != null) audioSourceIndex = am.PlayConstantSFX(cookingSound);
    }

    protected override void HandleEndTimer(){
        myIngredient.ChangeCookedState();
    }

    protected override void HandlePickUp(){
        if (CheckSwap()) StartMeter(true);
        else if (player.handFree){
            if (am != null) am.StopConstantSFX(audioSourceIndex);
            meter.StopMeter();
            myIngredient.cookedTime = meter.callerTime;
            meter.ResetVars();
            player.PickUpItem(myIngredient.gameObject);
            myIngredient = null;
            meter.gameObject.SetActive(false);
        }
    }

    private bool CheckSwap(){
        if (player.handFree || (player.holdingIngredient &&(player.ingredient.type != myIngredientType || player.ingredient.imgState >= player.ingredient.maxImageState)))
            return false;

        meter.StopMeter();
        myIngredient.cookedTime = meter.callerTime;
        meter.ResetVars();
        Ingredient i = player.DropItem("ingredient").GetComponent<Ingredient>();
        player.PickUpItem(myIngredient.gameObject);
        myIngredient = i;
        return true;
    }
}
