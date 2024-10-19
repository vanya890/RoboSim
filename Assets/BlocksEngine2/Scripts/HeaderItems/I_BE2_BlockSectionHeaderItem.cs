using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MG_BlocksEngine2.Block
{
    public interface I_BE2_BlockSectionHeaderItem
    {
        Transform Transform { get; }
        Vector2 Size { get; }
    }
}