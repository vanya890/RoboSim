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
    public class BE2_InputFieldDynamicResize : MonoBehaviour
    {
        RectTransform _rectTransform;
        BE2_InputField _inputField;
        float _minWidth = 70;
        float _offset = 35;

        // v2.2 - added optional max width to the text input
        public float maxWidth = 0;

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _inputField = BE2_InputField.GetBE2Component(transform);
        }

        void Start()
        {

        }

        void OnEnable()
        {
            if (_inputField != null)
                _inputField.onValueChanged.AddListener(delegate { Resize(); });
        }

        void OnDisable()
        {
            if (_inputField != null)
                _inputField.onValueChanged.RemoveAllListeners();
        }

        // v2.10 - dynamic inputfield resize called in coroutine to make sure it resizes correctly
        public void Resize()
        {
            StartCoroutine(C_Resize());
        }

        IEnumerator C_Resize()
        {
            yield return new WaitForEndOfFrame();

            float width = _offset + _inputField.preferredWidth;
            if (width < _minWidth)
                width = _minWidth;

            if (maxWidth > 0 && width > maxWidth)
                width = maxWidth;

            _rectTransform.sizeDelta = new Vector2(width, _rectTransform.sizeDelta.y);
        }
    }
}
