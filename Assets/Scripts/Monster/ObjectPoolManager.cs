using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{

    public static ObjectPoolManager instance;

    public int defaultCapacity = 20;
    public int maxPoolSize = 30;

    [HideInInspector]
    public static Monster monsterPrefab;




    public IObjectPool<GameObject> Pool { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);


        //Init();
    }

    public void Init(Monster mon)
    {
        Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
        OnDestroyPoolObject, true, defaultCapacity, maxPoolSize);

        // 미리 오브젝트 생성 해놓기
        for (int i = 0; i < defaultCapacity; i++)
        {
            mon = CreatePooledItem().GetComponent<Monster>();
            mon.Pool.Release(mon.gameObject);
        }
    }

    // 생성
    private GameObject CreatePooledItem()
    {
        GameObject poolGo = Instantiate(monsterPrefab.gameObject);
        poolGo.GetComponent<Monster>().Pool = this.Pool;
        return poolGo;
    }

    // 사용
    private void OnTakeFromPool(GameObject poolGo)
    {
        poolGo.SetActive(true);
    }

    // 반환
    private void OnReturnedToPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
    }

    // 삭제
    private void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo);
    }
}