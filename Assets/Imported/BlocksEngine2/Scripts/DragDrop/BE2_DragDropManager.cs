using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.Core;
using MG_BlocksEngine2.UI;
using MG_BlocksEngine2.Environment;

namespace MG_BlocksEngine2.DragDrop
{
    // v2.7 - BE2_DragDropManager refactored to use the BE2 Input Manager
    public class BE2_DragDropManager : MonoBehaviour
    {
        BE2_UI_ContextMenuManager _contextMenuManager;

        // v2.6 - BE2_DragDropManager using instance as property to guarantee return
        static BE2_DragDropManager _instance;
        public static BE2_DragDropManager Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GameObject.FindObjectOfType<BE2_DragDropManager>();
                }
                return _instance;
            }
            set => _instance = value;
        }

        public I_BE2_Raycaster Raycaster { get; set; }
        public Transform draggedObjectsTransform;
        public Transform DraggedObjectsTransform => draggedObjectsTransform;
        public I_BE2_Drag CurrentDrag { get; set; }
        public I_BE2_Spot CurrentSpot { get; set; }
        List<I_BE2_Spot> _spotsList;
        public List<I_BE2_Spot> SpotsList
        {
            get
            {
                if (_spotsList == null)
                    _spotsList = new List<I_BE2_Spot>();
                return _spotsList;
            }
            set
            {
                _spotsList = value;
            }
        }
        [SerializeField]
        Transform _ghostBlock;
        public Transform GhostBlockTransform => _ghostBlock;
        // v2.6 - removed unused ProgrammingEnv property from the Drag and Drop Manager
        public bool isDragging;
        public float detectionDistance = 40;

        // v2.6 - added property DragDropComponentsCanvas to the Drag and Drop Manager to be used as a reference Canvas 
        static Canvas _dragDropComponentsCanvas;
        public static Canvas DragDropComponentsCanvas
        {
            get
            {
                if (!_dragDropComponentsCanvas)
                {
                    _dragDropComponentsCanvas = Instance.draggedObjectsTransform.GetComponentInParent<Canvas>();
                }
                return _dragDropComponentsCanvas;
            }
        }

        void OnEnable()
        {
            Instance = this;
        }

        void Awake()
        {
            Raycaster = GetComponent<I_BE2_Raycaster>();
            _dragDropComponentsCanvas = BE2_DragDropManager.Instance.draggedObjectsTransform.GetComponentInParent<BE2_Canvas>().Canvas;
        }

        void Start()
        {
            _contextMenuManager = BE2_UI_ContextMenuManager.instance;

            BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnPrimaryKeyDown, OnPointerDown);
            BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnSecondaryKeyDown, OnRightPointerDownOrHold);
            BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnPrimaryKeyHold, OnRightPointerDownOrHold);
            BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnDrag, OnDrag);
            BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnPrimaryKeyUp, OnPointerUp);

        }

        IEnumerator C_OnPointerDown()
        {
            yield return new WaitForEndOfFrame();

            I_BE2_Drag drag = Raycaster.GetDragAtPosition(BE2_InputManager.Instance.ScreenPointerPosition);
            if (drag != null)
            {
                CurrentDrag = drag;
                drag.OnPointerDown();
            }
            else
            {
                CurrentDrag = null;
            }
        }
        void OnPointerDown()
        {
            StartCoroutine(C_OnPointerDown());
        }

        void OnRightPointerDownOrHold()
        {
            I_BE2_Drag drag = Raycaster.GetDragAtPosition(BE2_InputManager.Instance.ScreenPointerPosition);
            if (drag != null)
            {
                drag.OnRightPointerDownOrHold();
            }
        }

        void OnDrag()
        {
            if (CurrentDrag != null)
            {
                CurrentDrag.OnDrag();
                isDragging = true;
            }
        }

        // v2.11.1 - added a handler method on the BE2_DragDropManager.OnPointerUp to call the new Block drop events
        void OnPointerUp()
        {
            if (CurrentDrag != null && isDragging)
            {
                CurrentDrag.OnPointerUp();
                StartCoroutine(C_HandleDropEvents(CurrentDrag.Block));
            }

            CurrentDrag = null;
            CurrentSpot = null;
            GhostBlockTransform.SetParent(null);
            isDragging = false;

            BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnPrimaryKeyUpEnd);
        }

        IEnumerator C_HandleDropEvents(I_BE2_Block block)
        {
            yield return new WaitForEndOfFrame();

            BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypesBlock.OnDrop, block as Object ? block : null);

            if (block as Object != null)
            {
                if (block.Type == BlockTypeEnum.trigger || block.Instruction.InstructionBase.BlocksStack == null)
                {
                    BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypesBlock.OnDropAtProgrammingEnv, block);
                }
                else
                {
                    if (block.Type == BlockTypeEnum.operation)
                    {
                        BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypesBlock.OnDropAtInputSpot, block);
                    }
                    else
                    {
                        BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypesBlock.OnDropAtStack, block);
                    }
                }
            }
            else
            {
                BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypesBlock.OnDropDestroy, null);
            }
        }

        public void AddToSpotsList(I_BE2_Spot spot)
        {
            if (!SpotsList.Contains(spot) && spot != null)
                SpotsList.Add(spot);
        }

        public void RemoveFromSpotsList(I_BE2_Spot spot)
        {
            if (SpotsList.Contains(spot))
                SpotsList.Remove(spot);
        }
    }
}
