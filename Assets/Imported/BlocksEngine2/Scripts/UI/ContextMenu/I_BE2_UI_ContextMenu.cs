using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MG_BlocksEngine2.Utils;

namespace MG_BlocksEngine2.UI
{
    public interface I_BE2_UI_ContextMenu
    {
        // v2.1 - using BE2_Text to enable usage of Text or TMP components
        BE2_Text Title { get; }

        void Open<T>(T target, params string[] options);
        void Close();
    }
}