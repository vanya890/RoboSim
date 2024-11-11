using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.DragDrop;
using MG_BlocksEngine2.Utils;

namespace MG_BlocksEngine2.EditorScript
{
    [CustomEditor(typeof(BE2_Inspector))]
    public class BE2_Editor_Inspector : Editor
    {
        BE2_Inspector inspector;

        void OnEnable()
        {
            inspector = (BE2_Inspector)target;

            if (inspector.inputValues == null)
                inspector.inputValues = new string[0];
        }

        bool tryAddInstruction = false;
        string instructionCompleteName = "";
        bool _showTemplateBlockParts;
        Transform newBlockTransform = null;

        public override void OnInspectorGUI()
        {
            DrawSeparator();
            EditorGUILayout.LabelField("Block Builder", EditorStyles.boldLabel);

            inspector.instructionName = EditorGUILayout.TextField("Instruction Name", inspector.instructionName);
            inspector.blockType = (BlockTypeEnum)EditorGUILayout.EnumPopup("Block Type", inspector.blockType);
            inspector.blockColor = EditorGUILayout.ColorField("Blcok Color", inspector.blockColor);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Block Header Markup");
            EditorGUILayout.HelpBox(
                "Write the header text and inputs in a single line. Additional headers in new lines. \n" +
                "possible input types are $text or $dropdown."
                , MessageType.Info);

            inspector.blockHeaderMarkup = EditorGUILayout.TextArea(inspector.blockHeaderMarkup, GUILayout.MaxHeight(75));

            if (inspector.blockHeaderMarkup != "")
            {
                EditorGUILayout.HelpBox(
                "Below you can define the text $input default value or $dropdown option values separated by comma."
                , MessageType.Info);
                List<int> inputMarkIndexes = inspector.AllIndexesOf(inspector.blockHeaderMarkup, "$text");
                inputMarkIndexes.AddRange(inspector.AllIndexesOf(inspector.blockHeaderMarkup, "$dropdown"));
                BE2_ArrayUtils.Resize(ref inspector.inputValues, inputMarkIndexes.Count);
                for (int i = 0; i < inputMarkIndexes.Count; i++)
                {
                    inspector.inputValues[i] = EditorGUILayout.TextField("$input " + i, inspector.inputValues[i]);
                }
            }

            if (GUILayout.Button("Build Block"))
            {
                // ### instantiate block ###
                newBlockTransform = inspector.BuildAndInstantiateBlock(inspector.instructionName);

                // ### create instruction ###
                instructionCompleteName = inspector.CreateInstructionScript(inspector.instructionName);
                tryAddInstruction = true;
            }
            if (tryAddInstruction)
            {
                if (!EditorApplication.isCompiling)
                {
                    Debug.Log("+ End creating instruction");
                    tryAddInstruction = false;
                    // ### add instruction to block ###
                    inspector.TryAddInstructionToBlock(instructionCompleteName, newBlockTransform.gameObject);

                    // ### Add block to selection menu ###
                    inspector.AddBlockToSelectionMenu(newBlockTransform);
                }
            }

            DrawSeparator();
            // v2.3 - added new inspector section "Paths Settings" to configure where to store new blocks (editor creation) and the user created codes (play mode)
            EditorGUILayout.LabelField("Paths Settings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "You can use [dataPath] or [persistentDataPath] in the beginning of the path."
                , MessageType.Info);

            EditorGUILayout.LabelField("Block Builder", EditorStyles.miniBoldLabel);
            BE2_Paths.NewInstructionPath = EditorGUILayout.TextField("New Instruction Path", BE2_Paths.NewInstructionPath);
            if (!BE2_Paths.NewBlockPrefabPath.ToLower().Contains("resources"))
            {
                EditorGUILayout.HelpBox(
                    "The New Block Prefab must be saved inside a resources folder."
                    , MessageType.Warning);
            }
            BE2_Paths.NewBlockPrefabPath = EditorGUILayout.TextField("New Block Prefab Path", BE2_Paths.NewBlockPrefabPath);

            EditorGUILayout.LabelField("Saved/Load Codes", EditorStyles.miniBoldLabel);
            inspector.usePersistentPathOnBuild = EditorGUILayout.Toggle("Use Persistent Data Path on Build", inspector.usePersistentPathOnBuild);
            BE2_Paths.SavedCodesPath = EditorGUILayout.TextField("Saved Codes Path", BE2_Paths.SavedCodesPath);

            DrawSeparator();

            // v2.6 - added new inspection section "Scene Settings" containing the fields: Camera, Canvas Render Mode, Auto set Drag Drop Detection Distance, Drag Drop Detection Distance
            EditorGUILayout.LabelField("Scene Settings", EditorStyles.boldLabel);
            inspector.Camera = (Camera)EditorGUILayout.ObjectField("Camera", inspector.Camera, typeof(Camera), true);
            EditorGUILayout.HelpBox(
                "Changes on Canvas Render Mode will be applied during Editor mode too."
                , MessageType.Warning);
            inspector.CanvasRenderMode = (RenderMode)EditorGUILayout.EnumPopup("Canvas Render Mode", inspector.CanvasRenderMode);
            inspector.autoSetDragDropDetectionDistance = EditorGUILayout.Toggle("Auto set Drag Drop Detection Distance", inspector.autoSetDragDropDetectionDistance);
            if (inspector.autoSetDragDropDetectionDistance)
            {
                if (inspector.CanvasRenderMode == RenderMode.ScreenSpaceOverlay)
                    BE2_DragDropManager.Instance.detectionDistance = 40f;
                else
                    BE2_DragDropManager.Instance.detectionDistance = 0.5f;
            }
            BE2_DragDropManager.Instance.detectionDistance = EditorGUILayout.FloatField("Drag Drop Detection Distance", BE2_DragDropManager.Instance.detectionDistance);

            EditorGUILayout.LabelField("Run Instructions on FixedUpdate");
            EditorGUILayout.HelpBox(
                    "To run the instructions on FixedUpdate instead of Update, go to:\n" +
                    "'Edit > Project Settings... > Player > Other Settings > Scripting Define Symbols'\n" +
                    "and add the following define 'BE2_FIXED_UPDATE_INSTRUCTIONS'."
                    , MessageType.Info);

            DrawSeparator();

            // v2.6.1 - Inspector made cleaner by moving template block parts to foldout
            _showTemplateBlockParts = EditorGUILayout.BeginFoldoutHeaderGroup(_showTemplateBlockParts, "Template Block Parts");
            if (_showTemplateBlockParts)
            {
                inspector.BlockTemplate = (GameObject)EditorGUILayout.ObjectField("Block Template", inspector.BlockTemplate, typeof(GameObject), false);
                inspector.SimpleTemplate = (GameObject)EditorGUILayout.ObjectField("Simple Template", inspector.SimpleTemplate, typeof(GameObject), false);
                inspector.TriggerTemplate = (GameObject)EditorGUILayout.ObjectField("Trigger Template", inspector.TriggerTemplate, typeof(GameObject), false);
                inspector.OperationTemplate = (GameObject)EditorGUILayout.ObjectField("Operation Template", inspector.OperationTemplate, typeof(GameObject), false);
                inspector.SectionTemplate = (GameObject)EditorGUILayout.ObjectField("Section Template", inspector.SectionTemplate, typeof(GameObject), false);
                inspector.HeaderTemplate = (GameObject)EditorGUILayout.ObjectField("Block Header", inspector.HeaderTemplate, typeof(GameObject), false);
                inspector.HeaderMiddleTemplate = (GameObject)EditorGUILayout.ObjectField("Block Middle Header", inspector.HeaderMiddleTemplate, typeof(GameObject), false);
                inspector.BodyMiddleTemplate = (GameObject)EditorGUILayout.ObjectField("Block Middle Body", inspector.BodyMiddleTemplate, typeof(GameObject), false);
                inspector.BodyEndTemplate = (GameObject)EditorGUILayout.ObjectField("Block End Body", inspector.BodyEndTemplate, typeof(GameObject), false);
                inspector.OuterAreaTemplate = (GameObject)EditorGUILayout.ObjectField("Block Outer Area", inspector.OuterAreaTemplate, typeof(GameObject), false);
                inspector.DropdownTemplate = (GameObject)EditorGUILayout.ObjectField("Dropdown Template", inspector.DropdownTemplate, typeof(GameObject), false);
                inspector.InputFieldTemplate = (GameObject)EditorGUILayout.ObjectField("InputField Template", inspector.InputFieldTemplate, typeof(GameObject), false);
                inspector.LabelTextTemplate = (GameObject)EditorGUILayout.ObjectField("LabelText Template", inspector.LabelTextTemplate, typeof(GameObject), false);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void DrawSeparator()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            DrawThinSeparator();
            EditorGUILayout.Space();
        }

        void DrawThinSeparator()
        {
            var rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.width + 15, rect.y));
            EditorGUILayout.EndHorizontal();
        }

        void DrawVerticalSeparator()
        {
            var rect = EditorGUILayout.BeginVertical();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x, rect.yMin));
            EditorGUILayout.EndVertical();
        }
    }
}