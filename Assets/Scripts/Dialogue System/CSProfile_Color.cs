using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSProfile_Color : MonoBehaviour
{
    [SerializeField] private List<int> frames = new List<int>();
    [SerializeField] private List<Color> colors = new List<Color>();
    Image m_Image;

    Color m_NewColor;
    float m_Red, m_Blue, m_Green;

    [SerializeField] int endFrame;

    private void Awake(){
        if (colors.Count != 0) FindObjectOfType<EventManager>().OnDialogueChange += CheckAdjust;
        m_Image = GetComponent<Image>();
    }

    private void CheckAdjust(int frame){
        if (endFrame == frame){
            this.gameObject.SetActive(false);
            return;
        }
        if (frames[0] != frame) return;
        AdjustImage();
    }

    private void AdjustImage(){
        m_Image.color = colors[0];
        frames.RemoveAt(0);
        colors.RemoveAt(0);
    }
}
