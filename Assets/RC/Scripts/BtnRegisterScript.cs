using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BtnRegisterScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }
	
	void OnClick()
    {
        SceneManager.LoadScene("RegisterScene");
    }
}
