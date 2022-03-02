using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // ==============   variables   ==============
    public Text tempOrderText;
    private bool isHoldingBase;
    public bool holdingBase{get{return isHoldingBase;}}
    
    [SerializeField] private Tool heldTool;
    [SerializeField] private Ingredient heldIngredient;
    [SerializeField] private GameObject heldItem;

    private ToolLine myToolLine;
    public ToolLine toolLine {set{myToolLine = value;}}
    public bool isHoldingTool{get{return heldTool!=null;}}
    
    private bool isHandFree = true;
    public bool handFree {get {return isHandFree;}}

    [SerializeField] private List <Ingredient> currentOrder = new List <Ingredient>();
    public List<Ingredient> order {get {return currentOrder;}}

    //vars to create protein
    private float endTime;
    private bool canCreateProtein;
    private int wantedProtein;

    private KeyCode[] keyCodes = {
         KeyCode.Alpha1
         //FIX: add keycodes as needed
     };

    CameraManager cam;
    ProteinManager pm;
    GameManager gm;

    Vector3 mousePos;
    Plane mousePlane;
    Vector3 mouseDistanceFromCamera;

     //This is the distance the clickable plane is from the camera. Set it in the Inspector before running.
    [SerializeField] float mouseDistanceZ = -15f;

    // ==============   functions   ==============
    private void Awake(){
        cam = FindObjectOfType<CameraManager>();
        pm = GetComponent<ProteinManager>();
        gm = FindObjectOfType<GameManager>();

        mouseDistanceFromCamera = new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z - mouseDistanceZ);
        //Create a new plane with normal (0,0,1) at the position away from the camera you define in the Inspector. This is the plane that you can click so make sure it is reachable.
        mousePlane = new Plane(Vector3.up, mouseDistanceFromCamera);
    }
    public void Update(){
        if (heldItem !=null) UpdateMouseItem();
        CheckInput();
    }

    private void UpdateMouseItem(){
        if (!gm.in3d) {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            if (heldItem !=null) heldItem.transform.position = Vector3.MoveTowards(heldItem.transform.position, new Vector3(mousePos.x, mousePos.y, heldItem.transform.position.z), 40 * Time.deltaTime);
        }
        else{
            //Create a ray from the Mouse click position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Initialise the enter variable
            float enter = 0.0f;
            Debug.DrawLine(Camera.main.ScreenToWorldPoint(Input.mousePosition), mouseDistanceFromCamera, Color.green);
            //move the object to where the mouse is
            if (mousePlane.Raycast(ray, out enter)){
                mousePos = ray.GetPoint(enter);
                heldItem.transform.position = new Vector3(mousePos.x, heldItem.transform.position.y, mousePos.z);
            }
        }
    }

    private void CheckInput(){
        if (Input.GetKeyDown(KeyCode.R)){
            SceneManager.LoadSceneAsync("MainScene");
        }

        ValidateCreateProtein();
    }

    private void ValidateCreateProtein(){
        for(int n = 0; n < keyCodes.Length; n++){
            if (Input.GetKeyDown(keyCodes[n]) && !canCreateProtein) {
                if (!isHandFree) return;
                canCreateProtein = true;
                float startTime = Time.time;
                endTime = startTime + pm.countdown;
                Debug.Log(string.Format("start time: %d, end time: %d", startTime, endTime));

                pm.AnimateCreateProtein();
            }
            if (Input.GetKeyUp(keyCodes[n]) && canCreateProtein) {
                canCreateProtein = false;
                pm.StopAnim();
            }
            if (Time.time >= endTime && canCreateProtein) {
                canCreateProtein = false;
                pm.CreateProtein(n);
                pm.StopAnim();
            }
        } 
    }

    public void PickUpItem(GameObject item){ //pick up an item, and sort it into the correct script type
        HandleHasItem();
        heldItem = item;
        //disable collider
        Collider c = heldItem.GetComponent<Collider>();
        if (c !=null) c.enabled = false;
        Collider2D c2 = heldItem.GetComponent<Collider2D>();
        if (c2 !=null) c2.enabled = false;
        
        if (item.GetComponent<Ingredient>()){
            heldIngredient = item.GetComponent<Ingredient>();
        }
        else if(item.GetComponent<Tool>()){
            heldTool = item.GetComponent<Tool>();
        }
        else {
            isHoldingBase = true;
        }
    }

    //drop the held item, if its type matches what is wanted by caller 
    public GameObject DropItem(string type){
        GameObject held = null;
        if (type == "ingredient" && heldIngredient!= null && heldIngredient.type!= Ingredient.Type.Base){
            held = heldItem;
            heldIngredient = null;
            heldItem = null;
        }
        else if (type == "tool" && heldTool!= null){
            held = heldItem;
            heldTool = null;
            heldItem = null;
        }
        else if (type == "base" && heldIngredient== null && heldTool== null){
            held = heldItem;
            heldItem = null;
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
        isHoldingBase = false;
        
        isHandFree = true;
        //cam.ShowButtons();
    }

    private void HandleHasItem(){
        isHandFree = false;
        //cam.HideButtons();
    }

    //add held ingredient to the order
    public void AddToCurrentOrder(){
        if (heldIngredient!= null && heldIngredient.AtEndState()){
            //check if the type is accepted, if it is then add the ingredient
            if (CheckCanAddIngredient(heldIngredient.type, currentOrder.Count)){
                currentOrder.Add(heldIngredient);
                UpdateOrderUI(heldIngredient.name); //FIX: delete
                heldIngredient.gameObject.SetActive(false);
                HandleNoItems();
            }
        }
    }
    private bool CheckCanAddIngredient(Ingredient.Type t, int ingredientsAdded){
        if((ingredientsAdded == 0 && t == Ingredient.Type.Base) 
        || (ingredientsAdded == 1 && t == Ingredient.Type.Carb) 
        || (ingredientsAdded >=2 && (t != Ingredient.Type.Base && t != Ingredient.Type.Carb)))
            return true;
        return false;
    }

    public void ClearOrder(){
        // Debug.Log(currentOrder.Count);
        foreach (Ingredient i in currentOrder){
            Destroy(i.gameObject);
        }
        currentOrder.Clear();
        tempOrderText.text = "Order:";//FIX: delete
    }

    private void UpdateOrderUI(string name){ //FIX: delete
        tempOrderText.text = tempOrderText.text + "  " + name;
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
