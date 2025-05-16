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

  public Gun()
  {
    this.FireSound = Resources.Load<AudioClip>("ShootSFX");
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
