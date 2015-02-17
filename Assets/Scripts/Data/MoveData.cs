using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


   public class MoveData
    {
       public string name;
       public int ID;
       public float cooldown { get { return cooldown; } set { cooldown = value; currentCooldown = value; } }
       public float currentCooldown;
       public float chargeTime { get { return chargeTime; } set { chargeTime = value; currentChargeTime = 0; } }
       public float currentChargeTime;
       public bool isCharging;
       public float Force;
       //public IconSlot icon;
       
       public void ResetCharge()
       {
           currentChargeTime = 0;
       }
    }

