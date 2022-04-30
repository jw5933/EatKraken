using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProteinSection : MonoBehaviour
{
    [SerializeField] GameObject proteinPrefab;
    ProteinSelector ps;
    private Material material;

    private void Awake(){
        ps = GetComponentInParent<ProteinSelector>();
        //material = GetComponent<Image>().material;
    }

    private void OnMouseEnter(){
        //material.SetFloat("_Outline", 1);
    }

    private void OnMouseExit(){
        //material.SetFloat("_Outline", 0);
    }

    private void OnMouseUpAsButton(){
        //create protein but show tentacle being cut before showing protein
        //show tentacle cutting
        ps.AnimateCutTentacle(CreateProtein());
    }

    private GameObject CreateProtein(){
        if (proteinPrefab != null){
            return Instantiate(proteinPrefab, ps.gm.proteinParent);
        }
        return null;
    }
}
