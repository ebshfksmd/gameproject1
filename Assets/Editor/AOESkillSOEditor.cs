using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AOESkillSO))]
public class AOESkillSOEditor : Editor
{
    // �� �信 �׸��� ���� ����
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
        // �⺻ �ν����� �׸���
        DrawDefaultInspector();

        // �ݰ� ���� ������ ���� ������ �����̴� �߰�
        AOESkillSO skill = (AOESkillSO)target;
        EditorGUILayout.Space();
        skill.range = EditorGUILayout.Slider("Gizmo Radius", skill.range, 0f, 10f);

        // ���� ���� ����
        if (GUI.changed)
            EditorUtility.SetDirty(skill);
    }

    // ���� �� �信 �� �׸���
    private void OnSceneGUI(SceneView sceneView)
    {
        AOESkillSO skill = (AOESkillSO)target;
        Handles.color = Color.red;
        // AOESkillSO�� �����̹Ƿ� ���� ��ġ�� ������,
        // ��ų �ߵ� �� caster.position �������� �̵���Ű����
        // ���⼭�� (0,0,0) �Ǵ� ���ϴ� ��ġ�� �׷����ϴ�.
        Handles.DrawWireDisc(Vector3.zero, Vector3.forward, skill.range);
    }
}
