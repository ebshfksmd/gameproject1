using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSwitcher : MonoBehaviour
{
    [Header("Players (순서 중요)")]
    public Player[] players;

    private PlayerTest[] tests;
    private int currentIndex = 0;

    [Header("걷기 전환 설정")]
    public float switchOffset = 1f;
    public float walkInSpeed = 3f;

    [Header("왼쪽 메인 UI")]
    public Image playerIcon;
    public Sprite[] playerIconImage;
    public Image[] skillIcons = new Image[4];
    public Sprite[] allSkillSprites = new Sprite[16];
    public TextMeshProUGUI playerNameText;

    private readonly string[] playerNames = { "이 현 | 딜러", "이도은 | 누커", "장원철 | 탱커", "장예린 | 힐러" };

    [Header("Health UI 연결")]
    public Slider[] hpSliders = new Slider[4];
    public TextMeshProUGUI[] hpTexts = new TextMeshProUGUI[4];
    public HealthBarUI[] otherPlayerUIs;
    [SerializeField] private HealthBarUI currentPlayerUI;

    [Header("Game Over창")]
    public GameObject GameOver;
    public GameObject []CurrentObj;
    public GameObject UI;

    public static PlayerSwitcher instance;
    private Coroutine walkInRoutine = null;
    private List<int> deathOrder = new List<int>();

    void Awake()
    {
        instance = this;
        tests = new PlayerTest[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            tests[i] = players[i]?.GetComponent<PlayerTest>();
        }
    }

    void Start()
    {
        currentIndex = FindNextAlive(-1);
        ActivatePlayer(currentIndex);
        UpdateMainUI(currentIndex);
        UpdateMainHealthUI();
        UpdateAllPlayerHealthUI();
        UpdateOtherPlayerUIs();
        SetPlayerTestInstance(currentIndex);
    }

    void Update()
    {
        if (DialogueIsActive()) return;

        if (tests[currentIndex].hp <= 0)
        {
            HandleDeath(currentIndex);
            return;
        }

        var currentPlayer = players[currentIndex];
        if (!currentPlayer.IsGrounded) return;
        if (PlayerSkillController.IsCastingSkill) return;

        if (Input.GetKeyDown(KeyCode.Tab))
            SwitchToNextPublic();
    }

    private bool DialogueIsActive()
    {
        DialogueManager dm = Object.FindFirstObjectByType<DialogueManager>();
        return dm != null && dm.IsDialogueActive();
    }

    private void SetPlayerTestInstance(int idx)
    {
        if (players[idx].TryGetComponent(out PlayerTest pt))
            PlayerTest.instance = pt;
    }

    public void OnPlayerDeath(GameObject deadPlayer)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].gameObject == deadPlayer)
            {
                HandleDeath(i);
                break;
            }
        }
    }

    private void HandleDeath(int deadIndex)
    {
        if (!deathOrder.Contains(deadIndex))
            deathOrder.Add(deadIndex);

        if (currentIndex == deadIndex)
        {
            int next = FindNextAlive(deadIndex);
            if (next != deadIndex && tests[next] != null && tests[next].hp > 0)
            {
                ForceSwitchTo(next);
            }
            else
            {
                Debug.LogWarning($"[HandleDeath] {playerNames[deadIndex]} 사망. 전환할 생존자가 없습니다.");
                Debug.LogWarning("게임 오버");
                UI.SetActive(false);

                for (int i = 0; i < 5; i ++){
                    CurrentObj[i].SetActive(false);
                }
                GameOver.SetActive(true);
            }
        }

        UpdateOtherPlayerUIs();
    }

    private int FindNextAlive(int startIndex)
    {
        int total = players.Length;
        for (int offset = 1; offset < total; offset++)
        {
            int idx = (startIndex + offset) % total;
            if (tests[idx].hp > 0)
                return idx;
            else
                Debug.Log($"[TAB] {playerNames[idx]}는 죽었으므로 건너뜀.");
        }

        Debug.Log("[TAB] 생존자가 없음.");
        return startIndex;
    }

    public void SwitchToNextPublic()
    {
        int total = players.Length;
        for (int offset = 1; offset < total; offset++)
        {
            int idx = (currentIndex + offset) % total;
            if (tests[idx].hp > 0)
            {
                ForceSwitchTo(idx);
                return;
            }
        }

    }

    private void ForceSwitchTo(int nextIndex)
    {
        var prev = players[currentIndex];
        var next = players[nextIndex];

        currentIndex = nextIndex;

        prev.canControl = false;
        prev.gameObject.SetActive(false);
        next.gameObject.SetActive(true);
        next.canControl = false;

        Vector3 facing = prev.transform.localScale.x >= 0 ? Vector3.right : Vector3.left;
        Vector3 spawnPos = prev.transform.position - facing * switchOffset;
        next.transform.position = spawnPos;

        if (walkInRoutine != null) StopCoroutine(walkInRoutine);
        walkInRoutine = StartCoroutine(WalkIn(next, prev.transform.position));

        Monster.target = next.transform;
        SetPlayerTestInstance(currentIndex);

        UpdateMainUI(currentIndex);
        UpdateMainHealthUI();
        UpdateAllPlayerHealthUI();
        UpdateOtherPlayerUIs();
    }

    private void ActivatePlayer(int index)
    {
        for (int i = 0; i < players.Length; i++)
        {
            bool isActive = (i == index);
            players[i].gameObject.SetActive(isActive);
            players[i].canControl = isActive;
        }
        Monster.target = players[index].transform;
    }

    private IEnumerator WalkIn(Player next, Vector3 targetPos)
    {
        var rb = next.GetComponent<Rigidbody2D>();
        var anim = next.GetComponent<Animator>();
        float baseScale = Mathf.Abs(next.transform.localScale.x);

        Vector3 dir = (targetPos - next.transform.position).normalized;
        next.transform.localScale = dir.x >= 0
            ? new Vector3(baseScale, baseScale, baseScale)
            : new Vector3(-baseScale, baseScale, baseScale);

        if (anim) anim.SetBool("isWalking", true);

        while (Vector3.Distance(next.transform.position, targetPos) > 0.05f)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, walkInSpeed * Time.deltaTime);
            rb.MovePosition(newPos);
            yield return null;
        }

        if (anim) anim.SetBool("isWalking", false);
        next.canControl = true;
        walkInRoutine = null;
    }

    private void UpdateMainUI(int index)
    {
        if (playerIcon != null && playerIconImage.Length > index)
            playerIcon.sprite = playerIconImage[index];

        UpdateSkillIcons(index);

        if (playerNameText != null && index < playerNames.Length)
            playerNameText.text = playerNames[index];
    }

    private void UpdateMainHealthUI()
    {
        if (tests[currentIndex] == null || currentPlayerUI == null) return;

        currentPlayerUI.Init(tests[currentIndex], playerIconImage[currentIndex], playerNames[currentIndex], flipX: false);
    }

    private void UpdateSkillIcons(int playerIndex)
    {
        int start = playerIndex * 4;
        for (int i = 0; i < skillIcons.Length; i++)
        {
            if (start + i < allSkillSprites.Length && allSkillSprites[start + i] != null)
            {
                skillIcons[i].sprite = allSkillSprites[start + i];
                skillIcons[i].enabled = true;
            }
            else
            {
                skillIcons[i].sprite = null;
                skillIcons[i].enabled = false;
            }
        }
    }

    private void UpdateAllPlayerHealthUI()
    {
        int n = players.Length;
        for (int i = 0; i < n; i++)
        {
            int uiSlot = (i - currentIndex + n) % n;
            if (tests[i] != null && tests[i].hp > 0)
            {
                tests[i].SetHealthUI(hpSliders[uiSlot], hpTexts[uiSlot]);
            }
        }
    }

    private void UpdateOtherPlayerUIs()
    {
        int total = 4;
        List<int> clockwise = new List<int>();
        List<int> alive = new List<int>();

        for (int offset = 1; offset <= 3; offset++)
        {
            int idx = (currentIndex + offset) % total;
            clockwise.Add(idx);

            if (tests[idx].hp > 0)
                alive.Add(idx);
        }

        List<int> dead = new List<int>();
        foreach (int idx in deathOrder)
        {
            if (clockwise.Contains(idx))
                dead.Add(idx);
        }

        int deadCount = dead.Count;
        int uiIdx = 0;

        for (int i = 0; i < alive.Count && uiIdx < (3 - deadCount); i++, uiIdx++)
        {
            int idx = alive[i];
            SetRightUI(uiIdx, idx);
        }

        for (int i = 0; i < dead.Count && uiIdx < 3; i++, uiIdx++)
        {
            int slot = 2 - (dead.Count - 1 - i);
            int idx = dead[i];
            SetRightUI(slot, idx);
            Debug.Log($"[UI] 죽은 캐릭터 {playerNames[idx]} 오른쪽 슬롯 {slot}에 고정됨.");
        }

        for (int i = 0; i < 3; i++)
        {
            if (!otherPlayerUIs[i].gameObject.activeSelf)
                otherPlayerUIs[i].gameObject.SetActive(false);
        }
    }

    private void SetRightUI(int slot, int playerIdx)
    {
        var icon = playerIconImage[playerIdx];
        var pt = tests[playerIdx];
        bool isDead = pt.hp <= 0;

        otherPlayerUIs[slot].SetProfileImage(icon, flipX: true, newName: playerNames[playerIdx]);

        if (pt != null)
        {
            int curHp = pt.hp;
            int maxHp = 100; // 필요 시 pt.MaxHp로 교체
            otherPlayerUIs[slot].hpSource = pt;

            var slider = otherPlayerUIs[slot].GetComponentInChildren<Slider>();
            if (slider != null)
            {
                slider.maxValue = maxHp;
                slider.value = curHp;
            }
        }

        otherPlayerUIs[slot].SetGrayscale(isDead);
        otherPlayerUIs[slot].gameObject.SetActive(true);
    }
}
