using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class UIController : MonoBehaviour
    {
        public Canvas HUDCanvas;
        public GameObject backpack;
        public GameObject map;

        private Camera player_main_camera;
        private Camera canvasCamera;

        // Use this for initialization
        void Start()
        {
            //canvasCamera = HUDCanvas.GetComponent<Canvas>().worldCamera;
            backpack.SetActive(false);
            map.SetActive(false);

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Backpack"))
            {
                backpack.SetActive(true);
            }
            if (Input.GetButtonUp("Backpack"))
            {
                backpack.SetActive(false);
            }
            if (Input.GetButtonDown("Map"))
            {
                map.SetActive(true);
            }
            if (Input.GetButtonUp("Map"))
            {
                map.SetActive(false);
            }
        }
    }
}
