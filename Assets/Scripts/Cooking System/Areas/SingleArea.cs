using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleArea : MonoBehaviour
{
    [SerializeField] Tool.Required myToolType = Tool.Required.None;

    [SerializeField] private GameObject myItem;

    private bool freeArea = true;
    private Player player;
    private GameManager gm;

    Collider myCollider;

    // ==============   functions   ==============
    private void Awake(){
        player = FindObjectOfType<Player>();
        gm = FindObjectOfType<GameManager>();
        myCollider = GetComponent<Collider>();
        freeArea = false;
    }

    public void OnMouseDown(){
        //Debug.Log("hit area");
        if(!player.handFree){
            if (freeArea) PlaceObjectOnShared();
        }
        else{
            if (!freeArea){ //if the area is for 
                myItem.SetActive(true);
                player.PickUpItem(myItem);
                HandlePickUp();
            }
        }
    }

    //shared board
    private void PlaceObjectOnShared(){
        myItem = player.DropItem("tool");
        if (myItem == null) return;
        
        Tool t = myItem.GetComponent<Tool>();
        t.ResetVars();
        if (t.type != myToolType){
            player.PickUpItem(t.gameObject);
            return;
        }
    
        freeArea = false;
        myItem.SetActive(false);
        myItem.transform.position = myCollider.bounds.center; // + (gm.in3d? new Vector3(0,0,-0.01f): new Vector3(0,0,-1));
    }

    public void HandlePickUp(){
        freeArea = true;
        myCollider.enabled = true;
        myItem = null;
    }
}
