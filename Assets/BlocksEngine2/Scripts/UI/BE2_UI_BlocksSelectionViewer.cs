using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.Utils;
using MG_BlocksEngine2.Core;
using UnityEditor;

namespace MG_BlocksEngine2.UI
{
    [ExecuteInEditMode]
    public class BE2_UI_BlocksSelectionViewer : MonoBehaviour
    {
        // v2.6 - bugfix: fixed intermittent null value on the BE2_UI_BlocksSelectionViewer's instance, now using property to guarantee return
        static BE2_UI_BlocksSelectionViewer _instance;
        public static BE2_UI_BlocksSelectionViewer Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GameObject.FindObjectOfType<BE2_UI_BlocksSelectionViewer>();
                }
                return _instance;
            }
            set => _instance = value;
        }

        public List<BE2_UI_SelectionPanel> selectionPanelsList;
        [Header("Add Block To Panel")]
        public Transform blockToAddTransform;
        public int panelIndex;
        public bool addBlock = false;

        // v2.4 - bugfix: fixed blocks selection panel not scrolling after block being dragged to ProgrammingEnv
        ScrollRect _scrollRect;

        void Awake()
        {
            Instance = this;
            selectionPanelsList = new List<BE2_UI_SelectionPanel>();

            _scrollRect = GetComponent<ScrollRect>();
        }

        void Start()
        {
            selectionPanelsList.AddRange(GetComponentsInChildren<BE2_UI_SelectionPanel>());
        }

#if UNITY_EDITOR
        void Update()
        {
            if (addBlock)
            {
                AddBlockToPanel(blockToAddTransform, selectionPanelsList[panelIndex]);
                addBlock = false;
            }
            // v2.10 - blocks selection viewer will only constantly call ForceLayoutRebuild during Unity edit mode
            if (!EditorApplication.isPlaying)
            {
                ForceRebuildLayout();
            }
        }
#endif

        public void UpdateSelectionPanels()
        {
            selectionPanelsList = new List<BE2_UI_SelectionPanel>();
            selectionPanelsList.AddRange(GetComponentsInChildren<BE2_UI_SelectionPanel>());
        }

        public void AddBlockToPanel(Transform blockTransform, BE2_UI_SelectionPanel selectionPanel)
        {
            Transform blockCopy = Instantiate(blockTransform, Vector3.zero, Quaternion.identity, selectionPanel.transform);
            blockCopy.name = blockCopy.name.Replace("(Clone)", "");

            BE2_BlockUtils.RemoveEngineComponents(blockCopy);
            BE2_BlockUtils.AddSelectionMenuComponents(blockCopy);
            Debug.Log("+ Block added to selection menu");

            GameObject prefabBlock = BE2_BlockUtils.CreatePrefab(blockTransform.GetComponent<I_BE2_Block>());
            BE2_UI_SelectionBlock selectionBlock = blockCopy.GetComponent<BE2_UI_SelectionBlock>();
            selectionBlock.prefabBlock = prefabBlock;

            // v2.10 - when a new block is added to the selection viewer, unnecessary components are removed to increase performance
            selectionBlock.PerformCleanAndResize();

            Debug.Log("+ Block prefab created");
        }

        // v2.4 - bugfix: fixed blocks selection panel not scrolling after block being dragged to ProgrammingEnv.
        //                Changed EnableScroll subscription to pointer up event from BE2_DragSelectionBlock and BE2_DragSelectionVariable to BE2_UI_BlocksSelectionViewer
        void OnEnable()
        {
            BE2_MainEventsManager.Instance.StartListening(BE2EventTypes.OnPrimaryKeyUpEnd, EnableScroll);
        }

        void OnDisable()
        {
            BE2_MainEventsManager.Instance.StopListening(BE2EventTypes.OnPrimaryKeyUpEnd, EnableScroll);
        }

        void EnableScroll()
        {
            _scrollRect.enabled = true;
        }

        // v2.9 - ForceRebuildLayout method added to the BE2_UI_BlocksSelectionViewer to be called after resizing this panel
        public void ForceRebuildLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        }
    }
}
