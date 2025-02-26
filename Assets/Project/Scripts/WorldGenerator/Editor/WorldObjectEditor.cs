using System;
using UnityEditor;
using UnityEngine;

namespace Game.WorldGenerator.Editor
{
    [CustomPropertyDrawer(typeof(WorldObject))]
    public class WorldObjectEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lineCount = 5; // Name, Prefab, Size, IsMandatory, (MaxCount or MinMaxPrelinValue)
            SerializedProperty prefab = property.FindPropertyRelative("Prefab");

            if (prefab.isExpanded)
                lineCount += prefab.arraySize + 1;

            return lineCount * EditorGUIUtility.singleLineHeight + (lineCount - 1) * EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Rect
            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            // Get Property
            SerializedProperty nameProp             = property.FindPropertyRelative("Name");
            SerializedProperty prefabProp           = property.FindPropertyRelative("Prefab");
            SerializedProperty sizeProp             = property.FindPropertyRelative("Size");
            SerializedProperty isMandatoryProp      = property.FindPropertyRelative("IsMandatory");
            SerializedProperty maxCountProp         = property.FindPropertyRelative("MaxCount");
            SerializedProperty minMaxPerlinProp     = property.FindPropertyRelative("MinMaxPerlinValue");

            // Name Property
            EditorGUI.PropertyField(rect, nameProp);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // Prefab List Property
            DrawPrefabPropertyBackground(rect, prefabProp);
            prefabProp.isExpanded = EditorGUI.Foldout(rect, prefabProp.isExpanded, "Prefab");
            rect.y += EditorGUIUtility.singleLineHeight;

            if (prefabProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < prefabProp.arraySize; i++)
                {
                    SerializedProperty element = prefabProp.GetArrayElementAtIndex(i);
                    EditorGUI.PropertyField(rect, element, new GUIContent($"Prefab {i}"));
                    rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                if (GUI.Button(new Rect(rect.x, rect.y, rect.width / 2, rect.height), "Add Prefab"))
                {
                    prefabProp.arraySize++;
                }
                if (GUI.Button(new Rect(rect.x + (rect.width / 2), rect.y, rect.width / 2, rect.height), "delete Prefab"))
                {
                    prefabProp.arraySize--;
                }
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.indentLevel--;
            }

            // Size Property
            EditorGUI.PropertyField(rect, sizeProp);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // isMandatory Property
            EditorGUI.PropertyField(rect, isMandatoryProp);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // MaxCount or MinMaxPrelinValue Property
            if (isMandatoryProp.boolValue)
                EditorGUI.PropertyField(rect, maxCountProp);
            else
                EditorGUI.PropertyField(rect, minMaxPerlinProp);

            EditorGUI.EndProperty();
        }

        private void DrawPrefabPropertyBackground(Rect rect, SerializedProperty prefabProp)
        {
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box)
            {
                border = new RectOffset(6, 6, 6, 6)
            };

            if (!prefabProp.isExpanded)
                GUI.Box(
                    new Rect(
                        rect.x - 15,
                        rect.y,
                        rect.width + 15,
                        rect.height),
                    GUIContent.none,
                    boxStyle
                    );
            else
                GUI.Box(
                    new Rect(
                        rect.x - 15,
                        rect.y,
                        rect.width + 15,
                        (prefabProp.arraySize + 1) * EditorGUIUtility.singleLineHeight + prefabProp.arraySize * EditorGUIUtility.standardVerticalSpacing),
                    GUIContent.none,
                    boxStyle
                    );
        }
    }
}
