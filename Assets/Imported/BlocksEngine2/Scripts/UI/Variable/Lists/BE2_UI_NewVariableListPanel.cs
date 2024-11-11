using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.Utils;
using MG_BlocksEngine2.Environment;

namespace MG_BlocksEngine2.UI
{
    // v2.9 - Script that manages the "new list" panel 
    public class BE2_UI_NewVariableListPanel : MonoBehaviour
    {
        Button _buttonCreate;
	// v2.10 - changed from Inputfield to BE2_InputField
        BE2_InputField _inputListName;

        public Transform variablePanelTemplate;

        void Awake()
        {
            _buttonCreate = transform.GetChild(2).GetComponent<Button>();
            _inputListName = BE2_InputField.GetBE2Component(transform.GetChild(1));
        }

        void Start()
        {
            _buttonCreate.onClick.AddListener(OnButtonCreateList);
        }

        //void Update()
        //{
        //
        //}

        void OnButtonCreateList()
        {
            string listName = _inputListName.text;
            if (listName != "")
            {
                CreateList(listName);
            }
        }

        public void CreateList(string listName)
        {
            if (!BE2_VariablesListManager.instance.ContainsList(listName))
            {
                Transform newListPanel = Instantiate(variablePanelTemplate, Vector3.zero, Quaternion.identity, transform.parent);
                newListPanel.SetSiblingIndex(transform.GetSiblingIndex() + 1);

                newListPanel.localPosition = new Vector3(newListPanel.localPosition.x, newListPanel.localPosition.y, 0);
                newListPanel.localEulerAngles = Vector3.zero;

                I_BE2_Block newBlock = newListPanel.GetChild(0).GetComponent<I_BE2_Block>();

                //                                                   | block                                                   | section   | header    | text      |
                BE2_Text newListName = BE2_Text.GetBE2Text(newListPanel.GetComponentInChildren<BE2_UI_SelectionBlock>().transform.GetChild(0).GetChild(0).GetChild(0));
                newListName.text = listName;

                BE2_UI_VariableListViewer listViewer = newListPanel.GetComponent<BE2_UI_VariableListViewer>();
                listViewer.RefreshViewer();

                BE2_UI_BlocksSelectionViewer.Instance.ForceRebuildLayout();
            }
        }
    }
}
