using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architecture;
using System;

public class ShootParticle : MonoBehaviour, IPooedObject
{
  public Action<IPooedObject> OnDisabled { get; set; }
  [SerializeField]
  ParticleSystem ps;
  float lifeTime;

  void Awake()
  {
    if (this.ps == null) {
      this.ps = this.GetComponent<ParticleSystem>();
    }
  }

  void OnEnable()
  {
    this.lifeTime = this.ps.totalTime;
  }

  // Update is called once per frame
  void Update()
  {
    this.lifeTime -= Time.deltaTime;
    if (this.lifeTime < 0) {
      this.gameObject.SetActive(false);
    }
  }

  void OnDisable()
  {
    if (this.OnDisabled != null) {
      this.OnDisabled.Invoke(this);
    }
  }
}
