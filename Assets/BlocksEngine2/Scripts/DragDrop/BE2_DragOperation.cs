using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.UI;
using MG_BlocksEngine2.Environment;

namespace MG_BlocksEngine2.DragDrop
{
    public class BE2_DragOperation : MonoBehaviour, I_BE2_Drag
    {
        // v2.11 - references to drag drop manager and execution manager refactored in drag scripts
        BE2_DragDropManager _dragDropManager => BE2_DragDropManager.Instance;
        RectTransform _rectTransform;
        // v2.10.2 - bugfix: blocks that start inside the programming env with an operation block as input get no input spot back after dragging operation block out
        [HideInInspector] [SerializeField] Transform _usedSpotTransform;  // former I_BE2_Spot _usedSpot

        Transform _transform;
        public Transform Transform => _transform ? _transform : transform;
        public Vector2 RayPoint => _rectTransform.position;
        public I_BE2_Block Block { get; set; }

        void Awake()
        {
            _transform = transform;
            _rectTransform = GetComponent<RectTransform>();
            Block = GetComponent<I_BE2_Block>();
        }

        //void Update()
        //{
        //
        //}

        public void OnPointerDown()
        {

        }

        public void OnRightPointerDownOrHold()
        {
            BE2_UI_ContextMenuManager.instance.OpenContextMenu(0, Block);
        }

        public void OnDrag()
        {
            if (_usedSpotTransform != null)
            {
                _usedSpotTransform.SetSiblingIndex(Transform.GetSiblingIndex());
                _usedSpotTransform.gameObject.SetActive(true);
                _usedSpotTransform = null;
            }

            if (Transform.parent != _dragDropManager.DraggedObjectsTransform)
                Transform.SetParent(_dragDropManager.DraggedObjectsTransform, true);

            // v2.6 - bugfix: fixed operation blocks not using drag ad drop detection distance as parameter 
            I_BE2_Spot spot = _dragDropManager.Raycaster.FindClosestSpotOfType<BE2_SpotBlockInput>(this, _dragDropManager.detectionDistance);

            if (spot != null)
            {
                // last selected spot
                if (_dragDropManager.CurrentSpot != null && _dragDropManager.CurrentSpot != spot)
                    (_dragDropManager.CurrentSpot as BE2_SpotBlockInput).outline.enabled = false;

                _dragDropManager.CurrentSpot = spot;
                (_dragDropManager.CurrentSpot as BE2_SpotBlockInput).outline.enabled = true;
            }
            else
            {
                if (_dragDropManager.CurrentSpot != null)
                {
                    (_dragDropManager.CurrentSpot as BE2_SpotBlockInput).outline.enabled = false;
                    _dragDropManager.CurrentSpot = null;
                }
            }
        }

        public void OnPointerUp()
        {
            if (_dragDropManager.CurrentSpot != null)
            {
                DropTo(_dragDropManager.CurrentSpot);

                _dragDropManager.CurrentSpot = null;
            }
            else
            {
                I_BE2_Spot spot = _dragDropManager.Raycaster.GetSpotAtPosition(RayPoint);
                if (spot != null)
                {
                    I_BE2_ProgrammingEnv programmingEnv = spot.Transform.GetComponentInParent<I_BE2_ProgrammingEnv>();
                    if (programmingEnv == null && spot.Transform.GetChild(0) != null)
                        programmingEnv = spot.Transform.GetChild(0).GetComponentInParent<I_BE2_ProgrammingEnv>();

                    if (programmingEnv != null)
                        Transform.SetParent(programmingEnv.Transform);
                    else
                        Destroy(Transform.gameObject);
                }
                else
                {
                    Destroy(Transform.gameObject);
                }
            }

            // v2.6 - adjustments on position and angle of blocks for supporting all canvas render modes
            Transform.localPosition = new Vector3(Transform.localPosition.x, Transform.localPosition.y, 0);
            Transform.localEulerAngles = Vector3.zero;

            // v2.9 - bugfix: TargetObject of blocks being null
            Block.Instruction.InstructionBase.UpdateTargetObject();
        }

        // v2.1 - bugfix: fixed destroying operations placed as inputs causing error 
        void OnDisable()
        {
            if (_usedSpotTransform != null)
            {
                // v2.3 - bigfix: fixed intermittent error "cannot change sibling OnDisable"
                _usedSpotTransform.gameObject.SetActive(true);
                _usedSpotTransform = null;
            }

            if (Transform.parent != _dragDropManager.DraggedObjectsTransform)
                Transform.gameObject.SetActive(false);
        }

        // v2.11 - added DropTo method to the BE2_DragBlock and BE2_DragOperation classes
        void DropTo(I_BE2_Spot spot)
        {
            Transform.SetParent(spot.Transform.parent);
            Transform.SetSiblingIndex(spot.Transform.GetSiblingIndex());

            (spot as BE2_SpotBlockInput).outline.enabled = false;
            _usedSpotTransform = spot.Transform;
            _usedSpotTransform.gameObject.SetActive(false);
        }
        public void DropTo(I_BE2_Block parentBlock, int sectionIndex, int inputIndex)
        {
            if (parentBlock.Layout.SectionsArray.Length > sectionIndex && parentBlock.Layout.SectionsArray[sectionIndex].Header.InputsArray.Length > inputIndex) // make sure the spot exists
            {
                I_BE2_Spot spot = parentBlock.Layout.SectionsArray[sectionIndex].Header.InputsArray[inputIndex].Transform.GetComponent<I_BE2_Spot>();
                if (spot != null)
                {
                    DropTo(spot);

                    parentBlock.Layout.SectionsArray[sectionIndex].Header.UpdateInputsArray();
                    parentBlock.Layout.SectionsArray[sectionIndex].Header.UpdateItemsArray();
                    parentBlock.Instruction.InstructionBase.BlocksStack.PopulateStack();
                    parentBlock.Instruction.InstructionBase.UpdateTargetObject();
                }
            }
        }
    }
}
