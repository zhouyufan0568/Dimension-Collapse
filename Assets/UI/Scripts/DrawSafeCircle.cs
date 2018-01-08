using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class DrawSafeCircle : MonoBehaviour
    {

        private float[] circle_now;
        private float[] circle_bef;
        public int numOfEdge = 1000;

        private static Material lineMaterial;

        static void CreateLineMaterial()
        {
            if (!lineMaterial)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Custom/SafeCircle");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
                // Turn on alpha blending
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }

        void OnPostRender()
        {
            CreateLineMaterial();
            GL.LoadOrtho();//设置绘制2D图像

            // Apply the line material
            lineMaterial.SetPass(0);

            circle_now = new float[3] { 0.5f, 0.5f, 0.1f };
            circle_bef = new float[3] { 0.3f, 0.3f, 0.1f };

            //now the safe area 
            GL.Begin(GL.LINES);//如果是填充圆则用GL.Begin(GL.QUADS);  
            GL.Color(new Color(1, 1, 1, 1));
            Vector2 Circle_center_point = new Vector2(circle_now[0], circle_now[1]);
            float Circle_r_x = circle_now[2];
            float Circle_r_y = Circle_r_x * Screen.width / Screen.height;
            int n = numOfEdge;//实质是绘制一个正numOfEdge边形 
            GL.Vertex(new Vector2(Circle_center_point.x + Circle_r_x * Mathf.Cos(2 * Mathf.PI / n * 0), Circle_center_point.y + Circle_r_y * Mathf.Sin(2 * Mathf.PI / n * 0)));
            for (int i = 1; i < n; i++)//割圆术画圆  
            {
                GL.Vertex(new Vector2(Circle_center_point.x + Circle_r_x * Mathf.Cos(2 * Mathf.PI / n * i), Circle_center_point.y + Circle_r_y * Mathf.Sin(2 * Mathf.PI / n * i)));
                GL.Vertex(new Vector2(Circle_center_point.x + Circle_r_x * Mathf.Cos(2 * Mathf.PI / n * i), Circle_center_point.y + Circle_r_y * Mathf.Sin(2 * Mathf.PI / n * i)));
            }
            GL.Vertex(new Vector2(Circle_center_point.x + Circle_r_x * Mathf.Cos(2 * Mathf.PI / n * 0), Circle_center_point.y + Circle_r_y * Mathf.Sin(2 * Mathf.PI / n * 0)));

            GL.End();

            //before the safe area 
            GL.Begin(GL.LINES);//如果是填充圆则用GL.Begin(GL.QUADS);  
            GL.Color(new Color(1, 1, 1, 1));
            Circle_center_point = new Vector2(circle_bef[0], circle_bef[1]);
            Circle_r_x = circle_bef[2];
            Circle_r_y = Circle_r_x * Screen.width / Screen.height;
            GL.Vertex(new Vector2(Circle_center_point.x + Circle_r_x * Mathf.Cos(2 * Mathf.PI / n * 0), Circle_center_point.y + Circle_r_y * Mathf.Sin(2 * Mathf.PI / n * 0)));
            for (int i = 1; i < n; i++)//割圆术画圆  
            {
                GL.Vertex(new Vector2(Circle_center_point.x + Circle_r_x * Mathf.Cos(2 * Mathf.PI / n * i), Circle_center_point.y + Circle_r_y * Mathf.Sin(2 * Mathf.PI / n * i)));
                GL.Vertex(new Vector2(Circle_center_point.x + Circle_r_x * Mathf.Cos(2 * Mathf.PI / n * i), Circle_center_point.y + Circle_r_y * Mathf.Sin(2 * Mathf.PI / n * i)));
            }
            GL.Vertex(new Vector2(Circle_center_point.x + Circle_r_x * Mathf.Cos(2 * Mathf.PI / n * 0), Circle_center_point.y + Circle_r_y * Mathf.Sin(2 * Mathf.PI / n * 0)));
            GL.End();
        }
        void OnGUI()
        {

        }

        // Use this for initialization
        void Start()
        {
            this.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
