using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    //��¼����ֵ
    private float InputH;
    private float InputV;
    //��¼����
    private Vector3 Direction;
    //��ȡ��ҿ�����
    public CharacterController Controller;

    //����ٶ�
    public float moveSpeed = 10;

    public Camera PersonCamera;

    //������ת�Ƕ�����
    private float TurnSmooth;
    //��תʱ��
    public float TurnTime = 0.1f;

    //��ȡ����״̬��
    public Animator Animator01;

    private int ConboNum = 0;

    private bool IsAttacking = false;

    //�������
    public float AttackDelayTime = 0.9f;

    //����ʱ��
    public float DelayAttackNumTime = 3f;

    //��ȡ������Ч���
    public GameObject WeaponEffect;

    //�Ƿ��ܹ��ƶ�
    public bool CanMove = false;

    //��Ծ,�Ƿ��ڵ���
    public bool IsGround = true;

    //�Ƿ�������
    public bool IsDodge = false;

    //�������
    public AudioSource AudioSource01;

    //��һ����������
    public AudioClip AudioClip01;

    //�ڶ�����������
    public AudioClip AudioClip02;

    //��������������
    public AudioClip AudioClip03;

    ////��������
    //public AudioClip[] audioClips;

    //����������С
    public float SoundValue = 1.0f;

    //�����˺�����λ��
    public Transform AttackLocation;

    //�������˺����� 
    public GameObject AttackObject;

    //�������˺���λ��
    public Transform GetHitShowLocation;

    //���������˺��� 
    public GameObject GetHitShowObject;

    //Ѫ��
    public float HPNow = 100;
    public float HPMax = 100;
    //�Ƿ�����
    public bool IsDead = false;

    private bool doOnce = false;

    public Slider Blood;

    public Text BloodNow;
    public Text BloodMax;

    public GameObject DeadUI01;
    public GameObject WinUI01;


    public GameObject[] Enemys;

    private void Awake()
    {
        HPNow = HPMax;
    }

    void Start()
    {
        LockMouse();
    }

    void Update()
    {
        if (!IsDead)
        {
        Move(); 
        DoAttack();
        Jump();
        Dodge();
        }
        Dead();
        MouseControl();
        UI();
        WinUI();
    }  

    private void OnTriggerEnter(Collider other)
    {
        if (!IsDead)
        {
            if(other.gameObject.tag == "EnemyAttackBox")
            {
                GetHit();
            }
        }
    }

    //�����ƶ��¼�
    public void Move()
    {
       
            //��ȡ��ֱ��ˮƽ��
            InputH = Input.GetAxis("Horizontal");
            InputV = Input.GetAxis("Vertical");
            Direction = new Vector3(InputH, 0, InputV);

        if (Direction.magnitude >= 0.01f) //����Ƿ����������
        {
            if (IsAttacking == false && CanMove == true )
            {
                //����
                Animator01.SetBool("isRun", true); 
                Animator01.SetFloat("speed", Direction.magnitude);

                //����Ŀ��Ƕ�
                float TargetAngle = Mathf.Atan2(Direction.x, Direction.z) * Mathf.Rad2Deg + PersonCamera.transform.eulerAngles.y;

                //ƽ���Ƕȹ���
                float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, TargetAngle, ref TurnSmooth, TurnTime);

                //���ý�ɫ��ת
                this.transform.rotation = Quaternion.Euler(0, Angle, 0);

                //�����µ��ƶ�����
                Vector3 MoveDirection = Quaternion.Euler(0, TargetAngle, 0) * Vector3.forward;

                //�ƶ�
                Controller.Move(MoveDirection * moveSpeed * Time.deltaTime);

            }
            else
            {
                Animator01.SetFloat("speed", 0f);
            }
        }
        else
        {
            Animator01.SetBool("isRun", false);
            Animator01.SetFloat("speed", 0f);
        }
        
    }

    public void LockMouse()
    {
        //�ŵ��м�
        Cursor.lockState = CursorLockMode.Locked;
        //���ɼ�
        Cursor.visible = false;
    }

    public void UnLockMouse()
    {
        //�ŵ��м�
        Cursor.lockState = CursorLockMode.Confined;
        //���ɼ�
        Cursor.visible = true;
    }

    public void MouseControl()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            UnLockMouse();
        }else if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            LockMouse();
        }
    }

    public void DoAttack()
    {
        if(IsAttacking == false && IsGround)
        {
            if (Input.GetMouseButtonDown(0) && !IsDodge)
            {
                WeaponEffect.SetActive(true);

                if (ConboNum == 0)
                {
                    Animator01.CrossFade("combo_01_1", 0.1f);
                    ConboNum = 1;
                }
                else if (ConboNum == 1)
                {
                    Animator01.CrossFade("combo_01_2", 0.1f);
                    ConboNum = 2;
                }

                else
                {
                    Animator01.CrossFade("combo_01_3", 0.1f);
                    ConboNum = 0;
                }

                IsAttacking = true;
                CanMove = false;
                Animator01.SetBool("canMove", false);

                Invoke("AttackEnd", AttackDelayTime);

                //���ù���״̬
                CancelInvoke("DelayAttackNum");
                Invoke("DelayAttackNum", DelayAttackNumTime);
            }
        }
        
    }

    public void DelayAttackNum()
    {
        ConboNum = 0;
    }

    public void AttackEnd()
    {
        IsAttacking = false;
        WeaponEffect.SetActive(false);
    }

    public void CharacterCanMove()
    {
        CanMove = true;
        Animator01.SetBool("canMove", true);
    }

    //����
    public void AttackSound(AnimationEvent AnimationEvent1)
    {
        if (AnimationEvent1.intParameter == 1)
        {
            AudioSource01.PlayOneShot(AudioClip01, SoundValue);
        }
        else if (AnimationEvent1.intParameter == 2)
        {
            AudioSource01.PlayOneShot(AudioClip02, SoundValue);
        }
        else if (AnimationEvent1.intParameter == 0)
        {
            AudioSource01.PlayOneShot(AudioClip03, SoundValue);
        }
        else
        {

        }
        
    }

    public void Jump()
    {
        if (IsGround&&Input.GetKeyDown(KeyCode.Space)&&!IsAttacking&&!IsDodge)
        {
            Animator01.SetBool("isGround", false);
            Animator01.CrossFade("jump", 0.1f);
            IsGround = false;
            Invoke("JumpEnd", 1.0f);
        }
    }
    //��Ծ����
    public void JumpEnd()
    {
        IsGround = true;
        Animator01.SetBool("isGround", true);
    }

    //����
    public void Dodge()
    {
        if (IsGround)
        {
            if (Input.GetMouseButtonDown(1) && !IsDodge)
            {
                IsDodge = true;
                Animator01.CrossFade("avoid_back", 0f);
                Invoke("EndDodge", 0.5f);
            }
        }
    }

    public void EndDodge()
    {
        IsDodge = false;
    }

    //�����˺�����
    public void CreatDamage(AnimationEvent AnimationEvent1)
    {
        Instantiate(AttackObject, AttackLocation.position,AttackLocation.rotation);
    }


    //�ܻ�
    public void GetHit()
    {
        if(!IsDodge)
        {
            HPNow = HPNow - Random.Range(5, 10);
         
            //������Ч
            Instantiate(GetHitShowObject, GetHitShowLocation.position, GetHitShowLocation.rotation);
        }
        

        //Animator01.CrossFade("GetHit", 0.1f);
    }

    public void Dead()
    {
        if(HPNow <= 0)
        {
            IsDead = true;
            if (!doOnce)
            {
                Animator01.CrossFade("dead", 0f);
                doOnce = true;
                Invoke("DeadUI", 2.0f);
            }
        }
    }

    public void UI()
    {
        Blood.value = HPNow / HPMax;
        BloodNow.text = HPNow.ToString();
        BloodMax.text = HPMax.ToString();
    }

    public void DeadUI()
    {
        DeadUI01.SetActive(true);
        UnLockMouse();
        Time.timeScale = 0f;
    }

    public void WinUI()
    {
        Enemys = GameObject.FindGameObjectsWithTag("Enemy");
        if (Enemys.Length <= 0)
        {
            if (!doOnce)
            {
                WinUI01.SetActive(true);
                UnLockMouse();
                Time.timeScale = 0f;
                doOnce = true;
            }
            
        }
    }
} 
