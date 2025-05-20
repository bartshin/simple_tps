using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architecture;

public struct PlayerStat
{
  public float Acceleration;
  public float MaxSpeed; 
  public float MovingSpeedWhenAiming;
  public float RotationSpeed;
  public ObservableValue<(int current, int max)> Hp { get; set; }

  public static PlayerStat GetDummy()
  {
    return (new PlayerStat {
      Acceleration = 10f, 
      MaxSpeed = 20f,
      MovingSpeedWhenAiming = 2f,
      RotationSpeed = 30f,
      Hp = new ObservableValue<(int, int)>((100, 100))
    });
  }
}
