using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : MonoBehaviour
{
    // Start is called before the first frame update
    Player p;
    Vector3 mousePos;

    void Start()
    {
        p = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        //Create a ray from the Mouse click position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Initialise the enter variable
        float enter = 0.0f;
        
        //move the object to where the mouse is
        if (p.currPlane.Raycast(ray, out enter)){
            mousePos = ray.GetPoint(enter);
            //Debug.DrawLine(Camera.main.ScreenToWorldPoint(Input.mousePosition), mousePos, Color.green);
            this.transform.position = mousePos;
        }
    }
}
