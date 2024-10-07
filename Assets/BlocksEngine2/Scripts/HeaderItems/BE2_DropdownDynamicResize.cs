using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Utils;

namespace MG_BlocksEngine2.Block
{
    // v2.10 - Dropdown and InputField references in the block header inputs replaced by BE2_Dropdown and BE2_InputField to enable the use of legacy or TMP components
    [ExecuteInEditMode]
    public class BE2_DropdownDynamicResize : MonoBehaviour
    {
        RectTransform _rectTransform;
        BE2_Dropdown _dropdown;
        float _minWidth = 70;
        // v2.9 - bugfix: dropdown text being cropped on the Blocks Selection Panel
        float _offset = 45;

        // v2.2 - added optional max width to the dropdown input
        public float maxWidth = 0;

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _dropdown = BE2_Dropdown.GetBE2Component(transform);
        }

        void Start()
        {
            Resize();
        }

        void OnEnable()
        {
            if (_dropdown != null)
                _dropdown.onValueChanged.AddListener(delegate { Resize(); });
        }

        void OnDisable()
        {
            if (_dropdown != null)
                _dropdown.onValueChanged.RemoveAllListeners();
        }

        public void Resize()
        {
            if (_dropdown != null && !_dropdown.isNull)
            {
                float width = _offset + _dropdown.captionTextpreferredWidth;
                if (width < _minWidth)
                    width = _minWidth;

                if (maxWidth > 0 && width > maxWidth)
                    width = maxWidth;

                _rectTransform.sizeDelta = new Vector2(width, _rectTransform.sizeDelta.y);
            }
        }
    }
}
