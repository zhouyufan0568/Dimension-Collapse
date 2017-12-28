using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Text;

public class InputAccountScript : MonoBehaviour {
    private bool isInput = false;
    private InputField inputField;
    public int CHARACTER_LIMIT = 18;

    void Start () {
        inputField = GetComponent<InputField>();
        inputField.characterLimit = CHARACTER_LIMIT;
    }
	
	void Update () {
        //if (inputField.isFocused)
        //{
        //    isInput = true;
        //}
        //else
        //{
        //    if (isInput)
        //    {
        //        CheckInput();
        //    }
        //}
    }

    public void CheckInput()
    {
        string inputText = inputField.text;
        StringBuilder stringBuilder = new StringBuilder();
        if (inputText.Length < 6)
            stringBuilder.Append("账号长度需为6-18位");

        if (!Regex.IsMatch(inputText, "^[a-zA-Z]"))
        {
            stringBuilder.Append(" 账号首字母需为英文");
        }
        GameObject.Find("TextAccountHint").GetComponent<Text>().text = stringBuilder.ToString();
    }

}
