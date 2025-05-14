using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraHandler : MonoBehaviour
{
  PlayerController player;
  [SerializeField]
  CinemachineVirtualCamera playerCamera;
  [SerializeField]
  CinemachineVirtualCamera playerAimCamera;

  // Start is called before the first frame update
  void Start()
  {
    this.player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    this.player.IsAiming.OnChanged += this.OnChangeIsAiming;
  }

  void OnEnable()
  {
    if (this.player != null) {
      this.player.IsAiming.OnChanged += this.OnChangeIsAiming;
    }
  }

  void OnDisable()
  {
    this.player.IsAiming.OnChanged -= this.OnChangeIsAiming;
  }

  void OnChangeIsAiming(bool isAiming)
  {
    if (isAiming) {
      this.playerAimCamera.Priority = 1;
      this.playerCamera.Priority = 0;
    }
    else {
      this.playerAimCamera.Priority = 0;
      this.playerCamera.Priority = 1;
    }
  }
}
