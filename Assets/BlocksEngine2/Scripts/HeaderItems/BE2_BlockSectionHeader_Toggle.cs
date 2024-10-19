using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace MG_BlocksEngine2.Block
{
    // v2.7 - added BE2_BlockSectionHeader_Toggle input type for blocks (must be added manually, not yet available on the inspector's Block Builder)
    public class BE2_BlockSectionHeader_Toggle : MonoBehaviour, I_BE2_BlockSectionHeaderItem, I_BE2_BlockSectionHeaderInput
    {
        Toggle _toggle;
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
            _toggle = GetComponent<Toggle>();
            Spot = GetComponent<I_BE2_Spot>();
        }

        void OnEnable()
        {
            UpdateValues();
            _toggle.onValueChanged.AddListener(delegate { UpdateValues(); });
        }

        void OnDisable()
        {
            _toggle.onValueChanged.RemoveAllListeners();
        }

        void Start()
        {
            UpdateValues();
        }

        public void UpdateValues()
        {
            StringValue = "false";
            FloatValue = 0;
            if (_toggle.isOn)
            {
                StringValue = "true";
                FloatValue = 1;
            }

            InputValues = new BE2_InputValues(StringValue, FloatValue, true);
        }

    }
}