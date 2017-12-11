using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class Health : MonoBehaviour {

        public string player_tag;
        public int max_HP = 100;
        private float tempTime;
        private bool isDestroying = false;
        private Renderer rd;
        public float destroyDelay = 1f;
        public ParticleSystem[] particles;

        [SerializeField]private int HP;
        //private Camera camera;

        ////红色血条贴图
        //public Texture2D blood_red;
        ////黑色血条贴图
        //public Texture2D blood_black;

        public bool isLive = true;

        //显示人物头顶血条与名字
        //private void OnGUI()
        //{
        //    //得到NPC头顶在3D世界中的坐标
        //    //默认NPC坐标点在脚底下，所以这里加上npcHeight它模型的高度即可
        //    Vector3 worldPosition = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        //    //根据NPC头顶的3D坐标换算成它在2D屏幕中的坐标
        //    Vector3 position = camera.WorldToScreenPoint(worldPosition);
        //    //得到真实NPC头顶的2D坐标
        //    if (position.z > 0)
        //    {
        //        position = new Vector2(position.x, Screen.height - position.y);
        //        //注解2
        //        //计算出血条的宽高
        //        Vector2 bloodSize = GUI.skin.label.CalcSize(new GUIContent(blood_black)) / 2;

        //        //通过血值计算红色血条显示区域
        //        float blood_width = blood_black.width * HP / max_HP;
        //        //先绘制黑色血条
        //        GUI.DrawTexture(new Rect(position.x - (bloodSize.x / 2), position.y - bloodSize.y, bloodSize.x, bloodSize.y), blood_black);
        //        //在绘制红色血条
        //        GUI.DrawTexture(new Rect(position.x - (bloodSize.x / 2), position.y - bloodSize.y, blood_width / 2, bloodSize.y), blood_red);

        //        //注解3
        //        //计算NPC名称的宽高
        //        Vector2 nameSize = GUI.skin.label.CalcSize(new GUIContent(id));
        //        //设置显示颜色为黄色
        //        GUI.color = Color.white;
        //        //绘制NPC名称
        //        GUI.Label(new Rect(position.x - (nameSize.x / 2), position.y - nameSize.y - bloodSize.y, nameSize.x, nameSize.y), id);
        //    }

        //}

        void Start()
        {
            HP = max_HP;
            tempTime = 0;
            //获取材质本来的属性
            rd = GetComponent<Renderer>();
            if (rd != null)
            {
                rd.material.color = new Color
                (
                    this.GetComponent<Renderer>().material.color.r,
                    this.GetComponent<Renderer>().material.color.g,
                    this.GetComponent<Renderer>().material.color.b,   
                    this.GetComponent<Renderer>().material.color.a
                );
            }
           
        }

        private void Update()
        {
            if (!isLive&&!isDestroying)
            {
                if(rd!=null)
                    rd.material.color = new Color(0, 0, 0, 0);
                for(int i = 0; i < particles.Length; i++)
                {
                    particles[i].Play();
                }
                Destroy(this.gameObject, destroyDelay);
                isDestroying = true;
            }
        }

        //被击中事件处理
        public void onDamage(int demage,Vector3 point)
        {
            HP -= demage;
            if (HP <= 0)
            {
                HP = 0;
                isLive = false;
            }
            Debug.Log("被击中HP:"+HP);
        }

    }
}
