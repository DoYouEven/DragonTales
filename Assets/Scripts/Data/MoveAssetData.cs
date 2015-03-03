﻿using UnityEngine;
using System;

[System.Serializable]
public class MoveAssetData
{
    public int ID;
    public string Name;
    public float ChargeTime;
    public float Cooldown;
    public Texture icon;
    public string audioClipName;
    public GameObject VFXPrefab;
    public GameObject MoveUI;

}