using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Block.Instruction;
using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.Environment;

public class BE2_Ins_SpacecraftShoot : BE2_InstructionBase, I_BE2_Instruction
{
    //protected override void OnAwake()
    //{
    //
    //}

    //protected override void OnStart()
    //{
    //    
    //}

    public new void Function()
    {
        if (TargetObject is BE2_TargetObjectSpacecraft3D)
        {
            (TargetObject as BE2_TargetObjectSpacecraft3D).Shoot();
        }
        ExecuteNextInstruction();
    }
}
