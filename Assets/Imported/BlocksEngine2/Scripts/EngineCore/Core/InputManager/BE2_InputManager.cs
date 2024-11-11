using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.DragDrop;
using MG_BlocksEngine2.EditorScript;
using MG_BlocksEngine2.UI;

namespace MG_BlocksEngine2.Core
{
    // v2.7 - added the BE2 Input Manager class to the system 
    public class BE2_InputManager : MonoBehaviour, I_BE2_InputManager
    {
        static I_BE2_InputManager _instance;
        public static I_BE2_InputManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    // v2.11 - custom InputManagers derived only from the I_BE2_InputManager interface (not from the BE2_InputManager class) can be used 
                    foreach (GameObject go in FindObjectsOfType<GameObject>())
                    {
                        _instance = go.GetComponent<I_BE2_InputManager>();
                        if (_instance != null)
                            break;
                    }
                    // _instance = GameObject.FindObjectOfType<BE2_InputManager>() as I_BE2_InputManager;
                }
                return _instance;
            }
            set => _instance = value;
        }

        public KeyCode primaryKey = KeyCode.Mouse0;
        public KeyCode secondaryKey = KeyCode.Mouse1;
        public KeyCode deleteKey = KeyCode.Delete;

        // v2.10 - added new possible key, auxKey0, to the input manager
        public KeyCode auxKey0 = KeyCode.LeftControl;

        public Vector3 ScreenPointerPosition => Input.mousePosition;
        public Vector3 CanvasPointerPosition
        {
            get
            {
                return GetCanvasPointerPosition();
            }
        }

        BE2_EventsManager _mainEventsManager;
        BE2_DragDropManager _dragDropManager;

        // v2.9 - bugfix: changing the Canvas Render Mode needed recompiling to work correctly 
        BE2_Inspector _inspector => BE2_Inspector.Instance;

        void OnEnable()
        {
            _mainEventsManager = BE2_MainEventsManager.Instance;
            _dragDropManager = BE2_DragDropManager.Instance;
        }

        // void Start()
        // {

        // } 

        float _holdCounter = 0;
        Vector2 _lastPosition;

        public void OnUpdate()
        {
            // pointer 0 down
            if (Input.GetKeyDown(primaryKey))
            {
                _mainEventsManager.TriggerEvent(BE2EventTypes.OnPrimaryKeyDown);
            }

            // pointer 1 down or pointer 0 hold
            if (Input.GetKeyDown(secondaryKey))
            {
                _mainEventsManager.TriggerEvent(BE2EventTypes.OnSecondaryKeyDown);
            }
            if (_dragDropManager.CurrentDrag != null && !_dragDropManager.isDragging)
            {
                _holdCounter += Time.deltaTime;
                if (_holdCounter > 0.6f)
                {
                    _mainEventsManager.TriggerEvent(BE2EventTypes.OnPrimaryKeyHold);
                    _holdCounter = 0;
                }
            }

            // pointer 0
            if (Input.GetKey(primaryKey))
            {
                _mainEventsManager.TriggerEvent(BE2EventTypes.OnPrimaryKey);
                // v2.6 - using BE2_Pointer as main pointer input source
                float distance = Vector2.Distance(_lastPosition, (Vector2)ScreenPointerPosition);
                if (distance > 0.5f && !BE2_UI_ContextMenuManager.instance.isActive)
                {
                    _mainEventsManager.TriggerEvent(BE2EventTypes.OnDrag);
                }
            }

            // pointer 0 up
            if (Input.GetKeyUp(primaryKey))
            {
                _mainEventsManager.TriggerEvent(BE2EventTypes.OnPrimaryKeyUp);
                _holdCounter = 0;
            }

            // v2.10 - added new events to the input manager
            if (Input.GetKeyUp(secondaryKey))
            {
                _mainEventsManager.TriggerEvent(BE2EventTypes.OnSecondaryKeyUp);
            }
            if (Input.GetKeyDown(auxKey0))
            {
                _mainEventsManager.TriggerEvent(BE2EventTypes.OnAuxKeyDown);
            }
            if (Input.GetKeyUp(auxKey0))
            {
                _mainEventsManager.TriggerEvent(BE2EventTypes.OnAuxKeyUp);
            }

            if (Input.GetKeyDown(deleteKey))
            {
                _mainEventsManager.TriggerEvent(BE2EventTypes.OnDeleteKeyDown);
            }

            _lastPosition = ScreenPointerPosition;
        }

        Vector3 GetCanvasPointerPosition()
        {
            Camera mainCamera = _inspector.Camera;
            if (_inspector.CanvasRenderMode == RenderMode.ScreenSpaceOverlay)
            {
                return ScreenPointerPosition;
            }
            else if (_inspector.CanvasRenderMode == RenderMode.ScreenSpaceCamera)
            {
                var screenPoint = ScreenPointerPosition;
                screenPoint.z = BE2_DragDropManager.DragDropComponentsCanvas.transform.position.z - mainCamera.transform.position.z; //distance of the plane from the camera
                return GetMouseInCanvas(screenPoint);
            }
            else if (_inspector.CanvasRenderMode == RenderMode.WorldSpace)
            {
                var screenPoint = ScreenPointerPosition;
                screenPoint.z = BE2_DragDropManager.DragDropComponentsCanvas.transform.position.z - mainCamera.transform.position.z; //distance of the plane from the camera
                return GetMouseInCanvas(screenPoint);
            }

            return Vector3.zero;
        }

        Vector3 GetMouseInCanvas(Vector3 position)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                BE2_DragDropManager.DragDropComponentsCanvas.transform as RectTransform,
                position,
                _inspector.Camera,
                out Vector3 mousePosition
            );
            return mousePosition;
        }
    }
}
