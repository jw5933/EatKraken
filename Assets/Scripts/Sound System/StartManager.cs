using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    public GameObject audio;
    void Awake(){
        AudioManager am = FindObjectOfType<AudioManager>();
        if (am == null) Instantiate(audio);
    }
}
