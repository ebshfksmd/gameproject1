using UnityEngine;

public abstract class SkillSO : ScriptableObject
{
    [Header("Skill Info")]
    public string skillName;
    public Sprite icon;
    public float range;
    public string animationName;

    [Header("Cast Settings")]
    [Tooltip("��ų ������ �ɸ��� �ð�(��)")]
    public float castTime = 0f;

    [Header("Cooldown")]
    [Tooltip("��ų ��� �� ���� ��� �ð�(��)")]
    public float cooldown = 5f;

    // ���� ��ų ȿ���� ���
    public abstract void Cast(Transform caster, KeyCode keyUsed);
}