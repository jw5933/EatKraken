using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolLine: Draggable
{
    // ==============   variables   ==============
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

        this.minDistance = Mathf.Max(myCollider.bounds.size.y, myCollider.bounds.size.x) * minPercentOfDist;
    }

    private void OnMouseEnter(){
        if (player.handFree) return;
        //make sure player is moving on this items (parent) plane
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
        mySpriteRend.color = Color.green;
        initialPos = base.GetProjectionOnPlane();
    }

    private void OnMouseUpAsButton(){
        if (player.handFree || !iCanClick) return;
        Vector3 endPos = base.GetProjectionOnPlane();

        base.VerifyDistance(endPos, initialPos);
    }
    protected override void HandleDragged(){
        ingredient.ChangeImageState();
        ingredient.RemoveToolLine(this);
        Destroy(this.gameObject);
    }
    protected override void HandleNotDragged(){
        mySpriteRend.color = Color.yellow;
    }

    protected void ResetVars(){
        mySpriteRend.color = Color.white;
    }
}
