using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MG_BlocksEngine2.Block
{
    // v2.8 - BE2_InputValues moved to its own script file
    public class BE2_InputValues
    {
        public bool isText;
        public string stringValue;
        public float floatValue;

        public BE2_InputValues(string stringValue, float floatValue, bool isText)
        {
            this.isText = isText;
            this.stringValue = stringValue;
            this.floatValue = floatValue;
        }
    }
}