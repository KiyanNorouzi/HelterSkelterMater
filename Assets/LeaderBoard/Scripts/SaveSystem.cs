using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

[System.Serializable]
public class UserData
{
    public int      number = 0;
    public string   name    =   "";
    public int      score   =   0;
    public int      time    =   0;

    public UserData()
    {
        number = 0;
        name = "";
        score = 0;
        time = 0;
    }
}

public class SaveSystem : MonoBehaviour {

    
    private UserData user = new UserData();
    private string address;
 
	void Start () {
        address = Application.persistentDataPath + "\\local.txt";
	}

    public void saving()
    {
        if (user.score < 1)
            return;

        StreamWriter swBell = new StreamWriter(address);
        swBell.WriteLine(user.name + ":" + user.score + "%" + user.time + ";");
        swBell.Close();
    }

    public void SetName (Text txtField)
    {
        user.name = txtField.text;
    }

    void AddScore(int amount)
    {
        user.score += amount;
        SendMessage("ShowScore", user.score);
    }

    void AddTime()
    {
        user.time += 1;
    }
}
