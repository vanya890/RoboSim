using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MG_BlocksEngine2.Core
{
    public class BE2_MainEventsManager : MonoBehaviour
    {
        static BE2_EventsManager _instance;
        public static BE2_EventsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BE2_EventsManager();
                return _instance;
            }
        }

        void Init()
        {
            if (_instance == null)
                _instance = new BE2_EventsManager();
        }

        void Awake()
        {
            Init();
        }
    }
}