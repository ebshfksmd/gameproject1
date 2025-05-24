using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;

    private class PoolData
    {
        public IObjectPool<Monster> pool;
        public Monster prefab;
        public int activeCount = 0;
        public int maxSize;
    }

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
                mon.IsPooled = true; // ← 새로 생성된 몬스터는 풀에서 나온 것
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

        for (int i = 0; i < defaultCapacity; i++)
        {
            var mon = pool.Get();
            pool.Release(mon);
        }
    }

    public Monster GetFromPool(Monster prefab)
    {
        string key = prefab.name;

        if (!monsterPools.ContainsKey(key))
        {
            Debug.LogWarning($"{key} 풀 자동 생성");
            Init(prefab, defaultCapacity, maxPoolSize);
        }

        var data = monsterPools[key];

        if (data.activeCount >= data.maxSize)
        {
            Debug.LogWarning($"{key} 풀의 최대 수({data.maxSize})를 초과하여 생성 차단됨");
            return null;
        }

        var mon = data.pool.Get();
        mon.IsPooled = true; // 반드시 설정
        data.activeCount++;
        return mon;
    }

    public void ReturnToPool(Monster monster)
    {
        if (!monster.IsPooled)
        {
            Destroy(monster.gameObject);
            return;
        }

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
