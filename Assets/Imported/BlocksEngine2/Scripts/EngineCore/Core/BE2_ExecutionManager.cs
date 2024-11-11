using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using MG_BlocksEngine2.DragDrop;
using MG_BlocksEngine2.Core;
using MG_BlocksEngine2.Environment;
using MG_BlocksEngine2.Utils;

namespace MG_BlocksEngine2.Core
{
    public class BE2_ExecutionManager : MonoBehaviour
    {
        List<I_BE2_TargetObject> _targetObjectsList;

        List<I_BE2_ProgrammingEnv> _programmingEnvsList;
        // v2.7 - added public ProgrammingEnvsList property to the Execution Manager
        public List<I_BE2_ProgrammingEnv> ProgrammingEnvsList => _programmingEnvsList;
        // v2.1 - blocksStack array of the ExecutionManager made public
        public I_BE2_BlocksStack[] blocksStacksArray;

        // v2.7 - Execution Manager instance changed to property to guarantee return
        static BE2_ExecutionManager _instance;
        public static BE2_ExecutionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<BE2_ExecutionManager>();
                }
                return _instance;
            }
            set => _instance = value;
        }

        // system components
        BE2_Pointer _pointer;
        I_BE2_InputManager _inputManager;

        // v2.9 - added list of actions executed by the Execution Manager
        List<UnityAction> _actions = new List<UnityAction>();

        // v2.9 - Execution manager now has OnUpdate event, by default used to execute the blocks stacks
        UnityEvent OnUpdate = new UnityEvent();
        public void AddToUpdate(UnityAction action)
        {
            if (!_actions.Contains(action))
            {
                OnUpdate.AddListener(action);
                _actions.Add(action);
            }
        }
        public void RemoveFromUpdate(UnityAction action)
        {
            if (_actions.Contains(action))
            {
                OnUpdate.RemoveListener(action);
                _actions.Remove(action);
            }
        }

        // v2.9 - Execution manager now has OnLateUpdate event, by default used to execute the blocks layout update
        UnityEvent OnLateUpdate = new UnityEvent();
        public void AddToLateUpdate(UnityAction action)
        {
            if (!_actions.Contains(action))
            {
                OnLateUpdate.AddListener(action);
                _actions.Add(action);
            }
        }
        public void RemoveFromLateUpdate(UnityAction action)
        {
            if (_actions.Contains(action))
            {
                OnLateUpdate.RemoveListener(action);
                _actions.Remove(action);
            }
        }

        void Awake()
        {
            _pointer = BE2_Pointer.Instance;
            _inputManager = BE2_InputManager.Instance;

            UpdateTargetObjects();
            UpdateProgrammingEnvsList();
            Instance = this;
        }

        void Start()
        {
            // v2.10 - bugfix: update and fixedupdate events were being cleared after receiving listeners
            UpdateBlocksStackList();
        }

        void Update()
        {
            // v2.7 - Execution Manager agregates the Pointer and Input Manager updates to be execute in the same update call to improve performance 
            _pointer.OnUpdate();
            _inputManager.OnUpdate();

            // v2.9 - added possibility to run the block instructions in FixedUpdate by adding BE2_FIXED_UPDATE_INSTRUCTIONS scripting define symbol 
#if !BE2_FIXED_UPDATE_INSTRUCTIONS

            // v2.9 - ExecuteInstructions method removed from the execution manager, previous calls are now managed using the OnUpdate event
            OnUpdate.Invoke();
# endif
        }

#if BE2_FIXED_UPDATE_INSTRUCTIONS
        void FixedUpdate()
        {
            OnUpdate.Invoke();
        }
#endif

        // v2.9 - LateUpdate calls are now managed by the ExecutionManager
        void LateUpdate()
        {
            OnLateUpdate.Invoke();
        }

        public void Play()
        {
            BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnPlay);
            EventSystem.current.SetSelectedGameObject(null);
        }

        public void Stop()
        {
            BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnStop);
            EventSystem.current.SetSelectedGameObject(null);
        }

        // v2.3 - method UpdateBlocksStackList from the Execution Manager made public
        public void UpdateBlocksStackList()
        {
            blocksStacksArray = new I_BE2_BlocksStack[0];
            int envsCount = _programmingEnvsList.Count;
            for (int i = 0; i < envsCount; i++)
            {
                I_BE2_ProgrammingEnv programmingEnv = _programmingEnvsList[i];

                int childCount = programmingEnv.Transform.childCount;
                for (int j = 0; j < childCount; j++)
                {
                    I_BE2_BlocksStack blocksStack = programmingEnv.Transform.GetChild(j).GetComponent<I_BE2_BlocksStack>();
                    if (blocksStack != null)
                    {
                        BE2_ArrayUtils.Add(ref blocksStacksArray, blocksStack);
                        blocksStack.TargetObject = programmingEnv.TargetObject;

                        // v2.9 - BlocksStack Execute action is now executed from the OnUpdate event
                        AddToUpdate(blocksStack.Execute);
                    }
                }
            }

            BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnBlocksStackArrayUpdate);
        }

        public void AddToBlocksStackArray(I_BE2_BlocksStack blocksStack, I_BE2_TargetObject targetObject)
        {
            I_BE2_BlocksStack[] tempStacks = BE2_ArrayUtils.FindAll(ref blocksStacksArray, (x => x == blocksStack));
            if (tempStacks.Length <= 0)
            {
                BE2_ArrayUtils.Add(ref blocksStacksArray, blocksStack);
                blocksStack.TargetObject = targetObject;

                BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnBlocksStackArrayUpdate);

                // v2.9 - BlocksStack Execute action is now executed from the OnUpdate event
                AddToUpdate(blocksStack.Execute);
            }
        }

        public void RemoveFromBlocksStackList(I_BE2_BlocksStack blocksStack)
        {
            I_BE2_BlocksStack[] tempStacks = BE2_ArrayUtils.FindAll(ref blocksStacksArray, (x => x == blocksStack));

            if (tempStacks.Length > 0)
            {
                BE2_ArrayUtils.Remove(ref blocksStacksArray, blocksStack);

                BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnBlocksStackArrayUpdate);

                // v2.9 - BlocksStack Execute action is now executed from the OnUpdate event
                RemoveFromUpdate(blocksStack.Execute);
            }
        }

        void UpdateTargetObjects()
        {
            _targetObjectsList = new List<I_BE2_TargetObject>();

            GameObject[] gos = FindObjectsOfType<GameObject>();
            int gosCount = gos.Length;
            for (int i = 0; i < gosCount; i++)
            {
                I_BE2_TargetObject targetObject = gos[i].GetComponent<I_BE2_TargetObject>();
                if (targetObject != null)
                    _targetObjectsList.Add(targetObject);
            }
        }

        // v2.7 - UpdateProgrammingEnvsList method of Execution Manager made public
        public void UpdateProgrammingEnvsList()
        {
            _programmingEnvsList = new List<I_BE2_ProgrammingEnv>();

            GameObject[] gos = FindObjectsOfType<GameObject>();
            int gosCount = gos.Length;
            for (int i = 0; i < gosCount; i++)
            {
                I_BE2_ProgrammingEnv programmingEnv = gos[i].GetComponent<I_BE2_ProgrammingEnv>();
                if (programmingEnv != null)
                    _programmingEnvsList.Add(programmingEnv);
            }
        }
    }
}