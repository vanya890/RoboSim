using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Block.Instruction;
using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.Utils;
using MG_BlocksEngine2.Core;

// v2.11 - WhenJoystickKeyPressed refactored to be compatible with the new way the trigger blocks are executed
// v2.10 - Dropdown and InputField references in the default instructions replaced by BE2_Dropdown and BE2_InputField to enable the use of legacy or TMP components as Block inputs
public class BE2_Ins_WhenJoystickKeyPressed : BE2_InstructionBase, I_BE2_Instruction
{
    BE2_Dropdown _dropdown;
    BE2_VirtualJoystick _virtualJoystick;

    protected override void OnStart()
    {
        _dropdown = BE2_Dropdown.GetBE2Component(GetSectionInputs(0)[0].Transform);
        _virtualJoystick = BE2_VirtualJoystick.instance;
    }

    protected override void OnEnableInstruction()
    {
        BE2_ExecutionManager.Instance.AddToUpdate(OnUpdate);
    }
    protected override void OnDisableInstruction()
    {
        BE2_ExecutionManager.Instance.RemoveFromUpdate(OnUpdate);
    }

    protected override void OnAwake()
    {
        BlocksStack.OnStackLastBlockExecuted.AddListener(EndExecution);
    }

    void EndExecution()
    {
        if (!_virtualJoystick.keys[_dropdown.value].isPressed)
        {
            BlocksStack.IsActive = false;
        }
    }

    void OnUpdate()
    {
        if (!BlocksStack.IsActive && _virtualJoystick && _virtualJoystick.keys[_dropdown.value].isPressed)
        {
            BlocksStack.IsActive = true;
        }
    }

    public new void Function()
    {
        ExecuteSection(0);
    }
}
