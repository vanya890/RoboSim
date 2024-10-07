using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Utils;

namespace MG_BlocksEngine2.Block
{
    [ExecuteInEditMode]
    public class BE2_BlockSectionBody : MonoBehaviour, I_BE2_BlockSectionBody
    {
        RectTransform _rectTransform;
        I_BE2_BlockSection _section;
        I_BE2_BlockLayout _blockLayout;
        Image _image;
        public RectTransform RectTransform => _rectTransform;
        public I_BE2_Block[] ChildBlocksArray { get; set; }
        public I_BE2_BlockSection BlockSection { get; set; }
        public Vector2 Size
        {
            get
            {
                return _rectTransform.sizeDelta;
            }
            set
            {
                _rectTransform.sizeDelta = value;
            }
        }
        public I_BE2_Spot Spot { get; set; }
        public int ChildBlocksCount { get; set; }
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
            _rectTransform = GetComponent<RectTransform>();

            if (transform.parent)
            {
                _section = transform.parent.GetComponent<I_BE2_BlockSection>();
                _blockLayout = transform.parent.parent.GetComponent<I_BE2_BlockLayout>();
                BlockSection = transform.parent.GetComponent<I_BE2_BlockSection>();
            }

            _image = GetComponent<Image>();
            _image.type = Image.Type.Sliced;
            _image.pixelsPerUnitMultiplier = 2;

            ChildBlocksArray = new I_BE2_Block[0];
            Spot = GetComponent<I_BE2_Spot>();
        }

        //void Start()
        //{
        //
        //}

        //void Update()
        //{
        //    //UpdateLayout();
        //}

        public void UpdateChildBlocksList()
        {
            ChildBlocksArray = new I_BE2_Block[0];
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                I_BE2_Block childBlock = transform.GetChild(i).GetComponent<I_BE2_Block>();
                if (childBlock != null)
                {
                    ChildBlocksArray = BE2_ArrayUtils.AddReturn(ChildBlocksArray, childBlock);
                }
            }
            ChildBlocksCount = ChildBlocksArray.Length;
        }

        public void UpdateLayout()
        {
            if (_image.sprite != null && _blockLayout != null)
                _image.color = _blockLayout.Color;

            float minHeight = 50;
            if (_section.Block.Type == BlockTypeEnum.trigger)
                minHeight = 0;

            float height = 0;

            UpdateChildBlocksList();
            int childsLength = ChildBlocksArray.Length;
            for (int i = 0; i < childsLength; i++)
            {
                height += ChildBlocksArray[i].Layout.Size.y - 10;
            }
            height -= 10;

            if (height < minHeight)
                height = minHeight;

            if (_section.RectTransform.transform.GetSiblingIndex() == _section.RectTransform.transform.parent.childCount - 2)
            {
                if (_section.Block.Type != BlockTypeEnum.trigger)
                {
                    height += 50;
                }
            }

            _rectTransform.sizeDelta = new Vector2(_section.Size.x, height);
        }
    }
}