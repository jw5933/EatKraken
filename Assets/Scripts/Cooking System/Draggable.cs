using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable: MonoBehaviour
{

    protected Player player;
    protected float minDistance;

    private void Awake(){
        player = FindObjectOfType<Player>();
    }

    protected Vector3 GetProjectionOnPlane(){
        //Initialize variables
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter = 0.0f;
        //find point on plane
        if(player.currPlane.Raycast(ray, out enter)){
            return ray.GetPoint(enter);
        }
        return Vector3.zero;
    }
    
    protected void VerifyDistance(Vector3 end, Vector3 start){
        Debug.Log(end.ToString());
        Debug.Log(start.ToString());
        Vector3 offset = end - start;
        float distance = offset.sqrMagnitude;

        if (distance >= minDistance * minDistance)
            HandleDragged();
        else
            HandleNotDragged();
    }
    
    protected virtual void HandleDragged(){
        Debug.Log("called handle dragged");
    }

    protected virtual void HandleNotDragged(){
        Debug.Log("called handle not dragged");
    }
}
