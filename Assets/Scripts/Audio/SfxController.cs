using System;
using System.Collections.Generic;
using UnityEngine;
using Architecture;

public class SfxController : MonoBehaviour, IPooedObject
{
  public Action<IPooedObject> OnDisabled { get; set; }
  AudioSource source;

  float remainingPlayTime;

  public void PlaySound(AudioClip clip)
  {
    this.source.Stop();
    this.source.clip = clip;
    this.source.Play();
    this.remainingPlayTime = clip.length;
  }

  void Awake()
  {
    this.source = this.GetComponent<AudioSource>();
  }

  // Update is called once per frame
  void Update()
  {
    this.remainingPlayTime -= Time.deltaTime;
    if (this.remainingPlayTime <= 0) {
      this.source.Stop();
      this.source.clip = null;
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
