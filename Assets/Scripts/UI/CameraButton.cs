using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraButton : Button, IPointerClickHandler
{
    CameraManager cam;
    Tentacle t;

    private void Awake(){
        cam = FindObjectOfType<CameraManager>();
        t = FindObjectOfType<Tentacle>();
    }
    public void OnPointerClick(PointerEventData eventData){
        if (gameObject.name == "left"){
            cam.SwapToCam(cam.camIndex-1 >=0 ? cam.camIndex-1: cam.camIndex);
            t.UpdateEndPos(-1);
        }
            
        if (gameObject.name == "right"){
            cam.SwapToCam(cam.camIndex+1 < cam.maxCamIndex ? cam.camIndex+1: cam.camIndex);
            t.UpdateEndPos(1);
        }
    }
}
