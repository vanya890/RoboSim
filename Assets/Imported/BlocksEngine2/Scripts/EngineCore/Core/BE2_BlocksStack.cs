using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.Block.Instruction;
using MG_BlocksEngine2.Utils;
using MG_BlocksEngine2.Environment;
using UnityEngine.Events;

namespace MG_BlocksEngine2.Core
{
    public class BE2_BlocksStack : MonoBehaviour, I_BE2_BlocksStack
    {
        int _arrayLength;
        bool _isActive = false;

        public int Pointer { get; set; }
        public I_BE2_Instruction[] InstructionsArray { get; set; }
        public I_BE2_TargetObject TargetObject { get; set; }
        public I_BE2_Instruction TriggerInstruction { get; set; }
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (IsActive == false && value == true)
                {
                    // v2.11.2 - bugfix: blocks stack not starting from where they stopped when set "BE2_BlocksStack.IsActive = true"
                    if (_isStepPlay == false)
                    {
                        int instructionsCount = InstructionsArray.Length;
                        for (int i = 0; i < instructionsCount; i++)
                        {
                            InstructionsArray[i].InstructionBase.OnStackActive();
                        }
                    }

                    _isStepPlay = false;

                    // activate all shadows
                    foreach (I_BE2_Instruction instruction in InstructionsArray)
                    {
                        instruction.InstructionBase.Block.SetShadowActive(true);
                    }
                }
                else if (IsActive == true && value == false)
                {
                    // deactivate all shadows
                    foreach (I_BE2_Instruction instruction in InstructionsArray)
                    {
                        instruction.InstructionBase.Block.SetShadowActive(false);
                    }
                }

                _isActive = value;
            }
        }

        // v2.11 - events OnStackStart and OnStackEnd added to the BlocksStack to allow more control  
        public UnityEvent OnStackStart { get; set; } = new UnityEvent();
        public UnityEvent OnStackLastBlockExecuted { get; set; } = new UnityEvent();

        void Awake()
        {
            TriggerInstruction = GetComponent<I_BE2_Instruction>();
            IsActive = false;
        }

        void Start()
        {
            PopulateStack();
        }

        // v2.10 - bugfix: trigger blocks won't work if programmingEnv is disable and enabled again
        void OnEnable()
        {
            BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnPrimaryKeyUpEnd, PopulateStack);
            BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnStop, StopStack);
            if (TargetObject != null)
                BE2_ExecutionManager.Instance.AddToBlocksStackArray(this, TargetObject);
        }

        void OnDisable()
        {
            BE2_MainEventsManager.Instance.StopListening(BE2EventTypes.OnPrimaryKeyUpEnd, PopulateStack);
            BE2_MainEventsManager.Instance.StopListening(BE2EventTypes.OnStop, StopStack);
            // v2.10.2 - bugfix: BE2_ExecutionManager.Instance being null before it is called inside OnDisable when scene is closed
            BE2_ExecutionManager.Instance?.RemoveFromBlocksStackList(this);
        }

        void StopStack()
        {
            Pointer = 0;
            IsActive = false;
        }

        // void Update()
        // {

        // }

        public int OverflowGuard { get; set; }

        // v2.4 - Execute method of Blocks Stack refactored 
        public void Execute()
        {
            if (IsActive && _arrayLength > Pointer)
            {
                if (Pointer == 0)
                {
                    I_BE2_Block firstBlock = TriggerInstruction.InstructionBase.Block;
                    BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypesBlock.OnStackExecutionStart, firstBlock);
                }

                InstructionsArray[Pointer].Function();
                OverflowGuard = 0;
            }

            if (InstructionsArray != null && Pointer == InstructionsArray.Length && InstructionsArray.Length > 0)
            {
                I_BE2_Block lastBlock = InstructionsArray[InstructionsArray.Length - 1].InstructionBase.Block;
                BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypesBlock.OnStackExecutionEnd, lastBlock);

                Pointer = 0;
                IsActive = false;
            }
        }

        // v2.9 - added StepPlay and Pause methods to the BlockStack to play this BlocksStack step-by-step or pause the current full execution
        bool _isStepPlay = false;
        public bool IsStepPlay => _isStepPlay;
        public void StepPlay()
        {
            _isStepPlay = true;
            PopulateStack();
            _isActive = true;
        }

        public void Pause()
        {
            _isStepPlay = true;
        }

        public void PopulateStack()
        {
            InstructionsArray = new I_BE2_Instruction[0];
            PopulateStackRecursive(TriggerInstruction.InstructionBase.Block);
            _arrayLength = InstructionsArray.Length;
        }

        // v2.11 - PopulateStackRecursive refactored to improved way to run the block stack. The tigger block function is executed as the other block types,
        // making the conditional triggers blocks more efficient without delay when executed continuously
        void PopulateStackRecursive(I_BE2_Block parentBlock)
        {
            int locationsCount = 0;

            I_BE2_Instruction parentInstruction = parentBlock.Instruction;
            I_BE2_InstructionBase parentInstructionBase = parentInstruction as I_BE2_InstructionBase;
            parentInstructionBase.TargetObject = TargetObject;
            parentInstructionBase.BlocksStack = this;

            I_BE2_BlockSection[] tempSectionsArr = parentInstructionBase.Block.Layout.SectionsArray;
            parentInstructionBase.LocationsArray = new int[
                BE2_ArrayUtils.FindAll(ref tempSectionsArr, (x => x.Body != null)).Length + 1];

            InstructionsArray = BE2_ArrayUtils.AddReturn(InstructionsArray, parentInstruction);

            int sectionsCount = parentBlock.Layout.SectionsArray.Length;
            for (int i = 0; i < sectionsCount; i++)
            {
                I_BE2_BlockSection section = parentBlock.Layout.SectionsArray[i];
                if (section.Body != null)
                {

                    parentInstructionBase.LocationsArray[locationsCount] = InstructionsArray.Length;
                    locationsCount++;

                    section.Body.UpdateChildBlocksList();
                    I_BE2_Block[] childBlocks = section.Body.ChildBlocksArray;

                    int childBlocksCount = childBlocks.Length;
                    for (int j = 0; j < childBlocksCount; j++)
                    {
                        PopulateStackRecursive(childBlocks[j]);
                    }

                    InstructionsArray = BE2_ArrayUtils.AddReturn(InstructionsArray, parentInstruction);
                }
            }

            parentInstructionBase.LocationsArray[locationsCount] = InstructionsArray.Length;

            parentInstructionBase.PrepareToPlay();
        }
    }
}