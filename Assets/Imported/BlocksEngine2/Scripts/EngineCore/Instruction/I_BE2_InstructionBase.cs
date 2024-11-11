using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Core;
using MG_BlocksEngine2.Environment;

namespace MG_BlocksEngine2.Block.Instruction
{
    public interface I_BE2_InstructionBase
    {
        I_BE2_Instruction Instruction { get; }
        int[] LocationsArray { get; set; }
        I_BE2_Block Block { get; }
        I_BE2_BlocksStack BlocksStack { get; set; }
        I_BE2_TargetObject TargetObject { get; set; }

        /// <summary>
        /// Return an array with all the inputs of the indicated section
        /// </summary>
        I_BE2_BlockSectionHeaderInput[] GetSectionInputs(int sectionIndex);

        /// <summary>
        /// Return an array with all the inputs of the section 0
        /// </summary>
        /// <value></value>
        I_BE2_BlockSectionHeaderInput[] Section0Inputs { get; }

        /// <summary>
        /// Method that makes the stack point to the first child block’s instruction of the indicated section.
        /// Commonly called in the instruction of Trigger, Loop and Conditional blocks
        /// </summary>
        void ExecuteSection(int sectionIndex);

        /// <summary>
        /// Method that makes the stack point to the next block’s instruction in the same level as the current instruction.
        /// Commonly called in instruction of Simple, Loop and Conditional blocks
        /// </summary>
        void ExecuteNextInstruction();

        void OnStackActive();

        void PrepareToPlay();

        // v2.9 - UpdateTargetObject method added to the InstructionBase interface
        void UpdateTargetObject();
    }
}
