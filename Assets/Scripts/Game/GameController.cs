using UnityEngine;
using System.Collections;
using System.Collections.Generic;



   public class GameController : MonoBehaviour
    {
       public float currentTime { get; protected set; }
       public float MaxGameTime = 60;
       public MoveAssetDatabase moveAssetDatabase;

       public static GameController instance;
       void Awake()
       {
           instance = this;
       }
       void Update()
       {
           currentTime += Time.deltaTime;
       }

    }

