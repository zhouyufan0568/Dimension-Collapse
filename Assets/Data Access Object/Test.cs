using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DimensionCollapse;

public class Test : MonoBehaviour {

	// Use this for initialization
	async void Start () {
        MySqlManagers.AccountManager accountManager = MySqlManagers.AccountManager.INSTANCE;
        accountManager.ShowAllAccountsAsync();
        Debug.Log(await accountManager.IsOnlineAsync("swt369"));
        Debug.Log(await accountManager.IsAccountExistsAsync("swt369"));
        Debug.Log(await accountManager.IsAccountExistsAsync("asdsad"));
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("update");
    }
}
