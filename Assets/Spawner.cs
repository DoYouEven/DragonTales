using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour
{

    public GameObject spawnobj;
    public float spawnTime = 3f;
    public Transform[] spawnPoints;
    public GameObject[] spawn_prefeb;
    public MoveAssetDatabase data;
    // Use this for initialization
    void Start()
    {

        InvokeRepeating("Spawn", spawnTime, spawnTime);

    }

    // Update is called once per frame
    void Update()
    {


        //  int spawnPoints_num = Random.Range (0,0); //1spawnpoints
        //int prefeb_num = Random.Range (0, 0); //2prefeb
        int spawnPoints_num = Random.Range(0, 1);
        int prefeb_num = Random.Range(0, 1);
        float time = Random.Range(0, 7);

        Instantiate(data.GetByID(r).PowerUpPrefab, new Vector3(160, 1, 150), Quaternion.identity);
        Destroy(GameObject.Find(spawn_prefeb[prefeb_num].tag + "(Clone)"), time);

    }
}