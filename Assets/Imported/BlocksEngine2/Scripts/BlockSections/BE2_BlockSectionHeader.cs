using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Utils;
using MG_BlocksEngine2.Core;

namespace MG_BlocksEngine2.Block
{
    [ExecuteInEditMode]
    public class BE2_BlockSectionHeader : MonoBehaviour, I_BE2_BlockSectionHeader
    {
        RectTransform _rectTransform;
        I_BE2_BlockSection _section;
        I_BE2_BlockLayout _blockLayout;
        Image _image;
        public float minHeight;
        public Vector2 Size
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform.sizeDelta;
            }
            set
            {
                _rectTransform.sizeDelta = value;
            }
        }
        I_BE2_BlockSectionHeaderItem[] _itemsArray;
        public I_BE2_BlockSectionHeaderItem[] ItemsArray => _itemsArray;
        I_BE2_BlockSectionHeaderInput[] _inputsArray;
        public I_BE2_BlockSectionHeaderInput[] InputsArray => _inputsArray;
        Shadow _shadow;
        public Shadow Shadow
        {
            get
            {
                if (!_shadow)
                {
                    if (GetComponent<Shadow>())
                        _shadow = GetComponent<Shadow>();
                    else
                        _shadow = gameObject.AddComponent<Shadow>();

                    _shadow.effectColor = Color.green;
                    _shadow.effectDistance = new Vector2(-6, -6);
                }

                return _shadow;
            }
        }

        void OnValidate()
        {
            Awake();
        }

        void Awake()
        {
            UpdateItemsArray();
            UpdateInputsArray();

            _rectTransform = GetComponent<RectTransform>();

            if (transform.parent)
            {
                _section = transform.parent.GetComponent<I_BE2_BlockSection>();
                _blockLayout = transform.parent.parent.GetComponent<I_BE2_BlockLayout>();
            }

            _image = GetComponent<Image>();
            _image.type = Image.Type.Sliced;
            _image.pixelsPerUnitMultiplier = 2;
        }

        void Start()
        {
            BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnDrag, UpdateItemsArray);
            BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnPrimaryKeyUpEnd, UpdateItemsArray);
            BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnPrimaryKeyUpEnd, UpdateInputsArray);
        }

        void OnDisable()
        {
            BE2_MainEventsManager.Instance.StopListening(BE2EventTypes.OnDrag, UpdateItemsArray);
            BE2_MainEventsManager.Instance.StopListening(BE2EventTypes.OnPrimaryKeyUpEnd, UpdateItemsArray);
            BE2_MainEventsManager.Instance.StopListening(BE2EventTypes.OnPrimaryKeyUpEnd, UpdateInputsArray);
        }

        //void Update()
        //{
        //    
        //}

        public void UpdateItemsArray()
        {
            _itemsArray = new I_BE2_BlockSectionHeaderItem[0];
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                I_BE2_BlockSectionHeaderItem item = transform.GetChild(i).GetComponent<I_BE2_BlockSectionHeaderItem>();
                if (item != null && item.Transform.gameObject.activeSelf)
                {
                    BE2_ArrayUtils.Add(ref _itemsArray, item);
                }
            }
        }

        public void UpdateInputsArray()
        {
            _inputsArray = new I_BE2_BlockSectionHeaderInput[0];
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                I_BE2_BlockSectionHeaderInput input = transform.GetChild(i).GetComponent<I_BE2_BlockSectionHeaderInput>();
                if (input != null && input.Transform.gameObject.activeSelf)
                {
                    BE2_ArrayUtils.Add(ref _inputsArray, input);
                }
            }
        }

        public void UpdateLayout()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                UpdateItemsArray();
            }
#endif

            if (_blockLayout != null)
                _image.color = _blockLayout.Color;

            if (_section.RectTransform.transform.GetSiblingIndex() == 0)
            {
                float width = 0;
                float height = minHeight - 40;
                float h = 0;
                int itemsLength = _itemsArray.Length;
                for (int i = 0; i < itemsLength; i++)
                {
                    I_BE2_BlockSectionHeaderItem item = _itemsArray[i];
                    width += item.Size.x + 15;
                    if (item.Size.y > h)
                        h = item.Size.y;
                }
                width += 15;

                if (width < 150)
                    width = 150;

                height += h;
                if (height < minHeight)
                    height = minHeight;

                _rectTransform.sizeDelta = new Vector2(width, height);
            }
            else
            {
                _rectTransform.sizeDelta = new Vector2(_blockLayout.SectionsArray[0].Header.Size.x, _rectTransform.sizeDelta.y);
            }
        }
    }
}