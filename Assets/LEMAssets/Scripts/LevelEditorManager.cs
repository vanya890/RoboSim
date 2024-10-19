using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class LevelEditorManager : MonoBehaviour
{
    public ItemController[] ItemButtons;
    public GameObject[] ItemPrefabs;
    public GameObject[] ItemIntro;
    public int CurrentButtonClicked;
    public int rotation = 0;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            rotation += 90;

            if (rotation >= 360)
            {
                rotation = 0;
            }
            Debug.Log(rotation.ToString());
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            if (Input.GetMouseButtonDown(0) && ItemButtons[CurrentButtonClicked].Clicked)
            {
                ItemButtons[CurrentButtonClicked].Clicked = false;
                Instantiate(ItemPrefabs[CurrentButtonClicked], new Vector3((float)Math.Floor(raycastHit.point.x) + 0.5f, (float)Math.Floor(raycastHit.point.y) + 0.5f, (float)Math.Floor(raycastHit.point.z) + 0.5f), Quaternion.Euler(0, rotation, 0));
                Destroy(GameObject.FindGameObjectWithTag("Intro"));
            }
        }
    }
}
