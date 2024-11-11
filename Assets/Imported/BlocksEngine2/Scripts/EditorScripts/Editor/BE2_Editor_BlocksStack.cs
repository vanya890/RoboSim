using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using MG_BlocksEngine2.Core;
using MG_BlocksEngine2.Block;

namespace MG_BlocksEngine2.EditorScript
{
    // v2.9 - added BlocksStack editor script with inspector buttons for step-by-step play and pause 
    [CustomEditor(typeof(BE2_BlocksStack))]
    public class BE2_Editor_BlocksStack : Editor
    {
        BE2_BlocksStack inspector;

        void OnEnable()
        {
            inspector = (BE2_BlocksStack)target;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Play"))
            {
                inspector.PopulateStack();
                inspector.IsActive = true;
            }

            if (GUILayout.Button("Stop"))
            {
                inspector.Pointer = 0;
                inspector.IsActive = false;
            }

            if (GUILayout.Button("Step Play"))
            {
                inspector.StepPlay();
            }

            if (GUILayout.Button("Pause"))
            {
                inspector.Pause();
            }
        }
    }
}