using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DimensionCollapse;

public class Test : MonoBehaviour {

	// Use this for initialization
	async void Start () {
        var manager = SupplyPointManager.INSTANCE;

        MySqlManagers.AccountManager accountManager = MySqlManagers.AccountManager.INSTANCE;
        accountManager.ShowAllAccountsAsync();
        Debug.Log(await accountManager.IsOnlineAsync("swt369"));
        Debug.Log(await accountManager.IsAccountExistsAsync("swt369"));
        Debug.Log(await accountManager.IsAccountExistsAsync("asdsad"));
        Debug.Log(await accountManager.VerifyAccountAndPasswordAsync("swt369", "21751001"));
        Debug.Log(await accountManager.VerifyAccountAndPasswordAsync("swt369", "2175100df1"));
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("update");
    }
}
