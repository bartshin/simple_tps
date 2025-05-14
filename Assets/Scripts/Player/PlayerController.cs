using System;
using System.Collections.Generic;
using UnityEngine;
using Architecture;

public struct PlayerStat
{
  public float MoveAcceleration { get; set; }
  public float MaxSpeed { get; set; }
  public float RotationSpeed { get; set; } 
}

public class PlayerController : MonoBehaviour
{

  PlayerMovement playerMovement;
  [SerializeField]
  Transform aim;
  [SerializeField]
  Transform avatar;
  [SerializeField]
  PlayerStat stat = new PlayerStat { 
    MoveAcceleration = 5f, 
    MaxSpeed = 20f,
    RotationSpeed = 30f
  };

  public ObservableValue<bool> isAiming = new (false);

  void Awake()
  {
    this.playerMovement = this.CreateMovement();
  }

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    this.playerMovement.Update();
  }

  void OnDestory()
  {
    this.isAiming.DestorySelf();
  }

  PlayerMovement CreateMovement()
  {
    if (this.aim == null) { 
      this.aim = this.transform.Find("Aim");
    }
    if (this.avatar == null) {
      this.avatar = this.transform.Find("Avatar");
    }
    var rigidbody = this.avatar.GetComponent<Rigidbody>();

    return (new PlayerMovement(
          rigidbody: rigidbody,
          avatar: this.avatar,
          aim: this.aim,
          isAiming: this.isAiming,
          stat: this.stat
          ));
  }
}
