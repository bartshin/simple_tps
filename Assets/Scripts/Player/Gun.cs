using System;
using System.Collections.Generic;
using UnityEngine;

public class Gun 
{
  public bool IsHot => this.remaingDelay > 0; 
  public Action OnFire;
  public AudioClip FireSound { get; private set; }

  public float ShootingDelay { get; private set; } = 0.5f;
  float remaingDelay = 0;
  public int Damage { get; private set; } 
  public float Range { get; private set; }

  public Gun(int damage = 20, float range = 50f)
  {
    this.FireSound = Resources.Load<AudioClip>("ShootSFX");
    this.Damage = damage;
    this.Range = range;
  }

  public void Fire()
  {
    this.remaingDelay = this.ShootingDelay;
    if (this.OnFire != null) {
      this.OnFire.Invoke();
    }
  }

  public void Update()
  {
    this.remaingDelay -= Time.deltaTime;
  }
}
