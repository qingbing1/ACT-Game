using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum BossState
{
    idle,
    attack,
    run
}

public class Enemy : MonoBehaviour
{
    //枚举状态
    private BossState CurrentState = BossState.idle;
    //怪物动画控制器
    private Animator Anim;
    //导航网格
    private NavMeshAgent Agent;
    //判断敌人是否死亡
    public bool IsDead = false;

    private bool doOnce = false;

    private int DieNum = 0;

    //距离
    public float IdleDis = 100f;
    public float AttackDis = 3f;
    //最大血量
    public float HPMax = 100.0f;
    //目前血量
    public float HPNow = 100.0f;
    //玩家位置
    public Transform player;

    public AudioSource AudioSource01;

    public AudioClip AudioClip01;

    public float SoundValue = 1.0f;

    //创建受伤盒子位置
    public Transform GetHitShowLocation;

    //创建的受伤盒子 
    public GameObject GetHitShowObject;

    //创建伤害盒子位置
    public Transform AttackLocation;

    //创建的伤害盒子 
    public GameObject AttackObject;

    public Slider Blood;


    public  void Start()
    {
        HPNow = HPMax;
        //获取组件
        Anim = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        UI();
        LookAtPlayer();
        EnemyState();
        Dead();
    }

    //触发事件 
    public void OnTriggerEnter(Collider other)
    {
        if (!IsDead)
        {
            if(other.gameObject.tag == "CharacterAttackBox")
            {
                GetHit();
            }
        }
    }

    //始终朝向玩家
    public void LookAtPlayer()
    {
        if (!IsDead)
        {
            Vector3 Playerpos = player.position;
            Playerpos.y = transform.position.y;
            transform.LookAt(Playerpos);
        }
    }

    //切换怪物状态
    public void EnemyState()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (!IsDead)
        {
            switch (CurrentState)
            {
                //静止状态
                case BossState.idle:
                    Anim.Play("Run Tree");  
                    Anim.SetFloat("Speed", 0f);
                    Anim.SetBool("Attack", false);
                    Agent.isStopped = true;
                    if (distance > AttackDis && distance <= IdleDis)
                    {
                        CurrentState = BossState.run;
                    }
                    break;

                //追踪状态
                case BossState.run:
                    Agent.isStopped = false;
                    Agent.SetDestination(player.position);
                    Anim.SetFloat("Speed", 1f);
                    Anim.SetBool("Attack", false);
                    if (distance > IdleDis)
                    {
                        CurrentState = BossState.idle;
                    }else if (distance <= AttackDis)
                    {
                        CurrentState = BossState.attack;
                    }
                    break;

                //攻击状态
                case BossState.attack:
                    Agent.isStopped = true;
                    BossAttack();
                    if (distance > AttackDis)
                    {
                        CurrentState = BossState.run;
                    }else 
                    {
                        Anim.SetFloat("Speed", 0f);
                    }
                    break;
            }
        }
        else
        {
            Agent.isStopped = true;
        }
    }

    //攻击          
    public void BossAttack()
    {
        if (Anim.GetBool("Attack"))
        {
            return;
        }
        Anim.SetBool("Attack", true); 
    }

    public void AttackSound()
    {
        if(Random.Range(1, 5) == 4)
        {
            AudioSource01.PlayOneShot(AudioClip01, SoundValue);
        }
        
    }

    //受击
    public void GetHit()
    {
        HPNow = HPNow - Random.Range(15,70);
        

        //生成特效
        Instantiate(GetHitShowObject, GetHitShowLocation.position, GetHitShowLocation.rotation);

        Anim.CrossFade("GetHit", 0.1f);
    }

    public void CreatAttactBox()
    {
        Instantiate(AttackObject, AttackLocation.position, AttackLocation.rotation);
    }

    //死亡
    public void Dead()
    {
        if (HPNow <= 0)
        {
            
            IsDead = true;
            if (!doOnce)
            {
                Anim.CrossFade("dead", 0f);
                Destroy(gameObject, 1.5f);
                doOnce = true;
            }
        }
    }

    public void UI()
    {
        Blood.value = HPNow / HPMax;
    }

    
}

