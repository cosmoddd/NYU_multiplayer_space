using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;

public class LoginInfoReader : MonoBehaviour
{
    public LoginInfo[] loginInfoFile;

    void Awake() {
        //WriteTestFile();
        //loginInfoFile = ReadFile();
        
    }
    
    public LoginInfo[] ReadFile()
    {
        
        string filePath = "EmailList.json";
        filePath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "LoginInfo" + Path.DirectorySeparatorChar + filePath;
        //save all of file into a string
        string alltxt;
        using (StreamReader fileReader = new StreamReader(filePath))
        {
            alltxt = fileReader.ReadToEnd();
            fileReader.Close();
        }

        Debug.Log("read json");

        List<LoginInfo> entries = JsonConvert.DeserializeObject<List<LoginInfo>>(alltxt);
        return entries.ToArray();

    }

    void WriteTestFile()
    {
        string[] test = {"test", "test"};
        LoginInfo testOne = new LoginInfo("username","password", test);
        LoginInfo testTwo = new LoginInfo("username","password", test);
        LoginInfo testThree = new LoginInfo("username","password", test);
        LoginInfo[] testLoginInfo = { testOne, testTwo,testThree};
        string _testEntry = JsonConvert.SerializeObject(testLoginInfo, Formatting.Indented);
        string filePath = "EmailList.json";
        Debug.Log("write json");
        System.IO.File.WriteAllText( Application.streamingAssetsPath + Path.DirectorySeparatorChar + "LoginInfo" + Path.DirectorySeparatorChar + filePath, _testEntry);
    }
}


public class PasswordRandomizer
{
    string[] poolOne;
    string[] poolTwo;
    string[] poolThree;

    PasswordRandomizer(string[] p1, string[] p2, string[] p3 )
    {
        poolOne = p1;
        poolTwo = p2;
        poolThree = p3;
    }

//count is how many passwords to generate
    public string[] GeneratePasswords(int count)
    {
        List<string> passwords = new List<string>();
        int x = 0;
        int y = 0;
        int z = 0;

        for(int i = 0; i < count; i++)
        {
            string tmp = poolOne[x++] + poolTwo[y++] + poolThree[z++];

            passwords.Add(tmp);
        }

        return passwords.ToArray();


    }
}
