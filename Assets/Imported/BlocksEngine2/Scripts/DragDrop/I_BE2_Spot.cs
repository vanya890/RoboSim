using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MG_BlocksEngine2.Block
{
    public interface I_BE2_Spot
    {
        Transform Transform { get; }
        Vector2 DropPosition { get; }
        I_BE2_Block Block { get; }
    }
}