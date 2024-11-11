using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Block;

namespace MG_BlocksEngine2.Environment
{
    public interface I_BE2_ProgrammingEnv
    {
        Transform Transform { get; }
        List<I_BE2_Block> BlocksList { get; }
        I_BE2_TargetObject TargetObject { get; }

        /// <summary>
        /// Set if the Programming Environment is visible (alpha) and interactable (blocks raycast)
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Updates the BlockList with all the blocks in this environment 
        /// </summary>
        void UpdateBlocksList();

        /// <summary>
        /// Removes/Destroys all the blocks from the environment
        /// </summary>
        void ClearBlocks();
    }
}