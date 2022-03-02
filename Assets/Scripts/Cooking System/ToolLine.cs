using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolLine: MonoBehaviour
{
    // ==============   variables   ==============
    private Player player;
    private SpriteRenderer mySpriteRend;

    private bool iCanClick;
    public bool canClick {set{iCanClick = value;}}

    [SerializeField] private float minPercentOfDist;
    private Collider2D myCollider;
    private float minDistance;
    private Vector3 initialPos;

    Ingredient i;

    // ==============   functions   ==============
    private void Awake(){
        player = FindObjectOfType<Player>();
        mySpriteRend = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();

        minDistance = myCollider.bounds.size.y * minPercentOfDist;
    }

    private void OnMouseEnter(){
        if (player.handFree) return;
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

        i = transform.parent.GetComponent<Ingredient>();
        //Initialize variables
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter = 0.0f;
        //find point on plane
        if(i.plane.Raycast(ray, out enter)){
            initialPos = ray.GetPoint(enter);
            Debug.Log(string.Format("{0} {1}", "initial: ", initialPos.ToString()));
            Debug.DrawLine(initialPos, Camera.main.ScreenToWorldPoint(Input.mousePosition), Color.yellow, 100f);
        }
    }

    private void OnMouseUpAsButton(){
        if (player.handFree || !iCanClick) return;
        //Vector3 offset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - initialPos;
        
        //Initialize variables
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter = 0.0f;
        Vector3 projection = Vector3.zero;
        //find point on plane
        if(i.plane.Raycast(ray, out enter)){
            projection = ray.GetPoint(enter);
            Debug.DrawLine(projection, Camera.main.ScreenToWorldPoint(Input.mousePosition), Color.yellow, 100f);
        }

        Vector3 offset = projection - initialPos;
        float distance = offset.sqrMagnitude;

        Debug.Log(string.Format("{0} {1} {2} {3}", "initial: ", initialPos.ToString(), ", offset: ", offset.ToString()));
        
        if (distance >= minDistance * minDistance){
            i.ChangeState();
            i.RemoveToolLine(this);
            Destroy(this.gameObject);
        }
        else{
            mySpriteRend.color = Color.yellow;
        }
    }

    public void ResetVars(){
        initialPos = Vector3.zero;
        mySpriteRend.color = Color.white;
    }
}
