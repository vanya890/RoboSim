using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Block.Instruction;
using MG_BlocksEngine2.Block;

public class BE2_Ins_SlideForward : BE2_InstructionBase, I_BE2_Instruction
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
    float _absValue;
    bool _firstPlay = true;
    public new bool ExecuteInUpdate => true;

    protected override void OnButtonStop()
    {
        _firstPlay = true;
        _timer = 0;
        _counter = 0;
    }

    public override void OnStackActive()
    {
        _firstPlay = true;
        _timer = 0;
        _counter = 0;
    }

    float _timer = 0;
    int _counter = 0;
    Vector3 _initialPosition;
    
    public new void Function()
    {
        if (_firstPlay)
        {
            _input0 = Section0Inputs[0];
            _value = _input0.FloatValue;
            _absValue = Mathf.Abs(_value);
            _initialPosition = TargetObject.Transform.position;
            _firstPlay = false;
        }

        if (_counter < _absValue)
        {
            // v2.8 - adjusted the SlideForward function so the TargetObject always end in the same position
            if (_timer < 1)
            {
                _timer += Time.deltaTime / 0.2f;

                if (_timer > 1)
                    _timer = 1;

                TargetObject.Transform.position = Vector3.Lerp(_initialPosition, _initialPosition +
                            (TargetObject.Transform.forward * (_value / _absValue)), _timer);
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
}
