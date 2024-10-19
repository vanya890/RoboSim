using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Block.Instruction;
using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.Core;

public class BE2_Ins_Return : BE2_InstructionBase, I_BE2_Instruction
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
        // v2.8 - bugfix: return block not stopping execution
        BE2_ExecutionManager.Instance.Stop();
    }
}
