using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MG_BlocksEngine2.Utils
{
    // v2.1 - BE2_Text class added to enable the use of either Text or Text Mesh Pro (TMP) component to display text in the Blocks.
    // internally verifies which type is added to the GameObject and apply the relative action  
    public class BE2_Text
    {
        Transform _transform;
        Text _textComponent;
        TMP_Text _tmpComponent;
        // v2.10 - BE2_Text isNull attribute made public
        bool _isNull;
        public bool isNull => _isNull;

        public BE2_Text(Transform transform)
        {
            this._transform = transform;
        }

        void Init()
        {
            _textComponent = _transform.GetComponent<Text>();
            _tmpComponent = _transform.GetComponent<TMP_Text>();

            _isNull = !_textComponent && !_tmpComponent ? true : false;
        }

        /// <summary>
        /// Loads the Text/TMP component reference
        /// </summary>
        public static BE2_Text GetBE2Text(Transform transform)
        {
            BE2_Text be2Text = new BE2_Text(transform);
            be2Text.Init();

            return be2Text.isNull ? null : be2Text;
        }

        public static BE2_Text GetBE2TextInChildren(Transform transform)
        {
            BE2_Text childText = null;
            Text text = transform.GetComponentInChildren<Text>();
            if (text != null)
            {
                childText = GetBE2Text(text.transform);
                return childText;
            }

            TMP_Text tmp = transform.GetComponentInChildren<TMP_Text>();
            if (tmp != null)
            {
                childText = GetBE2Text(tmp.transform);
                return childText;
            }

            return null;
        }

        public static BE2_Text[] GetBE2TextsInChildren(Transform transform)
        {
            List<BE2_Text> be2Texts = new List<BE2_Text>();

            BE2_Text childText = GetBE2Text(transform);
            if (childText != null && !childText.isNull)
            {
                be2Texts.Add(childText);
                childText.Init();
            }

            foreach (Transform child in transform)
            {
                childText = GetBE2Text(child);
                if (childText != null && !childText.isNull)
                {
                    be2Texts.Add(childText);
                    childText.Init();
                }

                be2Texts.AddRange(GetBE2TextsInChildren(child));
            }

            return be2Texts.ToArray();
        }

        //--- mirror elements
        public string text
        {
            get
            {
                if (_tmpComponent)
                    return _tmpComponent.text;
                else if (_textComponent)
                    return _textComponent.text;
                else
                    return "";
            }
            set
            {
                if (_tmpComponent)
                    _tmpComponent.text = value;
                else if (_textComponent)
                    _textComponent.text = value;
            }
        }

        public bool raycastTarget
        {
            get
            {
                if (_tmpComponent)
                    return _tmpComponent.raycastTarget;
                else if (_textComponent)
                    return _textComponent.raycastTarget;
                else
                    return false;
            }
            set
            {
                if (_tmpComponent)
                    _tmpComponent.raycastTarget = value;
                else if (_textComponent)
                    _textComponent.raycastTarget = value;
            }
        }

        public T GetComponent<T>()
        {
            return _transform.GetComponent<T>();
        }
    }
}