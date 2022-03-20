using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable: MonoBehaviour
{

protected Plane myPlane;
protected float minDistance;

    public Vector3 GetProjectionOnPlane(){
        //Initialize variables
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter = 0.0f;
        //find point on plane
        if(myPlane.Raycast(ray, out enter)){
            return ray.GetPoint(enter);
        }
        return Vector3.zero;
    }
    
    public void VerifyDistance(Vector3 end, Vector3 start){
        Debug.Log(end.ToString());
        Debug.Log(start.ToString());
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
