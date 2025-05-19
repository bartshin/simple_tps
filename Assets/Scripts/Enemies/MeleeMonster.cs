using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeMonster : Monster
{
  enum State
  {
    Idle,
    ChasingPlayer,
    InCombat
  }

  public override int MAX_HP => this.maxHp;
  [SerializeField]
  int maxHp;
  [SerializeField]
  NavMeshAgent navMeshAgent;
  Coroutine currentRoutine;
  YieldInstruction idleWait = new WaitForSeconds(1f);
  float detectDistance = 5f;
  Transform player;
  State state;

  protected override void Awake()
  {
    base.Awake();
    if (this.navMeshAgent == null) {
      this.navMeshAgent = this.GetComponent<NavMeshAgent>();
    }
    this.state = State.Idle;
  }

  // Start is called before the first frame update
  void Start()
  {
    this.player = GameObject.FindWithTag("Player").transform;
  }

  // Update is called once per frame
  void Update()
  {
    if (this.currentRoutine == null && this.player != null) {
      this.currentRoutine = this.StartCoroutine(
          this.ChooseRoutine(this.state));
    }
  }

  IEnumerator ChooseRoutine(State state)
  {
    switch (state)
    {
      case State.Idle:
        return (this.OnIdle());
      default:
        throw (new NotImplementedException());
    }
  }

  IEnumerator OnIdle()
  {
    var distToPlayer = Vector3.Distance(
        this.player.position,
        this.transform.position
        );
    if (distToPlayer < this.detectDistance) {
      this.state = State.ChasingPlayer;    
    }
    else {
      yield return (idleWait);
    }
  }

  IEnumerator OnChase()
  {
    this.RotateTo(new Vector2(
          this.player.position.x,
          this.player.position.z
          ));
    this.navMeshAgent.SetDestination(this.player.position);
    yield return (null); 
  }

  void RotateTo(Vector2 dir)
  {
    this.transform.LookAt(dir);
  }
}
