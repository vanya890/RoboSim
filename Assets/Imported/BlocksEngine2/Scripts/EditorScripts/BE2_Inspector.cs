using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.UI;
using MG_BlocksEngine2.Utils;
using MG_BlocksEngine2.Environment;

namespace MG_BlocksEngine2.EditorScript
{
    public class BE2_Inspector : MonoBehaviour
    {
        // v2.6 - added property Instance to BE2 Inspector
        static BE2_Inspector _instance;
        public static BE2_Inspector Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GameObject.FindObjectOfType<BE2_Inspector>();
                }
                return _instance;
            }
            set => _instance = value;
        }

        public GameObject BlockTemplate;
        public GameObject SimpleTemplate;
        public GameObject TriggerTemplate;
        public GameObject OperationTemplate;
        public GameObject SectionTemplate;
        public GameObject HeaderTemplate;
        public GameObject HeaderMiddleTemplate;
        public GameObject BodyEndTemplate;
        public GameObject BodyMiddleTemplate;
        public GameObject OuterAreaTemplate;
        public GameObject DropdownTemplate;
        public GameObject InputFieldTemplate;
        public GameObject LabelTextTemplate;
        public string instructionName;
        public BlockTypeEnum blockType;
        public Color blockColor;
        public string blockHeaderMarkup;
        public string[] inputValues;

        // v2.6.2 - bugfix: fixed changes on BE2 Inspector paths not perssiting 
        // path variables made not static 
        public string newInstructionPath = "[dataPath]/BlocksEngine2/Scripts/EngineCore/Instruction/BlockInstructions/Custom/";
        public string newBlockPrefabPath = "Assets/BlocksEngine2/Prefabs/Resources/Blocks/Custom/";

        // v2.7 - saved codes path are now, by default, set to the "persistentDataPath" on Build. The setting "usePersistentPathOnBuild" can be set from the BE2 inspector 
        public bool usePersistentPathOnBuild = true;
        // v2.7 - savedCodesPath variable moved from context menu manager to inspector 
        // v2.6.2 - bugfix: fixed changes on BE2 Inspector paths not perssiting 
        // path variables made not static
        public string savedCodesPath = "[dataPath]/BlocksEngine2/Saves/";

        // v2.6.1 - bugfix: fixed Camera and Canvas Render Mode resetting  
        // v2.6 - New settings variables added to the BE2_Inspector to be used in the new section, "Scene Settings"
        [SerializeField]
        Camera _camera;
        public Camera Camera
        {
            get
            {
                if (!_camera)
                {
                    _camera = Camera.main;
                }
                return _camera;
            }
            set => _camera = value;
        }
        [SerializeField]
        RenderMode _canvasRenderMode;
        public RenderMode CanvasRenderMode
        {
            get => _canvasRenderMode;
            set
            {
                BE2_Canvas[] be2CanvasArray = FindObjectsOfType<BE2_Canvas>();
                foreach (BE2_Canvas be2Canvas in be2CanvasArray)
                {
                    be2Canvas.Canvas.renderMode = value;
                    be2Canvas.Canvas.worldCamera = Camera;
                    be2Canvas.Canvas.planeDistance = 10;
                }
                _canvasRenderMode = value;
            }
        }
        public bool autoSetDragDropDetectionDistance = true;

        public void TryAddInstructionToBlock(string className, GameObject blockGO)
        {
            if (TryGetType(className) == null)
            {
                // no class
                Debug.Log("- Instruction not added");
            }
            else
            {
                // add instruction
                blockGO.AddComponent(TryGetType(className));
                Debug.Log("+ Instruction added");
            }
        }

        /// <summary>
        /// Try to get type from assemblies.
        /// From: https://stackoverflow.com/a/11811046
        /// </summary>
        /// <param name="typeName">Type name</param>
        /// <returns>Type or null</returns>
        public System.Type TryGetType(string typeName)
        {
            var type = System.Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }

        public void AddBlockToSelectionMenu(Transform blockTransform)
        {
            StartCoroutine(C_AddBlockToSelectionMenu(blockTransform));
        }
        IEnumerator C_AddBlockToSelectionMenu(Transform blockTransform)
        {
            // v2.8 - bugfix: block not being added to the menu when building a new block
            // Previously "return null" used on v2.3 was changed back to "return WaitForEndOfFrame" due to this persistent error.
            // Feel free to use the v2.3 line or "return WaitForSeconds" if needed.
            yield return new WaitForEndOfFrame();
            // yield return null;

            BE2_UI_BlocksSelectionViewer blocksSelectionViewer = FindObjectOfType<BE2_UI_BlocksSelectionViewer>();
            blocksSelectionViewer.UpdateSelectionPanels();
            blocksSelectionViewer.AddBlockToPanel(blockTransform, blocksSelectionViewer.selectionPanelsList[blocksSelectionViewer.selectionPanelsList.Count - 1]);
        }

        public Transform BuildAndInstantiateBlock(string instructionName)
        {
            GameObject newBlockGO;
            if (blockType == BlockTypeEnum.simple)
            {
                newBlockGO = Instantiate(SimpleTemplate, new Vector3(219, -219, 0), Quaternion.identity, FindObjectOfType<BE2_ProgrammingEnv>().transform);
            }
            else if (blockType == BlockTypeEnum.trigger)
            {
                newBlockGO = Instantiate(TriggerTemplate, new Vector3(219, -219, 0), Quaternion.identity, FindObjectOfType<BE2_ProgrammingEnv>().transform);
            }
            else if (blockType == BlockTypeEnum.operation)
            {
                newBlockGO = Instantiate(OperationTemplate, new Vector3(219, -219, 0), Quaternion.identity, FindObjectOfType<BE2_ProgrammingEnv>().transform);
            }
            else
            {
                newBlockGO = Instantiate(BlockTemplate, new Vector3(219, -219, 0), Quaternion.identity, FindObjectOfType<BE2_ProgrammingEnv>().transform);
            }
            newBlockGO.name = "Block Cst " + instructionName;
            newBlockGO.transform.localPosition = new Vector3(219, -219, 0);

            Transform newBlockTransform = newBlockGO.transform;
            I_BE2_Block newBlock = newBlockGO.GetComponent<I_BE2_Block>();
            I_BE2_BlockLayout newBlockLayout = newBlockGO.GetComponent<I_BE2_BlockLayout>();

            // v2.1 - bugfix: fixed assigned wrong Type when building Loop Blocks
            if (blockType == BlockTypeEnum.loop)
            {
                newBlock.Type = BlockTypeEnum.loop;
            }

            newBlockLayout.Color = blockColor;

            string[] headersMarkup = blockHeaderMarkup.Split('\n');
            int inputValuesIndex = 0;
            for (int h = 0; h < headersMarkup.Length; h++)
            {
                // ### create section and header ###
                GameObject section;
                GameObject header;
                if (h == 0)
                {
                    section = newBlockTransform.GetChild(0).gameObject;
                    header = section.transform.GetChild(0).gameObject;
                }
                else
                {
                    section = Instantiate(SectionTemplate, Vector3.zero, Quaternion.identity, newBlockTransform);
                    header = Instantiate(HeaderMiddleTemplate, Vector3.zero, Quaternion.identity, section.transform);
                }

                // ### create body if needed ###
                GameObject body;
                if (blockType == BlockTypeEnum.condition || blockType == BlockTypeEnum.loop)
                {
                    if (h == headersMarkup.Length - 1) // last
                        body = Instantiate(BodyEndTemplate, Vector3.zero, Quaternion.identity, section.transform);
                    else
                        body = Instantiate(BodyMiddleTemplate, Vector3.zero, Quaternion.identity, section.transform);
                }

                // parse items
                List<string> items = new List<string>();
                string tempString = "";
                bool input = false;
                for (int i = 0; i < headersMarkup[h].Length; i++)
                {
                    if (headersMarkup[h][i] == '$')
                    {
                        items.Add(tempString);
                        tempString = "";
                        input = true;
                    }
                    if (input && headersMarkup[h][i] == ' ')
                    {
                        items.Add(tempString);
                        tempString = "";
                        input = false;
                    }

                    tempString += headersMarkup[h][i];
                }
                items.Add(tempString);

                // ### create header items ###
                for (int i = 0; i < items.Count; i++)
                {
                    // v2.10 - adjusted to use BE2_InputField and BE2_Dropdown
                    GameObject item = null;
                    if (items[i] == "$text")
                    {
                        item = Instantiate(InputFieldTemplate, Vector3.zero, Quaternion.identity, header.transform);
                        BE2_InputField inputField = BE2_InputField.GetBE2Component(item.transform);
                        inputField.text = inputValues[inputValuesIndex].TrimEnd(' ').TrimStart(' ');
                        inputValuesIndex++;
                    }
                    else if (items[i] == "$dropdown")
                    {
                        item = Instantiate(DropdownTemplate, Vector3.zero, Quaternion.identity, header.transform);
                        BE2_Dropdown dropdown = BE2_Dropdown.GetBE2Component(item.transform);
                        dropdown.ClearOptions();
                        foreach (string v in inputValues[inputValuesIndex].Split(','))
                        {
                            dropdown.AddOption(v.TrimEnd(' ').TrimStart(' '));
                        }
                        inputValuesIndex++;
                    }
                    else
                    {
                        item = Instantiate(LabelTextTemplate, Vector3.zero, Quaternion.identity, header.transform);

                        // v2.1 - using BE2_Text to enable usage of Text or TMP components
                        BE2_Text.GetBE2Text(item.transform).text = items[i].TrimEnd(' ').TrimStart(' ');
                    }

                }

            }

            // ### create outer area if needed ###
            GameObject outerArea;
            if (blockType == BlockTypeEnum.condition || blockType == BlockTypeEnum.loop)
                outerArea = Instantiate(OuterAreaTemplate, Vector3.zero, Quaternion.identity, newBlockTransform);

            newBlockGO.GetComponent<I_BE2_BlockLayout>().UpdateLayout();

            Debug.Log("+ Block created");

            return newBlockTransform;
        }

        public List<int> AllIndexesOf(string str, string value)
        {
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        public string CreateInstructionScript(string instructionName)
        {
            Debug.Log("+ Start creating instruction");
            // v2.3 - using settable paths
            string instructionsPath = BE2_Paths.TranslateMarkupPath(BE2_Paths.NewInstructionPath);

            var sr = new StreamReader(Application.dataPath + "/BlocksEngine2/Scripts/EditorScripts/InstructionScriptTemplate.txt");
            var fileContents = sr.ReadToEnd();
            sr.Close();

            string[] lines = fileContents.Split("\n"[0]);

            string className = "BE2_Cst_" + instructionName;

            string fullPath = instructionsPath + className + ".cs";

            if (File.Exists(fullPath) == false)
            {
                using (StreamWriter file =
                    new StreamWriter(fullPath))
                {
                    foreach (string line in lines)
                    {
                        string toWrite = line;
                        if (line.Contains("[instructionName]"))
                            toWrite = toWrite.Replace("[instructionName]", className);

                        file.WriteLine(toWrite);
                    }
                }//File written
            }
            else
            {
                Debug.Log("- Instruction already exists");
            }

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif

            return className;
        }
    }

}