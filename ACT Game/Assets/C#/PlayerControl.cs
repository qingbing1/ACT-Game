using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    //记录轴向值
    private float InputH;
    private float InputV;
    //记录方向
    private Vector3 Direction;
    //获取玩家控制器
    public CharacterController Controller;

    //玩家速度
    public float moveSpeed = 10;

    public Camera PersonCamera;

    //计算旋转角度引用
    private float TurnSmooth;
    //旋转时长
    public float TurnTime = 0.1f;

    //获取动画状态机
    public Animator Animator01;

    private int ConboNum = 0;

    private bool IsAttacking = false;

    //攻击间隔
    public float AttackDelayTime = 0.9f;

    //连段时间
    public float DelayAttackNumTime = 3f;

    //获取武器特效组件
    public GameObject WeaponEffect;

    //是否能够移动
    public bool CanMove = false;

    //跳跃,是否处于地面
    public bool IsGround = true;

    //是否在闪避
    public bool IsDodge = false;

    //声音组件
    public AudioSource AudioSource01;

    //第一个攻击声音
    public AudioClip AudioClip01;

    //第二个攻击声音
    public AudioClip AudioClip02;

    //第三个攻击声音
    public AudioClip AudioClip03;

    ////声音数组
    //public AudioClip[] audioClips;

    //播放声音大小
    public float SoundValue = 1.0f;

    //创建伤害盒子位置
    public Transform AttackLocation;

    //创建的伤害盒子 
    public GameObject AttackObject;

    //创建受伤盒子位置
    public Transform GetHitShowLocation;

    //创建的受伤盒子 
    public GameObject GetHitShowObject;

    //血量
    public float HPNow = 100;
    public float HPMax = 100;
    //是否死亡
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

    //人物移动事件
    public void Move()
    {
       
            //获取垂直，水平轴
            InputH = Input.GetAxis("Horizontal");
            InputV = Input.GetAxis("Vertical");
            Direction = new Vector3(InputH, 0, InputV);

        if (Direction.magnitude >= 0.01f) //检测是否有玩家输入
        {
            if (IsAttacking == false && CanMove == true )
            {
                //动画
                Animator01.SetBool("isRun", true); 
                Animator01.SetFloat("speed", Direction.magnitude);

                //计算目标角度
                float TargetAngle = Mathf.Atan2(Direction.x, Direction.z) * Mathf.Rad2Deg + PersonCamera.transform.eulerAngles.y;

                //平滑角度过度
                float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, TargetAngle, ref TurnSmooth, TurnTime);

                //设置角色旋转
                this.transform.rotation = Quaternion.Euler(0, Angle, 0);

                //计算新的移动方向
                Vector3 MoveDirection = Quaternion.Euler(0, TargetAngle, 0) * Vector3.forward;

                //移动
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
        //放到中间
        Cursor.lockState = CursorLockMode.Locked;
        //不可见
        Cursor.visible = false;
    }

    public void UnLockMouse()
    {
        //放到中间
        Cursor.lockState = CursorLockMode.Confined;
        //不可见
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

                //重置攻击状态
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

    //声音
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
    //跳跃结束
    public void JumpEnd()
    {
        IsGround = true;
        Animator01.SetBool("isGround", true);
    }

    //闪避
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

    //生成伤害盒子
    public void CreatDamage(AnimationEvent AnimationEvent1)
    {
        Instantiate(AttackObject, AttackLocation.position,AttackLocation.rotation);
    }


    //受击
    public void GetHit()
    {
        if(!IsDodge)
        {
            HPNow = HPNow - Random.Range(5, 10);
         
            //生成特效
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
