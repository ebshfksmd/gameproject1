using UnityEngine;
using System.Collections.Generic;

public class MonsterDeathWatcher : MonoBehaviour
{
    private static bool firstMonsterKilled = false;
    private static MonsterDeathWatcher instance;

    public GameObject dialogueObject1; // 켤 오브젝트
    public GameObject dialogueObject2; // 끌 오브젝트
    public GameObject[] dialogueTriggers; // 이 중 하나라도 활성화되면 몬스터 멈춤

    private Dictionary<MonoBehaviour, float> monsterSpeeds = new Dictionary<MonoBehaviour, float>();
    private bool isMonstersStopped = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        firstMonsterKilled = false;
    }

    void Update()
    {
        if (dialogueTriggers == null || dialogueTriggers.Length == 0) return;

        bool anyDialogueActive = false;
        foreach (var obj in dialogueTriggers)
        {
            if (obj != null && obj.activeSelf)
            {
                anyDialogueActive = true;
                break;
            }
        }

        if (anyDialogueActive && !isMonstersStopped)
        {
            StopAllMonsters();
        }
        else if (!anyDialogueActive && isMonstersStopped)
        {
            RestoreAllMonsters();
        }
    }

    public static void NotifyMonsterKilled()
    {
        if (!firstMonsterKilled && instance != null)
        {
            firstMonsterKilled = true;

            if (instance.dialogueObject2 != null)
                instance.dialogueObject2.SetActive(false);

            Player player = FindObjectOfType<Player>();
            if (player != null)
                player.canControl = false;

            if (instance.dialogueObject1 != null)
                instance.dialogueObject1.SetActive(true);
        }
    }

    private void StopAllMonsters()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters)
        {
            var mono = monster.GetComponent<MonoBehaviour>();
            if (mono == null) continue;

            var speedField = mono.GetType().GetField("speed");
            if (speedField != null)
            {
                float currentSpeed = (float)speedField.GetValue(mono);
                if (!monsterSpeeds.ContainsKey(mono))
                {
                    monsterSpeeds[mono] = currentSpeed;
                    speedField.SetValue(mono, 0f);
                }
            }
        }

        isMonstersStopped = true;
    }

    private void RestoreAllMonsters()
    {
        foreach (var entry in monsterSpeeds)
        {
            var mono = entry.Key;
            if (mono == null) continue;

            var speedField = mono.GetType().GetField("speed");
            if (speedField != null)
            {
                speedField.SetValue(mono, entry.Value);
            }
        }

        monsterSpeeds.Clear();
        isMonstersStopped = false;
    }
}
