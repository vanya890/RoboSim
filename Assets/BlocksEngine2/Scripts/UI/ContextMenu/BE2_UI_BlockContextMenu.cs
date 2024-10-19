using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.DragDrop;
using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.Utils;

namespace MG_BlocksEngine2.UI
{
    public class BE2_UI_BlockContextMenu : MonoBehaviour, I_BE2_UI_ContextMenu
    {
        BE2_UI_ContextMenuManager _contextMenuManager;
        I_BE2_Block _targetBlock;
        BE2_DragDropManager _dragDropManager;

        // v2.1 - using BE2_Text to enable usage of Text or TMP components
        public BE2_Text Title { get; set; }

        void Awake()
        {
            _contextMenuManager = GetComponentInParent<BE2_UI_ContextMenuManager>();
            Title = BE2_Text.GetBE2Text(transform.GetChild(0));
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

            _targetBlock = target as I_BE2_Block;

            Title.text = _targetBlock.Instruction.GetType().ToString().Split('_')[2];
            transform.position = BE2_Pointer.Instance.transform.position;

            gameObject.SetActive(true);
        }

        public void Close()
        {
            _targetBlock = null;
            gameObject.SetActive(false);
        }

        public void Duplicate()
        {
            BE2_BlockUtils.DuplicateBlock(_targetBlock);
            _contextMenuManager.CloseContextMenu();
        }

        public void Delete()
        {
            BE2_BlockUtils.RemoveBlock(_targetBlock);
            _contextMenuManager.CloseContextMenu();
        }

        public void Cancel()
        {
            _contextMenuManager.CloseContextMenu();
        }
    }
}