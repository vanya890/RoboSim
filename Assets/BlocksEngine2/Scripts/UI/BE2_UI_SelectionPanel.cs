using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Utils;

namespace MG_BlocksEngine2.UI
{
    [ExecuteInEditMode]
    public class BE2_UI_SelectionPanel : MonoBehaviour
    {
        LayoutGroup _layoutGroup;
        RectTransform _rectTransform;

        void OnValidate()
        {
            GetComponent<Image>().raycastTarget = false;
            // v2.10 - avoid null error if section has no text title 
            // v2.1 - using BE2_Text to enable usage of Text or TMP components
            BE2_Text beText = BE2_Text.GetBE2Text(transform.GetChild(0));
            if (beText != null && !beText.isNull)
                beText.raycastTarget = false;
        }

        void Awake()
        {
            _layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
            _rectTransform = GetComponent<RectTransform>();
        }

        void Start()
        {
            UpdateLayout();
        }

#if UNITY_EDITOR
        void Update()
        {
            if (!EditorApplication.isPlaying)
            {
                UpdateLayout();
            }
        }
#endif

        // v2.7 - UpdateLayout method of the Selection Panel class made public
        public void UpdateLayout()
        {
            StartCoroutine(C_UpdateLayout());
        }
        IEnumerator C_UpdateLayout()
        {
            yield return new WaitForEndOfFrame();

            // v2.2 - using preferred width for selection panels 
            _rectTransform.sizeDelta = new Vector2(_layoutGroup.preferredWidth, _layoutGroup.preferredHeight);
        }
    }
}