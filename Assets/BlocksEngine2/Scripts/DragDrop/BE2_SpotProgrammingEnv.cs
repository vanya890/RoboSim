using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.DragDrop;

namespace MG_BlocksEngine2.Block
{
    public class BE2_SpotProgrammingEnv : MonoBehaviour, I_BE2_Spot
    {
        BE2_DragDropManager _dragDropManager;

        public string Type => "programmingEnv";
        Transform _transform;
        public Transform Transform => _transform ? _transform : transform;
        public Vector2 DropPosition => Vector2.zero;
        public I_BE2_Block Block => null;

        void Awake()
        {
            _transform = transform;
            // v2.8 - improve performance by using BE2_DragDropManager.Instance
            _dragDropManager = BE2_DragDropManager.Instance;
        }

        //void Start()
        //{
        //
        //}

        //void Update()
        //{
        //
        //}

        public void OnPointerUp()
        {
            I_BE2_Drag drag = _dragDropManager.CurrentDrag;
            if (drag != null)
                drag.Transform.SetParent(transform);
        }

        void OnEnable()
        {
            _dragDropManager.AddToSpotsList(this);
        }

        void OnDisable()
        {
            _dragDropManager.RemoveFromSpotsList(this);
        }
    }
}