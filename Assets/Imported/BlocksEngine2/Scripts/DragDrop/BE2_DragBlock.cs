using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.UI;
using MG_BlocksEngine2.Environment;

namespace MG_BlocksEngine2.DragDrop
{
    public class BE2_DragBlock : MonoBehaviour, I_BE2_Drag
    {
        RectTransform _rectTransform;
        // v2.11 - references to drag drop manager and execution manager refactored in drag scripts
        BE2_DragDropManager _dragDropManager => BE2_DragDropManager.Instance;
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
            if (!_isDetectingSpot)
                StartCoroutine(DetectSpotOnEndOfFrame());
        }

        bool _isDetectingSpot = false;
        // v2.1 - detection of spot on drag of Block moved to coroutine and performed on end of frame to avoid glith on detecting new spot
        IEnumerator DetectSpotOnEndOfFrame()
        {
            _isDetectingSpot = true;
            yield return new WaitForEndOfFrame();

            if (Transform.parent != _dragDropManager.DraggedObjectsTransform)
                Transform.SetParent(_dragDropManager.DraggedObjectsTransform, true);

            I_BE2_Spot spot = _dragDropManager.Raycaster.FindClosestSpotForBlock(this, _dragDropManager.detectionDistance);

            Transform ghostBlockTransform = _dragDropManager.GhostBlockTransform;
            if (spot is BE2_SpotBlockBody && spot.Block != Block)
            {
                ghostBlockTransform.SetParent(spot.Transform);
                ghostBlockTransform.localScale = Vector3.one;
                ghostBlockTransform.gameObject.SetActive(true);
                ghostBlockTransform.SetSiblingIndex(0);

                _dragDropManager.CurrentSpot = spot;
            }
            else if (spot is BE2_SpotOuterArea)
            {
                ghostBlockTransform.SetParent(spot.Block.Transform.parent);
                ghostBlockTransform.localScale = Vector3.one;
                ghostBlockTransform.gameObject.SetActive(true);
                ghostBlockTransform.SetSiblingIndex(spot.Block.Transform.GetSiblingIndex() + 1);

                spot.Block.ParentSection.UpdateLayout();
                _dragDropManager.CurrentSpot = spot;
            }
            else
            {
                ghostBlockTransform.gameObject.SetActive(false);
                // v2.6 - bugfix: fixed null exception when starting the scene with no Target Objects
                _dragDropManager.CurrentSpot = null;
            }
            _isDetectingSpot = false;

            // v2.6 - adjustments on position and angle of blocks for supporting all canvas render modes
            ghostBlockTransform.localPosition = new Vector3(ghostBlockTransform.localPosition.x, ghostBlockTransform.localPosition.y, 0);
            ghostBlockTransform.localEulerAngles = Vector3.zero;
        }

        public void OnPointerUp()
        {
            if (_dragDropManager.CurrentSpot != null)
            {
                if (_dragDropManager.CurrentSpot is BE2_SpotBlockBody)
                    DropTo(_dragDropManager.CurrentSpot, 0);
                else
                {
                    DropTo(_dragDropManager.CurrentSpot.Block.Transform.parent, _dragDropManager.CurrentSpot.Block.Transform.GetSiblingIndex() + 1);
                }

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

        // v2.11 - added DropTo method to the BE2_DragBlock and BE2_DragOperation classes
        void DropTo(Transform spot, int siblinIndex)
        {
            Transform.SetParent(spot);
            Transform.SetSiblingIndex(siblinIndex);
        }
        void DropTo(I_BE2_Spot spot, int siblinIndex)
        {
            DropTo(spot.Transform, siblinIndex);
        }
        public void DropTo(I_BE2_Block parentBlock, int sectionIndex, int siblinIndex)
        {
            if (parentBlock.Layout.SectionsArray.Length > sectionIndex && parentBlock.Layout.SectionsArray[sectionIndex].Body != null) // make sure the body exists
            {
                DropTo(parentBlock.Layout.SectionsArray[sectionIndex].Body.Spot, siblinIndex);

                parentBlock.Instruction.InstructionBase.BlocksStack.PopulateStack();
            }
        }
    }
}
