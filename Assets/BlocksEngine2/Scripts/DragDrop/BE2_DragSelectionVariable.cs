using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.Utils;
using MG_BlocksEngine2.UI;
using MG_BlocksEngine2.Core;

namespace MG_BlocksEngine2.DragDrop
{
    public class BE2_DragSelectionVariable : MonoBehaviour, I_BE2_Drag
    {
        // v2.11 - references to drag drop manager and execution manager refactored in drag scripts
        BE2_DragDropManager _dragDropManager => BE2_DragDropManager.Instance;
        RectTransform _rectTransform;
        BE2_UI_SelectionBlock _uiSelectionBlock;
        ScrollRect _scrollRect;

        // v2.1 - using BE2_Text to enable usage of Text or TMP components
        BE2_Text _text;

        Transform _transform;
        public Transform Transform => _transform ? _transform : transform;
        public Vector2 RayPoint => _rectTransform.position;
        public I_BE2_Block Block => null;

        void Awake()
        {
            _transform = transform;
            _rectTransform = GetComponent<RectTransform>();
            _uiSelectionBlock = GetComponent<BE2_UI_SelectionBlock>();
            _scrollRect = GetComponentInParent<ScrollRect>();
            _text = BE2_Text.GetBE2TextInChildren(transform);
        }

        Vector3 _envScale = Vector3.one;

        public void OnPointerDown()
        {
            _envScale = BE2_ExecutionManager.Instance.ProgrammingEnvsList.Find(x => x.Visible == true).Transform.localScale;
        }

        public void OnRightPointerDownOrHold()
        {

        }

        public void OnDrag()
        {
            _scrollRect.StopMovement();
            _scrollRect.enabled = false;

            GameObject prefabBlock = Instantiate(_uiSelectionBlock.prefabBlock);
            prefabBlock.name = _uiSelectionBlock.prefabBlock.name;
            I_BE2_Block newBlock = prefabBlock.GetComponent<I_BE2_Block>();
            newBlock.Drag.Transform.SetParent(_dragDropManager.DraggedObjectsTransform, true);

            // v2.1 - using BE2_Text to enable usage of Text or TMP components
            BE2_Text newVariableText = BE2_Text.GetBE2Text(newBlock.Layout.SectionsArray[0].Header.ItemsArray[0].Transform);
            newVariableText.text = _text.text;
            newVariableText.GetComponent<BE2_BlockSectionHeader_VariableLabel>().UpdateValues();

            // v2.10 - scales the new block to the programming env's zoom
            prefabBlock.transform.localScale = _envScale;

            prefabBlock.transform.position = transform.position;
            _dragDropManager.CurrentDrag = newBlock.Drag;

            newBlock.Drag.OnPointerDown();
            newBlock.Drag.OnDrag();

            // v2.6 - adjustments on position and angle of blocks for supporting all canvas render modes
            newBlock.Transform.localEulerAngles = Vector3.zero;
        }

        public void OnPointerUp()
        {

        }
    }
}