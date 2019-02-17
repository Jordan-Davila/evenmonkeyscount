using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class Data
{
    public int timeHighScore;
    public int endlessHighScore;
    public bool ads;
    public bool isFirstTime;
    public string hash;
}

public class DataManager : MonoBehaviour
{
    [HideInInspector]
    public GameManager gm;
    public UIManager ui;
    private void Awake()
    {
        gm = UnityEngine.Object.FindObjectOfType<GameManager>();
        ui = UnityEngine.Object.FindObjectOfType<UIManager>();
        Init();
    }

    public void Init()
    {
        if (PlayerPrefs.HasKey("savedata"))
            LoadData();
        else
            SaveData();
    }

    public void SaveData()
    {
        Debug.LogWarning("SaveCloudData: Saving...");
        PlayerPrefs.SetString("savedata", ConvertToBase64(InstanceToJson()));
    }

    public void LoadData()
    {
        string dataAsJson = ConvertToJSON(PlayerPrefs.GetString("savedata"));
        Debug.Log("LoadCloudData: Loading...");

        Debug.LogWarning("GetCloud Data as JSON: " + dataAsJson);
        Data loadedData = JsonUtility.FromJson<Data>(dataAsJson);

        if (loadedData.hash == HashData(loadedData))
        {
            // Load Cloud Data to this instance
            LoadToInstance(loadedData);
        }
        else
        {
            Debug.LogError("Hacked Data File! Starting new data file.");

            // Save A New Copy
            SaveData();
        }
    }

    public void LoadToInstance(Data loadedData)
    {
        gm.timeHighScore = loadedData.timeHighScore;
        gm.endlessHighScore = loadedData.endlessHighScore;
        gm.isFirstTime = loadedData.isFirstTime;
        gm.ads = loadedData.ads;

        // Update GUIS
        ui.UpdateMenuUI();
    }

    public string InstanceToJson()
    {
        Data newDataToSave = new Data();
        newDataToSave.timeHighScore = gm.timeHighScore;
        newDataToSave.endlessHighScore = gm.endlessHighScore;
        newDataToSave.isFirstTime = gm.isFirstTime;
        newDataToSave.ads = gm.ads;

        newDataToSave.hash = HashData(newDataToSave);
        return JsonUtility.ToJson(newDataToSave, true);
    }

    public string HashData(Data data)
    {
        // Important! Don't hash our own hash value
        data.hash = String.Empty;

        // Data to JSON
        string stringData = JsonUtility.ToJson(data, true);

        //Setup SHA
        SHA256Managed crypt = new SHA256Managed();
        string hash = String.Empty; // Same as null or ""

        //Compute Hash
        byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(stringData), 0, Encoding.UTF8.GetByteCount(stringData));

        //Convert to Hex
        foreach (byte bit in crypto)
        {
            hash += bit.ToString("x2");
        }

        return hash.Trim();
    }

    public string ConvertToBase64(string jsonString)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));
    }

    public string ConvertToJSON(string base64String)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(base64String));
    }

    public void ResetData()
    {
        Debug.Log("Resetting Data");
        // Used when user has signed-out
        Data resetData = new Data();
        resetData.timeHighScore = 0;
        resetData.endlessHighScore = 0;
        resetData.isFirstTime = true;
        resetData.ads = true;
        LoadToInstance(resetData);
    }
}