using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DimensionCollapse;

namespace DimensionCollapse
{


    //武器范围测试
    public class Temp_Scarecrow : MonoBehaviour
    {

        public float health;
        // Use this for initialization
        void Start()
        {
            health = 200;
        }

        // Update is called once per frame
        void Update()
        {
            if (health < 0)
            {
                health = 0;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                this.health = 200;
            }
        }
        void OnGUI()
        {
            Rect rect = new Rect(50, 0, 100, 50);
            GUI.TextField(rect, "剩余" + health + "血量");
        }
        public void OnAttacked(int primaryDamge, ContactPoint contact)
        {
            //Debug.Log(primaryDamge + "受到的伤害");
            this.health -= primaryDamge;
        }
        public void OnAttacked(int primaryDamge)
        {
            //Debug.Log(primaryDamge + "受到的伤害");
            this.health -= primaryDamge;
        }

        /* 
        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("It is from Scarecrow's OnCollisionEnter function");

        }
        void OnCollisionStay(Collision other)
        {
            Debug.Log("It is from Scarecrow's OnCollisionStay function");
        }
        private void OnCollisionExit(Collision other)
        {
            Debug.Log("It is from Scarecrow's OnCollisionExit function");
        }
        */
    }
}