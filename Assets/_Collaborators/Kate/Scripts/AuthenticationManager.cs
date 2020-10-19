using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Authenticators;

public class AuthenticationManager : MonoBehaviour
{
    // Start is called before the first frame update
    //public NewNetworkAuthenticator authenticator;

    public List<LoginInfo> loginInfoList = new List<LoginInfo>();

    Dictionary<string,string> loginInfoDictionary;

    public CharacterCustomizerScript customizerScript;

    public NetworkManager manager;

    private void Awake() 
    {
        loginInfoDictionary = new Dictionary<string, string>();
        foreach(LoginInfo info in loginInfoList)
        {
            loginInfoDictionary.Add(info.username, info.password);
        }
    }

    private void Start()
     {
        print(loginInfoDictionary["Greg"]);
        
    }

    public bool Login(string username, string password)
    {
        if(loginInfoDictionary.ContainsKey(username))
        {
            //print("dictionary contains : "+ username);
            if(loginInfoDictionary[username] == password)
            {
                //print("correct password");
                return true;
            }else
            {
                //print("wrong password: "+ loginInfoDictionary[username]);
            }
        }
        return false;

    }
     public void EnterHost()
    {
        //SAVE PRESET
        customizerScript.SaveTraitsToScript();
        manager.StartHost();
    }

    public void EnterClient()
    {
        //SAVE PRESET
        customizerScript.SaveTraitsToScript();
        manager.StartClient();
        
    }
}

[System.Serializable]
public class LoginInfo
{
    public string username;
    public string password;

    LoginInfo(string _u, string _p)
    {
        username = _u;
        password = _p;

    }

}
