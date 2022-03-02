using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MessageButton : MonoBehaviour, IPointerClickHandler
{
    private PopUpMessage myParentMessage;
    // Start is called before the first frame update
    void Awake(){
        GameObject o = transform.parent.gameObject;
        while (o.GetComponent<PopUpMessage>() == null) o = o.transform.parent.gameObject;
        myParentMessage = o.GetComponent<PopUpMessage>();
    }

    public void OnPointerClick(PointerEventData eventData){
        Debug.Log("clicked button");
        myParentMessage.choice = true;
        //myParentMessage.choiceMade = true;
        myParentMessage.ReturnChoice();
    }
}
