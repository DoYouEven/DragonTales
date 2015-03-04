using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class GameController : MonoBehaviour
{
	public float currentTime { get; protected set; }
	public float MaxGameTime = 60;
	public MoveAssetDatabase moveAssetDatabase;
	public GameObject player1, player2;
    public AudioSource audioClip;
	public static GameController instance;
	void Awake()
	{
		if(player1 == null || player2 == null)
		{
			Debug.Log("Please Attach players to gamecontroller");
		}
		instance = this;
        audioClip.Play();
	}

	void Start()
	{
      
		StartCoroutine (Winner ());
	}

	void Update()
	{
		currentTime += Time.deltaTime;
	}

	IEnumerator Winner() 
	{
		yield return new WaitForSeconds (MaxGameTime);
		int p1Tails = player1.GetComponent<DragonBase> ().tails.Count;
		int p2Tails = player2.GetComponent<DragonBase> ().tails.Count;

		if (p1Tails > p2Tails) {
			Debug.Log ("Player 1 Wins!");
			// TODO: display who wins
		} 
		else if (p1Tails < p2Tails) 
		{
			Debug.Log ("Player 2 Wins!");
			// TODO: display who wins	
		}
		else
		{
			Debug.Log ("Tie!");
			// TODO: disaply tie game
		}
	}

}

