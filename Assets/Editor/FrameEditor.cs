﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;

namespace Framer
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Frame))]
    public class FrameEditor : Editor
    {
        private Frame frame;

        private SerializedProperty frameColor;
        private SerializedProperty cornerType;
        private SerializedProperty splitCorners;
        private SerializedProperty cornerRadii;
        private SerializedProperty levelOfDetail;

        void OnEnable()
        {
            frame = (Frame)target;

            frameColor = serializedObject.FindProperty("frameColor");
            cornerType = serializedObject.FindProperty("cornerType");
            splitCorners = serializedObject.FindProperty("splitCorners");
            cornerRadii = serializedObject.FindProperty("cornerRadii");
            levelOfDetail = serializedObject.FindProperty("levelOfDetail");
        }

        void OnSceneGUI()
        {
            //Drag and drop stuff
            if (Event.current.type == EventType.MouseUp)
            {
                if (frame.GetComponentInParent<Stack>() != null)
                {
                    Stack stack = frame.GetComponentInParent<Stack>();
                    stack.SortStack();
                    stack.ForceStack();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            frameColor.colorValue = EditorGUILayout.ColorField("Frame Color", frameColor.colorValue);

            EditorGUILayout.Space();

            cornerType.enumValueIndex = (int)(FrameCornerType)EditorGUILayout.EnumPopup("Corner Type", (FrameCornerType)cornerType.enumValueIndex);

            EditorGUILayout.Space();

            //This toggle allows for easy uniform corner radii
            if (splitCorners.boolValue = EditorGUILayout.Toggle("Split Corners", splitCorners.boolValue))
            {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Corner Radii", EditorStyles.boldLabel);
                if (cornerRadii.arraySize != 4)
                {
                    cornerRadii.arraySize = 4;
                }
                cornerRadii.GetArrayElementAtIndex(0).floatValue = EditorGUILayout.Slider("Top Right", cornerRadii.GetArrayElementAtIndex(0).floatValue, 0, Mathf.Min(
                                                                                                        frame.rectTransform.rect.width - cornerRadii.GetArrayElementAtIndex(1).floatValue,
                                                                                                        frame.rectTransform.rect.height - cornerRadii.GetArrayElementAtIndex(3).floatValue)
                                                                                                    );
                cornerRadii.GetArrayElementAtIndex(1).floatValue = EditorGUILayout.Slider("Top Left", cornerRadii.GetArrayElementAtIndex(1).floatValue, 0, Mathf.Min(
                                                                                                        frame.rectTransform.rect.width - cornerRadii.GetArrayElementAtIndex(2).floatValue,
                                                                                                        frame.rectTransform.rect.height - cornerRadii.GetArrayElementAtIndex(0).floatValue)
                                                                                                    );
                cornerRadii.GetArrayElementAtIndex(2).floatValue = EditorGUILayout.Slider("Bottom Left", cornerRadii.GetArrayElementAtIndex(2).floatValue, 0, Mathf.Min(
                                                                                                        frame.rectTransform.rect.width - cornerRadii.GetArrayElementAtIndex(3).floatValue,
                                                                                                        frame.rectTransform.rect.height - cornerRadii.GetArrayElementAtIndex(1).floatValue)
                                                                                                    );
                cornerRadii.GetArrayElementAtIndex(3).floatValue = EditorGUILayout.Slider("Bottom Right", cornerRadii.GetArrayElementAtIndex(3).floatValue, 0, Mathf.Min(
                                                                                                        frame.rectTransform.rect.width - cornerRadii.GetArrayElementAtIndex(2).floatValue,
                                                                                                        frame.rectTransform.rect.height - cornerRadii.GetArrayElementAtIndex(0).floatValue)
                                                                                                    );
            }
            else
            {
                cornerRadii.GetArrayElementAtIndex(0).floatValue = EditorGUILayout.Slider("Uniform Corner Radius", cornerRadii.GetArrayElementAtIndex(0).floatValue, 0, Mathf.Min(
                                                                                                                    frame.rectTransform.rect.width / 2,
                                                                                                                    frame.rectTransform.rect.height / 2)
                                                                                                                );

                for (int i = 1; i < 4; i++)
                {
                    cornerRadii.GetArrayElementAtIndex(i).floatValue = cornerRadii.GetArrayElementAtIndex(0).floatValue;
                }
            }

            //You can probably increase it, but 32 should be a high enough max for it to still look good.  Even 4 looks good if the frame is small enough
            levelOfDetail.intValue = EditorGUILayout.IntSlider("Vertices Per Corner", levelOfDetail.intValue, 4, 32);

            EditorGUILayout.Space();

            if (GUILayout.Button("Force Create Frame Mesh"))
            {
                frame.CreateFrameMesh();
            }

            if (GUI.changed && !EditorApplication.isPlaying)
            {
                serializedObject.ApplyModifiedProperties();
                frame.CreateFrameMesh();
                EditorUtility.SetDirty(frame);
            }
        }
    }
}