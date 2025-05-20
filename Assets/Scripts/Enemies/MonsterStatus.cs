using System;
using System.Collections.Generic;
using UnityEngine;
using Architecture;

public class MonsterStatus 
{
  public ObservableValue<bool> IsMoving;
  public ObservableValue<bool> IsAttacking;
  public ObservableValue<float> MovingSpeed;
  public ObservableValue<(int current, int max)> Hp;
  public float MaxSpeed;

  public MonsterStatus(int maxHp, float maxSpeed)
  {
    this.IsMoving = new (false);
    this.IsAttacking = new (false);
    this.MovingSpeed = new (0f);
    this.Hp = new ((maxHp, maxHp));
    this.MaxSpeed = maxSpeed;
  }

  public static MonsterStatus GetDummy()
  {
    return (new MonsterStatus (
      maxHp: 100,
      maxSpeed: 5f
      ));
  }
}
