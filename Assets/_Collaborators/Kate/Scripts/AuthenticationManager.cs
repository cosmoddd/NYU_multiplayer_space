using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Authenticators;


public class AuthenticationManager : MonoBehaviour
{
    // Start is called before the first frame update
    //public NewNetworkAuthenticator authenticator;

    public LoginInfo[] loginInfoList;

    Dictionary<string,string> loginInfoDictionary;

    public Customizer customizerScript;

    public NetworkManager manager;

    public LoginInfoReader loginReader;

    private void Awake() 
    {
        loginInfoList = loginReader.ReadFile();
        
        loginInfoDictionary = new Dictionary<string, string>();
        foreach(LoginInfo info in loginInfoList)
        {
            loginInfoDictionary.Add(info.email, info.password);
        }
        
    }

    private void Start()
     {
        //print(loginInfoDictionary["Greg"]);
        
    }

    public bool Login(string email, string password)
    {
        if(loginInfoDictionary.ContainsKey(email))
        {
            //print("dictionary contains : "+ email);
            if(loginInfoDictionary[email] == password)
            {
                //print("correct password");
                return true;
            }else
            {
                //print("wrong password: "+ loginInfoDictionary[email]);
            }
        }
        return false;

    }
     public void EnterHost()
    {
        //SAVE PRESET
        customizerScript.SaveData();
        manager.StartHost();
    }

    public void EnterClient()
    {
        //SAVE PRESET
        customizerScript.SaveData();
        manager.StartClient();
        
    }
}

[System.Serializable]
public class LoginInfo
{
    public string email;
    public string password;

    public string[] tags;

    public bool isType(string _tag)
    {
        foreach(string tag in tags)
        {
            if(tag == _tag) return true;
        }
        return false;

    }

    public LoginInfo(string _u, string _p, string[] _tagString)
    {
        email = _u;
        password = _p;



        //string[] Ttags = _tagString.Split(' ');
        tags = _tagString;
    }

}
