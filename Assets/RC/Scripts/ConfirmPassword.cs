using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class ConfirmPassword : MonoBehaviour {

    private bool isInput = false;
    private InputField inputField;
    public int CHARACTER_LIMIT = 12;

    void Start () {
        inputField = GetComponent<InputField>();
        inputField.characterLimit = CHARACTER_LIMIT;
    }
	
	void Update () {
		
	}

    public void CheckInput()
    {
        InputField inputPwd = GameObject.Find("InputPassword").GetComponent<InputField>();
        string password = inputPwd.text;
        string confirmPwd = inputField.text;
        StringBuilder stringBuilder = new StringBuilder();
        if (confirmPwd.Length < 6)
            stringBuilder.Append("密码长度需为6-12位");
        else if (!password.Equals(confirmPwd))
        {
            stringBuilder.Append("两次输入的密码不一致");
        }
        GameObject.Find("TextConfirmPasswordHint").GetComponent<Text>().text = stringBuilder.ToString();

    }
}
