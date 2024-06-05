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
    //ö��״̬
    private BossState CurrentState = BossState.idle;
    //���ﶯ��������
    private Animator Anim;
    //��������
    private NavMeshAgent Agent;
    //�жϵ����Ƿ�����
    public bool IsDead = false;

    private bool doOnce = false;

    private int DieNum = 0;

    //����
    public float IdleDis = 100f;
    public float AttackDis = 3f;
    //���Ѫ��
    public float HPMax = 100.0f;
    //ĿǰѪ��
    public float HPNow = 100.0f;
    //���λ��
    public Transform player;

    public AudioSource AudioSource01;

    public AudioClip AudioClip01;

    public float SoundValue = 1.0f;

    //�������˺���λ��
    public Transform GetHitShowLocation;

    //���������˺��� 
    public GameObject GetHitShowObject;

    //�����˺�����λ��
    public Transform AttackLocation;

    //�������˺����� 
    public GameObject AttackObject;

    public Slider Blood;


    public  void Start()
    {
        HPNow = HPMax;
        //��ȡ���
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

    //�����¼� 
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

    //ʼ�ճ������
    public void LookAtPlayer()
    {
        if (!IsDead)
        {
            Vector3 Playerpos = player.position;
            Playerpos.y = transform.position.y;
            transform.LookAt(Playerpos);
        }
    }

    //�л�����״̬
    public void EnemyState()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (!IsDead)
        {
            switch (CurrentState)
            {
                //��ֹ״̬
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

                //׷��״̬
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

                //����״̬
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

    //����          
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

    //�ܻ�
    public void GetHit()
    {
        HPNow = HPNow - Random.Range(15,70);
        

        //������Ч
        Instantiate(GetHitShowObject, GetHitShowLocation.position, GetHitShowLocation.rotation);

        Anim.CrossFade("GetHit", 0.1f);
    }

    public void CreatAttactBox()
    {
        Instantiate(AttackObject, AttackLocation.position, AttackLocation.rotation);
    }

    //����
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

