using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Block.Instruction;
using MG_BlocksEngine2.Block;

public class BE2_Cst_LookAtAndMove : BE2_InstructionBase, I_BE2_Instruction
{
    public new bool ExecuteInUpdate => true;

    bool _firstPlay = true;
    float _timer = 0;
    int _counter = 0;
    Vector3 _initialPosition;
    Quaternion _initialRotation;
    Vector3 _direction;

    public override void OnStackActive()
    {
        _firstPlay = true;
        _timer = 0;
        _counter = 0;
    }

    public new void Function()
    {
        if (_firstPlay)
        {
            _initialPosition = TargetObject.Transform.position;
            _initialRotation = TargetObject.Transform.rotation;
            _direction = GetDirection(Section0Inputs[0].StringValue);
            _firstPlay = false;
        }

        if (_counter < Mathf.Abs(Section0Inputs[1].FloatValue))
        {
            if (_timer <= 1)
            {
                _timer += Time.deltaTime / 0.2f;
                
                TargetObject.Transform.position = Vector3.Lerp(_initialPosition, _initialPosition +
                            (TargetObject.Transform.forward * (Section0Inputs[1].FloatValue / Mathf.Abs(Section0Inputs[1].FloatValue))), _timer);

                TargetObject.Transform.rotation = Quaternion.Lerp(_initialRotation, Quaternion.LookRotation(_direction), _timer);
            }
            else
            {
                _timer = 0;
                _counter++;
                _firstPlay = true;
            }
        }
        else
        {
            ExecuteNextInstruction();
            _counter = 0;
            _timer = 0;
            _firstPlay = true;
        }
    }

    Vector3 GetDirection(string option)
    {
        // returns the look direction based on the string value
        switch (option)
        {
            case "Up":
                return Vector3.forward;
            case "Down":
                return Vector3.back;
            case "Right":
                return Vector3.right;
            case "Left":
                return Vector3.left;
            default:
                return Vector3.zero;
        }
    }
}
