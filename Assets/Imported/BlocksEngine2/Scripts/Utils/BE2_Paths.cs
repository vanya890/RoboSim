using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MG_BlocksEngine2.EditorScript;

namespace MG_BlocksEngine2.Utils
{
    // v2.3 - new helper class to support the setting of needed paths
    public class BE2_Paths
    {
        static public string TranslateMarkupPath(string pathMarkup)
        {
            string fullPath = pathMarkup;

            // v2.7 - saved codes path are now, by default, set to the "persistentDataPath" on Build. The setting "usePersistentPathOnBuild" can be set from the BE2 inspector
            if (BE2_Inspector.Instance.usePersistentPathOnBuild && !Application.isEditor)
            {
                fullPath = fullPath.Replace("[dataPath]", Application.persistentDataPath);
                fullPath = fullPath.Replace("[persistentDataPath]", Application.persistentDataPath);
                return fullPath;
            }

            fullPath = fullPath.Replace("[dataPath]", Application.dataPath);
            fullPath = fullPath.Replace("[persistentDataPath]", Application.persistentDataPath);
            return fullPath;
        }

        static public string PathToResources(string pathMarkup)
        {
            string resourcesPath = pathMarkup;

            if (!pathMarkup.ToLower().Contains("resources"))
            {
                Debug.LogError("The path is not set to a Resources folder.");

                return resourcesPath;
            }

            int idx = resourcesPath.ToLower().IndexOf("resources/");
            resourcesPath = resourcesPath.Substring(idx + 10);

            return resourcesPath;
        }

        // v2.6.2 - bugfix: fixed changes on BE2 Inspector paths not perssiting 
        static public string NewInstructionPath
        {
            get => BE2_Inspector.Instance.newInstructionPath;
            set => BE2_Inspector.Instance.newInstructionPath = value;
        }
        static public string NewBlockPrefabPath
        {
            get => BE2_Inspector.Instance.newBlockPrefabPath;
            set => BE2_Inspector.Instance.newBlockPrefabPath = value;
        }
        static public string SavedCodesPath
        {
            get => BE2_Inspector.Instance.savedCodesPath;
            set => BE2_Inspector.Instance.savedCodesPath = value;
        }
    }
}