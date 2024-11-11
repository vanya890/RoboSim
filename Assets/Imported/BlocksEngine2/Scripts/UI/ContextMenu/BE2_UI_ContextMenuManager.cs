using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Utils;

namespace MG_BlocksEngine2.UI
{
    public class BE2_UI_ContextMenuManager : MonoBehaviour
    {
        I_BE2_UI_ContextMenu[] _contextMenuArray;
        I_BE2_UI_ContextMenu currentContextMenu;

        // v2.6.2 - bugfix: fixed changes on BE2 Inspector paths not perssiting 
        // UI ContextMenuManager instance changed to property to avoid null exception
        //public static BE2_UI_ContextMenuManager instance;
        static BE2_UI_ContextMenuManager _instance;
        public static BE2_UI_ContextMenuManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<BE2_UI_ContextMenuManager>();
                }
                return _instance;
            }
            set => _instance = value;
        }
        public BE2_UI_PanelCancel panelCancel;
        public bool isActive = false;

        void Start()
        {
            _contextMenuArray = new I_BE2_UI_ContextMenu[0];
            foreach (Transform child in transform)
            {
                I_BE2_UI_ContextMenu context = child.GetComponent<I_BE2_UI_ContextMenu>();
                if (context != null)
                    BE2_ArrayUtils.Add(ref _contextMenuArray, context);
            }

            CloseContextMenu();
        }

        //void Update()
        //{
        //
        //}

        public void OpenContextMenu<T>(int menuIndex, T target, params string[] options)
        {
            if (!isActive)
            {
                currentContextMenu = _contextMenuArray[menuIndex];
                currentContextMenu.Open(target);
                isActive = true;
                panelCancel.transform.gameObject.SetActive(true);
            }
        }

        public void CloseContextMenu()
        {
            if (isActive)
            {
                if (currentContextMenu != null)
                {
                    currentContextMenu.Close();
                    currentContextMenu = null;
                }
                isActive = false;
                panelCancel.transform.gameObject.SetActive(false);
            }
        }
    }
}