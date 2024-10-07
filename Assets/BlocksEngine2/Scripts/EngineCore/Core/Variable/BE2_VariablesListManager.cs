using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

using MG_BlocksEngine2.Core;
using MG_BlocksEngine2.Block;
using MG_BlocksEngine2.UI;

namespace MG_BlocksEngine2.Environment
{
    // v2.9 - added scripts to manager lists as variables and BE2_VariablesListManager component added to the scenes
    public class BE2_VariablesListManager : MonoBehaviour, I_BE2_VariablesManager
    {
        public static BE2_VariablesListManager instance;
        public BE2_UI_NewVariableListPanel newListPanel;
        /// <summary>
        /// [list name, value]
        /// </summary>
        public Dictionary<string, List<string>> lists = new Dictionary<string, List<string>>();

        void Awake()
        {
            instance = this;
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

        public bool ContainsList(string listName)
        {
            return lists.ContainsKey(listName);
        }

        public bool ListContainsValue(string listName, string value)
        {
            if (lists.ContainsKey(listName))
            {
                return lists[listName].Contains(value);
            }
            else
                return false;
        }

        public void AddOrUpdateList(string listName, List<string> value)
        {
            if (!lists.ContainsKey(listName))
            {
                lists.Add(listName, value);
                BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnAnyVariableAddedOrRemoved);
            }
            else
            {
                lists[listName] = value;
            }

            BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnAnyVariableValueChanged);
        }

        public void AddValueInList(string listName, string value)
        {
            if (!lists.ContainsKey(listName))
            {
                lists.Add(listName, new List<string> { value });
                BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnAnyVariableAddedOrRemoved);
            }
            else
            {
                lists[listName].Add(value);
            }

            BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnAnyVariableValueChanged);
        }

        public void InsertValueInList(string listName, string value, int index)
        {
            if (lists.ContainsKey(listName))
            {
                if (index >= 0 && lists[listName].Count >= index)
                {
                    lists[listName].Insert(index, value);

                    BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnAnyVariableValueChanged);
                }
            }
        }

        public void ReplaceValueInList(string listName, string value, int index)
        {
            if (lists.ContainsKey(listName))
            {
                if (index >= 0 && lists[listName].Count > index)
                {
                    lists[listName][index] = value;

                    BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnAnyVariableValueChanged);
                }
            }
        }

        public void RemoveList(string listName)
        {
            if (lists.ContainsKey(listName))
            {
                lists.Remove(listName);
                BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnAnyVariableAddedOrRemoved);
                BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnAnyVariableValueChanged);
            }
        }

        public void ClearList(string listName)
        {
            if (lists.ContainsKey(listName))
            {
                lists[listName].Clear();
                BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnAnyVariableAddedOrRemoved);
                BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnAnyVariableValueChanged);
            }
        }

        public void RemoveListItem(string listName, int index)
        {
            if (lists.ContainsKey(listName))
            {
                if (index >= 0 && lists[listName].Count > index)
                {
                    lists[listName].RemoveAt(index);

                    BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnAnyVariableAddedOrRemoved);
                    BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnAnyVariableValueChanged);
                }
            }
        }

        public void RemoveListItem(string listName, string value)
        {
            if (lists.ContainsKey(listName))
            {
                if (lists[listName].Contains(value))
                {
                    lists[listName].Remove(value);

                    BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnAnyVariableAddedOrRemoved);
                    BE2_MainEventsManager.Instance.TriggerEvent(BE2EventTypes.OnAnyVariableValueChanged);
                }
            }
        }

        public int GetValueIndexAtList(string listName, string value)
        {
            if (lists.ContainsKey(listName))
            {
                if (lists[listName].Contains(value))
                {
                    return lists[listName].IndexOf(value);
                }
                else
                    return -1;
            }
            else
                return -1;
        }

        public List<string> GetListStringValues(string listName)
        {
            if (lists.ContainsKey(listName))
                return lists[listName];
            else
                return new List<string>() { "" };
        }

        public string GetListStringValue(string listName, int index)
        {
            List<string> list = GetListStringValues(listName);
            if (list.Count > index)
                return list[index];
            else
                return "";
        }

        public List<float> GetListFloatValues(string listName)
        {
            if (lists.ContainsKey(listName))
            {
                List<float> floatValues = new List<float>();
                foreach (string stringValue in lists[listName])
                {
                    try
                    {
                        float value = float.Parse(stringValue, CultureInfo.InvariantCulture);
                        floatValues.Add(value);
                    }
                    catch
                    {
                        floatValues.Add(0);
                    }
                }
                return floatValues;
            }
            else
                return new List<float>() { 0 };
        }

        public float GetListFloatValue(string listName, int index)
        {
            List<float> list = GetListFloatValues(listName);
            if (list.Count > index)
                return list[index];
            else
                return 0;
        }

        public List<BE2_InputValues> GetListValues(string listName)
        {
            List<BE2_InputValues> values = new List<BE2_InputValues>();

            if (lists.ContainsKey(listName))
            {
                foreach (string stringValue in lists[listName])
                {
                    bool isText = false;
                    float floatValue = 0;
                    try
                    {
                        floatValue = float.Parse(stringValue, CultureInfo.InvariantCulture);
                        isText = false;
                    }
                    catch
                    {
                        isText = true;
                    }
                    values.Add(new BE2_InputValues(stringValue, floatValue, isText));
                }

                return values;
            }
            else
                return new List<BE2_InputValues>() { new BE2_InputValues("", 0, false) };
        }

        public BE2_InputValues GetListValue(string listName, int index)
        {
            List<BE2_InputValues> list = GetListValues(listName);
            if (list.Count > index)
                return list[index];
            else
                return new BE2_InputValues("", 0, false);
        }

        public void CreateAndAddVarToPanel(string listName)
        {
            if (newListPanel)
            {
                newListPanel.CreateList(listName);
            }
        }
    }
}