using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;

    // ���� Ǯ �����͸� ��� ���� Ŭ����
    private class PoolData
    {
        public IObjectPool<Monster> pool;
        public Monster prefab;
        public int activeCount = 0; // ���� Ȱ��ȭ�� ��
        public int maxSize;
    }

    // ���� �̸�(Key)�� Ǯ �����
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
    // ���� Ǯ �ʱ�ȭ
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

        // �̸� Ǯ ä������
        for (int i = 0; i < defaultCapacity; i++)
        {
            var mon = pool.Get();
            pool.Release(mon);
        }
    }


    // ���� ������
    public Monster GetFromPool(Monster prefab)
    {
        string key = prefab.name;

        if (!monsterPools.ContainsKey(key))
        {
            Debug.LogWarning($"{key} Ǯ �ڵ� ����");
            Init(prefab, defaultCapacity, maxPoolSize);
        }

        var data = monsterPools[key];

        // �ִ�ġ �ʰ� �� null ��ȯ (Ȥ�� �α׸� ��� �Ѿ �� ����)
        if (data.activeCount >= data.maxSize)
        {
            Debug.LogWarning($"{key} Ǯ�� �ִ� ��({data.maxSize})�� �ʰ��Ͽ� ���� ���ܵ�");
            return null;
        }

        var mon = data.pool.Get();
        data.activeCount++;
        return mon;
    }

    // ���� ��ȯ
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
            Debug.LogWarning($"{key} Ǯ�� ã�� �� ���� Destroy ó����");
            Destroy(monster.gameObject);
        }
    }


}
