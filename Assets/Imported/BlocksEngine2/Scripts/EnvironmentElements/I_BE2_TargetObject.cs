using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MG_BlocksEngine2.Environment
{
    public interface I_BE2_TargetObject
    {
        Transform Transform { get; }
        // v2.5 - added a Programming Environment reference to the Target Object interface 
        I_BE2_ProgrammingEnv ProgrammingEnv { get; set; }
    }
}