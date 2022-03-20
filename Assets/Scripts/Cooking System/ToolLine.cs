using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolLine: Draggable
{
    // ==============   variables   ==============
    private Player player;
    private SpriteRenderer mySpriteRend;

    private bool iCanClick;
    public bool canClick {set{iCanClick = value;}}

    [SerializeField] private float minPercentOfDist;
    private Collider2D myCollider;
    private Vector3 initialPos;

    Ingredient ingredient;

    // ==============   functions   ==============
    private void Awake(){
        player = FindObjectOfType<Player>();
        mySpriteRend = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        ingredient = transform.parent.GetComponent<Ingredient>();
        this.myPlane = ingredient.plane;

        this.minDistance = Mathf.Max(myCollider.bounds.size.y, myCollider.bounds.size.x) * minPercentOfDist;
    }

    private void OnMouseEnter(){
        if (player.handFree) return;
        //make sure player is moving on this items (parent) plane
        player.currPlane = ingredient.plane;
        player.inSpace = false;
        if (iCanClick){
            mySpriteRend.color = Color.green;
        }
        else mySpriteRend.color = Color.red;
    }

    private void OnMouseExit(){
        if (player.handFree) return;
        ResetVars();
    }

    private void OnMouseDown(){
        if (player.handFree || !iCanClick) return;
        //Initialize variables
        initialPos = base.GetProjectionOnPlane();
    }

    private void OnMouseUpAsButton(){
        if (player.handFree || !iCanClick) return;
        Vector3 endPos = base.GetProjectionOnPlane();

        base.VerifyDistance(endPos, initialPos);
    }
    public override void HandleDragged(){
        ingredient.ChangeState();
        ingredient.RemoveToolLine(this);
        Destroy(this.gameObject);
    }
    public override void HandleNotDragged(){
        mySpriteRend.color = Color.yellow;
    }

    public void ResetVars(){
        mySpriteRend.color = Color.white;
    }
}
