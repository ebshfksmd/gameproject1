using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSwitcher : MonoBehaviour
{
    [Header("Players (order matters)")]
    public Player[] players;

    private Player_health[] healths;
    private int currentIndex = 0;

    [Header("Walk-in Settings")]
    public float switchOffset = 1f;
    public float walkInSpeed = 3f;

    [Header("Player Icon Setting")]
    public Image playerIcon;
    public Sprite[] playerIconImage;

    [Header("Skill UI Settings")]
    public Image[] skillIcons = new Image[4];
    public Sprite[] allSkillSprites = new Sprite[16];

    [Header("Player Name UI")]
    public TextMeshProUGUI playerNameText;

    private readonly string[] playerNames = { "이 현", "이도은", "장예린", "장원철" };

    private Coroutine walkInRoutine = null;

    void Awake()
    {
        healths = new Player_health[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            healths[i] = players[i]?.GetComponent<Player_health>();
        }
    }

    void Start()
    {
        currentIndex = FindNextAlive(-1);
        for (int i = 0; i < players.Length; i++)
        {
            bool isThis = (i == currentIndex);
            players[i].gameObject.SetActive(isThis);
            players[i].canControl = isThis;
        }

        if (playerIcon != null && playerIconImage.Length > currentIndex)
        {
            playerIcon.sprite = playerIconImage[currentIndex];
        }

        UpdateSkillIcons(currentIndex);

        if (playerNameText != null && currentIndex < playerNames.Length)
        {
            playerNameText.text = playerNames[currentIndex];
        }
    }

    void Update()
    {
        if (DialogueIsActive()) return;

        var currentPlayer = players[currentIndex];
        if (currentPlayer != null && !currentPlayer.IsGrounded)
            return;

        if (PlayerSkillController.IsCastingSkill)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchToNextPublic();
        }
    }

    private bool DialogueIsActive()
    {
        DialogueManager dm = FindObjectOfType<DialogueManager>();
        return dm != null && dm.IsDialogueActive();
    }

    public void SwitchToNextPublic()
    {
        int prevIndex = currentIndex;
        int nextIndex = FindNextAlive(prevIndex);
        if (nextIndex == prevIndex) return;

        var prev = players[prevIndex];
        var next = players[nextIndex];

        if (playerIcon != null && playerIconImage.Length > nextIndex)
        {
            playerIcon.sprite = playerIconImage[nextIndex];
        }

        UpdateSkillIcons(nextIndex);

        if (playerNameText != null && nextIndex < playerNames.Length)
        {
            playerNameText.text = playerNames[nextIndex];
        }

        currentIndex = nextIndex;

        prev.canControl = false;
        prev.gameObject.SetActive(false);

        next.gameObject.SetActive(true);
        next.canControl = false;

        Vector3 facing = prev.transform.localScale.x >= 0 ? Vector3.right : Vector3.left;
        Vector3 spawnPos = prev.transform.position - facing * switchOffset;
        next.transform.position = spawnPos;

        if (walkInRoutine != null)
        {
            StopCoroutine(walkInRoutine);
            walkInRoutine = null;
        }

        walkInRoutine = StartCoroutine(WalkIn(next, prev.transform.position));
    }

    private int FindNextAlive(int startIndex)
    {
        int n = players.Length;
        for (int offset = 1; offset <= n; offset++)
        {
            int idx = (startIndex + offset + n) % n;
            var h = healths[idx];
            if (h != null && !h.IsDead)
                return idx;
        }
        return Mathf.Clamp(startIndex, 0, n - 1);
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
            Vector2 newPos = Vector2.MoveTowards(
                rb.position, targetPos, walkInSpeed * Time.deltaTime);
            rb.MovePosition(newPos);
            yield return null;
        }

        if (anim) anim.SetBool("isWalking", false);
        next.canControl = true;
        walkInRoutine = null;
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
}
