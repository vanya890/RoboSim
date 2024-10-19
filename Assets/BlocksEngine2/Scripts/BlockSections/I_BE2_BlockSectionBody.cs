using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MG_BlocksEngine2.Block
{
    public interface I_BE2_BlockSectionBody
    {
        RectTransform RectTransform { get; }
        Vector2 Size { get; }
        I_BE2_Block[] ChildBlocksArray { get; }
        I_BE2_Spot Spot { get; set; }
        I_BE2_BlockSection BlockSection { get; }
        int ChildBlocksCount { get; }
        Shadow Shadow { get; }

        /// <summary>
        /// Updates ChildBlocksCount and ChildBlocksArray with the current child blocks
        /// </summary>
        void UpdateChildBlocksList();

        /// <summary>
        /// Updates the layout of an individual block body. Used to correctly resize the body after adding child blocks
        /// </summary>
        void UpdateLayout();
    }
}