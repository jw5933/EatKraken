using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSprites: MonoBehaviour
{
    [SerializeField] private Sprite[] mySprites;
    public Sprite[] sprites {get{return mySprites;}}
}
