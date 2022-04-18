using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProteinSection : MonoBehaviour
{
    [SerializeField] GameObject proteinPrefab;
    Color initialColour = Color.white;
    [SerializeField] Color highlightColour = Color.green;
    Image img;
    ProteinSelector ps;

    private void Awake(){
        ps = GetComponentInParent<ProteinSelector>();
        img = GetComponent<Image>();
    }

    private void OnMouseEnter(){
        //highlight section
        img.color = highlightColour;
    }

    private void OnMouseExit(){
        //reset section
        img.color = initialColour;
    }

    private void OnMouseUpAsButton(){
        //create protein but show tentacle being cut before showing protein
        //show tentacle cutting
        ps.AnimateCutTentacle(CreateProtein());
    }

    private GameObject CreateProtein(){
        if (proteinPrefab != null){
            return Instantiate(proteinPrefab, ps.gm.ingredientParent);
        }
        return null;
    }

    private void ResetVars(){
        img.color = initialColour;
        ps.CloseSelector();
    }
}
