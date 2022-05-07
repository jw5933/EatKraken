using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverOutline : MonoBehaviour
{
    private Material material;
    public void Awake(){
        material = GetComponent<SpriteRenderer>().material;
    }
    private void OnMouseEnter(){
        material.SetFloat("_Outline", 1);
    }
    private void OnMouseExit(){
        material.SetFloat("_Outline", 0);
    }
}
