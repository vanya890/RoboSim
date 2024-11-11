using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Block.Instruction;
using MG_BlocksEngine2.Block;

public class BE2_Ins_Repeat : BE2_InstructionBase, I_BE2_Instruction
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
    int _counter = 0;
    float _value;

    protected override void OnButtonStop()
    {
        _counter = 0;
    }

    public override void OnStackActive()
    {
        _counter = 0;
    }

    public new void Function()
    {
        _input0 = Section0Inputs[0];
        _value = _input0.FloatValue;

        if (_counter != _value)
        {
            _counter++;
            ExecuteSection(0);
        }
        else
        {
            _counter = 0;
            ExecuteNextInstruction();
        }
    }
}
