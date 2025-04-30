using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using TMPro;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;
using static UnityEngine.UI.Image;

[System.Serializable]
class GameData
{
    public string type;
    public string tag;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public bool active;
    public GameData(string tagt, string typ,Vector3 pos,Quaternion rot,Vector3 sca, bool act)
    {
        tag = tagt; 
        type = typ;
        position = pos;
        rotation = rot;
        scale = sca;
        active = act;
    }
    public string Type()
    {
        return type;
    }
    public string Tag()
    {
        return tag;
    }
    public Vector3 Position()
    {
        return position;
    }
    public Quaternion Rotation()
    {
        return rotation;
    }
    public bool Active()
    {
        return active;
    }
    public Vector3 Scale()
    {
        return scale;
    }
}
public enum LevelObject
{
    polygon,
    wall,
    @static,
    dynamic,
    interactive,
    hazard,
}
public static class JsonHelper
{
    public static List<T> FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(List<T> array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(List<T> array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
    }
}
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class Save : MonoBehaviour
{

    string[] tags = new string[5] { "Dynamic", "Static", "Interactive", "Player", "Road" };
    List<List<List<LevelObject>>> levelObjects;
    private void LoadPrefab(LevelObject prefabName, Vector3 position)
    {
        var asset = Resources.Load<GameObject>($"{tag}\\{prefabName}");
#if UNITY_EDITOR
        GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(asset);
#else
        GameObject prefab = (GameObject)GameObject.Instantiate(asset);
#endif
        prefab.transform.position = new Vector3(position.x, position.y, position.z);
    } 
    private void LoadLevel()
    {
        string path = Path.GetFullPath($"{Application.persistentDataPath}\\Saves.json");
        StreamReader reader = new StreamReader(path);
        string levelJson = reader.ReadToEnd();
        var data = JsonHelper.FromJson<List<List<LevelObject>>>(File.ReadAllText(path));
        for (int i = 0; i < data.Count; i++)
        {
            for (int j = 0; j < data[i].Count; j++)
            {
                for(int k = 0; k < data[i][j].Count; k++)
                {
                    try
                    {
                        LoadPrefab(data[i][j][k], new Vector3(i,j,k));
                    }
                    catch { Debug.Log(data[i][j][k]); }
                    
                }
            }
        }
    }
    public void SaveAll(int Level2d, int Level3d, int Level1d)
    {
        levelObjects = new(Level1d);
        for (int i= 0;i<Level2d; i++)
        {
            levelObjects[i] = new(Level2d);
            for(int j= 0;j<Level3d;j++)
            {
                levelObjects[i][j] = new(j);
            }
        }
        foreach (string tag in tags)
        {
            foreach (GameObject thing in GameObject.FindGameObjectsWithTag(tag))
            {
                levelObjects[(int)thing.transform.position.x][(int)thing.transform.position.y][(int)thing.transform.position.z] = LevelObject.interactive;
            }
        }
        string json = "";

        json = JsonHelper.ToJson<List<List<LevelObject>>>(levelObjects, true);
        File.WriteAllText(Application.persistentDataPath + "\\Saves.json", json);
        Debug.Log(json);
    }
    void OnGUI()
    {                                        
        if (GUI.Button(new Rect(10, 10, 150, 100), "Save"))                                                                                                                                                 

        {
            SaveAll(40,40,3);
        }
        if (GUI.Button(new Rect(10, 10 + 100, 150, 100), "Load"))
        {       
            LoadLevel();
        }
    }
}
//Проверка!!!