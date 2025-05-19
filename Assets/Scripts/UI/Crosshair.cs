using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Architecture;

public class Crosshair : MonoBehaviour
{
  PlayerController player;
  Image icon;
  Material iconMaterial;
  const float FADE_ALPHA_STEP = 1.8f;
  const float ENLARGE_STEP = 0.5f;
  const float MIN_SIZE_VALUE = 0.1f;
  const float MAX_SIZE_VALUE = 0.2f;
  bool isFadeIn;
  bool isExpanding;
  bool isShrinking;
  bool isWaving;
  Color defaultColor = new Color(0, 255f/256f, 156f/256f, 1f);
  Color transparentAlpha = new Color(0, 255f/256f, 156f/256f, 0f);

  void Awake()
  {
    this.icon = this.GetComponentInChildren<Image>();
    this.iconMaterial = this.icon.material;
    this.iconMaterial.SetVector("_Color", this.defaultColor);
  }

  void Start()
  {
    if (this.player == null) {
      this.player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
      this.player.IsAiming.OnDestory += () => this.player = null;
      this.AddEventListeners();
    }
  }

  void OnEnable()
  {
    if (this.player != null) {
      this.AddEventListeners();
    }
  }

  void OnDisable()
  {
    if (this.player != null) {
      this.RemoveEventListeners();
    }
  }

  void AddEventListeners()
  {
    this.player.IsAiming.OnChanged += this.OnChangedAiming;
    this.player.OnShooting += this.OnShooting;
    this.player.IsMovinig.OnChanged += this.OnChangedMoving;
  }

  void RemoveEventListeners()
  {
    this.player.IsAiming.OnChanged -= this.OnChangedAiming;
    this.player.OnShooting -= this.OnShooting;
    this.player.IsMovinig.OnChanged -= this.OnChangedMoving;
  }

  // Update is called once per frame
  void Update()
  {
    var animateValues = this.iconMaterial.GetVector("_AnimateValues");
    bool isUpdated = false;
    if (this.isFadeIn) {
      animateValues.w += FADE_ALPHA_STEP * Time.deltaTime;
      this.isFadeIn = animateValues.w < 1.0f;
      isUpdated = true;
    }
    if (this.isShrinking) {
      animateValues.x = Mathf.Max(
          animateValues.x - ENLARGE_STEP * Time.deltaTime, MIN_SIZE_VALUE);
      this.isShrinking = animateValues.x > MIN_SIZE_VALUE;
      isUpdated = true;
    }
    if (isUpdated) {
      this.iconMaterial.SetVector("_AnimateValues", animateValues);
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

  void OnShooting()
  {
    var currentValues = this.iconMaterial.GetVector("_AnimateValues");
    currentValues.x = MAX_SIZE_VALUE;
    currentValues.w = 1f;
    this.iconMaterial.SetVector(
        "_AnimateValues",
        currentValues);
    this.isShrinking = true;
    this.isExpanding = false;
    this.isFadeIn = false;
  }

  void OnChangedMoving(bool isMoving) {
    var currentValues = this.iconMaterial.GetVector("_AnimateValues");
    currentValues.y = isMoving ? 1: 0;
    this.iconMaterial.SetVector("_AnimateValues", currentValues);
  }

  void Show()
  {
    this.isFadeIn = true;
    this.isShrinking = true;
    this.isExpanding = false;
  }

  void Hide()
  {
    var currentValues = this.iconMaterial.GetVector("_AnimateValues");
    currentValues.w = 0;
    this.iconMaterial.SetVector(
        "_AnimateValues",
        currentValues
        );
    this.isFadeIn = false;
    this.isExpanding = false;
    this.isShrinking = false;
  }

  void Expand()
  {
    this.isShrinking = false;
    this.isExpanding = true;
  }

  void Shrink()
  {
    this.isShrinking = true;
    this.isExpanding = false;
  }
}
