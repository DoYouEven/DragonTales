using UnityEngine;
using System.Collections;
using System.Collections.Generic;



   public class GameController : MonoBehaviour
    {
       public float currentTime { get; protected set; }
       public float MaxGameTime = 60;



       void Update()
       {
           currentTime += Time.deltaTime;
       }

    }

