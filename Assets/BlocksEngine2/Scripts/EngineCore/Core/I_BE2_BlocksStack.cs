using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Block.Instruction;
using MG_BlocksEngine2.Environment;
using UnityEngine.Events;

namespace MG_BlocksEngine2.Core
{
    public interface I_BE2_BlocksStack
    {
        I_BE2_TargetObject TargetObject { get; set; }

        /// <summary>
        /// Starts/stops the execution stack
        /// </summary>
        bool IsActive { get; set; }

        // v2.11 - removed the need to force BlocksStack to be added to the execution array using the MarkToAdd variable

        /// <summary>
        /// Instructions count per cycle
        /// </summary>
        int OverflowGuard { get; set; }

        /// <summary>
        /// Index of the current stack instruction 
        /// </summary>
        int Pointer { get; set; }

        I_BE2_Instruction TriggerInstruction { get; }
        I_BE2_Instruction[] InstructionsArray { get; set; }

        void Execute();

        /// <summary>
        /// Prepares the stack for execution
        /// </summary>
        void PopulateStack();

        // v2.9 - new properties and methods added to the Blocks Stack interface to enable step play and pause
        bool IsStepPlay { get; }
        void StepPlay();
        void Pause();

        // v2.11 - events OnStackStart and OnStackEnd added to the BlocksStack to allow more control 
        UnityEvent OnStackStart { get; set; }
        UnityEvent OnStackLastBlockExecuted { get; set; }
    }
}