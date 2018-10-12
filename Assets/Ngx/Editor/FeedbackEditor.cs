using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Ngx
{
    [CustomEditor(typeof(Feedback))]
    class FeedbackEditor : Editor
    {
        SerializedProperty _modelIndex1;
        SerializedProperty _modelIndex2;
        SerializedProperty _mixParameter;

        SerializedProperty _feedbackRate;
        SerializedProperty _noiseInjection1;
        SerializedProperty _noiseInjection2;
        SerializedProperty _noiseInjection3;

        ReorderableList _modelNames;

        void OnEnable()
        {
            _modelIndex1 = serializedObject.FindProperty("_modelIndex1");
            _modelIndex2 = serializedObject.FindProperty("_modelIndex2");
            _mixParameter = serializedObject.FindProperty("_mixParameter");

            _feedbackRate = serializedObject.FindProperty("_feedbackRate");
            _noiseInjection1 = serializedObject.FindProperty("_noiseInjection1");
            _noiseInjection2 = serializedObject.FindProperty("_noiseInjection2");
            _noiseInjection3 = serializedObject.FindProperty("_noiseInjection3");

            _modelNames = new ReorderableList(
                serializedObject,
                serializedObject.FindProperty("_modelNames"),
                true, // draggable
                true, // displayHeader
                true, // displayAddButton
                true  // displayRemoveButton
            );

            _modelNames.drawHeaderCallback = (Rect rect) => {  
                EditorGUI.LabelField(rect, "Model Names");
            };

            _modelNames.drawElementCallback =
                (Rect frame, int index, bool isActive, bool isFocused) => {
                    var rect = frame;
                    rect.y += 2;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    var element = _modelNames.serializedProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, element, GUIContent.none);
                };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_modelIndex1);
            EditorGUILayout.PropertyField(_modelIndex2);
            EditorGUILayout.PropertyField(_mixParameter);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_feedbackRate);
            EditorGUILayout.PropertyField(_noiseInjection1);
            EditorGUILayout.PropertyField(_noiseInjection2);
            EditorGUILayout.PropertyField(_noiseInjection3);

            EditorGUILayout.Space();

            _modelNames.DoLayoutList();

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
