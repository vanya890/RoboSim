using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MG_BlocksEngine2.Block
{
    public interface I_BE2_BlockSectionHeaderInput
    {
        Transform Transform { get; }
        I_BE2_Spot Spot { get; }

        float FloatValue { get; }
        string StringValue { get; }

        /// <summary>
        /// Returns the input value as BE2_InputValues, that contains it as text (BE2_InputValues.stringValue) and number (BE2_InputValues.floatValue)
        /// </summary>
        BE2_InputValues InputValues { get; }

        void UpdateValues();
    }
}