using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Block.Instruction;
using MG_BlocksEngine2.Block;

public class BE2_Ins_MoveForward : BE2_InstructionBase, I_BE2_Instruction
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
    float _value;

    //protected override void OnPlay()
    //{
    //    
    //}

    public new void Function()
    {
        _input0 = Section0Inputs[0];
        _value = _input0.FloatValue;
        TargetObject.Transform.position += TargetObject.Transform.forward * _value;
        ExecuteNextInstruction();
    }
}
