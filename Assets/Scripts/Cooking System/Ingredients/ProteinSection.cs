using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProteinSection : MonoBehaviour
{
    [SerializeField] GameObject proteinPrefab;
    ProteinSelector ps;
    private Material material;
    private Image image;
    Color highlightColor = Color.green;
    Color initialColor = Color.white;

    private void Awake(){
        ps = GetComponentInParent<ProteinSelector>();
        image = GetComponent<Image>();
        //material = GetComponent<Image>().material;
    }

    private void OnMouseEnter(){
        image.color = highlightColor;
        //material.SetFloat("_Outline", 1);
    }

    private void OnMouseExit(){
        image.color = initialColor;
        //material.SetFloat("_Outline", 0);
    }

    private void OnMouseUpAsButton(){
        //create protein but show tentacle being cut before showing protein
        //show tentacle cutting
        ps.AnimateCutTentacle(CreateProtein());
        image.color = initialColor;
    }

    private GameObject CreateProtein(){
        if (proteinPrefab != null){
            return Instantiate(proteinPrefab, ps.gameManager.proteinParent);
        }
        return null;
    }
}
