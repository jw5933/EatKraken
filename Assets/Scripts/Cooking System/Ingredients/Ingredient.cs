using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    // ==============   variables   ==============
    //personalized variables for each kind of non-protein (order checking)
    [SerializeField] private string myName;
    public new string name {get{return myName;}}
    [SerializeField] private float myPrice;
    public float price {get{return myPrice;}}

    //type of ingredient
    public enum Type {Base, Carb, Vegetable, Protein}
    [SerializeField] private Type myType = Ingredient.Type.Vegetable;
    public Type type {get{return myType;}}

    //ingredient cooked state
    public enum CookedState {Raw, Cooked, Burnt}
    private CookedState myCookedState = CookedState.Raw;
    public CookedState cookedState {get{return myCookedState;}}
    [SerializeField] private CookedState myRequiredCookedState;
    public CookedState requiredCookedState {get{return myRequiredCookedState;}}

    [SerializeField] private float rawTime;
    public float raw {get{return rawTime;}}
    [SerializeField] private float cookTime;
    public float cooked {get{return cookTime;}}
    [SerializeField] private float burntTime;
    public float burnt {get{return burntTime;}}
    public bool finishedCookedStage {get{return myCookedState != CookedState.Raw;}}

    private float myCookedTime;
    public float cookedTime {get{return myCookedTime;} set{myCookedTime = value;}}

    //image states
    [SerializeField] private Sprite[] imageStates;
    public int maxImageState {get{return imageStates.Length-1;}}
    private int myImageState = 0; //initial state is 0
    public int imgState{get{return myImageState;}}
    [SerializeField] int perfectSpriteState;
    public Sprite initialSprite {get{return imageStates[perfectSpriteState];}}

    //final image state
    [SerializeField] private Sprite finalImageState;
    public Sprite orderSprite {get{return finalImageState;}}
    public bool hasCookStage{get{return (myRequiredCookedState != CookedState.Raw);}}
    [SerializeField] private Sprite[] finalCookedImageStates = new Sprite[3];
    private int cookedImageState;

    //player interaction
    private int motionsToStateChange; //needed number of motions to change state
    private int myMotionsLeft; //number of motions left until state change -> resets to neededMotions
    public bool hasCutStage {get{return (motionsToStateChange > 0);}}
    public int motionsLeft {get{return myMotionsLeft;}}
    private bool isSliced;
    public bool finishedCutStage {get{return isSliced;}}

    //tool and visual lines
    [SerializeField] private Tool.Required myRequired = Tool.Required.None;
    public Tool.Required required {get{return myRequired;}}
    private List<ToolLine> myToolLines = new List<ToolLine>(); //debugging
    private bool hovered;

    //sounds
    [SerializeField] AudioClip sliceToolSound;

    //references
    Collider myCollider;
    private SpriteRenderer mySpriteRenderer;
    private Player player;
    private SharedArea myArea;
    public SharedArea area{set{myArea = value;}}
    private Transform spawner;
    public Transform parent {set{spawner = value;}}

    // ==============   methods   ==============
    public void Awake(){
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider>();

        foreach (Transform child in transform){
            if (child.gameObject.activeSelf && child.GetComponent<ToolLine>()){
                ToolLine t = child.GetComponent<ToolLine>();
                myToolLines.Add(t);
                t.sound = sliceToolSound;
            }
        }
        motionsToStateChange = myToolLines.Count;
        if (myToolLines.Count == 0) isSliced = true;
        InactivateToolLines();
    }
    private void Start(){
        player = FindObjectOfType<Player>();
        myMotionsLeft = motionsToStateChange;
    }

    //pick up item
    private void OnMouseUp(){ //if the player isnt holding anything, pick up this ingredient
        if (player.holdingIngredient){
            if (myArea != null && myArea.CheckSwapIngredient())
                myArea = null;
        }
        else if (!player.handFree) return;
        else{
            player.PickUpItem(this.gameObject);
            SetParent(spawner);
            if (myArea != null){
                myArea.HandlePickUp();
                myArea = null;
            }
        }
        //Debug.Log(this.gameObject.name);
        InactivateToolLines();
    }
    
    //check if this ingredient is on a cutting board and accepts the tool held by player
    private void OnMouseEnter(){
        //Debug.Log("entered ingredient: " + this.name);
        if (AtEndState()) return;
        if (!hovered && myArea !=null && myArea.type == SharedArea.AreaType.CuttingBoard && player.holdingTool){
            hovered = true;
            //if the player is holding a tool, activate the toollines and the plane for calculation
            if (player.ValidateToolLines(this) && myArea.type == SharedArea.AreaType.CuttingBoard){
                //UpdatePlane();
                ActivateToolLines();
            }
        }
    }

    public void SetParent(Transform t){
        transform.SetParent(t);
    }

    public void ValidateToolLines(){ //allows tool lines to be used (green)
        foreach (ToolLine t in myToolLines){
            t.canClick = true;
        }
    }

    private void InvalidateToolLines(){ //invalidates tool lines (red)
        foreach (ToolLine t in myToolLines){
            t.canClick = false;
        }
        hovered = false;
    }

    private void ActivateToolLines(){ //show tool lines
        //Debug.Log("activing tool lines");
        foreach (ToolLine t in myToolLines){
            t.gameObject.SetActive(true);
        }
    }
    public void InactivateToolLines(){
        //Debug.Log("deactivating tool lines");
        foreach (ToolLine t in myToolLines){ //hide tool lines
            t.gameObject.SetActive(false);
        }
    }

    public void RemoveToolLine(ToolLine t){
        myToolLines.Remove(t);
        if (myToolLines.Count == 0) isSliced = true;
    }
    
    public void ChangeImageState(){ //check if the image state of the object needs to be changed based on motions used
        if (myImageState >= imageStates.Length) return;
        myMotionsLeft--;
        if (myMotionsLeft <= 0) {
            myMotionsLeft = motionsToStateChange;
            myImageState++;
            if (myImageState < imageStates.Length) mySpriteRenderer.sprite = imageStates[myImageState];
        }
    }

    public virtual void ChangeCookedState(){
        ChangeImageState();
        if (cookedImageState < 2) cookedImageState++;
        switch(myCookedState){
            case CookedState.Raw:
                myCookedState = CookedState.Cooked;
            break;
            case CookedState.Cooked:
                myCookedState = CookedState.Burnt;
            break;
            case CookedState.Burnt:
                //FIX: what happens if its already burnt?? -> overcooked (fire)?
            break;
        }
    }

    public bool AtEndState(){ //check if this ingredient has reached its end state
        return isSliced;
    }

    public void HandleAddToOrder(){
        if (hasCookStage){
            mySpriteRenderer.sprite = finalCookedImageStates[cookedImageState];
        }
        else{
            if (finalImageState == null) return;
            mySpriteRenderer.sprite = finalImageState;
        }
    }

    public void ResetVars(){ //reset some variables: tool lines, 
        InactivateToolLines();
        InvalidateToolLines();
        myCollider.enabled = true;
    }
}
