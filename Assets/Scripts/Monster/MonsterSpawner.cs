using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterSpawner : MonoBehaviour
{
    public Transform cameraPos;
    [SerializeField] M_mouse mouse;
    [SerializeField] M_Bat bat;
    [SerializeField] M_Centipede centipede;
    [SerializeField] M_Mantis mantis;
    [SerializeField] M_Mongkey mongkey;
    [SerializeField] M_Rabbit rabbit;
    [SerializeField] M_Summoner summoner;




    IEnumerator SpawnCoroutine(Monster m,float time)
    {
        ObjectPoolManager.monsterPrefab = m;
        ObjectPoolManager.instance.Init(m);
        while(true)
        { 
            yield return new WaitForSeconds(0.5f);
            Spawn(m);
        }
    }



    void Spawn(Monster mon)
    {
        //Instantiate(mon.GameObject,new Vector3(cameraPos.position.x+Random.Range(-8,8),1,0),new Quaternion(0,0,0,0));
        var monster = ObjectPoolManager.instance.Pool.Get();
        monster.gameObject.transform.position = new Vector3(cameraPos.position.x + Random.Range(-8, 8), 1, 0);
    }

    void Start()
    {
        StartCoroutine(SpawnCoroutine(mouse,1));
    }



    void Update()
    {
        
    }
}
