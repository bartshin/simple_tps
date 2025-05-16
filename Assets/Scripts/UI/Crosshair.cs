using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Architecture;

public class Crosshair : MonoBehaviour
{
  Image icon;
  ObservableValue<bool> isAiming;

  void Awake()
  {
    this.icon = this.GetComponentInChildren<Image>();
    var player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    this.isAiming = player.IsAiming;
    this.isAiming.OnDestory += () => this.isAiming = null;
  }

  void OnEnable()
  {
    if (this.isAiming != null) {
      this.isAiming.OnChanged += this.OnChangedAiming;
    }
  }

  void OnDisable()
  {
    if (this.isAiming != null) {
      this.isAiming.OnChanged -= this.OnChangedAiming;
    }
  }

  void Start()
  {
    this.Hide();
  }

  // Update is called once per frame
  void Update()
  {

  }

  void OnChangedAiming(bool isAiming) 
  {
    if (isAiming) {
      this.Show();
    }
    else {
      this.Hide();
    }
  }

  void Show()
  {
    this.icon.enabled = true;
  }

  void Hide()
  {
    this.icon.enabled = false;
  }
}
