using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteredAppliance : Appliance
{
    private Meter meter;

    protected override void Awake(){
        base.Awake();
        base.myType = Appliance.Type.Metered;
    }

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

        myIngredient.transform.position = myCollider.bounds.center;

        //start meter
        if (meter == null){
            meter = Instantiate(gm.stationaryMeterPrefab, gm.stationaryCanvas.transform).GetComponent<Meter>();
            meter.gameObject.transform.position = this.transform.position - new Vector3(0, myCollider.bounds.size.y, 0);
        }

        timer = meter.Init(myIngredient.raw, myIngredient.cooked, myIngredient.burnt, myIngredient.cookedTime, HandleEndTimer);
        meter.gameObject.SetActive(true);
        meter.StartMeter();
    }

    protected override void HandleEndTimer(){
        myIngredient.ChangeCookedState();
    }

    protected override void HandlePickUp(){
        meter.StopMeter();
        myIngredient.cookedTime = meter.callerTime;
        meter.ResetVars();
        
        if (CheckSwap()) StartMeter(true);
        else{
            if (player.handFree){
                player.PickUpItem(myIngredient.gameObject);
                myIngredient = null;
            }
        }
    }

    private bool CheckSwap(){
        if (player.handFree) return false;

        //see if the player is holding an ingredient
        GameObject i = player.DropItem("ingredient");
        if (i == null) return false;
        Ingredient si = i.GetComponent<Ingredient>();
        if (si.type != myIngredientType || si.imgState >= si.maxImageState){
            player.PickUpItem(i);
            return false;
        }

        player.PickUpItem(myIngredient.gameObject);
        myIngredient = si;
        return true;
    }
}
