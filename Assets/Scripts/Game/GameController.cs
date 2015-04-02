using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



public class GameController : MonoBehaviour
{
	public float currentTime { get; protected set; }
	public float MaxGameTime = 60;
	public MoveAssetDatabase moveAssetDatabase;
	public GameObject player1, player2;
    public AudioSource audioClip;
	public static GameController instance;
    public UILabel clock;
	public UISlider timebar;
    public GameObject GameOverPanel;
    public List<GameObject> WinnerPanel;
    private bool gameover = false;
	public bool suddenDeath;
    public float minutes = 1;
    public float seconds = 0;
    float miliseconds = 0;


    public EnergyBar P1Score;
    public EnergyBar P2Score;
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
        P1Score.SetValueMax(player1.GetComponent<DragonBase>().tails.Count + player2.GetComponent<DragonBase>().tails.Count);
        P2Score.SetValueMax(player1.GetComponent<DragonBase>().tails.Count + player2.GetComponent<DragonBase>().tails.Count);
        P1Score.SetValueCurrent(player1.GetComponent<DragonBase>().tails.Count);
        P2Score.SetValueCurrent(player2.GetComponent<DragonBase>().tails.Count);
		if (!suddenDeath)
			StartCoroutine (Winner ());
	}

	void Update()
	{
        if (!gameover && !suddenDeath)
        {
            P1Score.SetValueMax(player1.GetComponent<DragonBase>().tails.Count + player2.GetComponent<DragonBase>().tails.Count);
            P2Score.SetValueMax(player1.GetComponent<DragonBase>().tails.Count + player2.GetComponent<DragonBase>().tails.Count);

            P1Score.SetValueCurrent(player1.GetComponent<DragonBase>().tails.Count);
            P2Score.SetValueCurrent(player2.GetComponent<DragonBase>().tails.Count);
             if(miliseconds <= 0){
                 if(seconds <= 0){
                     minutes--;
                     seconds = 59;
                 }
                 else if(seconds >= 0){
                     seconds--;
                 }
                 
                 miliseconds = 100;
             }
             
             miliseconds -= Time.deltaTime * 100;
             
             //Debug.Log(string.Format("{0}:{1}:{2}", minutes, seconds, (int)miliseconds));
             clock.text = string.Format("{0}:{1}:{2}", minutes, seconds, (int)miliseconds);
          
        }
		if (!gameover && !suddenDeath)
			timebar.value = (seconds + (miliseconds/100))/MaxGameTime;
	}

	public void SuddenDeathWinner(int player)
	{
 		StartCoroutine (SuddenDeathWinnerStart (player));
	}

	public IEnumerator SuddenDeathWinnerStart(int player)
	{
		gameover = true;
		yield return new WaitForSeconds(1.5f);
		StopCoroutine (Winner ());
		GameOverPanel.SetActive(true);
		if (player == 1) 
		{
			Debug.Log ("Player 1 Wins!");
			WinnerPanel[0].SetActive(true);
		} 
		else if (player == 2) 
		{
			Debug.Log ("Player 2 Wins!");
			WinnerPanel[1].SetActive(true);
		}
	}

	IEnumerator Winner() 
	{
        yield return new WaitForSeconds(minutes*60);
		int p1Tails = player1.GetComponent<DragonBase> ().tails.Count;
		int p2Tails = player2.GetComponent<DragonBase> ().tails.Count;
		player1.GetComponent<MovementController> ().moveSpeed = 0;
		player2.GetComponent<MovementController> ().moveSpeed = 0;
        gameover = true;
		yield return new WaitForSeconds (0.5f);
        GameOverPanel.SetActive(true);
		if (p1Tails > p2Tails) {
			Debug.Log ("Player 1 Wins!");
            WinnerPanel[0].SetActive(true);
		} 
		else if (p1Tails < p2Tails) 
		{
			Debug.Log ("Player 2 Wins!");
            WinnerPanel[1].SetActive(true);

		}
		else
		{
			Debug.Log ("Tie!");
			WinnerPanel[2].SetActive(true);
			GameOverPanel.transform.FindChild("Logo").gameObject.SetActive(false);
		}
	}

}

