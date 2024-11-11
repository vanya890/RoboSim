using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MG_BlocksEngine2.Utils
{
    // v2.10 - BE2_InputField class added to enable the use of either InputField or Text Mesh Pro (TMP) component in the Blocks
    public class BE2_InputField
    {
        Transform _transform;
        InputField _legacyComponent;
        TMP_InputField _tmpComponent;
        bool _isNull;
        public bool isNull => _isNull;

        public BE2_InputField(Transform transform)
        {
            this._transform = transform;
        }

        void Init()
        {
            _legacyComponent = _transform.GetComponent<InputField>();
            _tmpComponent = _transform.GetComponent<TMP_InputField>();

            _isNull = !_legacyComponent && !_tmpComponent ? true : false;
        }

        /// <summary>
        /// Loads the legacy/TMP component reference
        /// </summary>
        public static BE2_InputField GetBE2Component(Transform transform)
        {
            BE2_InputField be2Component = new BE2_InputField(transform);
            be2Component.Init();

            return be2Component.isNull ? null : be2Component;
        }

        public static BE2_InputField GetBE2ComponentInChildren(Transform transform)
        {
            BE2_InputField childComponent = null;
            InputField legacy = transform.GetComponentInChildren<InputField>();
            if (legacy != null)
            {
                childComponent = GetBE2Component(legacy.transform);
                return childComponent;
            }

            TMP_InputField tmp = transform.GetComponentInChildren<TMP_InputField>();
            if (tmp != null)
            {
                childComponent = GetBE2Component(tmp.transform);
                return childComponent;
            }

            return null;
        }

        public static BE2_InputField[] GetBE2ComponentsInChildren(Transform transform)
        {
            List<BE2_InputField> be2Components = new List<BE2_InputField>();

            BE2_InputField childComponent = GetBE2Component(transform);
            if (childComponent != null && !childComponent.isNull)
            {
                be2Components.Add(childComponent);
                childComponent.Init();
            }

            foreach (Transform child in transform)
            {
                childComponent = GetBE2Component(child);
                if (childComponent != null && !childComponent.isNull)
                {
                    be2Components.Add(childComponent);
                    childComponent.Init();
                }

                be2Components.AddRange(GetBE2ComponentsInChildren(child));
            }

            return be2Components.ToArray();
        }

        //--- mirror elements
        public UnityEngine.Events.UnityEvent<string> onEndEdit
        {
            get
            {
                if (_tmpComponent)
                    return _tmpComponent.onEndEdit;
                else if (_legacyComponent)
                    return _legacyComponent.onEndEdit;
                else
                    return null;
            }
        }

        public UnityEngine.Events.UnityEvent<string> onValueChanged
        {
            get
            {
                if (_tmpComponent)
                    return _tmpComponent.onValueChanged;
                else if (_legacyComponent)
                    return _legacyComponent.onValueChanged;
                else
                    return null;
            }
        }

        public float preferredWidth
        {
            get
            {
                if (_tmpComponent)
                    return _tmpComponent.preferredWidth;
                else if (_legacyComponent)
                    return _legacyComponent.preferredWidth;
                else
                    return 0;
            }
        }

        public string text
        {
            get
            {
                if (_tmpComponent)
                    return _tmpComponent.text;
                else if (_legacyComponent)
                    return _legacyComponent.text;
                else
                    return "";
            }
            set
            {
                if (_tmpComponent)
                    _tmpComponent.text = value;
                else if (_legacyComponent)
                    _legacyComponent.text = value;
            }
        }

        public bool enabled
        {
            get
            {
                if (_tmpComponent)
                    return _tmpComponent.enabled;
                else if (_legacyComponent)
                    return _legacyComponent.enabled;
                else
                    return false;
            }
            set
            {
                if (_tmpComponent)
                    _tmpComponent.enabled = value;
                else if (_legacyComponent)
                    _legacyComponent.enabled = value;
            }
        }
    }
}