using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MG_BlocksEngine2.Environment
{
    // v2.9 - variable managers now inherit from the interface I_BE2_VariablesManager to enable and facilitate new variable types
    public interface I_BE2_VariablesManager
    {
        void CreateAndAddVarToPanel(string varName);
    }
}