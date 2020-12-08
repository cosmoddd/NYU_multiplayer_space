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

    Dictionary<string,string> passwordDictionary;

    Dictionary<string,LoginInfo> loginInfoDictionary;

    public Customizer customizerScript;

    public NetworkManager manager;

    public LoginInfoReader loginReader;

    private void Awake() 
    {
        loginInfoList = loginReader.ReadFile();
        
        passwordDictionary = new Dictionary<string, string>();
        loginInfoDictionary = new Dictionary<string, LoginInfo>();
        foreach(LoginInfo info in loginInfoList)
        {
            passwordDictionary.Add(info.email, info.password);
            loginInfoDictionary.Add(info.email, info);
        }
        
    }

    private void Start()
     {
        //print(loginInfoDictionary["Greg"]);
        
    }

    public bool Login(string email, string password)
    {
        if(passwordDictionary.ContainsKey(email))
        {
            //print("dictionary contains : "+ email);
            if(passwordDictionary[email] == password)
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

    public LoginInfo GetLoginInfo(string email)
    {
      if(loginInfoDictionary.ContainsKey(email)) return loginInfoDictionary[email];
      else return null;
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
