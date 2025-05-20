using System;
using System.Collections.Generic;
using UnityEngine;
using Architecture;

public class Gun 
{
  public bool IsHot => this.remaingDelay > 0; 
  public Action OnFire;
  public AudioClip FireSound { get; private set; }

  public float ShootingDelay { get; private set; } = 0.2f;
  float remaingDelay = 0;
  public int Damage { get; private set; } 
  public float Range { get; private set; }
  public ObjectPool<ShootParticle> sparkParticles { get; private set; }
  public ObjectPool<ShootParticle> fireParticles { get; private set; }

  public Gun(int damage = 20, float range = 50f)
  {
    this.FireSound = Resources.Load<AudioClip>("ShootSFX");
    this.Damage = damage;
    this.Range = range;
    var sparkPrefab = Resources.Load<GameObject>("SparkParticle");
    this.sparkParticles = new MonoBehaviourPool<ShootParticle>(
      poolSize: 20,
      maxPoolSize: 30,
      prefab: sparkPrefab
      );
    var firePrefab = Resources.Load<GameObject>("GunFireParticle");
    this.fireParticles = new MonoBehaviourPool<ShootParticle>(
      poolSize: 20,
      maxPoolSize: 40,
      prefab: firePrefab
    );
  }

  public void Fire()
  {
    this.remaingDelay = this.ShootingDelay;
    if (this.OnFire != null) {
      this.OnFire.Invoke();
    }
  }

  public ShootParticle GetSpark()
  {
    return (this.sparkParticles.Get());
  }

  public ShootParticle GetFire()
  {
    return (this.fireParticles.Get());
  }

  public void Update()
  {
    this.remaingDelay -= Time.deltaTime;
  }
}
