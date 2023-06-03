
using UnityEngine;
using System.Collections;
using System.IO;

public class LoadSystem : MonoBehaviour {

    public UserData[] data = new UserData[10];
    private string localList;
    private string address;
    private ScoreManager manager;
	// Use this for initialization
	void Start () {
        address = Application.persistentDataPath + "\\list.txt";
        localList = Application.persistentDataPath + "\\local.txt";
        manager = GetComponent<ScoreManager>();

        FileInfo check = new FileInfo(address);
        if (!check.Exists)
        {
            Stream temp = check.Create();
            temp.Close();
        }

        check = new FileInfo(localList);
        if (!check.Exists)
        {
            Stream temp = check.Create();
            temp.Close();
        }

        BasicLoading();
        if(LocalLoading())
        {
            Destructed();
        }
        NewChangeSave();

        StartCoroutine(SetBoard());

        
	}


    void NewChangeSave()
    {
        StreamWriter swBell = new StreamWriter(address);

        for (int i = 0; i < data.Length;i++)
        {
            if(data[i] != null)
                swBell.WriteLine(data[i].name + ":" + data[i].score + "%" + data[i].time + ";");
        }
            
        swBell.Close();
    }

    void Destructed()
    {
        FileInfo check = new FileInfo(localList);
        check.Delete();
    }

    IEnumerator SetBoard()
    {
        yield return new WaitForEndOfFrame();
        for(int i = 0;i < data.Length;i++)
        {
            if(data[i] != null && data[i].score != 0)
            {
                manager.SetScore(data[i].name, "time", data[i].time);
                manager.SetScore(data[i].name, "score", data[i].score);
            }
            
        }
    }

    bool LocalLoading()
    {
        StreamReader swBell = new StreamReader(localList);
        string tempStr = "";
        string tempStr_2 = "";
        UserData tempUser = new UserData();

        tempStr = swBell.ReadLine();

        if (tempStr == "" || tempStr == null)
        {
            swBell.Close();
            return false;
        }
            

        tempStr_2 = tempStr.Substring(0, tempStr.IndexOf(":"));
        tempUser.name = tempStr_2;
        tempStr_2 = tempStr.Substring(tempStr.IndexOf(":") + 1, tempStr.IndexOf("%") - tempStr.IndexOf(":") - 1);
        tempUser.score = int.Parse(tempStr_2);
        tempStr_2 = tempStr.Substring(tempStr.IndexOf("%") + 1, tempStr.IndexOf(";") - tempStr.IndexOf("%") - 1);
        tempUser.time = int.Parse(tempStr_2);

		for(int i = 0; i < data.Length; i++)
		{
			if(data[i].name == tempUser.name)
			{
				if(data[i].score <= tempUser.score)
				{
					data[i] = tempUser;
					i = data.Length + 1;
					swBell.Close();
					return true;
				}
				else
				{
					i = data.Length + 1;
					swBell.Close();
					return true;
				}
			}
		}



        for (int i = 0; i < data.Length; i++)
        {
            if(data[i].score == 0)
            {
                data[i] = tempUser;
                i = data.Length + 1;
                swBell.Close();
                return true;
            }
        }

        int tempInt = data[0].score;
        int tempInt_2 = 0;

        for(int i = 0;i < data.Length;i++)
        {
            if (data[i].score < tempUser.score && data[i].score < tempInt)
            {
                tempInt = data[i].score;
                tempInt_2 = i;
            }
        }

        data[tempInt_2] = tempUser;
        swBell.Close();
        return true;
    }

    void BasicLoading()
    {
        StreamReader swBell = new StreamReader(address);
        string tempStr = "";
        string tempStr_2 = "";

        for (int i = 0; i < data.Length; i++)
        {

            tempStr = swBell.ReadLine();

            if (tempStr != "" && tempStr != null)
            {
                tempStr_2 = tempStr.Substring(0, tempStr.IndexOf(":"));
                data[i].name = tempStr_2;
                tempStr_2 = tempStr.Substring(tempStr.IndexOf(":") + 1, tempStr.IndexOf("%") - tempStr.IndexOf(":") - 1);
                data[i].score = int.Parse(tempStr_2);
                tempStr_2 = tempStr.Substring(tempStr.IndexOf("%") + 1, tempStr.IndexOf(";") - tempStr.IndexOf("%") - 1);
                data[i].time = int.Parse(tempStr_2);
            }
            else
            {
                i = data.Length + 1;
            }

 
        }

        swBell.Close();

    }
}

