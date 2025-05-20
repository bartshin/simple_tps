using System;
using System.Collections.Generic;
using UnityEngine;
using Architecture;

public class InputSettings : SingletonBehaviour<InputSettings>
{
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
  static void BeforeSceneLoad()
  {
    SingletonBehaviour<InputSettings>.Create(); 
  }

  public enum ControlMode {
    KeyboardWithMouse,
    Joystick
  }

  public ControlMode Control = ControlMode.KeyboardWithMouse;
  public float MinVerticalPOV = -40f;
  public float MaxVerticalPOV = 40f;
  public float MouseSpeedForPOV = 60f;

  void Start()
  {
    Cursor.lockState = CursorLockMode.Locked;
  }
}
