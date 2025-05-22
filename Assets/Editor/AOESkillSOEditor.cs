using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AOESkillSO))]
public class AOESkillSOEditor : Editor
{
    // 씬 뷰에 그리기 위해 구독
    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public override void OnInspectorGUI()
    {
        // 기본 인스펙터 그리기
        DrawDefaultInspector();

        // 반경 직접 수정해 보고 싶으면 슬라이더 추가
        AOESkillSO skill = (AOESkillSO)target;
        EditorGUILayout.Space();
        skill.range = EditorGUILayout.Slider("Gizmo Radius", skill.range, 0f, 10f);

        // 변경 사항 저장
        if (GUI.changed)
            EditorUtility.SetDirty(skill);
    }

    // 실제 씬 뷰에 원 그리기
    private void OnSceneGUI(SceneView sceneView)
    {
        AOESkillSO skill = (AOESkillSO)target;
        Handles.color = Color.red;
        // AOESkillSO는 에셋이므로 월드 위치가 없지만,
        // 스킬 발동 시 caster.position 기준으로 이동시키려면
        // 여기서는 (0,0,0) 또는 원하는 위치로 그려봅니다.
        Handles.DrawWireDisc(Vector3.zero, Vector3.forward, skill.range);
    }
}
