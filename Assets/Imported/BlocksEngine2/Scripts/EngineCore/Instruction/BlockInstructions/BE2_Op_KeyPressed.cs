using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Block.Instruction;
using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.Utils;

// v2.10 - Dropdown and InputField references in the default instructions replaced by BE2_Dropdown and BE2_InputField to enable the use of legacy or TMP components as Block inputs
public class BE2_Op_KeyPressed : BE2_InstructionBase, I_BE2_Instruction
{
    BE2_Dropdown _dropdown;

    //protected override void OnAwake()
    //{
    //
    //}

    protected override void OnStart()
    {
        _dropdown = BE2_Dropdown.GetBE2Component(Section0Inputs[0].Transform);

        PopulateDropdown();
        _dropdown.value = _dropdown.GetIndexOf("A");
        ParseKeyCode();
        _dropdown.onValueChanged.AddListener(delegate { ParseKeyCode(); });
    }

    void PopulateDropdown()
    {
        _dropdown.ClearOptions();
        string[] keys = System.Enum.GetNames(typeof(KeyCode));
        foreach (string key in keys)
        {
            _dropdown.AddOption(key);
        }
        _dropdown.RefreshShownValue();
    }

    KeyCode _key;
    void ParseKeyCode()
    {
        KeyCode key = KeyCode.A;
        try
        {
            key = (KeyCode)System.Enum.Parse(typeof(KeyCode), Section0Inputs[0].StringValue);
        }
        catch { }
        _key = key;
    }

    public new string Operation()
    {
        if (Input.GetKey(_key))
        {
            return "1";
        }
        else
        {
            return "0";
        }
    }
}
