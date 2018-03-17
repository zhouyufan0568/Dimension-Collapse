using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class BloodUI : MonoBehaviour
    {
        public static BloodUI Instance;
        private CanvasGroup canvasGroup;

        private bool hasBlood = false;
        private float i = 0;
        // Use this for initialization
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }
        void Start()
        {
            canvasGroup = this.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                Debug.Log("Waring: this blood UI don't have CanvasGroup component");
            }
            else { this.canvasGroup.alpha = 0; }
        }

        // Update is called once per frame
        void Update()
        {
            // if (Input.GetKeyDown(KeyCode.P))
            // {
            //     displayBlood();
            // }
        }

        public void displayBlood()
        {
            if (canvasGroup == null)
            {
                return;
            }
            i = 0f;
            if (!hasBlood)
            {
                hasBlood = true;
                StartCoroutine(bloodAnimation());
            }
        }
        private IEnumerator bloodAnimation()
        {
            //等待延迟时间
            for (; i < 1.0; i += Time.deltaTime)
            {
                canvasGroup.alpha = 1.0f - i;
                yield return 0;
            }
            this.canvasGroup.alpha = 0;
            hasBlood = false;
        }
    }

}
