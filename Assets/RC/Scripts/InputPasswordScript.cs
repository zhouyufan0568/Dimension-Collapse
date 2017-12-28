using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class InputPasswordScript : MonoBehaviour
{
    private bool isInput = false;
    private InputField inputField;
    public int CHARACTER_LIMIT = 12;

    void Start()
    {
        inputField = GetComponent<InputField>();
        inputField.characterLimit = CHARACTER_LIMIT;
    }

    void Update()
    {
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
            stringBuilder.Append("密码长度需为6-12位");
        GameObject.Find("TextPasswordHint").GetComponent<Text>().text = stringBuilder.ToString();

    }
}
