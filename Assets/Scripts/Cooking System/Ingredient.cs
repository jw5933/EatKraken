using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    // ==============   variables   ==============
    //references
    private Player player;
    private SharedArea myArea;
    public SharedArea area{set{myArea = value;}}


    //type of ingredient
    public enum Type {Base, Carb, Vegetable, Protein}
    [SerializeField] private Type myType = Ingredient.Type.Vegetable;
    public Type type {get{return myType;}}

    //personalized variables for each kind of non-protein (order checking)
    [SerializeField] private string myName;
    public new string name {get{return myName;}}
    [SerializeField] private float myPrice;
    public float price {get{return myPrice;}}

    //image states
    [SerializeField] private Sprite[] imageStates;
    [HideInInspector][SerializeField] private int myImageState = 0; //initial state is 0
    public int state{get{return myImageState;}}
    public Sprite initialSprite {get{return imageStates[0];}}
    private SpriteRenderer mySpriteRenderer;

    //final image state
    [SerializeField] private Sprite[] finalImageStates;
    private int myFinalImageState;

    //player interaction
    [SerializeField] private int motionsToStateChange = 1; //needed number of motions to change state
    [HideInInspector][SerializeField] private int myMotionsLeft; //number of motions left until state change -> resets to neededMotions
    public int motionsLeft {get{return myMotionsLeft;}}

    //tool and visual lines
    [SerializeField] private Tool.Required myRequired = Tool.Required.None;
    public Tool.Required required {get{return myRequired;}}
    [SerializeField] private List<ToolLine> myToolLines = new List<ToolLine>(); //debugging
    private bool hovered;

    //vectors
    Collider myCollider;
    Plane myPlane;
    public Plane plane {get{return myPlane;}}

    // ==============   methods   ==============
    public void OnEnable(){
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider>();
        foreach (Transform child in transform){
            myToolLines.Add(child.GetComponent<ToolLine>());
        }
        InactivateToolLines();
    }
    private void Start(){
        player = FindObjectOfType<Player>();
        myMotionsLeft = motionsToStateChange;
    }

    //pick up item
    private void OnMouseDown(){ //if the player isnt holding anything, pick up this ingredient
        if (!player.handFree) return;
        //Debug.Log(this.gameObject.name);

        player.PickUpItem(this.gameObject);
        
        if (myArea != null){
            myArea.HandlePickUp();
            myArea = null;
        }
        InactivateToolLines();
    }
    
    //check if this ingredient is on a cutting board and accepts the tool held by player
    private void OnMouseEnter(){
        //Debug.Log("entered ingredient: " + this.name);
        if (AtEndState()) return;
        if (!hovered && myArea !=null && myArea.type == SharedArea.AreaType.CuttingBoard && !player.handFree){
            hovered = true;
            //if the player is holding a tool, activate the toollines and the plane for calculation
            if (player.ValidateToolLines(this) && myArea.type == SharedArea.AreaType.CuttingBoard){
                UpdatePlane();
                ActivateToolLines();
            }
        }
    }

    private void UpdatePlane(){
        Vector3 max = myCollider.bounds.max;
        Vector3 min = myCollider.bounds.min;
        Vector3 center = myCollider.bounds.center;
        //get the vector sides
        Vector3 side1 = transform.right + center;
        Vector3 side2 = transform.up + center;
        //get the perpendicular vector
        Vector3 perp = Vector3.Cross(side1, side2);
        //normalize perp vector
        Vector3 norm = perp.normalized;
        myPlane = new Plane(norm, center);

        //debugging
        // Debug.DrawLine(transform.right + center, center, Color.red, 100f);
        // Debug.DrawLine(transform.up + center, center, Color.green, 100f);
        // Debug.DrawLine(norm + center, center, Color.blue, 100f);
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
    }
    
    public void ChangeState(){ //check if the image state of the object needs to be changed based on motions used
        if (myImageState >= imageStates.Length) return;
        myMotionsLeft--;
        if (myMotionsLeft <= 0) {
            myMotionsLeft = motionsToStateChange;
            myImageState++;
            if (myImageState < imageStates.Length) mySpriteRenderer.sprite = imageStates[myImageState];
        }
    }

    public bool AtEndState(){ //check if this ingredient has reached its end state
        if (myImageState >= imageStates.Length-1) return true;
        return false;
    }

    public void HandleAddToOrder(){
        if (myFinalImageState >= finalImageStates.Length) return;
        mySpriteRenderer.sprite = finalImageStates[myFinalImageState++];
    }

    public void ResetVars(){ //reset some variables: tool lines, 
        InactivateToolLines();
        InvalidateToolLines();
    }
}
