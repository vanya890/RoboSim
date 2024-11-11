using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Block.Instruction;
using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.Core;
using MG_BlocksEngine2.Environment;
using System.Globalization;
using MG_BlocksEngine2.Utils;

// v2.10 - Dropdown and InputField references in the default instructions replaced by BE2_Dropdown and BE2_InputField to enable the use of legacy or TMP components as Block inputs
public class BE2_Ins_AddVariable : BE2_InstructionBase, I_BE2_Instruction
{
    //protected override void OnAwake()
    //{
    //
    //}

    protected override void OnStart()
    {
        _variablesManager = BE2_VariablesManager.instance;
        _dropdown = BE2_Dropdown.GetBE2Component(GetSectionInputs(0)[0].Transform);
        _dropdown.onValueChanged.AddListener(delegate { _lastValue = _dropdown.GetSelectedOptionText(); });

        BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnAnyVariableAddedOrRemoved, PopulateDropdown);

        // v2.1 - bugfix: fixed variable blocks not updating dropdown when new variables were crated
        PopulateDropdown();
    }

    void OnDisable()
    {
        BE2_MainEventsManager.Instance.StopListening(BE2EventTypes.OnAnyVariableAddedOrRemoved, PopulateDropdown);
    }

    void PopulateDropdown()
    {
        _dropdown.ClearOptions();
        foreach (KeyValuePair<string, string> variable in _variablesManager.variablesList)
        {
            _dropdown.AddOption(variable.Key);
        }
        _dropdown.RefreshShownValue();
        _dropdown.value = _dropdown.GetIndexOf(_lastValue);
    }

    I_BE2_BlockSectionHeaderInput _input0;
    I_BE2_BlockSectionHeaderInput _input1;
    string _vs0;
    BE2_InputValues _v1;
    BE2_InputValues _varValues;

    string _lastValue;
    BE2_Dropdown _dropdown;
    BE2_VariablesManager _variablesManager;

    //protected override void OnPlay()
    //{
    //    
    //}

    public new void Function()
    {
        _input0 = Section0Inputs[0];
        _input1 = Section0Inputs[1];
        _vs0 = _input0.StringValue;
        _v1 = _input1.InputValues;
        _varValues = _variablesManager.GetVariableValues(_vs0);

        if (_varValues.isText || _v1.isText)
        {
            _variablesManager.AddOrUpdateVariable(_vs0, _varValues.stringValue + _v1.stringValue);
        }
        else
        {
            float result = _varValues.floatValue + _v1.floatValue;
            // v2.8 - bugfix: float values breaking for different locales
            _variablesManager.AddOrUpdateVariable(_vs0, result.ToString(CultureInfo.InvariantCulture));
        }

        ExecuteNextInstruction();
    }
}
