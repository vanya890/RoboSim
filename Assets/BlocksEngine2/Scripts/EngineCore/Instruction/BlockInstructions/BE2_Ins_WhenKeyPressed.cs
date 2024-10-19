using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Block.Instruction;
using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.Utils;
using MG_BlocksEngine2.Core;

// v2.11 - WhenKeyPressed refactored to be compatible with the new way the trigger blocks are executed
// v2.10 - Dropdown and InputField references in the default instructions replaced by BE2_Dropdown and BE2_InputField to enable the use of legacy or TMP components as Block inputs
public class BE2_Ins_WhenKeyPressed : BE2_InstructionBase, I_BE2_Instruction
{
    BE2_Dropdown _dropdown;

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

    protected override void OnEnableInstruction()
    {
        BE2_ExecutionManager.Instance.AddToUpdate(OnUpdate);
    }
    protected override void OnDisableInstruction()
    {
        BE2_ExecutionManager.Instance.RemoveFromUpdate(OnUpdate);
    }

    void OnUpdate()
    {
        if (Input.GetKeyDown(_key))
        {
            BlocksStack.IsActive = true;
        }

        if (Input.GetKeyUp(_key))
        {
            BlocksStack.IsActive = false;
        }
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

    public new void Function()
    {
        ExecuteSection(0);
    }
}
