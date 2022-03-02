using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraButton : Button, IPointerClickHandler
{
    CameraManager cam;
    private void Awake(){
        cam = FindObjectOfType<CameraManager>();
    }
    public void OnPointerClick(PointerEventData eventData){
        if (gameObject.name == "left")
            cam.SwapToCam(cam.camIndex-1 >=0 ? cam.camIndex-1: cam.camIndex);
        if (gameObject.name == "right")
            cam.SwapToCam(cam.camIndex+1 < cam.maxCamIndex ? cam.camIndex+1: cam.camIndex);
        if (gameObject.name == "up" || gameObject.name == "down")
            cam.SwapUpDownCam();
    }
}
