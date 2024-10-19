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
    public class BE2_BlockVerticalLayout : MonoBehaviour, I_BE2_BlockLayout
    {
        public Color blockColor = Color.white;
        RectTransform _rectTransform;
        public RectTransform RectTransform { get => _rectTransform; set => _rectTransform = value; }
        I_BE2_BlockSection[] _sectionsArray;
        public I_BE2_BlockSection[] SectionsArray => _sectionsArray;
        public Color Color { get => blockColor; set => blockColor = value; }
        public Vector2 Size
        {
            get
            {
                Vector2 size = Vector2.zero;

                int sectionsLength = SectionsArray.Length;
                for (int i = 0; i < sectionsLength; i++)
                {
                    I_BE2_BlockSection section = SectionsArray[i];
                    size.y += section.Size.y;
                    if (section.Size.x > size.x)
                        size.x = section.Size.x;
                }

                return size;
            }
        }

        void OnValidate()
        {
            Awake();
        }

        void Awake()
        {
            Initialize();
        }

        void Start()
        {
            _rectTransform.pivot = new Vector2(0, 1);
            UpdateLayout();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);

            // use invoke repeating and remove UpdateLayout from the Uptade method if needed to increase performance 
            //InvokeRepeating("UpdateLayout", 0, 0.08f);
        }

#if UNITY_EDITOR
        // v2.1 - moved blocks LayoutUpdate from Update to LateUpdate to avoid glitch on resizing 
        void LateUpdate()
        {
            UpdateLayout();

            if (!EditorApplication.isPlaying)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
            }
        }
#endif

        void OnEnable()
        {
            // v2.9 - Blocks layout update is now executed by the execution manager
            BE2_ExecutionManager.Instance.AddToLateUpdate(UpdateLayout);
        }
        void OnDisable()
        {
            // v2.9 - Blocks layout update is now executed by the execution manager
            BE2_ExecutionManager.Instance?.RemoveFromLateUpdate(UpdateLayout);
        }

        public void Initialize()
        {
            _rectTransform = GetComponent<RectTransform>();

            _sectionsArray = new I_BE2_BlockSection[0];

            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                I_BE2_BlockSection section = transform.GetChild(i).GetComponent<I_BE2_BlockSection>();
                if (section != null)
                    BE2_ArrayUtils.Add(ref _sectionsArray, section);
            }
        }

        // v2.9 - Block layout update refactored (executed as coroutine at the end of frame) to be executed by the execution manager 
        public void UpdateLayout()
        {
            StartCoroutine(C_UpdateLayout());
        }

        public IEnumerator C_UpdateLayout()
        {
            yield return new WaitForEndOfFrame();
            _rectTransform.sizeDelta = Size;

            int sectionsLength = SectionsArray.Length;
            for (int i = 0; i < sectionsLength; i++)
            {
                SectionsArray[i].UpdateLayout();
            }
        }
    }
}