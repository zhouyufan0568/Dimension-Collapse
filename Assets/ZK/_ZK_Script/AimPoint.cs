using UnityEngine;  
using System.Collections;  
  
public class AimPoint : MonoBehaviour {  
  
   public  Texture texture;  
    // Use this for initialization  
    void Start () {  
    }  
      
    // Update is called once per frame  
    void Update () {  
      
    }  
    void OnGUI()  
    {  
        Rect rect = new Rect((Screen.width >> 1) - (texture.width >> 1),  
            (Screen.height >> 1) - (texture.height >> 1),  
            texture.width, texture.height);  
  
        GUI.DrawTexture(rect, texture);  
    }  
} 