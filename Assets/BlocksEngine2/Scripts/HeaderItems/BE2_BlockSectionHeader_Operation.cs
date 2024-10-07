using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

using MG_BlocksEngine2.Block.Instruction;

namespace MG_BlocksEngine2.Block
{
    public class BE2_BlockSectionHeader_Operation : MonoBehaviour, I_BE2_BlockSectionHeaderItem, I_BE2_BlockSectionHeaderInput
    {
        I_BE2_Block _block;
        I_BE2_Instruction _instruction;
        RectTransform _rectTransform;

        public Transform Transform => transform;
        public Vector2 Size => _rectTransform ? _rectTransform.sizeDelta : GetComponent<RectTransform>().sizeDelta;
        public I_BE2_Spot Spot => null;
        public float FloatValue => GetFloatValue();
        public string StringValue => GetStringValue();
        public BE2_InputValues InputValues => GetInputValues();

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _block = GetComponent<I_BE2_Block>();
            _instruction = GetComponent<I_BE2_Instruction>();
        }

        //void Start()
        //{
        //    
        //}

        //void Update()
        //{
        //
        //}

        string GetStringValue()
        {
            return _instruction.Operation();
        }

        bool _isText;
        float GetFloatValue()
        {
            float floatValue = 0;
            try
            {
                floatValue = float.Parse(_instruction.Operation(), CultureInfo.InvariantCulture);
                _isText = false;
            }
            catch
            {
                _isText = true;
            }
            return floatValue;
        }

        BE2_InputValues GetInputValues()
        {
            return new BE2_InputValues(GetStringValue(), GetFloatValue(), _isText);
        }

        public void UpdateValues()
        {
            GetInputValues();
        }
    }
}