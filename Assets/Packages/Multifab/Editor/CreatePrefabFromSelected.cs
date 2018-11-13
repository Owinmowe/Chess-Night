/* --------------------------------------------------
 * Multifab
 * Copyright © 2017 Chaos Theory Games Pty. Ltd.
 * Proprietary and Confidential. All Rights Reserved.
 *
 * www.chaostheorygames.com
 * --------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Multifab
{
    public class CreatePrefabFromSelected : ScriptableObject
    {
        private const string saveLocKey = "multifab-saveLoc";
        private const string menuString = "Tools/Multifab/Create Prefab From Selected &%#c";

        /// <summary>
        /// Creates a prefab from the selected game object.
        /// </summary>
        [MenuItem(menuString)]
        public static string CreatePrefab()
        {
            var objs = Selection.gameObjects;
            int prefabCount = 0;

            if (objs.Length <= 1)
            {
                return "Error: Methods will only work with multiple items selected.";
            }

            if (!SelectedItemsValid(objs))
            {
                return "Error: Cannot create prefabs that already exist.";
            }

            // Allow for the selection of a save location.
            string lastPath = PlayerPrefs.HasKey(saveLocKey) ? PlayerPrefs.GetString(saveLocKey) : "Assets";
            string pathBase = EditorUtility.SaveFolderPanel("Choose save folder", lastPath, "");
            PlayerPrefs.SetString(saveLocKey, pathBase);

            if (!string.IsNullOrEmpty(pathBase))
            {
                pathBase = pathBase.Remove(0, pathBase.IndexOf("Assets")) + Path.DirectorySeparatorChar;

                foreach (var go in objs)
                {
                    string localPath = CreateFileName(pathBase, go.name, ".prefab");

                    // Check to see if the prefab already exists. If it does, show a dialog.
                    if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
                    {
                        int suffix = 1;
                        //string originalPath = CreateFileName(pathBase, go.name, ".prefab");
                        string newPath = CreateFileName(pathBase, go.name, ".prefab", suffix.ToString());
                        while (AssetDatabase.LoadAssetAtPath(newPath, typeof(GameObject)))
                        {
                            suffix++;
                            newPath = CreateFileName(pathBase, go.name, ".prefab", suffix.ToString());
                        }
                        localPath = newPath;
                        
                        CreateNew(go, localPath);
                        prefabCount++;
                    }
                    else
                    {
                        CreateNew(go, localPath);
                        prefabCount++;
                    }
                }

                if (objs.Length > 0)
                    return prefabCount + " prefabs created.";
                else
                    return "";
            }
            else
                return "";
        }

        private static string CreateFileName(string path, string name, string ext, string suffix = "")
        {
            string fileName = string.Empty;
            if (string.IsNullOrEmpty(suffix))
                fileName = string.Format("{0}{1}{2}", path, name, ext);
            else
                fileName = string.Format("{0}{1} {2}{3}", path, name, suffix, ext);

            return fileName.Replace(@"\", @"/");
        }

        private static void CreateNew(GameObject obj, string localPath)
        {
            UnityEngine.Object prefab = PrefabUtility.CreatePrefab(localPath, obj);
            PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
            AssetDatabase.Refresh();
        }

        // Checks to see if the functions should run based off the selection.
        public static bool SelectedItemsValid(GameObject[] selectedObjects)
        {
            return !selectedObjects.Any(o =>
            {
                PrefabType prefabType = PrefabUtility.GetPrefabType(o);
                return (prefabType == PrefabType.ModelPrefab || prefabType == PrefabType.ModelPrefabInstance ||
                        prefabType == PrefabType.Prefab || prefabType == PrefabType.PrefabInstance ||
                        prefabType == PrefabType.DisconnectedPrefabInstance || prefabType == PrefabType.DisconnectedModelPrefabInstance);
            }) && !selectedObjects.Any(o => WasParentSelected(o, selectedObjects)) 
            && selectedObjects.Length > 1;
        }

        public static bool WasParentSelected(GameObject go, GameObject[] objs)
        {
            List<GameObject> objsList = new List<GameObject>(objs);
            GameObject current = go;
            while (current.transform.parent != null)
            {
                if (objsList.Contains(current.transform.parent.gameObject))
                {
                    return true;
                }
                current = current.transform.parent.gameObject;
            }

            return false;
        }

        /// <summary>
        /// Validates the menu.
        /// </summary>
        /// <remarks>The item will be disabled if no game object is selected.</remarks>
        [MenuItem(menuString, true)]
        static bool ValidateCreatePrefab()
        {
            return SelectedItemsValid(Selection.gameObjects);
        }
    }
}
