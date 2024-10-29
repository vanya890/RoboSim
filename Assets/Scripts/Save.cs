using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public bool active;
    public GameData(string typ,Vector3 pos,Quaternion rot,Vector3 sca, bool act)
    {
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
    private void LoadPrefab(string prefabName, Vector3 position, Quaternion rotation, Vector3 scale, bool active)
    {
        var asset = Resources.Load<GameObject>(prefabName);
        GameObject prefab = GameObject.Instantiate(asset);
        prefab.transform.position = new Vector3(position.x, position.y, position.z);
        prefab.transform.rotation = rotation;
        prefab.transform.localScale = scale;
        prefab.SetActive(active);
    }
    private void LoadLevel()
    {
        string path = Path.GetFullPath($"{Application.persistentDataPath}\\Saves.json");
        StreamReader reader = new StreamReader(path);
        string levelJson = reader.ReadToEnd();
        List<GameData> data = JsonHelper.FromJson<GameData>(File.ReadAllText(path));
        foreach (GameData b in data)
        {
            try
            {
                LoadPrefab(b.Type(), b.Position(), b.Rotation(), b.Scale(), b.Active());
            }
            catch { Debug.Log(b.Type()); }
        }
    }
    public void SaveAll()
    {
        List<GameData> alldata = new List<GameData>();
        foreach (GameObject t in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            alldata.Add(new GameData(t.name, t.transform.position, t.transform.rotation, t.transform.localScale, t.activeSelf));
        }
        string json = "";
        json = JsonHelper.ToJson<GameData>(alldata, true);
        File.WriteAllText(Application.persistentDataPath + "\\Saves.json", json);
        Debug.Log(json);
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "Save"))
        {
            SaveAll();
        }
        if (GUI.Button(new Rect(10, 10 + 100, 150, 100), "Load"))
        {
            LoadLevel();
        }
    }
}
