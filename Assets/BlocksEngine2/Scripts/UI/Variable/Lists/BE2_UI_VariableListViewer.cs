using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Utils;
using MG_BlocksEngine2.Core;
using MG_BlocksEngine2.Environment;

namespace MG_BlocksEngine2.UI
{
    // v2.9 - Script that manages the viwer where the list items are shown and can be manipulated by the user 
    public class BE2_UI_VariableListViewer : MonoBehaviour
    {
        public string listName;

        Transform _panelListOp;
        Transform _panelList;

        Button _removeListButton;
        Toggle _showListToggle;

        Button _createItemButton;

        public Transform panelListItem;

        void Awake()
        {
            _panelListOp = transform.GetChild(0);
            _panelList = transform.GetChild(1);

            _removeListButton = _panelListOp.GetChild(0).GetComponent<Button>();
            _showListToggle = _panelListOp.GetChild(2).GetComponent<Toggle>();

            _createItemButton = _panelList.GetChild(_panelList.childCount - 1).GetComponent<Button>();

            listName = GetVariableName();
        }

        void OnEnable()
        {
            _removeListButton.onClick.AddListener(RemoveList);
            _createItemButton.onClick.AddListener(delegate { AddListItem("", true); });
            _showListToggle.onValueChanged.AddListener(delegate
            {
                _panelList.gameObject.SetActive(_showListToggle.isOn);
                BE2_UI_BlocksSelectionViewer.Instance.ForceRebuildLayout();
            });

            BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnAnyVariableValueChanged, UpdateViewerValues);
            UpdateViewerValues();
        }

        void OnDisable()
        {
            _removeListButton.onClick.RemoveAllListeners();
            _createItemButton.onClick.RemoveAllListeners();
            _showListToggle.onValueChanged.RemoveAllListeners();

            BE2_MainEventsManager.Instance.StopListening(BE2EventTypes.OnAnyVariableValueChanged, UpdateViewerValues);
        }

        void Start()
        {
            UpdateViewerValues();
            UpdateListValues();
        }

        //void Update()
        //{
        //
        //}

        public void RefreshViewer()
        {
            listName = GetVariableName();
            UpdateViewerValues();
            UpdateListValues();
        }

        void UpdateViewerValues()
        {
            StartCoroutine(C_UpdateViewerValues());
        }

        IEnumerator C_UpdateViewerValues()
        {
            yield return new WaitForEndOfFrame();
            List<string> list = BE2_VariablesListManager.instance.GetListStringValues(listName);
            
            for (int i = 0; i < 10000; i++)
            {
                BE2_UI_ListItem item = _panelList.GetChild(i).GetComponent<BE2_UI_ListItem>();
                if (list.Count > i && item)
                {
                    item.inputField.text = list[i];
                }
                else if (list.Count > i && !item)
                {
                    AddListItem(list[i]);
                }
                else if (list.Count <= i && item)
                {
                    item.RemoveItem();
                }
                else
                {
                    break;
                }
            }
        }

        public void UpdateListValues()
        {
            List<string> list = new List<string>();

            foreach (Transform child in _panelList)
            {
                BE2_UI_ListItem item = child.GetComponent<BE2_UI_ListItem>();
                if (item)
                {
                    list.Add(item.inputField.text);
                }
            }

            BE2_VariablesListManager.instance.AddOrUpdateList(listName, list);
        }

        string GetVariableName()
        {
            string varName = "";

            Transform varBlockTransform = transform.GetComponentInChildren<BE2_UI_SelectionBlock>().transform;

            //                                  | block     | section   | header    | text      |
            varName = BE2_Text.GetBE2Text(varBlockTransform.GetChild(0).GetChild(0).GetChild(0)).text;

            return varName;
        }

        public void RemoveList()
        {
            BE2_VariablesListManager.instance.RemoveList(listName);
            gameObject.SetActive(false);
            BE2_UI_BlocksSelectionViewer.Instance.ForceRebuildLayout();
            Destroy(gameObject);
        }

        public void AddListItem(string value, bool select = false)
        {
            Transform listItem = Instantiate(panelListItem, _panelList);
            listItem.localScale = Vector3.one;
            listItem.SetSiblingIndex(_panelList.childCount - 2);

            BE2_UI_ListItem item = listItem.GetComponent<BE2_UI_ListItem>();
            item.inputField.text = value;
            if (select)
                item.inputField.Select();

            BE2_UI_BlocksSelectionViewer.Instance.ForceRebuildLayout();
        }
    }
}