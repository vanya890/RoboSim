using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.Utils;
using MG_BlocksEngine2.Environment;

namespace MG_BlocksEngine2.UI
{
    public class BE2_UI_NewVariablePanel : MonoBehaviour
    {
        Button _buttonCreate;
	// v2.10 - changed from Inputfield to BE2_InputField
        BE2_InputField _inputVarName;

        public Transform variablePanelTemplate;

        void Awake()
        {
            _buttonCreate = transform.GetChild(2).GetComponent<Button>();
            _inputVarName = BE2_InputField.GetBE2Component(transform.GetChild(1));
        }

        void Start()
        {
            _buttonCreate.onClick.AddListener(OnButtonCreateVariable);
        }

        //void Update()
        //{
        //
        //}

        void OnButtonCreateVariable()
        {
            string varName = _inputVarName.text;
            // v2.8 - added condition to verify if variable exists before creating new variables 
            if (varName != "")
            {
                CreateVariable(varName);
            }
        }

        public void CreateVariable(string varName)
        {
            if (!BE2_VariablesManager.instance.ContainsVariable(varName))
            {
                Transform newVarPanel = Instantiate(variablePanelTemplate, Vector3.zero, Quaternion.identity, transform.parent);
                newVarPanel.SetSiblingIndex(transform.GetSiblingIndex() + 1);

                // v2.6 - adjustments on position and angle of blocks for supporting all canvas render modes
                newVarPanel.localPosition = new Vector3(newVarPanel.localPosition.x, newVarPanel.localPosition.y, 0);
                newVarPanel.localEulerAngles = Vector3.zero;

                I_BE2_Block newBlock = newVarPanel.GetChild(0).GetComponent<I_BE2_Block>();

                // v2.8 - adjusted variable viwer with "remove variable" button 
                // v2.1 - using BE2_Text to enable usage of Text or TMP components
                //                                                   | block                                                   | section   | header    | text      |
                BE2_Text newVarName = BE2_Text.GetBE2Text(newVarPanel.GetComponentInChildren<BE2_UI_SelectionBlock>().transform.GetChild(0).GetChild(0).GetChild(0));
                newVarName.text = varName;

                BE2_VariablesManager.instance.AddOrUpdateVariable(varName, "0");

                newVarPanel.GetComponent<BE2_UI_VariableViewer>().RefreshViewer();

                // v2.9 - bugfix: glitch on resizing the Blocks Selection Viewer
                BE2_UI_BlocksSelectionViewer.Instance.ForceRebuildLayout();
            }
        }
    }
}
