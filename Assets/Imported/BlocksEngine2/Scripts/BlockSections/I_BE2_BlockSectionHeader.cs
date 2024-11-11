using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MG_BlocksEngine2.Block
{
    public interface I_BE2_BlockSectionHeader
    {
        I_BE2_BlockSectionHeaderItem[] ItemsArray { get; }
        I_BE2_BlockSectionHeaderInput[] InputsArray { get; }
        Vector2 Size { get; }
        Shadow Shadow { get; }

        /// <summary>
        /// Updates the layout of an individual block header. Used to correctly resize the body after adding operation blocks
        /// </summary>
        void UpdateLayout();

        /// <summary>
        /// Updates the ItemsArray with all the current I_BE2_BlockSectionHeaderItem (labels and inputs) in the header
        /// </summary>
        void UpdateItemsArray();

        /// <summary>
        /// Updates the InputsArray with all the current I_BE2_BlockSectionHeaderInput (inputs only) in the header 
        /// </summary>
        void UpdateInputsArray();
    }
}