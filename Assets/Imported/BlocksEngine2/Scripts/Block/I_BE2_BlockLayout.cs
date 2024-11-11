using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MG_BlocksEngine2.Block
{
    public interface I_BE2_BlockLayout
    {
        RectTransform RectTransform { get; set; }
        I_BE2_BlockSection[] SectionsArray { get; }
        /// <summary>
        /// Block visible color
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// Returns the size of the whole block. Headers and Bodies with child blocks are counted on.
        /// </summary>
        Vector2 Size { get; }

        /// <summary>
        /// Updates the layout of the block. Used to correctly resize the blocks after adding child and operation blocks
        /// </summary>
        void UpdateLayout();
    }
}