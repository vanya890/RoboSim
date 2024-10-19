using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Block.Instruction;
using MG_BlocksEngine2.Block;
using System.Globalization;

public class BE2_Op_Multiply : BE2_InstructionBase, I_BE2_Instruction
{
    //protected override void OnAwake()
    //{
    //
    //}

    //protected override void OnStart()
    //{
    //    
    //}

    I_BE2_BlockSectionHeaderInput _input0;
    I_BE2_BlockSectionHeaderInput _input1;
    BE2_InputValues _v0;
    BE2_InputValues _v1;

    public new string Operation()
    {
        _input0 = Section0Inputs[0];
        _input1 = Section0Inputs[1];
        _v0 = _input0.InputValues;
        _v1 = _input1.InputValues;

        if (!_v0.isText && _v1.isText)
        {
            string result = "";
            for (int i = 0; i < (int)_v0.floatValue; i++)
            {
                result += _v1.stringValue;
            }
            return result;
        }
        if (_v0.isText && !_v1.isText)
        {
            string result = "";
            for (int i = 0; i < (int)_v1.floatValue; i++)
            {
                result += _v0.stringValue;
            }
            return result;
        }
        else
        {
            float result = _v0.floatValue * _v1.floatValue;
            return result.ToString(CultureInfo.InvariantCulture);
        }
    }
}
