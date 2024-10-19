using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public int ID;
    public bool Clicked = false;
    private LevelEditorManager editor;
    private int pastRotation;

    void Start()
    {
        editor = GameObject.FindGameObjectWithTag("LEM").GetComponent<LevelEditorManager>();
    }

    private void Update()
    {
        
    }

    public void ButtonClicked()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            Instantiate(editor.ItemIntro[ID], new Vector3((float)Math.Floor(raycastHit.point.x) + 0.5f, (float)Math.Floor(raycastHit.point.y) + 0.5f, (float)Math.Floor(raycastHit.point.z) + 0.5f), Quaternion.Euler(0, editor.rotation, 0));
        }
        Clicked = true;
        editor.CurrentButtonClicked = ID;
    }
}
