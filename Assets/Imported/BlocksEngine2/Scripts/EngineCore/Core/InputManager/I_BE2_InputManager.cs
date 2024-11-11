using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MG_BlocksEngine2.Core
{
    public interface I_BE2_InputManager
    {
        void OnUpdate();
        Vector3 ScreenPointerPosition { get; }
        Vector3 CanvasPointerPosition { get; }
    }
}