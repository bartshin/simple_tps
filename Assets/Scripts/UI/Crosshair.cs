using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Architecture;

public class Crosshair : MonoBehaviour
{
  Image icon;
  Material iconMaterial;
  ObservableValue<bool> isAiming;
  const float FADE_ALPHA_STEP = 1.8f;
  const float ZOOM_STEP = 0.5f;
  const float MIN_ZOOM_VALUE = 0.8f;
  bool isFadeIn;
  Color fullAlpha = new Color(1, 1, 1, 1);
  Color transparentAlpha = new Color(1, 1, 1, 0);

  void Awake()
  {
    this.icon = this.GetComponentInChildren<Image>();
    this.iconMaterial = this.icon.material;
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
  }

  // Update is called once per frame
  void Update()
  {
    if (this.isFadeIn) {
      var animateValues = this.iconMaterial.GetVector("_AnimateValues");
      animateValues.x = Mathf.Min(animateValues.x + ZOOM_STEP * Time.deltaTime, 1f);
      animateValues.w += FADE_ALPHA_STEP * Time.deltaTime;
      this.iconMaterial.SetVector("_AnimateValues", animateValues);
      if (animateValues.w >= 1.0f) {
        this.isFadeIn = false;
      }
    }
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
    //this.icon.enabled = true;
    this.isFadeIn = true;
  }

  void Hide()
  {
    this.iconMaterial.SetVector(
        "_AnimateValues",
        new (MIN_ZOOM_VALUE, 0, 0, 0)
        );
    //this.icon.enabled = false;
    this.isFadeIn = false;
  }
}
