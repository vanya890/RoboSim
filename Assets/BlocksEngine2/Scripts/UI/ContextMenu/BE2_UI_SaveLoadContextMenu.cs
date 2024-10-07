using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.DragDrop;
using MG_BlocksEngine2.Utils;
using MG_BlocksEngine2.Environment;
using MG_BlocksEngine2.Serializer;
using TMPro;

namespace MG_BlocksEngine2.UI
{
    public class BE2_UI_SaveLoadContextMenu : MonoBehaviour, I_BE2_UI_ContextMenu
    {
        string _codesPath;
        BE2_UI_ContextMenuManager _contextMenuManager;
        I_BE2_ProgrammingEnv _targetProgrammingEnv;
        BE2_DragDropManager _dragDropManager;

        public ToggleGroup scrollContentTG;
        public Toggle itemToggleTemplate;
        // v2.10 - changed from Inputfield to TMP_InputField
        public TMP_InputField fileInputField;
        public GameObject panelDefaultButtonsGO;
        public GameObject panelConfirmDeleteGO;
        public GameObject panelConfirmReplaceGO;
        // v2.1 - using BE2_Text to enable usage of Text or TMP components
        public BE2_Text Title { get; set; }

        void Awake()
        {
            _contextMenuManager = GetComponentInParent<BE2_UI_ContextMenuManager>();
            Title = BE2_Text.GetBE2Text(transform.GetChild(0));

            // v2.3 - using settable paths
            _codesPath = BE2_Paths.TranslateMarkupPath(BE2_Paths.SavedCodesPath);
        }

        void Start()
        {
            _dragDropManager = BE2_DragDropManager.Instance;
        }

        //void Update()
        //{
        //
        //}

        public void Open<T>(T target, params string[] options)
        {
            Awake();
            Start();

            _targetProgrammingEnv = target as I_BE2_ProgrammingEnv;

            panelDefaultButtonsGO.SetActive(true);
            panelConfirmDeleteGO.SetActive(false);
            panelConfirmReplaceGO.SetActive(false);

            RefreshScrollValues();

            gameObject.SetActive(true);
        }

        public void Close()
        {
            _targetProgrammingEnv = null;
            gameObject.SetActive(false);
        }

        void RefreshScrollValues()
        {
            for (int i = itemToggleTemplate.transform.parent.childCount - 1; i > 0; i--)
            {
                Destroy(itemToggleTemplate.transform.parent.GetChild(i).gameObject);
            }

            DirectoryInfo dataDir = new DirectoryInfo(_codesPath);
            if (!Directory.Exists(_codesPath))
            {
                Directory.CreateDirectory(_codesPath);
            }
            try
            {
                FileInfo[] fileinfo = dataDir.GetFiles();

                for (int i = 0; i < fileinfo.Length; i++)
                {
                    if (fileinfo[i].Extension.ToLower() == ".be2")
                    {
                        string name = fileinfo[i].Name;
                        GameObject toggleGO = Instantiate(itemToggleTemplate.gameObject) as GameObject;
                        Toggle toggle = toggleGO.GetComponent<Toggle>();

                        // v2.1 - using BE2_Text to enable usage of Text or TMP components
                        BE2_Text.GetBE2TextInChildren(toggle.transform).text = name;

                        toggle.transform.SetParent(itemToggleTemplate.transform.parent);
                        toggle.transform.localScale = Vector3.one;
                        toggle.group = itemToggleTemplate.transform.parent.GetComponent<ToggleGroup>();
                        toggle.transform.SetAsLastSibling();
                        toggleGO.SetActive(true);

                        // v2.6 - adjustments on position and angle of key components for supporting all canvas render modes 
                        toggle.transform.localPosition = new Vector3(toggle.transform.localPosition.x, toggle.transform.localPosition.y, 0);
                        toggle.transform.localEulerAngles = Vector3.zero;

                        toggle.onValueChanged.AddListener(delegate
                        {
                            // v2.1 - using BE2_Text to enable usage of Text or TMP components
                            fileInputField.text = BE2_Text.GetBE2TextInChildren(toggle.transform).text;
                        });
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
        }

        public void Save()
        {
            BE2_BlocksSerializer.SaveCode(GetFullPath(), _targetProgrammingEnv);

            RefreshScrollValues();

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif

            CancelConfirm();
        }

        string GetFileName()
        {
            string fileName = fileInputField.text;

            if (fileName.Length >= 4)
            {
                if (fileName.Substring(fileName.Length - 4, 4).ToLower() == ".be2")
                {
                    fileName = fileName.Substring(0, fileName.Length - 4);
                }
            }

            return Regex.Replace(fileName, "[^\\w\\._]", "");
        }

        string GetFullPath()
        {
            return _codesPath + GetFileName() + ".BE2";
        }

        public void ConfirmSave()
        {
            if (fileInputField.text != "")
            {
                if (File.Exists(GetFullPath()) == false)
                {
                    Save();
                }
                else
                {
                    panelDefaultButtonsGO.SetActive(false);
                    panelConfirmReplaceGO.SetActive(true);
                }
            }
        }

        public void Load()
        {
            bool loaded = BE2_BlocksSerializer.LoadCode(GetFullPath(), _targetProgrammingEnv);
            if (loaded)
            {
                _contextMenuManager.CloseContextMenu();
            }
        }

        public void Delete()
        {
            File.Delete(GetFullPath());

            RefreshScrollValues();

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif

            CancelConfirm();
        }

        public void ConfirmDelete()
        {
            if (File.Exists(GetFullPath()))
            {
                panelDefaultButtonsGO.SetActive(false);
                panelConfirmDeleteGO.SetActive(true);
            }
        }

        public void Cancel()
        {
            _contextMenuManager.CloseContextMenu();
        }

        public void CancelConfirm()
        {
            panelConfirmDeleteGO.SetActive(false);
            panelConfirmReplaceGO.SetActive(false);
            panelDefaultButtonsGO.SetActive(true);
        }
    }
}
