using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{
    //캐릭터 할당
    public GameObject[] characters;
    private int currentCharacterIndex = 0;

    void Start()
    {
        //첫번째만 활성화
        ActivateCharacter(currentCharacterIndex);
    }

    void Update()
    {
        // 탭키 입력 시 캐릭터 전환
        if (Input.GetKeyDown(KeyCode.Tab))
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
}
