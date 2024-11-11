using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Block.Instruction;
using MG_BlocksEngine2.Block;

// v2.11 - WhenPlayClicked refactored to be compatible with the new way the trigger blocks are executed 
public class BE2_Ins_WhenPlayClicked : BE2_InstructionBase, I_BE2_Instruction
{
    protected override void OnButtonPlay()
    {
        BlocksStack.IsActive = true;
    }

    protected override void OnAwake()
    {
        BlocksStack.OnStackLastBlockExecuted.AddListener(EndExecution);
    }

    void EndExecution()
    {
        BlocksStack.IsActive = false;
    }

    public new void Function()
    {
        ExecuteSection(0);
    }
}
