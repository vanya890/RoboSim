using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Block.Instruction;
using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.Environment;
using MG_BlocksEngine2.Attribute;

// v2.9 - attribute [SerializeAsVariable] is now mandatory for the dynamic variable operation blocks to be correcly
// serialized/deserialized and created by the correspondent Variables Manager
[SerializeAsVariable(typeof(BE2_VariablesManager))]
public class BE2_Op_Variable : BE2_InstructionBase, I_BE2_Instruction
{
    protected override void OnStart()
    {
        _variablesManager = BE2_VariablesManager.instance;
    }

    BE2_VariablesManager _variablesManager;
    
    public new string Operation()
    {
        return _variablesManager.GetVariableStringValue(Section0Inputs[0].StringValue);
    }
}

