using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSafeCircle : MonoBehaviour {

    void OnPostRender()
    {
        //画圆  
        GL.Begin(GL.LINES);//如果是填充圆则用GL.Begin(GL.QUADS);  
        GL.Color(Color.yellow);
        Vector2 Circle_center_point = new Vector2(0.75f, 0.75f);
        float Circle_r_x = 0.1f;
        float Circle_r_y = Circle_r_x * Screen.width / Screen.height;
        int n = 1000;//实质是绘制一个正1000边形  
        for (int i = 0; i < n; i++)//割圆术画圆  
        {
            GL.Vertex(new Vector2(Circle_center_point.x+Circle_r_x * Mathf.Cos(2 * Mathf.PI / n * i), Circle_center_point.y+Circle_r_y * Mathf.Sin(2 * Mathf.PI / n * i)));
        }
        GL.End();
    }
    void OnGUI()
    {
        string string_content = "矩形和圆形";
        GUIStyle fontStyle = new GUIStyle();
        fontStyle.normal.background = null;//设置背景填充    
        fontStyle.normal.textColor = Color.white;//设置字体颜色    
        fontStyle.fontSize = 24;//字体大小    
        Vector2 string_size = fontStyle.CalcSize(new GUIContent(string_content));
        GUI.Label(new Rect(Screen.width - string_size.x, Screen.height - string_size.y, string_size.x, string_size.y), string_content, fontStyle);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
