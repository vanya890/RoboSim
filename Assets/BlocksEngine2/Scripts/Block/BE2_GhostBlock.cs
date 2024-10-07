using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.DragDrop;
using MG_BlocksEngine2.Block.Instruction;

namespace MG_BlocksEngine2.Block
{
    public class BE2_GhostBlock : MonoBehaviour, I_BE2_Block
    {
        public BlockTypeEnum Type { get => BlockTypeEnum.none; set { } }
        public I_BE2_BlockLayout Layout { get; set; }
        public I_BE2_Instruction Instruction => null;
        Transform _transform;
        public Transform Transform => _transform ? _transform : transform;
        public I_BE2_BlockSection ParentSection { get; set; }
        public I_BE2_Drag Drag { get; set; }

        void Awake()
        {
            _transform = transform;
            Layout = GetComponent<I_BE2_BlockLayout>();
        }

        public void SetShadowActive(bool value) { }

        //void Start()
        //{
        //
        //}

        //void Update()
        //{
        //
        //}
    }
}