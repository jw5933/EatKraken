using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObj : MonoBehaviour
{
   [SerializeField] GameObject obj;

   void OnMouseDown(){
       obj.SetActive(true);
   }
}
