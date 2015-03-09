using UnityEngine;



   public class MoveData
    {
       public string name;
       public Texture icon;
       public int ID;
       private float cooldown;
       private float chargeTime;
       public AudioClip clip;
       public GameObject VFXPrefab;
       public float Cooldown { get { return cooldown; } set { cooldown = value; currentCooldown = value; } }
       public float currentCooldown;
       public float ChargeTime { get { return chargeTime; } set { chargeTime = value; currentChargeTime = 0; } }
       public float currentChargeTime;
       public bool isCharging;
       public float Force;
       //public IconSlot icon;
       
       public void ResetCharge()
       {
           currentChargeTime = 0;
       }
    }

