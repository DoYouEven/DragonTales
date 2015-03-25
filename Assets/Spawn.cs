using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour
{

    public GameObject spawnobj;
    public float spawnTime = 3f;
    public Transform[] spawnPoints;
    public GameObject[] spawn_prefeb;
	public GameObject tail_prefab;
    public MoveAssetDatabase data;
    public int maxSpawnCount, maxTailCount;
    private float randomAngle;
    private Vector3 randomSpawnVector;
    public float radius = 70f;
    public static int currentSpawnCount = 0;
	public static int currentTailCount = 0;
    // Use this for initialization
    void Start()
    {
		currentTailCount = 0;
        InvokeRepeating("SpawnPowerUps", 1.0f, 2.0f);
		Debug.Log (maxTailCount + "   " + currentTailCount);
        //StartCoroutine(SpawnPowerUps());
		for (int i = 0; i < maxTailCount; i++)
		{
			if (currentTailCount < maxTailCount)
			{
				GameObject Tail = (GameObject)Instantiate(tail_prefab, RandomPostion2(), Quaternion.identity);
				currentTailCount++;
			}
		}
    }
    public void DestroyPowerUp()
    {
        currentSpawnCount--;
    }
    // Update is called once per frame
    void Update()
    {
		if (currentTailCount < maxTailCount)
		{
			GameObject Tail = (GameObject)Instantiate(tail_prefab, RandomPostion2(), Quaternion.identity);
			currentTailCount++;
		}
        /*
        
        //  int spawnPoints_num = Random.Range (0,0); //1spawnpoints
        //int prefeb_num = Random.Range (0, 0); //2prefeb
        int spawnPoints_num = Random.Range(0, 1);
        int prefeb_num = Random.Range(0, 1);
        float time = Random.Range(0, 7);

        Instantiate(data.GetByID(r).PowerUpPrefab, new Vector3(160, 1, 150), Quaternion.identity);
        Destroy(GameObject.Find(spawn_prefeb[prefeb_num].tag + "(Clone)"), time);
        */
    }

    void SpawnPowerUps()
    {
       
        for (int i = 0; i < maxSpawnCount; i++)
        {
            if (currentSpawnCount < maxSpawnCount)
            {

                int id = Random.Range(1, 6);
                GameObject Powerup = (GameObject)Instantiate(data.GetByID(id).PowerUpPrefab, RandomPostion(), Quaternion.identity);
                currentSpawnCount++;
            }
        }


    }
    IEnumerator Wait(float duration)
    {
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
            yield return 0;
    }
    Vector3 RandomPostion()
    {
        randomAngle = Random.Range(0f, 80);
        int RandRad = (int)Random.Range(0, radius);
        randomSpawnVector.x = Mathf.Sin(randomAngle) * RandRad + transform.position.x;
        randomSpawnVector.z = Mathf.Cos(randomAngle) * RandRad + transform.position.z;
        randomSpawnVector.y = 2.5f;

        return randomSpawnVector;
    }
	Vector3 RandomPostion2()
	{

		randomSpawnVector.x = Random.Range (-radius/2, radius/2) + transform.position.x;
		randomSpawnVector.z = Random.Range (-radius/2, radius/2) + transform.position.z;
		randomSpawnVector.y = 2.5f;
		
		return randomSpawnVector;
	}
}