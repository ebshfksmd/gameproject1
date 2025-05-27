using UnityEngine;

public abstract class SkillSO : ScriptableObject
{
    [Header("Skill Info")]
    public string skillName;
    public Sprite icon;
    public float range;
    public string animationName;

    [Header("Cast Settings")]
    [Tooltip("스킬 시전에 걸리는 시간(초)")]
    public float castTime = 0f;

    [Header("Cooldown")]
    [Tooltip("스킬 사용 후 재사용 대기 시간(초)")]
    public float cooldown = 5f;

    // 실제 스킬 효과만 담당
    public abstract void Cast(Transform caster, KeyCode keyUsed);
}