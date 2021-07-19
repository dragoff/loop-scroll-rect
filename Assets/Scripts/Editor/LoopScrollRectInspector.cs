// (c) https://github.com/qiankanglai/LoopScrollRect
// modified version by dragoff

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(LoopScrollRect), true)]
public class LoopScrollRectInspector : Editor
{
    private int index = 0;
    private float speed = 1000;

    private LoopScrollRect scroll;

    private SerializedProperty prefab,
        prefabName,
        isFromResources,
        initCount,
        totalCount,
        initOnStart,
        reverse,
        rubberScale,
        content,
        horizontal,
        vertical,
        movement,
        elasticity,
        inetria,
        decelerationRate,
        scrollSensivity,
        viewport,
        horScrolbar,
        vertScrolbar,
        horScrolbarVisibility,
        vertScrolbarVisibility,
        horScrolbarSpacing,
        vertScrolbarSpacing;

    private void OnEnable()
    {
        scroll = serializedObject.targetObject as LoopScrollRect;
        var prefabSource = serializedObject.FindProperty("PrefabSource");
        prefab = prefabSource.FindPropertyRelative("Prefab");
        prefabName = prefabSource.FindPropertyRelative("PrefabNameFromResources");
        isFromResources = prefabSource.FindPropertyRelative("IsFromResources");
        initCount = prefabSource.FindPropertyRelative("InitPoolSize");
        totalCount = serializedObject.FindProperty("TotalCount");
        initOnStart = serializedObject.FindProperty("InitOnStart");

        
        reverse = serializedObject.FindProperty("ReverseDirection");
        rubberScale = serializedObject.FindProperty("RubberScale");
        content = serializedObject.FindProperty("m_Content");
        horizontal = serializedObject.FindProperty("m_Horizontal");
        vertical = serializedObject.FindProperty("m_Vertical");
        movement = serializedObject.FindProperty("movement");
        elasticity = serializedObject.FindProperty("m_Elasticity");
        inetria = serializedObject.FindProperty("m_Inertia");
        decelerationRate = serializedObject.FindProperty("m_DecelerationRate");
        scrollSensivity = serializedObject.FindProperty("m_ScrollSensitivity");
        viewport = serializedObject.FindProperty("m_Viewport");
        horScrolbar = serializedObject.FindProperty("m_HorizontalScrollbar");
        vertScrolbar = serializedObject.FindProperty("m_VerticalScrollbar");
        horScrolbarVisibility = serializedObject.FindProperty("m_HorizontalScrollbarVisibility");
        vertScrolbarVisibility = serializedObject.FindProperty("m_VerticalScrollbarVisibility");
        horScrolbarSpacing = serializedObject.FindProperty("m_HorizontalScrollbarSpacing");
        vertScrolbarSpacing = serializedObject.FindProperty("m_VerticalScrollbarSpacing");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("=====Source settings=====", EditorStyles.boldLabel);
        isFromResources.boolValue = EditorGUILayout.ToggleLeft("Load from Resource?", isFromResources.boolValue );
        EditorGUILayout.PropertyField(!isFromResources.boolValue ? prefab : prefabName);
        EditorGUILayout.PropertyField(initCount);
        EditorGUILayout.PropertyField(totalCount);
        EditorGUILayout.PropertyField(initOnStart);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("=====Scroll settings=====", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(reverse);
        EditorGUILayout.PropertyField(rubberScale);
        EditorGUILayout.PropertyField(content);
        EditorGUILayout.PropertyField(horizontal);
        EditorGUILayout.PropertyField(vertical);
        EditorGUILayout.PropertyField(movement);
        EditorGUILayout.PropertyField(elasticity);
        EditorGUILayout.PropertyField(inetria);
        EditorGUILayout.PropertyField(decelerationRate);
        EditorGUILayout.PropertyField(scrollSensivity);
        EditorGUILayout.PropertyField(viewport);
        EditorGUILayout.PropertyField(horScrolbar);
        EditorGUILayout.PropertyField(vertScrolbar);
        EditorGUILayout.PropertyField(horScrolbarVisibility);
        EditorGUILayout.PropertyField(vertScrolbarVisibility);
        EditorGUILayout.PropertyField(horScrolbarSpacing);
        EditorGUILayout.PropertyField(vertScrolbarSpacing);

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();

        GUI.enabled = Application.isPlaying;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear"))
        {
            scroll.ClearCells();
        }

        if (GUILayout.Button("Refresh"))
        {
            scroll.RefreshCells();
        }

        if (GUILayout.Button("Refill"))
        {
            scroll.RefillCells();
        }

        if (GUILayout.Button("RefillFromEnd"))
        {
            scroll.RefillCellsFromEnd();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = 45;
        float w = (EditorGUIUtility.currentViewWidth - 100) / 2;
        EditorGUILayout.BeginHorizontal();
        index = EditorGUILayout.IntField("Index", index, GUILayout.Width(w));
        speed = EditorGUILayout.FloatField("Speed", speed, GUILayout.Width(w));
        if (GUILayout.Button("Scroll", GUILayout.Width(45)))
        {
            scroll.SrollToCell(index, speed);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(" If you need scroll infinitely, you can simply set totalCount to a negative number.\n Quick Jump availiable by method <SrollToCell(int index, float speed)>", MessageType.Info);

    }
}