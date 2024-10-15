using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;

[System.Serializable]
class GameData
{
    public int type;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public bool active;
    public GameData(int typ,Vector3 pos,Quaternion rot,Vector3 sca, bool act)
    {
        type = typ;
        position = pos;
        rotation = rot;
        scale = sca;
        active = act;
    }
    public int Type()
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
public class Save : MonoBehaviour
{
    PrimitiveType[] index = new PrimitiveType[4] { PrimitiveType.Cube, PrimitiveType.Sphere, PrimitiveType.Plane, PrimitiveType.Capsule};
    
    public void SaveAll()
    {
        List<GameData> alldata = new List<GameData>();
        foreach (GameObject t in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            //0-Cube 1-Sphere 2-Plane 3-Capsule
            if (t.GetComponent<BoxCollider>() != null)
            {
                alldata.Add(new GameData(0, t.transform.position, t.transform.rotation, t.transform.localScale, t.activeSelf));
            }
            else if (t.GetComponent<SphereCollider>() != null)
            {
                alldata.Add(new GameData(1, t.transform.position, t.transform.rotation, t.transform.localScale, t.activeSelf));
            }
            else if(t.GetComponent<MeshCollider>() != null)
            {
                alldata.Add(new GameData(2, t.transform.position, t.transform.rotation, t.transform.localScale, t.activeSelf));
            }
            else if (t.GetComponent<CapsuleCollider>() != null)
            {
                alldata.Add(new GameData(3, t.transform.position, t.transform.rotation, t.transform.localScale, t.activeSelf));
            }
        }
        string json = "";
        json = JsonHelper.ToJson<GameData>(alldata, true);
        File.WriteAllText(Application.persistentDataPath + "/Saves.json", json);
        Debug.Log(json);
    }

    public void Load(string path)
    {
        foreach (GameData t in JsonHelper.FromJson<GameData>(File.ReadAllText(path)))
        {
            
            GameObject result = GameObject.Instantiate(GameObject.CreatePrimitive(index[t.Type()]),t.position,t.Rotation());
            result.transform.localScale = t.Scale();
            result.SetActive(t.Active());
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SaveAll();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load(Application.persistentDataPath + "/Saves.json");
        }
    }
}
