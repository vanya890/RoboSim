using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Core;

namespace MG_BlocksEngine2.DragDrop
{
    // v2.7 - BE2_Pointer refactored to use the BE2 Input Manager
    public class BE2_Pointer : MonoBehaviour
    {
        Transform _transform;
        Vector3 _mousePos;

        // v2.6 - added property Instance in the BE2_Pointer
        static BE2_Pointer _instance;
        public static BE2_Pointer Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GameObject.FindObjectOfType<BE2_Pointer>();
                }
                return _instance;
            }
            set => _instance = value;
        }

        void Awake()
        {
            _transform = transform;
        }

        public void OnUpdate()
        {
            UpdatePointerPosition();
        }

        public void UpdatePointerPosition()
        {
            _mousePos = BE2_InputManager.Instance.CanvasPointerPosition;

            _transform.position = new Vector3(_mousePos.x, _mousePos.y, _transform.position.z);
            _transform.localPosition = new Vector3(_transform.localPosition.x, _transform.localPosition.y, 0);
            _transform.localEulerAngles = Vector3.zero;
        }
    }
}