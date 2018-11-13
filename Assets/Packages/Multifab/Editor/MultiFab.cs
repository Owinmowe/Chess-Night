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
    // Custom editor extension that builds the controls that make up multifab.
    [CustomEditor(typeof(GameObject))]
    [CanEditMultipleObjects()]
    public class Multifab : DecoratorEditor
    {

        private static float refreshTime = 5.0f;
        private static double nextRefresh = 0;

        private string multifabMessage;
        public Multifab() : base("GameObjectInspector")
        {

        }

        public override void OnInspectorGUI()
        {
            // Draw default GUI for MonoBehaviours
            base.OnInspectorGUI();

            //EditorGUIUtility.whiteTexture = Color.white;
            GameObject[] selectedObjects = Selection.gameObjects;
            // Only work for multiple selections.
            if (selectedObjects.Length <= 1)
                return;
            // Only work in hierarchy, not project folder
            else if (selectedObjects.Any(o => AssetDatabase.Contains(o)))
                return;

            // Determine which buttons should be active
            bool showCreate = CreatePrefabFromSelected.SelectedItemsValid(selectedObjects);
            bool showApplyRevert = ApplySelectedPrefabs.SelectedItemsValid(selectedObjects);

            string errorMsg = string.Empty;

            // Ensure children and parent objects arent selected
            if (selectedObjects.Any(o => CreatePrefabFromSelected.WasParentSelected(o, selectedObjects)))
            {
                errorMsg = "Can't select GameObjects from same tree.";
            }
            // Only work if at least 1 control is active
            else if (!showApplyRevert && !showCreate)
            {
                errorMsg = "Can't mix prefabs and GameObjects.";
            }
            
            // ----
            // Draw the GUI
            // ----
            float designDPI = 96f;
            float dpiScale = Screen.dpi / designDPI;

            // Create a custom label style based on the current skin
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.margin = new RectOffset(0, 5, 5, 2);
            labelStyle.padding = new RectOffset(-1, 0, 0, 3);
            labelStyle.stretchWidth = false;

            // Begin the formatted area.
            GUILayout.BeginVertical(labelStyle);
            GUILayout.BeginHorizontal(labelStyle);

            GUILayout.Label(new GUIContent("Multifab"), labelStyle);

            // Create a custom box style for error messages
            GUIStyle errorBoxStyle = new GUIStyle(EditorStyles.helpBox);
            errorBoxStyle.fontSize = 10;
            errorBoxStyle.fixedHeight = 16;
            errorBoxStyle.margin = new RectOffset(4, -10, 4, 4);
            errorBoxStyle.padding = new RectOffset(0, 0, 0, 3);
            errorBoxStyle.alignment = TextAnchor.MiddleCenter;
            errorBoxStyle.normal.textColor = labelStyle.normal.textColor;

            if (string.IsNullOrEmpty(errorMsg))
            {
                // Create a new style for the buttons
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.stretchWidth = true;
                //buttonStyle.margin = new RectOffset(0, 0, 3, 3);
                //buttonStyle.padding = new RectOffset(0, 0, 2, 3);
                buttonStyle.fontSize = 10;
               

                GUI.enabled = showCreate;

                if (GUILayout.Button("Create", buttonStyle))
                {
                    multifabMessage = CreatePrefabFromSelected.CreatePrefab();
                    nextRefresh = 0;
                }

                GUILayout.Space(-5f);

                GUI.enabled = showApplyRevert;

                if (GUILayout.Button("Revert", buttonStyle))
                {
                    multifabMessage = ApplySelectedPrefabs.SearchPrefabConnections(ApplySelectedPrefabs.RevertToSelectedPrefabs, PrefabAction.Revert);
                    nextRefresh = 0;
                }

                GUILayout.Space(-5f);

                if (GUILayout.Button("Apply", buttonStyle))
                {
                    multifabMessage = ApplySelectedPrefabs.SearchPrefabConnections(ApplySelectedPrefabs.ApplyToSelectedPrefabs, PrefabAction.Apply);
                    nextRefresh = 0;
                }

                GUILayout.Space(-5f);
            }
            else
            {
                GUILayout.Box(errorMsg, errorBoxStyle);
            }

            // Ensure that the GUI state is enabled as we finish up.
            GUI.enabled = true;

            // End the formatted area.
            EditorGUILayout.EndHorizontal();

            Repaint();
            errorBoxStyle.margin = new RectOffset(0, 4, 4, 4);
            if (!string.IsNullOrEmpty(multifabMessage))
            {
                if (nextRefresh == 0)
                {
                    GUILayout.Box(multifabMessage, errorBoxStyle);
                    nextRefresh = EditorApplication.timeSinceStartup + refreshTime;
                }
                else if (nextRefresh > EditorApplication.timeSinceStartup)
                {
                    GUILayout.Box(multifabMessage, errorBoxStyle);
                }
                else
                {
                    multifabMessage = "";
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}
