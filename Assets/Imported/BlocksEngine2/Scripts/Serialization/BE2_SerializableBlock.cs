using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MG_BlocksEngine2.Serializer
{
    [System.Serializable]
    public class BE2_SerializableBlock
    {
        public string blockName;
        public Vector3 position;
        public List<BE2_SerializableSection> sections;
        // v2.11.1 - obsolete isVariable flag removed 
        // v2.9 - isVariable flag (made obsolete) replaced by varManagerName field on the BE2_SerializableBlock to enable and facilitate the addition of custom variable types
        // See [SerializeAsVariable] attribute
        /// <summary>
        /// if not "" (empty), indicates this block is a Variable defined by the variable manager of type System.Type.GetType(varManagerName) 
        /// </summary>
        public string varManagerName;
        public string varName;

        public BE2_SerializableBlock()
        {
            sections = new List<BE2_SerializableSection>();
        }
    }

    [System.Serializable]
    public class BE2_SerializableSection
    {
        public List<BE2_SerializableBlock> childBlocks;
        public List<BE2_SerializableInput> inputs;

        public BE2_SerializableSection()
        {
            childBlocks = new List<BE2_SerializableBlock>();
            inputs = new List<BE2_SerializableInput>();
        }
    }

    [System.Serializable]
    public class BE2_SerializableInput
    {
        public bool isOperation;
        public string value;
        public BE2_SerializableBlock operation;
    }
}
