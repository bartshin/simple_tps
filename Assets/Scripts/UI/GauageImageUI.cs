using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Architecture;

public class GauageImageUI : MonoBehaviour
{
  [SerializeField]
  Image gauageImage;
  public ObservableValue<(float, float)> WatchingFloatValue 
  {
    get => this.watchingFloatValue;
    set {
      if (this.watchingFloatValue != null) {
        this.watchingFloatValue.OnChanged -= this.OnValueChanged;
      }
      this.watchingFloatValue = value;
      if (value != null) {
        value.OnChanged += this.OnValueChanged;
      }
    }
  }
  public ObservableValue<(int, int)> WatchingIntValue
  { get => this.watchingIntValue;
    set {
      if (this.watchingIntValue != null) {
        this.watchingIntValue.OnChanged -= this.OnValueChanged; 
      }
      this.watchingIntValue = value;
      if (value != null) {
        value.OnChanged += this.OnValueChanged;
      }
    }
  }

  ObservableValue<(float, float)> watchingFloatValue;
  ObservableValue<(int, int)> watchingIntValue;

  float destFloatValue;
  int destIntValue;

  // Start is called before the first frame update
  void OnEnable()
  {
    if (this.watchingFloatValue != null) {
      this.watchingFloatValue.OnChanged += this.OnValueChanged;
    }
    else if (this.watchingIntValue != null) {
      this.watchingIntValue.OnChanged -= this.OnValueChanged; 
    }
  }

  void OnDisable()
  {
    if (this.watchingFloatValue != null) {
      this.watchingFloatValue.OnChanged -= this.OnValueChanged;
    }
    else if (this.watchingIntValue != null) {
      this.watchingIntValue.OnChanged -= this.OnValueChanged;
    }
  }

  // Update is called once per frame
  void Update()
  {

  }

  void OnValueChanged((float current, float max) value)
  {
    this.gauageImage.fillAmount = Math.Clamp(
        value.current / value.max, 0, 1);
  }
  void OnValueChanged((int current, int max) value) 
  {
    this.gauageImage.fillAmount = Math.Clamp(
        (float)value.current / (float)value.max, 0, 1);
  }
}
