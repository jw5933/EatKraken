using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable: MonoBehaviour
{

protected Plane myPlane;
protected float minDistance;

    public virtual Vector3 GetProjectionOnPlane(){
        //Initialize variables
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter = 0.0f;
        //find point on plane
        if(myPlane.Raycast(ray, out enter)){
            return ray.GetPoint(enter);
        }
        return Vector3.zero;
    }
    
    public virtual void VerifyDistance(Vector3 end, Vector3 start){
        Vector3 offset = end - start;
        float distance = offset.sqrMagnitude;

        if (distance >= minDistance * minDistance)
            HandleDragged();
        else
            HandleNotDragged();
    }
    
    public virtual void HandleDragged(){
        Debug.Log("called handle dragged");
    }

    public virtual void HandleNotDragged(){
        Debug.Log("called handle not dragged");
    }
}
