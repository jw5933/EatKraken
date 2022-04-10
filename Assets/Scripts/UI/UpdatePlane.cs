using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePlane : MonoBehaviour
{
    Player p;
    Plane myPlane;
    Collider myCollider;
    Collider2D myCollider2D;

    private void Awake(){
        p = FindObjectOfType<Player>();
        myCollider = GetComponent<Collider>();
        if (myCollider == null) myCollider = transform.parent.GetComponent<Collider>();
        myCollider2D = GetComponent<Collider2D>();
    }

    private void Start(){
        CalculatePlane();
    }

    private void OnMouseEnter(){
        //Debug.Log("updating plane of " + this.name);
        p.currPlane = myPlane;
    }

    private void CalculatePlane(){
        Vector3 center = myCollider != null ? myCollider.bounds.center: myCollider2D.bounds.center;
        //get the vector sides
        Vector3 side1 = transform.right;
        Vector3 side2 = transform.up;
        
        //get the perpendicular vector
        Vector3 perp = Vector3.Cross(side1, side2);
        
        //normalize perp vector
        Vector3 norm = perp.normalized;
        myPlane = new Plane(norm, center);

        //debugging
        /* Debug.DrawLine(transform.right + center, center, Color.red, 100f);
        Debug.DrawLine(transform.up + center, center, Color.green, 100f);
        Debug.DrawLine(norm + center, center, Color.blue, 100f); */
    }
}
