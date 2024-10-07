using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Block.Instruction;
using MG_BlocksEngine2.Block;

public class BE2_Ins_ChangeColor : BE2_InstructionBase, I_BE2_Instruction
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
    string _value;

    //protected override void OnPlay()
    //{
    //    
    //}

    public new void Function()
    {
        _input0 = Section0Inputs[0];
        _value = _input0.StringValue;

        Color newColor = Color.white;

        switch (_value)
        {
            case "Random":
                newColor = new Color(Random.Range(0f, 1f),
                                    Random.Range(0f, 1f),
                                    Random.Range(0f, 1f),
                                    255);
                break;
            case "Red":
                ColorUtility.TryParseHtmlString("#FF0000", out newColor);
                break;
            case "Orange":
                ColorUtility.TryParseHtmlString("#FF7F00", out newColor);
                break;
            case "Yellow":
                ColorUtility.TryParseHtmlString("#FFFF00", out newColor);
                break;
            case "Green":
                ColorUtility.TryParseHtmlString("#00FF00", out newColor);
                break;
            case "Blue":
                ColorUtility.TryParseHtmlString("#0000FF", out newColor);
                break;
            case "Indigo":
                ColorUtility.TryParseHtmlString("#2E2B5F", out newColor);
                break;
            case "Violet":
                ColorUtility.TryParseHtmlString("#8B00FF", out newColor);
                break;
            default:
                break;
        }

        TargetObject.Transform.GetComponent<Renderer>().materials[0].SetColor("_Color", newColor);
        ExecuteNextInstruction();
    }
}
