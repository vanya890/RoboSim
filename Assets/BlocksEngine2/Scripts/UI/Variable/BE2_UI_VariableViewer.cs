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
    public class BE2_UI_VariableViewer : MonoBehaviour
    {
        string _variable;
	// v2.10 - changed from Inputfield to BE2_InputField
        BE2_InputField _inputField;
        Button _removeButton;

        void Awake()
        {
            _variable = GetVariableName();
            // v2.8 - adjusted variable viwer with "remove variable" button 
            foreach (Transform child in transform)
            {
                _inputField = BE2_InputField.GetBE2Component(child);

                if (!_removeButton)
                    _removeButton = child.GetComponent<Button>();
            }

            _removeButton.onClick.AddListener(RemoveVariable);
        }

        void OnEnable()
        {
            BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnAnyVariableValueChanged, UpdateViewerValue);
            _inputField.onEndEdit.AddListener(delegate { UpdateVariableValue(); });
        }

        void OnDisable()
        {
            _inputField.onEndEdit.RemoveAllListeners();
            BE2_MainEventsManager.Instance.StopListening(BE2EventTypes.OnAnyVariableValueChanged, UpdateViewerValue);
        }

        void Start()
        {
            UpdateViewerValue();
            UpdateVariableValue();
        }

        //void Update()
        //{
        //
        //}

        public void RefreshViewer()
        {
            _variable = GetVariableName();
            UpdateViewerValue();
            UpdateVariableValue();
        }

        void UpdateViewerValue()
        {
            _inputField.text = BE2_VariablesManager.instance.GetVariableStringValue(_variable);
        }

        void UpdateVariableValue()
        {
            BE2_VariablesManager.instance.AddOrUpdateVariable(_variable, _inputField.text);
        }

        // v2.1 - added method in the variable viwer UI to get variable name
        string GetVariableName()
        {
            string varName = "";

            // v2.8 - adjusted variable viwer with "remove variable" button 
            Transform varBlockTransform = transform.GetComponentInChildren<BE2_UI_SelectionBlock>().transform;

            //                                  | block     | section   | header    | text      |
            varName = BE2_Text.GetBE2Text(varBlockTransform.GetChild(0).GetChild(0).GetChild(0)).text;

            return varName;
        }

        // v2.8 - added "remove variable" button to the variable viewer 
        public void RemoveVariable()
        {
            BE2_VariablesManager.instance.RemoveVariable(_variable);
            // v2.9 - bugfix: glitch on resizing the Blocks Selection Viewer 
            gameObject.SetActive(false);
            BE2_UI_BlocksSelectionViewer.Instance.ForceRebuildLayout();
            Destroy(gameObject);
        }
    }
}
