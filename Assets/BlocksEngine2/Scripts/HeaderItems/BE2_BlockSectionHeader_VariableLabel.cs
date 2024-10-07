using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Utils;

namespace MG_BlocksEngine2.Block
{
    public class BE2_BlockSectionHeader_VariableLabel : MonoBehaviour, I_BE2_BlockSectionHeaderItem, I_BE2_BlockSectionHeaderInput
    {
        // v2.1 - using BE2_Text to enable usage of Text or TMP components
        BE2_Text _text;
        RectTransform _rectTransform;

        public Transform Transform => transform;
        public Vector2 Size => _rectTransform ? _rectTransform.sizeDelta : GetComponent<RectTransform>().sizeDelta;
        public I_BE2_Spot Spot { get; set; }
        public float FloatValue { get; set; }
        public string StringValue { get; set; }
        public BE2_InputValues InputValues { get; set; }

        void OnValidate()
        {
            Awake();
        }

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _text = BE2_Text.GetBE2Text(transform);
            Spot = GetComponent<I_BE2_Spot>();
        }

        void OnEnable()
        {
            UpdateValues();
        }

        //void Start()
        //{
        //
        //}

        //void Update()
        //{
        //
        //}

        public void UpdateValues()
        {
            string stringValue = "";
            if (_text.text != null)
            {
                stringValue = _text.text;
            }
            StringValue = stringValue;

            FloatValue = 0;

            InputValues = new BE2_InputValues(StringValue, FloatValue, true);
        }
    }
}