Changelog                         {#changelog}
============

v2.11.2
- bugfix: blocks stack not starting from where they stopped when set "BE2_BlocksStack.IsActive = true"
- bugfix: blocks stack not starting from the beginning when execution was finished and new blocks are added

v2.11.1
- added new block events for drop actions: OnDropAtStack, OnDropAtInputSpot, OnDropAtProgrammingEnv, OnDropDestroy
- added a handler method on the BE2_DragDropManager.OnPointerUp to call the new Block drop events
- obsolete isVariable flag removed 
- bugfix: blocks stack not starting from the beginning when new blocks are added 

v2.11
- references to drag drop manager and execution manager refactored in drag scripts
- removed unecessary ChildBlocks list from Drag interface
- events OnStackStart and OnStackEnd added to the BlocksStack to allow more control 
- PopulateStackRecursive refactored to improved way to run the block stack. The tigger block function is executed as the other block types, making the conditional triggers blocks more efficient without delay when executed continuously
- removed the need to force BlocksStack to be added to the execution array using the MarkToAdd variable
- OnEnableInstruction and OnDisableInstruction virtual methods added to the instruction Base. Available to be used as needed but it is mainly used on trigger instructions with conditions that should be checked on Update
- WhenJoystickKeyPressed, WhenKeyPressed, WhenPlayClicked refactored to be compatible with the new way the trigger blocks are executed
- added method BE2_BlocksSerializer.BlocksCodeToXML to make it possible to save or send the code XML string without the need for generating a .BE2 file 
- BE2_BlocksSerializer.SaveCode refactored to use the BlocksCodeToXML method
- added method BE2_BlocksSerializer.XMLToBlocksCode to make it possible to load code from a XML string without the need for a .BE2 file 
- BE2_BlocksSerializer.LoadCode refactored to use the XMLToBlocksCode method
- dropdown arrow from the selection blocks made non raycast target to improve performance
- disable RectMask2D from the selection blocks to improve performance
- TMP text from the selection blocks made scaleStatic and non raycast target to improve performance
- added CallOnEndOfFrame(this MonoBehaviour, System.Action) method to the Utils class
- added DropTo method to the BE2_DragBlock and BE2_DragOperation classes 
- bugfix: trigger blocks loaded from the save menu didn't execut correctly if no PrimaryKey click was done before 

v2.10.2
- bugfix: BE2_ExecutionManager.Instance being null before it is called inside OnDisable when scene is closed
- bugfix: WebGL saves data not persisting after page reload
- bugfix: blocks that start inside the programming env with an operation block as input get no input spot back after dragging operation block out

v2.10.1
- bugfix: error when centering zoom without blocks in the ProgrammingEnv

v2.10
- Legacy Text, Dropdown and InputField components in scene and prefabs were replaced by their TextMeshPro corresponding ones
    (It is still possible to use legacy components and keep custom blocks without changin to TMP)
- BE2_Dropdown class added to enable the use of either Dropdown or Text Mesh Pro (TMP) component in the Blocks
- BE2_InputField class added to enable the use of either InputField or Text Mesh Pro (TMP) component in the Blocks
- Dropdown and InputField references replaced by BE2_Dropdown and BE2_InputField to enable the use of legacy or TMP components
- Dropdown and InputField references in the default instructions replaced by BE2_Dropdown and BE2_InputField to enable the use of legacy or TMP components as Block inputs
- Dropdown and InputField references in the block header inputs replaced by BE2_Dropdown and BE2_InputField to enable the use of legacy or TMP components
- adjusted to use BE2_InputField and BE2_Dropdown
- Added zoom feature to the ProgrammingEnv with the BE2_Zoom component
- custom scrollrect horizontal and vertical normal position are not reset on value change to enable zoom feature
- scales the new block to the programming env's zoom
- new events added to enable the use of an auxiliary key to change the programmingEnv zoom using key+scroll: OnSecondaryKeyUp, OnAuxKeyDown, OnAuxKeyUp
- added new possible key, auxKey0, to the input manager
- when a new block is added to the selection viewer, unnecessary components are removed to increase performance
- BE2_UI_SelectionBlock refactored to improve performance, blocks in the selection viewer are left with only needed components 
- selection blocks are cleared of unnecessary components 
- selection blocks are adjusted to have the same sizes of the corresponding block
- BE2_ArrayUtins FindAll and Find methods refactored to use System.Array class 
- BlockIsVariable method added to BE2_BlockUtils class to identify Variable blocks 
- dynamic inputfield resize called in coroutine to make sure it resizes correctly
- blocks selection viewer will only constantly call ForceLayoutRebuild during Unity edit mode
- section selection buttons don't need to have images in specific sibling positions
- avoid null error if section has no text title 
- BE2_Text isNull attribute made public
- bugfix: variable and list blocks not resizing on the selection panel
- bugfix: trigger blocks won't work if programmingEnv is disable and enabled again
- bugfix: update and fixedupdate events were being cleared after receiving listeners
- bugfix: hide programmingEnv on WebGl not working properly

v2.9
- Blocks layout update is now executed by the execution manager
- Block layout update refactored (executed as coroutine at the end of frame) to be executed by the execution manager
- added StepPlay and Pause methods to the BlockStack to play this BlocksStack step-by-step or pause the current full execution
- added BlocksStack editor script with inspector buttons for step-by-step play and pause 
- ExecuteSection and ExecuteNextInstruction refactored to enable StepPlay and Pause
- Execution manager now has OnUpdate event, by default used to execute the blocks stacks
- Execution manager now has OnLateUpdate event, by default used to execute the blocks layout update
- added possibility to run the block instructions in FixedUpdate by adding BE2_FIXED_UPDATE_INSTRUCTIONS scripting define symbol
- ExecuteInstructions method removed from the execution manager, calls are now managed using the OnUpdate event
- added List Blocks
- added scripts to manager lists as variables and BE2_VariablesListManager component added to the scenes
- variable managers now inherit from the interface I_BE2_VariablesManager to enable and facilitate new variable types
- attribute [SerializeAsVariable] is now mandatory for the dynamic variable operation blocks to be correcly serialized/deserialized and created by the correspondent Variables Manager
- BlockToSerializable refactored to enable and facilitate the addition of custom variable types
- SerializableToBlock refactored to enable and facilitate the addition of custom variable types
- isVariable flag (made obsolete) replaced by varManagerName field on the BE2_SerializableBlock to enable and facilitate the addition of custom variable types
- GetTargetObject method of the instruction base changed to UpdateTargetObject
- UpdateTargetObject method added to the InstructionBase interface
- ForceRebuildLayout method added to the BE2_UI_BlocksSelectionViewer to be called after resizing this panel-
- new blocks: List, AddToList, ClearList, InsertAtList, RemoveIndexFromList, RemoveValueFromList, ReplaceIndexOfList, IndexOfValueAtList, ListContains, ListLength, ValueAtIndexAtList
- audios changed from ogg to wav dues to Asset Store's requirements
- bugfix: TargetObject of blocks being null
- bugfix: changing the Canvas Render Mode needed recompiling to work correctly 
- bugfix: dropdown text being cropped on the Blocks Selection Panel
- bugfix: glitch on resizing the Blocks Selection Viewer

v2.8
- namespaces added to the project
- added needed namespaces to instructions and main namespaces to the isntruction template
- removed unused reference to Drag and Drop Manager from BE2_Block
- removed redundant AddSpotsToManager and RemoveSpotsFromManager methods from BE2_Block
- improve performance by using BE2_DragDropManager.Instance
- adjusted the SlideForward function so the TargetObject always end in the same position
- BE2_InputValues moved to its own script file
- added condition to verify if variable exists before creating new variables 
- added "remove variable" button to the variable viewer 
- adjusted variable viwer with "remove variable" button
- new variable viewers are now instantiated from prefab 
- bugfix: block not being added to the menu when building a new block
- bugfix: return block not stopping execution
- bugfix: operation block "Equal" not correctly comparing strings
- bugfix: float values breaking for different locales

v2.7.1
- bugfix: fixed BreakLoop block not working if played right after loading

v2.7
- added the BE2 Input Manager class to the system 
- BE2_DragDropManager refactored to use the BE2 Input Manager
- BE2_Pointer refactored to use the BE2 Input Manager
- saved codes path are now, by default, set to the "persistentDataPath" on Build. The setting "usePersistentPathOnBuild" can be set from the BE2 inspector 
- savedCodesPath variable moved from context menu manager to inspector 
- added public ProgrammingEnvsList property to the Execution Manager
- Execution Manager instance changed to property to guarantee return
- Execution Manager agregates the Pointer and Input Manager updates to be execute in the same update call to improve performance 
- UpdateProgrammingEnvsList method of Execution Manager made public
- OnPointerUpEnd event name changed to OnPrimaryKeyUpEnd
- added new events: OnPrimaryKeyDown, OnPrimaryKey, OnPrimaryKeyUp, OnSecondaryKeyDown, OnPrimaryKeyHold,
    OnDeleteKeyDown
- added method on the instruction to get the TargetObject (not null when block is placed in a ProgrammingEnv)
- added a class to the extras that implements the logic for show/hide the Blocks Selection panel  
- added BE2_BlockSectionHeader_Toggle input type for blocks (must be added manually, not yet available on the inspector's Block Builder)
- UpdateLayout method of the Selection Panel class made public

v2.6.2
- UI ContextMenuManager instance changed to property to avoid null exception
- path variables made not static
- bugfix: fixed changes on BE2 Inspector paths not perssiting 

v2.6.1
- Inspector made cleaner by moving template block parts to foldout
- bugfix: fixed Camera and Canvas Render Mode resetting  

v2.6
- added support for all the canvas render modes: Screen Space Overlay, Screen Space Canvas and World Space
- adjustments on position and angle of blocks and key components for supporting all canvas render modes
- BE2_DragDropManager using instance as property to guarantee return
- removed unused ProgrammingEnv property from the Drag and Drop Manager
- added property DragDropComponentsCanvas to the Drag and Drop Manager to be used as a reference Canvas 
- added property Instance in the BE2_Pointer
- Raycaster ray position adjusted base on the Canvas render mode
- added property Instance to BE2 Inspector
- New settings variables added to the BE2_Inspector to be used in the new section, "Scene Settings"
- added new inspection section "Scene Settings" containing the fields: Camera, Canvas Render Mode, Auto set Drag Drop Detection Distance, Drag Drop Detection Distance
- added BE2_Canvas component, used to identify all the BE2 key canvas
- changed way to find "bullet" child of Target Object
- new method added to Block Utils to get root parent block
- using BE2_Pointer as main pointer input source
- warningfix: added new modifier to all instructions that use inherited methods and properties to avoid warnings
- bugfix: fixed null exception when starting the scene with no Target Objects 
- bugfix: fixed operation blocks not using drag ad drop detection distance as parameter 
- bugfix: fixed null exception on play scene without target objects and programming envs
- bugfix: fixed intermittent null value on the BE2_UI_BlocksSelectionViewer's instance, now using property to guarantee return
- bugfix: fixed block being instantiated at wrong position on "Duplicate"

v2.5
- added a Programming Environment reference to the Target Object interface
- code documentation, added XML documentation to interfaces functions
- bugfix: fixed raycast bug that locked movement of blocks 

v2.4.1
- bugfix: fixed panel with wrong sprite associated covering the Programming Environment 
- bugfix: fixed Null Exception on opening the "Target Object and Programming Env" prefabs 

v2.4
- added new block "Return": Ends the current Blocks Stack 
- BE2 hierarcy changed to facilitate the use of multiple Target Objects + Programming Environments in the scene
- added prefab containing Target Object + Programming Environment that can be added dynamically to the scene
- added methods to add/remove raycasters from the BE2 Raycaster component
- added programming env check to the BE2_Raycaster to verify if the block is placed in a visible or hidden environment 
- Execute method of Blocks Stack refactored 
- added property to set visibility of Programming Environment, facilitates the use of multiple individualy programmable Target Objects in the same scene 
- added new method GetParentInstructionOfTypeAll to BE2_BlockUtils
- bugfix: fixed condition blocks not being reset on a loop break 
- bugfix: fixed blocks load in wrong position if resolution changes
- bugfix: fixed blocks selection panel not scrolling after block being dragged to ProgrammingEnv. Changed EnableScroll subscription to pointer up event from BE2_DragSelectionBlock and BE2_DragSelectionVariable to BE2_UI_BlocksSelectionViewer

v2.3
- added method SaveCode to facilitate the save of code by script
- added method LoadCode to facilitate the load of code by script
- method UpdateBlocksStackList from the Execution Manager made public
- new helper class to support the setting of needed paths
- added new inspector section "Paths Settings" to configure where to store new blocks (editor creation) and the user created codes (play mode)
- bigfix: fixed intermittent error "cannot change sibling OnDisable"
- bugfix: fixed blocks not being added to the selection menu atuomatically 

v2.2
- added optional max width to the dropdown input
- added optional max width to the text input
- bugfix: fixed empty blockString from XML file causing error on load
- using preferred width for selection panels

v2.1
# Needed action to update from v2.0: Remove the "Scripts/EngineCore/Core" folder before importing the new version

- moved blocks LayoutUpdate from Update to LateUpdate to avoid glitch on resizing 
- "Type" property of the I_BE2_Block made settable to fix and facilitate build of custom blocks
- detection of spot on drag of Block moved to coroutine and performed on end of frame to avoid glith on detecting new spot
- blocksStack array of the ExecutionManager made public
- BE2_EventsManager refactored to enable event types that pass I_BE2_Block when triggered
- previous BE2EtenvTypes misspell fixed to BE2EventTypes
- added new event types: OnBlocksStackArrayUpdate, OnStackExecutionStart, OnStackExecutionEnd
- Loops are now executed "in frame" instead of mandatorily "in update". Faster loop execution and nested loops without delay
- BE2_Text class added to enable the use of either Text or Text Mesh Pro (TMP) component to display text in the Blocks.
- bugfix: fixed detection of spots between child blocks before first block is dropped 
- bugfix: fixed destroying operations placed as inputs causing error 
- bugfix: fixed assigned wrong Type when building Loop Blocks
- bugfix: fixed variable blocks not updating dropdown when new variables were crated

v2.0
- Major Upgrade
- Full redesign and code
