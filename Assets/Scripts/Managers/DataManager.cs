using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using EasyMobile;
 
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
    public GameManager gm;
    private SavedGame _savedgame;
    private void Awake()
    {
        gm = UnityEngine.Object.FindObjectOfType<GameManager>();
    }

    private void Start() 
    {
        OpenSavedGame();
    }

    public void OpenSavedGame()
    {
        GameServices.SavedGames.OpenWithAutomaticConflictResolution("data", (SavedGame savedgame, string error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    Debug.LogWarning("Saved game " + savedgame.Name + " opened successfully!");
                    _savedgame = savedgame;

                    // Load Data after opening
                    LoadData();
                }
                else
                {
                    Debug.LogWarning("Open saved game failed with error: " + error);
                }
            }
        );
    }
 
    public void SaveData()
    {
        Debug.LogWarning("SaveCloudData: Saving...");
        byte[] newByteData = InstanceToByte();

        if (_savedgame.IsOpen)
        {            
            GameServices.SavedGames.WriteSavedGameData(_savedgame, newByteData, (SavedGame sg, string error) => {
                if (string.IsNullOrEmpty(error)) Debug.Log("Writing Game Worked Successfully");
                else Debug.LogWarning("Writing Game Failed");
            });
        }
        else
        {
            GameServices.SavedGames.OpenWithAutomaticConflictResolution("data", (SavedGame saveData, string error) =>
            {
                // Save SaveGame after you open it
                _savedgame = saveData;

                if (string.IsNullOrEmpty(error))
                {
                    Debug.LogWarning("Saved game " + saveData.Name + " opened successfully!");
                    GameServices.SavedGames.WriteSavedGameData(_savedgame, newByteData, (SavedGame sg, string err) => {
                        if (string.IsNullOrEmpty(err)) Debug.Log("Writing Game Worked Successfully");
                        else Debug.LogWarning("Writing Game Failed");
                    });
                }
                else
                    Debug.LogWarning("Open saved game failed with error: " + error);
            });
        }
    }
 
    public void LoadData()
    {
        // Load Cloud Data
        Debug.LogWarning("LoadCloudData: Loading...");

        if (_savedgame.IsOpen)
        {
            GameServices.SavedGames.ReadSavedGameData(_savedgame, (SavedGame sg, byte[] loadedData, string error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    ByteToInstance(loadedData);
                    Debug.LogWarning("Writing Game Worked Successfully");
                } 
                else Debug.Log("Writing Game Failed");
            });
        }
        else
        {
            GameServices.SavedGames.OpenWithAutomaticConflictResolution("data", (SavedGame saveData, string error) =>
            {
                // Save SaveGame after you open it
                _savedgame = saveData;

                if (string.IsNullOrEmpty(error))
                {
                    GameServices.SavedGames.ReadSavedGameData(_savedgame, (SavedGame sg, byte[] loadedData, string err) =>
                    {
                        if (string.IsNullOrEmpty(err))
                        {
                            ByteToInstance(loadedData);
                            Debug.LogWarning("Writing Game Worked Successfully");
                        }
                        else Debug.LogWarning("Writing Game Failed");
                    });
                }
                else
                    Debug.LogWarning("Open saved game failed with error: " + error);
            });
        }
    }
 
    public void ByteToInstance(byte[] data)
    {
        Data loadedData = ByteToData(data);
        gm.timeHighScore = loadedData.timeHighScore;
        gm.endlessHighScore = loadedData.endlessHighScore;
        gm.isFirstTime = loadedData.isFirstTime;
        gm.ads = loadedData.ads;
    }
 
    public byte[] InstanceToByte()
    {
        Data newDataToSave = new Data();
        newDataToSave.timeHighScore = gm.timeHighScore;
        newDataToSave.endlessHighScore = gm.endlessHighScore;
        newDataToSave.isFirstTime = gm.isFirstTime;
        newDataToSave.ads = gm.ads;
 
        newDataToSave.hash = HashData(newDataToSave);
        return DataToByte(newDataToSave);
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
 
    public byte[] DataToByte(Data data)
    {
        Debug.LogWarning("Converting to Byte");
        return System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
    }
 
    public Data ByteToData(byte[] data)
    {
        Debug.LogWarning("Converting to JSON");
        return JsonUtility.FromJson<Data>(System.Text.Encoding.UTF8.GetString(data));
    }
}