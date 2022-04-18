using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // ==============   variables   ==============
    private GameObject heldItem;
    public bool handFree {get {return heldItem == null;}}

    private BaseObject heldBase;
    public BaseObject baseObject {get{return heldBase;}}
    public bool holdingBase{get{return heldBase != null;}}
    
    private Tool heldTool;
    public bool holdingTool{get{return heldTool!=null;}}

    private Ingredient heldIngredient;
    public Ingredient ingredient {get {return heldIngredient;}}
    public bool holdingIngredient {get {return heldIngredient != null;}}
    [HideInInspector] public bool hasBaseIngredient;

    //vectors & planes
    Vector3 mousePos;
    Vector3 mouseDistanceFromCamera;
    Plane mousePlane;
    Plane currentPlane;
    public Plane currPlane {set{currentPlane = value;} get{return currentPlane;}}
    float mouseDistanceZ = 15f;

    //references
    CameraManager cam;
    GameManager gm;


    // ==============   functions   ==============
    private void Awake(){
        cam = FindObjectOfType<CameraManager>();
        gm = FindObjectOfType<GameManager>();

        mouseDistanceFromCamera = new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z + mouseDistanceZ);
        //Create a new plane with normal (0,0,1) at the position away from the camera you define in the Inspector. This is the plane that you can click so make sure it is reachable.
        mousePlane = new Plane(Vector3.up, mouseDistanceFromCamera);
        ResetPlane();
    }

    public void Update(){
        if (heldItem !=null) UpdateMouseItem(heldItem);     
    }

    public void UpdateMouseItem(GameObject o){
        if (!gm.in3d) {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            if (o !=null) o.transform.position = Vector3.MoveTowards(o.transform.position, new Vector3(mousePos.x, mousePos.y, o.transform.position.z), 40 * Time.deltaTime);
        }
        else{
            //Create a ray from the Mouse click position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Initialise the enter variable
            float enter = 0.0f;
            
            //move the object to where the mouse is
            if (currentPlane.Raycast(ray, out enter)){
                mousePos = ray.GetPoint(enter);
                //Debug.DrawLine(Camera.main.ScreenToWorldPoint(Input.mousePosition), mousePos, Color.green);
                o.transform.position = mousePos;
                //o.transform.position = new Vector3(mousePos.x, canMoveInSpace? o.transform.position.y : mousePos.y, mousePos.z);
            }
        }
    }

    public void ResetPlane(){
        currentPlane = mousePlane;
    }

    public void PickUpItem(GameObject item){ //pick up an item, and sort it into the correct script type
        if (item == null) return;
        heldItem = item;
        //disable collider
        //Debug.Log(heldItem.name);
        Collider c = heldItem.GetComponent<Collider>();
        if (c !=null) c.enabled = false;
        Collider2D c2 = heldItem.GetComponent<Collider2D>();
        if (c2 !=null) c2.enabled = false;
        
        //get the type of item player picked up
        heldIngredient = item.GetComponent<Ingredient>();
        if (!holdingIngredient) heldTool = item.GetComponent<Tool>();
        if (!holdingTool) heldBase = item.GetComponent<BaseObject>();
    }

    //drop the held item, if its type matches what is wanted by caller 
    public GameObject DropItem(string type){
        //Debug.Log("dropping item");
        GameObject held = null;
        switch(type){
            case "ingredient":
                if(holdingIngredient){
                    held = heldItem;
                    heldIngredient = null;
                    heldItem = null;
                }
            break;
            case "tool":
                if (holdingTool){
                    held = heldItem;
                    heldTool = null;
                    heldItem = null;
                }
            break;
            case "base":
                if (holdingBase){
                    held = heldItem;
                    heldBase = null;
                    heldItem = null;
                }
            break;
            
            case "any":
                held = heldItem;
                heldItem = null;
            break;
        }
        //if no item is being held, reset some vars; if there an object that will be returned to caller, enable back its collider
        if (heldItem == null) HandleNoItems();
        if (held != null) {
            //enable collider
            Collider c = held.GetComponent<Collider>();
            if (c !=null) c.enabled = true;
            Collider2D c2 = held.GetComponent<Collider2D>();
            if (c2 !=null) c2.enabled = true;
        }
        return held;
    }

    private void HandleNoItems(){
        heldTool = null;
        heldIngredient = null;
        heldItem = null;
        //cam.ShowButtons();
    }
    
    public bool ValidateToolLines(Ingredient i){ //validate by checking if player is holding the required tool
        if (heldTool != null && heldTool.ValidateMotion(i)) {
            heldTool.AddToHovered(i);
            i.ValidateToolLines();
            return true;
        }
        return false;
    }
}
