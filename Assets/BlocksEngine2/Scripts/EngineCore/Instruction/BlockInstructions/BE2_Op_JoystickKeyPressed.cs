using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Block.Instruction;
using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.Utils;

// v2.10 - Dropdown and InputField references in the default instructions replaced by BE2_Dropdown and BE2_InputField to enable the use of legacy or TMP components as Block inputs
public class BE2_Op_JoystickKeyPressed : BE2_InstructionBase, I_BE2_Instruction
{
    BE2_Dropdown _dropdown;
    BE2_VirtualJoystick _virtualJoystick;

    //protected override void OnAwake()
    //{
    //
    //}

    protected override void OnStart()
    {
        _dropdown = BE2_Dropdown.GetBE2Component(Section0Inputs[0].Transform);
        _virtualJoystick = BE2_VirtualJoystick.instance;
    }

    public new string Operation()
    {
        if (_virtualJoystick.keys[_dropdown.value].isPressed)
        {
            return "1";
        }
        else
        {
            return "0";
        }
    }
}
