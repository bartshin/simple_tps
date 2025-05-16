using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architecture;

public class AudioManager : SingletonBehaviour<AudioManager>
{

  const int DEFAULT_SFX_POOL_SIZE = 10;
  [SerializeField]
  GameObject sfxControllerPrefab;
  ObjectPool<SfxController> sfxPool;

  public SfxController GetSfxController()
  {
    return (this.sfxPool.Get());
  }

  void Awake()
  {
    base.OnAwake();
    this.sfxPool = new MonoBehaviourPool<SfxController>(
        poolSize: DEFAULT_SFX_POOL_SIZE,
        prefab: sfxControllerPrefab);
  }
}
