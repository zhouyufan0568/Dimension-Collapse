using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DimensionCollapse
{
    public class BtnStartGameScriptWithOnline : MonoBehaviour
    {

        Button btn;
        public Launcher launcher;

        void Start()
        {
            btn = this.GetComponent<Button>();
            btn.onClick.AddListener(OnClick);

            int width = Screen.width / 6;
            int height = Screen.height / 10;


            RectTransform rectTransform = this.transform as RectTransform;
            if (rectTransform != null)
            {
                //rectTransform.sizeDelta = new Vector2(width, height);
                //rectTransform.transform.position = new Vector3(400, 100, 100);
            }
        }

        private void OnClick()
        {
            //GameObject.Find("LoginUI").SetActive(false);
            //GoNextScene(this.gameObject);
            launcher.Connect();
            //SceneManager.LoadScene("Gaming");
        }
    }
}
