using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MG_BlocksEngine2.Utils
{
    // v2.10 - BE2_Dropdown class added to enable the use of either Dropdown or Text Mesh Pro (TMP) component in the Blocks
    public class BE2_Dropdown
    {
        Transform _transform;
        Dropdown _legacyComponent;
        TMP_Dropdown _tmpComponent;
        bool _isNull;
        public bool isNull => _isNull;

        public BE2_Dropdown(Transform transform)
        {
            this._transform = transform;
        }

        void Init()
        {
            _legacyComponent = _transform.GetComponent<Dropdown>();
            _tmpComponent = _transform.GetComponent<TMP_Dropdown>();

            _isNull = !_legacyComponent && !_tmpComponent ? true : false;
        }

        /// <summary>
        /// Loads the legacy/TMP component reference
        /// </summary>
        public static BE2_Dropdown GetBE2Component(Transform transform)
        {
            BE2_Dropdown be2Component = new BE2_Dropdown(transform);
            be2Component.Init();

            return be2Component.isNull ? null : be2Component;
        }

        public static BE2_Dropdown GetBE2ComponentInChildren(Transform transform)
        {
            BE2_Dropdown childComponent = null;
            Dropdown legacy = transform.GetComponentInChildren<Dropdown>();
            if (legacy != null)
            {
                childComponent = GetBE2Component(legacy.transform);
                return childComponent;
            }

            TMP_Dropdown tmp = transform.GetComponentInChildren<TMP_Dropdown>();
            if (tmp != null)
            {
                childComponent = GetBE2Component(tmp.transform);
                return childComponent;
            }

            return null;
        }

        public static BE2_Dropdown[] GetBE2ComponentsInChildren(Transform transform)
        {
            List<BE2_Dropdown> be2Components = new List<BE2_Dropdown>();

            BE2_Dropdown childComponent = GetBE2Component(transform);
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
        public void ClearOptions()
        {
            if (_tmpComponent)
                _tmpComponent.ClearOptions();
            else if (_legacyComponent)
                _legacyComponent.ClearOptions();
        }

        public void AddOption(string option)
        {
            if (_tmpComponent)
                _tmpComponent.options.Add(new TMP_Dropdown.OptionData(option));
            else if (_legacyComponent)
                _legacyComponent.options.Add(new Dropdown.OptionData(option));
        }

        public string GetOptionTextAtIndex(int index)
        {
            if (_tmpComponent)
                return _tmpComponent.options[index].text;
            else if (_legacyComponent)
                return _legacyComponent.options[index].text;
            else
                return "";
        }

        public string GetSelectedOptionText()
        {
            if (_tmpComponent)
                return _tmpComponent.options[_tmpComponent.value].text;
            else if (_legacyComponent)
                return _legacyComponent.options[_legacyComponent.value].text;
            else
                return "";
        }

        public int GetOptionsCount()
        {
            if (_tmpComponent)
                return _tmpComponent.options.Count;
            else if (_legacyComponent)
                return _legacyComponent.options.Count;
            else
                return -1;
        }

        public int GetIndexOf(string text)
        {
            if (_tmpComponent)
                return _tmpComponent.options.FindIndex(option => option.text == text);
            else if (_legacyComponent)
                return _legacyComponent.options.FindIndex(option => option.text == text);
            else
                return -1;
        }

        public int value
        {
            get
            {
                if (_tmpComponent)
                    return _tmpComponent.value;
                else if (_legacyComponent)
                    return _legacyComponent.value;
                else
                    return -1;
            }
            set
            {
                if (_tmpComponent)
                    _tmpComponent.value = value;
                else if (_legacyComponent)
                    _legacyComponent.value = value;
            }
        }

        public void RefreshShownValue()
        {
            if (_tmpComponent)
                _tmpComponent.RefreshShownValue();
            else if (_legacyComponent)
                _legacyComponent.RefreshShownValue();
        }

        public float captionTextpreferredWidth
        {
            get
            {
                if (_tmpComponent)
                    return _tmpComponent.captionText.preferredWidth;
                else if (_legacyComponent)
                    return _legacyComponent.captionText.preferredWidth;
                else
                    return -1;
            }
        }

        public UnityEngine.Events.UnityEvent<int> onValueChanged
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