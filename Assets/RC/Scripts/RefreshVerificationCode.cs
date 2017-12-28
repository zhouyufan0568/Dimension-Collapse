using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefreshVerificationCode : MonoBehaviour {

	void Start () {

        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
	}

    private void OnClick()
    {
        Animator animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
        int i = 0;
        while(i < 1)
        {
            
        }
    }
}
