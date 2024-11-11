using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using MG_BlocksEngine2.Block;

namespace MG_BlocksEngine2.Core
{
    // v2.1 - added new event types: OnBlocksStackArrayUpdate, OnStackExecutionStart, OnStackExecutionEnd
    // v2.1 - previous BE2EtenvTypes misspell fixed to BE2EventTypes
    public enum BE2EventTypes
    {
        OnPlay, OnStop,
        // v2.7 - OnPointerUpEnd event name changed to OnPrimaryKeyUpEnd
        OnDrag, OnPrimaryKeyUpEnd,
        OnAnyVariableValueChanged, OnAnyVariableAddedOrRemoved,
        OnBlocksStackArrayUpdate,
        // v2.7 - added new events
        OnPrimaryKeyDown, OnPrimaryKey, OnPrimaryKeyUp, OnSecondaryKeyDown, OnPrimaryKeyHold,
        OnDeleteKeyDown,
        // v2.10 - new events added to enable the use of an auxiliary key to change the programmingEnv zoom using key+scroll: OnSecondaryKeyUp, OnAuxKeyDown, OnAuxKeyUp
        OnSecondaryKeyUp, OnAuxKeyDown, OnAuxKeyUp
    }

    public enum BE2EventTypesBlock
    {
        OnStackExecutionStart, OnStackExecutionEnd,
        // v2.11.1 - added new block events for drop actions: OnDropAtStack, OnDropAtInputSpot, OnDropAtProgrammingEnv, OnDropDestroy
        OnDrop, OnDropAtStack, OnDropAtInputSpot, OnDropAtProgrammingEnv, OnDropDestroy
    }

    [System.Serializable]
    public class BE2_Event : UnityEvent<I_BE2_Block> { }

    // v2.1 - BE2_EventsManager refactored to enable event types that pass I_BE2_Block when triggered
    // v2.1 - OBS: Remove the "Scripts/EngineCore/Core" folder before importing the new version  
    public class BE2_EventsManager
    {
        Dictionary<BE2EventTypes, UnityEvent> _eventDictionary;
        Dictionary<BE2EventTypesBlock, BE2_Event> _eventDictionaryBlock;

        public BE2_EventsManager()
        {
            if (_eventDictionary == null)
            {
                _eventDictionary = new Dictionary<BE2EventTypes, UnityEvent>();
            }
            if (_eventDictionaryBlock == null)
            {
                _eventDictionaryBlock = new Dictionary<BE2EventTypesBlock, BE2_Event>();
            }
        }

        public void StartListening(BE2EventTypes eventName, UnityAction listener)
        {
            UnityEvent thisEvent = null;
            if (_eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.AddListener(listener);
            }
            else
            {
                thisEvent = new UnityEvent();
                thisEvent.AddListener(listener);
                _eventDictionary.Add(eventName, thisEvent);
            }
        }

        public void StartListening(BE2EventTypesBlock eventName, UnityAction<I_BE2_Block> listener)
        {
            BE2_Event thisEvent = null;
            if (_eventDictionaryBlock.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.AddListener(listener);
            }
            else
            {
                thisEvent = new BE2_Event();
                thisEvent.AddListener(listener);
                _eventDictionaryBlock.Add(eventName, thisEvent);
            }
        }

        public void StopListening(BE2EventTypes eventName, UnityAction listener)
        {
            UnityEvent thisEvent = null;
            if (_eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
        }

        public void StopListening(BE2EventTypesBlock eventName, UnityAction<I_BE2_Block> listener)
        {
            BE2_Event thisEvent = null;
            if (_eventDictionaryBlock.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
        }

        public void TriggerEvent(BE2EventTypes eventName)
        {
            UnityEvent thisEvent = null;
            if (_eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke();
            }
        }

        public void TriggerEvent(BE2EventTypesBlock eventName, I_BE2_Block block)
        {
            BE2_Event thisEvent = null;
            if (_eventDictionaryBlock.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke(block);
            }
        }
    }
}
