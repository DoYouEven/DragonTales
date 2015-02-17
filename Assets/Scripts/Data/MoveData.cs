using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


   public class MoveData
    {
       public string name;
       public int ID;
       private float cooldown;
       private float chargeTime;
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

