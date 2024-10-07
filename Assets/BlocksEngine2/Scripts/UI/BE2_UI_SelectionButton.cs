using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Utils;

namespace MG_BlocksEngine2.UI
{
    [ExecuteInEditMode]
    public class BE2_UI_SelectionButton : MonoBehaviour
    {
        Button _button;
        BE2_UI_BlocksSelectionViewer _blocksSelectionViewer;

        public BE2_UI_SelectionPanel selectionPanel;

        private void OnValidate()
        {
            // v2.10 - section selection buttons don't need to have images in specific sibling positions
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Image>())
                    child.GetComponent<Image>().raycastTarget = false;
            }
            transform.GetComponent<Image>().raycastTarget = true;

            // v2.1 - using BE2_Text to enable usage of Text or TMP components
            BE2_Text[] texts = BE2_Text.GetBE2TextsInChildren(transform);
            foreach (BE2_Text text in texts)
            {
                text.raycastTarget = false;
            }
        }

        void Awake()
        {
            _button = GetComponent<Button>();
        }

        void Start()
        {
            _blocksSelectionViewer = BE2_UI_BlocksSelectionViewer.Instance;
            _button.onClick.AddListener(ToggleSection);
        }

        //void Update()
        //{
        //
        //}

        public void ToggleSection()
        {
            foreach (BE2_UI_SelectionPanel panel in _blocksSelectionViewer.selectionPanelsList)
            {
                if (selectionPanel)
                {
                    if (panel == selectionPanel)
                        panel.gameObject.SetActive(true);
                    else
                        panel.gameObject.SetActive(false);
                }
                else
                {
                    panel.gameObject.SetActive(true);
                }
            }
        }
    }
}
