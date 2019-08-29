﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GGS_Framework
{
    public partial class ExtendedGUILayout
    {
        #region Class implementation
        public static TimeObject TimeObject (GUIContent label, TimeObject value)
        {
            Rect rect = EditorGUILayout.GetControlRect (GUILayout.ExpandWidth (true), GUILayout.Height (18));
            return ExtendedGUI.TimeObject (rect, label, value);
        }

        public static TimeObject TimeObject (string label, TimeObject value)
        {
            return TimeObject (new GUIContent (label), value);
        }

        public static void TimeObject (GUIContent label, SerializedProperty property)
        {
            Rect rect = EditorGUILayout.GetControlRect (GUILayout.ExpandWidth (true), GUILayout.Height (18));
            ExtendedGUI.TimeObject (rect, label, property);
        }

        public static void TimeObject (string label, SerializedProperty property)
        {
            TimeObject (new GUIContent (label), property);
        }
        #endregion
    }
}
#endif