/* --------------------------------------------------
 * Multifab
 * Copyright © 2017 Chaos Theory Games Pty. Ltd.
 * Proprietary and Confidential. All Rights Reserved.
 *
 * www.chaostheorygames.com
 * --------------------------------------------------
 */

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Multifab
{
    public enum PrefabAction
    {
        Apply,
        Revert
    }

    public class ApplySelectedPrefabs : EditorWindow
    {
        private const string applyMenuString = "Tools/Multifab/Apply all selected prefabs &%#a";
        private const string revertMenuString = "Tools/Multifab/Revert all selected prefabs &%#r";

        public delegate void ApplyOrRevert(GameObject _goCurrentGo, Object _ObjPrefabParent, ReplacePrefabOptions _eReplaceOptions);
        [MenuItem(applyMenuString)]
        static void ApplyPrefabs()
        {
            SearchPrefabConnections(ApplyToSelectedPrefabs, PrefabAction.Apply);
        }

        [MenuItem(revertMenuString)]
        static void ResetPrefabs()
        {
            SearchPrefabConnections(RevertToSelectedPrefabs, PrefabAction.Revert);
        }

        // Look for connections
        public static string SearchPrefabConnections(ApplyOrRevert _applyOrRevert, PrefabAction action)
        {
            GameObject[] objs = Selection.gameObjects;

            if (action == PrefabAction.Revert)
            {
                // Display warning dialog box if revert is chosen
                if (!EditorUtility.DisplayDialog("Revert multiple prefabs?",
                        "You are attempting to revert the properties of multiple prefabs. All current modifications to these prefabs will be lost. Are you sure?",
                        "Yes",
                        "No"))
                {
                    return "Revert cancelled.";
                }
            }
            if (!SelectedItemsValid(objs))
            {
                return "Error: Cannot create prefabs that already exist";
            }

            if (objs.Length > 1)
            {
                GameObject goPrefabRoot;
                GameObject goCur;
                bool isTopHierarchyFound;
                int prefabCount = 0;
                PrefabType prefabType;
                bool canApply;
                // Iterate through all the selected gameobjects
                foreach (GameObject go in objs)
                {

                    prefabType = PrefabUtility.GetPrefabType(go);
                    // Is the selected gameobject a prefab?
                    if (prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance)
                    {
                        // Prefab Root
                        goPrefabRoot = ((GameObject)PrefabUtility.GetPrefabParent(go)).transform.root.gameObject;
                        goCur = go;
                        isTopHierarchyFound = false;
                        canApply = true;
                        // We go up in the hierarchy to apply the root of the go to the prefab
                        while (goCur.transform.parent != null && !isTopHierarchyFound)
                        {
                            // Are we still in the same prefab?
                            if (PrefabUtility.GetPrefabParent(goCur.transform.parent.gameObject) != null && (goPrefabRoot == ((GameObject)PrefabUtility.GetPrefabParent(goCur.transform.parent.gameObject)).transform.root.gameObject))
                            {
                                goCur = goCur.transform.parent.gameObject;
                            }
                            else
                            {
                                // The GameObject parent is another prefab, we stop here
                                isTopHierarchyFound = true;
                                if (goPrefabRoot != ((GameObject)PrefabUtility.GetPrefabParent(goCur)))
                                {
                                    // GameObject is part of another prefab
                                    canApply = false;
                                }
                            }
                        }

                        if (_applyOrRevert != null && canApply)
                        {
                            prefabCount++;
                            _applyOrRevert(goCur, PrefabUtility.GetPrefabParent(goCur), ReplacePrefabOptions.ConnectToPrefab);
                        }
                    }
                }
                if (action == PrefabAction.Apply)
                    return prefabCount + " prefabs updated.";
                else
                    return prefabCount + " prefabs reverted.";
            }
            else
            {
                return "Error: Methods will only work with multiple items selected.";
            }
        }

        // Checks to see if the functions should run based off the selection.
        public static bool SelectedItemsValid(GameObject[] selectedObjects)
        {
            return selectedObjects.All(o =>
            {
                PrefabType prefabType = PrefabUtility.GetPrefabType(o);
                return (prefabType == PrefabType.ModelPrefab || prefabType == PrefabType.ModelPrefabInstance ||
                        prefabType == PrefabType.Prefab || prefabType == PrefabType.PrefabInstance ||
                        prefabType == PrefabType.DisconnectedPrefabInstance || prefabType == PrefabType.DisconnectedModelPrefabInstance);
            }) && selectedObjects.Length > 1;
        }

        // Apply      
        public static void ApplyToSelectedPrefabs(GameObject _goCurrentGo, Object _ObjPrefabParent, ReplacePrefabOptions _eReplaceOptions)
        {
            PrefabUtility.ReplacePrefab(_goCurrentGo, _ObjPrefabParent, _eReplaceOptions);
        }

        // Revert
        public static void RevertToSelectedPrefabs(GameObject _goCurrentGo, Object _ObjPrefabParent, ReplacePrefabOptions _eReplaceOptions)
        {
            PrefabUtility.ReconnectToLastPrefab(_goCurrentGo);
            PrefabUtility.RevertPrefabInstance(_goCurrentGo);
        }

        /// <summary>
        /// Validates the menu.
        /// </summary>
        /// <remarks>The item will be disabled if no game object is selected.</remarks>
        [MenuItem(applyMenuString, true)]
        static bool ValidateApplyPrefab()
        {
            return SelectedItemsValid(Selection.gameObjects);
        }

        [MenuItem(revertMenuString, true)]
        static bool ValidateRevertPrefab()
        {
            return SelectedItemsValid(Selection.gameObjects);
        }
    }
}
