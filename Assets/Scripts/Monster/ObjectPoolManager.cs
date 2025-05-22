using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;

    // 몬스터 풀 데이터를 담는 내부 클래스
    private class PoolData
    {
        public IObjectPool<Monster> pool;
        public Monster prefab;
        public int activeCount = 0; // 현재 활성화된 수
        public int maxSize;
    }

    // 몬스터 이름(Key)별 풀 저장소
    private Dictionary<string, PoolData> monsterPools = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    int defaultCapacity;
    int maxPoolSize;
    // 몬스터 풀 초기화
    public void Init(Monster prefab, int _defaultCapacity, int _maxPoolSize)
    {
        defaultCapacity = _defaultCapacity;
        maxPoolSize = _maxPoolSize;

        string key = prefab.name;

        if (monsterPools.ContainsKey(key))
            return;

        var pool = new ObjectPool<Monster>(
            () => {
                Monster mon = Instantiate(prefab);
                mon.PoolKey = key;
                return mon;
            },
            mon => mon.gameObject.SetActive(true),
            mon => mon.gameObject.SetActive(false),
            mon => Destroy(mon.gameObject),
            true, defaultCapacity, maxPoolSize
        );

        monsterPools[key] = new PoolData
        {
            pool = pool,
            prefab = prefab,
            maxSize = _maxPoolSize,
            activeCount = 0
        };

        // 미리 풀 채워놓기
        for (int i = 0; i < defaultCapacity; i++)
        {
            var mon = pool.Get();
            pool.Release(mon);
        }
    }


    // 몬스터 꺼내기
    public Monster GetFromPool(Monster prefab)
    {
        string key = prefab.name;

        if (!monsterPools.ContainsKey(key))
        {
            Debug.LogWarning($"{key} 풀 자동 생성");
            Init(prefab, defaultCapacity, maxPoolSize);
        }

        var data = monsterPools[key];

        // 최대치 초과 시 null 반환 (혹은 로그만 찍고 넘어갈 수 있음)
        if (data.activeCount >= data.maxSize)
        {
            Debug.LogWarning($"{key} 풀의 최대 수({data.maxSize})를 초과하여 생성 차단됨");
            return null;
        }

        var mon = data.pool.Get();
        data.activeCount++;
        return mon;
    }

    // 몬스터 반환
    public void ReturnToPool(Monster monster)
    {
        string key = monster.PoolKey;

        if (monsterPools.ContainsKey(key))
        {
            monsterPools[key].pool.Release(monster);
            monsterPools[key].activeCount--;
        }
        else
        {
            Debug.LogWarning($"{key} 풀을 찾을 수 없어 Destroy 처리됨");
            Destroy(monster.gameObject);
        }
    }


}
