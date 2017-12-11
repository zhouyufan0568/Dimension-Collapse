using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetBackgroundFullScreen : MonoBehaviour {

    void Start()
    {
        int width = Screen.width;
        int height = Screen.height;

        RectTransform rectTransform = this.transform as RectTransform;
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(width, height);
        }

      
    }
}
