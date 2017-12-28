using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BtnLoginScript : MonoBehaviour {

    private Button btn;
    private static int wrongTimes = 0;
    private enum LoginResult
    {
        success = 0,
        acount_not_exist = 1,
        password_wrong = 2
    }
    private static string ACCOUNT_NOT_EXIST = "账号不存在";
    private static string PWD_WRONG = "密码不正确";

    void Start()
    {
       
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);

        //int width = Screen.width/6;
        //int height = Screen.height/10;

        //string ip = "127.0.0.1";
        //int port = 8885;
        //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //socket.Connect(new IPEndPoint(IPAddress.Parse(ip),port));

        //socket.Send(Encoding.ASCII.GetBytes("sadsad,"));

        RectTransform rectTransform = transform as RectTransform;
        if (rectTransform != null)
        {
            //rectTransform.sizeDelta = new Vector2(width, height);
            //rectTransform.transform.position = new Vector3(400, 100, 100);
        }
    }

    private void OnClick()
    {
        string account = GameObject.Find("TxtAccount").GetComponent<Text>().text;
        InputField inputPwd = GameObject.Find("InputPassword").GetComponent<InputField>();
        string password = inputPwd.text;
        string accountHint = GameObject.Find("TextAccountHint").GetComponent<Text>().text;
        string passwordHint = GameObject.Find("TextPasswordHint").GetComponent<Text>().text;
        // 如果格式存在问题 则不进行账号正确性的判断
        if (accountHint != null && accountHint.Length > 0 || passwordHint != null && passwordHint.Length > 0)
        {
            if(!accountHint.Equals(ACCOUNT_NOT_EXIST) || !passwordHint.Equals(PWD_WRONG))
            {
                return;
            }
        }
        LoginResult isConfirm = ConfirmAccount(account,password);
        switch (isConfirm)
        {
            case LoginResult.success:
                SceneManager.LoadScene("MainScene");
                return;
            case LoginResult.acount_not_exist:
                GameObject.Find("TextAccountHint").GetComponent<Text>().text = ACCOUNT_NOT_EXIST;
                wrongTimes++;
                return;
            case LoginResult.password_wrong:
                GameObject.Find("TextPasswordHint").GetComponent<Text>().text = PWD_WRONG;
                wrongTimes++;
                return;
        }
    }

    private LoginResult ConfirmAccount(string account, string password)
    {     
        if(account.Equals("a11111"))
        {
            return LoginResult.acount_not_exist;
        }
        else if (account.Equals("a12345") && password.Equals("123456"))
        {
            return LoginResult.success;
        }
        else
        {
            return LoginResult.password_wrong;
        }
    }

}
