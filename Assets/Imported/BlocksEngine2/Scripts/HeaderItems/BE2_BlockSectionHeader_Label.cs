using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Utils;

namespace MG_BlocksEngine2.Block
{
    [ExecuteInEditMode]
    public class BE2_BlockSectionHeader_Label : MonoBehaviour, I_BE2_BlockSectionHeaderItem
    {
        // v2.1 - using BE2_Text to enable usage of Text or TMP components
        BE2_Text _text;
        RectTransform _rectTransform;

        public Transform Transform => transform;
        public Vector2 Size => _rectTransform ? _rectTransform.sizeDelta : GetComponent<RectTransform>().sizeDelta;

        void OnValidate()
        {
            Awake();

            _text = BE2_Text.GetBE2Text(transform);
            if (_text != null)
            {
                _text.raycastTarget = false;
            }
        }

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        //void Start()
        //{
        //
        //}

        //void Update()
        //{
        //
        //}
    }
}