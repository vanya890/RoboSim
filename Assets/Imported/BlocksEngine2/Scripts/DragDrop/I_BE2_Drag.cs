using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Block;

namespace MG_BlocksEngine2.DragDrop
{
    public interface I_BE2_Drag
    {
        /// <summary>
        /// Prepares the block to possibly be dragged
        /// </summary>
        void OnPointerDown();

        /// <summary>
        /// Opens up the context menu
        /// </summary>
        void OnRightPointerDownOrHold();

        /// <summary>
        /// Keeps detecting possible drop spots 
        /// </summary>
        void OnDrag();

        /// <summary>
        /// Places the block into a found spot, programming env, or deletes it
        /// </summary>
        void OnPointerUp();

        Transform Transform { get; }
        Vector2 RayPoint { get; }
        I_BE2_Block Block { get; }
        // v2.11 - removed unecessary ChildBlocks list from Drag interface
    }
}