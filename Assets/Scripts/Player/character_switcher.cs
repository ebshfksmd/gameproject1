using System.Collections;
using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{

    public float flyDuration = 1.5f;
    private bool isFlying = false;

    // 캐릭터 할당
    public GameObject[] characters;
    public GameObject player;
    // 캐릭터 전환 쿨타임 (초)
    public float switchCooldown = 3f;

    private int currentCharacterIndex = 0;
    private float lastSwitchTime = 0f;
    private PlayerMovement pmove;

    void Start()
    {
        pmove = player.GetComponent<PlayerMovement>();
        //첫 번째 캐릭터만 활성화
        ActivateCharacter(currentCharacterIndex);
        lastSwitchTime = Time.time; // 초기 전환 시간 설정
        if(player == null)
        {
            Debug.LogError("Player GameObject가 할당되지 않았습니다!");
        }
        if(pmove ==null)
        {
            Debug.LogError("PlayerMovement 컴포넌트가 없습니다!");
        }
    }


    void Update()
    {
        // 탭키 입력과 쿨타임 체크
        if (Input.GetKeyDown(KeyCode.Tab) && Time.time - lastSwitchTime >= switchCooldown)
        {
            // 현재 캐릭터 위치 저장
            Vector3 currentPosition = characters[currentCharacterIndex].transform.position;

            // 현재 캐릭터 비활성화
            characters[currentCharacterIndex].SetActive(false);

            // 다음 캐릭터 인덱스 계산 (배열의 끝이면 처음으로 돌아감)
            currentCharacterIndex = (currentCharacterIndex + 1) % characters.Length;

            // 다음 캐릭터 활성화
            ActivateCharacter(currentCharacterIndex);

            // 새 캐릭터의 위치를 이전 캐릭터의 위치로 설정
            characters[currentCharacterIndex].transform.position = currentPosition;

            FlyIn();

            // 전환 후 쿨타임 시작을 위해 시간 기록
            lastSwitchTime = Time.time;
        }
    }

    // 지정된 인덱스의 캐릭터만 활성화하는 메서드
    void ActivateCharacter(int index)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == index);
        }
    }

    public void FlyIn()
    {
        pmove.setCanMove(false);
        StartCoroutine(FlyInRoutine());
    }

    private IEnumerator FlyInRoutine()
    {
        isFlying = true;
        Vector3 startPosition;
        float elapsed = 0f;
        if (characters[currentCharacterIndex].transform.localScale == new Vector3(1, 1, 1))
        {
            startPosition = characters[currentCharacterIndex].transform.position + new Vector3(2, 3, 0);
        }
        else
        {
            startPosition = characters[currentCharacterIndex].transform.position + new Vector3(-2, 3, 0);
        }
        Vector3 landingPosition = characters[currentCharacterIndex].transform.position;

        while (elapsed < flyDuration)
        {
            
            elapsed += Time.deltaTime;
            float t = elapsed / flyDuration;

            // 수평 위치는 시작과 착지 위치를 선형 보간
            Vector3 horizontalPos = Vector3.Lerp(startPosition, landingPosition, t);

            Vector3 currentPos = new Vector3(horizontalPos.x, horizontalPos.y, horizontalPos.z);

            characters[currentCharacterIndex].transform.position = currentPos;
            yield return null;
        }
        characters[currentCharacterIndex].transform.position = characters[currentCharacterIndex].transform.parent.position;
        pmove.setCanMove(true);

        isFlying = false;
    }
}