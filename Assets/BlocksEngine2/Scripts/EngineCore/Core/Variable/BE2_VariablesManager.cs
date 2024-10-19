using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

using MG_BlocksEngine2.Core;
using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.UI;

namespace MG_BlocksEngine2.Environment
{
    public class BE2_VariablesManager : MonoBehaviour, I_BE2_VariablesManager
    {
        public static BE2_VariablesManager instance;
        public BE2_UI_NewVariablePanel newVariablePanel;
        /// <summary>
        /// [var name, value]
        /// </summary>
        public Dictionary<string, string> variablesList;

        void Awake()
        {
            instance = this;
            variablesList = new Dictionary<string, string>();
        }

        //void Start()
        //{
        //
        //}
        //
        //void Update()
        //{
        //
        //}

        public bool ContainsVariable(string variable)
        {
            return variablesList.ContainsKey(variable);
        }

        /// <summary>
        /// Adds a new variable and sets its value, or, if the variable already exists, updates its value
        /// </summary>
        public void AddOrUpdateVariable(string variable, string value)
        {
            if (!variablesList.ContainsKey(variable))
            {
                variablesList.Add(variable, value);
                BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnAnyVariableAddedOrRemoved);
            }
            else
            {
                variablesList[variable] = value;
            }

            BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnAnyVariableValueChanged);
        }

        public void RemoveVariable(string variable)
        {
            if (variablesList.ContainsKey(variable))
            {
                variablesList.Remove(variable);
            }

            BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnAnyVariableAddedOrRemoved);
        }

        public string GetVariableStringValue(string variable)
        {
            if (variablesList.ContainsKey(variable))
                return variablesList[variable];
            else
                return "";
        }

        public float GetVariableFloatValue(string variable)
        {
            if (variablesList.ContainsKey(variable))
            {
                float floatValue = 0;
                try
                {
                    floatValue = float.Parse(variablesList[variable], CultureInfo.InvariantCulture);
                    return floatValue;
                }
                catch
                {
                    return 0;
                }

            }
            else
                return 0;
        }

        /// <summary>
        /// Get the variable values as BE2_InputValues type, which contains the actual result in both string or float types
        /// </summary>
        public BE2_InputValues GetVariableValues(string variable)
        {
            bool isText = false;
            if (variablesList.ContainsKey(variable))
            {
                float floatValue = 0;
                string stringValue = variablesList[variable];
                try
                {
                    floatValue = float.Parse(stringValue, CultureInfo.InvariantCulture);
                    isText = false;
                }
                catch
                {

                    isText = true;
                }
                return new BE2_InputValues(stringValue, floatValue, isText);
            }
            else
                return new BE2_InputValues("", 0, false);
        }

        public void CreateAndAddVarToPanel(string varName)
        {
            if (newVariablePanel)
            {
                newVariablePanel.CreateVariable(varName);
            }
        }
    }
}