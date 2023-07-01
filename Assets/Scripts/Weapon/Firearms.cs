using Cinemachine;
using StarterAssets;
using UnityEngine;

namespace Scripts.Weapon
{
    public abstract class Firearms : MonoBehaviour, IWeapon
    {
        //System
        public GameObject Player;//玩家
        private FirstPersonController fpc;//第一人称控制器
        public Recoil recoil;//后坐力根目录
        public GameObject cameraRoot;//相机根目录
        public UIView uiViewer;//ui面板
        public GameObject aimCamera;//瞄准镜相机
        public AimSight aimSight;//瞄准镜物体


        public Transform MuzzlePoint;
        public Transform CasingPoint;

        public ParticleSystem MuzzlePartical;
        public ParticleSystem CasingPartical;

        public FirearmsAudio audioData;
        public AudioSource audioPlay;

        //状态
        public float FireRate;
        public int AmmoInMag = 30;
        public int MaxAmmoCarried = 120;
        public int Damage=40;
        public int peek;
        private float peekPos;//侧身位置
        private float peekAng;//侧身角度

        //武器设置
        protected int CurrentAmmo;//当前弹药
        protected int CurrentMaxAmmoCarried;//当前剩余
        protected float lastFireTime;
        [SerializeField]
        public int recoilAmmo;//后坐力计数
        public ReceilPosData receilPosData;
        //动画器
        protected Animator animator;

        //变量
        protected bool Aiming;
        protected bool ToAiming;
        protected int switchNext;
        public int switchCurrent;
        protected bool isReload;
        private bool peeking;
        private bool isSight;
        private bool sightUp;
        private float sightCurrent;
        private float timer;//定时执行器


        protected virtual void Start()
        {
            CurrentAmmo = AmmoInMag;
            CurrentMaxAmmoCarried = MaxAmmoCarried;
            //animator = GetComponentInChildren<Animator>();
            //animator = GetComponentInParent<Animator>();
            animator = GetComponent<Animator>();
            fpc = Player.GetComponent<FirstPersonController>();
            //recoil = GetComponent<Recoil>();
            fpc.allowRun = true;

            //UI
            uiViewer.SwitchWeapon(switchCurrent);
        }

        //攻击
        public void DoAttack()
        {
            Shooting();
        }
        //射击
        protected virtual void Shooting()
        {
            if (CurrentAmmo <= 0 || isReload || !IsAllowShooting()) return;

            CurrentAmmo -= 1;
            if (ToAiming) animator.Play("FireAim", 2, 0);
            else animator.Play("Fire", 2, 0);
            //命中
            ShootRay();
            //后坐力
            recoil.AddRecoil(recoilAmmo, receilPosData.GetDir(recoilAmmo));
            recoilAmmo++;
            //音频
            audioPlay.clip = audioData.ShootingAudio;
            audioPlay.Play();
            //ui
            uiViewer.UpdateAmmoMainCurrent(CurrentAmmo);
            //射速
            lastFireTime = Time.time;
        }
        protected bool IsAllowShooting()
        {
            return Time.time - lastFireTime > 1 / FireRate;
        }
        //射击-射线检测
        protected void ShootRay()
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            RaycastHit[] hits = new RaycastHit[10];
            LayerMask mask = ~((1 << 8) | (1 << 15) | (1 << 16));

            int hitCount = Physics.RaycastNonAlloc(ray, hits, 200,mask);
            if (hitCount > 0)
            {
                int damageReduce = 2;
                //for (int i = 0; i <= hitCount - 1; i++)
                //{
                //    Debug.Log(i + ":" + hits[i].collider.name);
                //}
                //for (int i = 0; i <= hitCount - 1; i++)
                for (int i = hitCount - 1; i >=0; i--)
                {
                    if (damageReduce <= 0) break;

                    //string tag = hits[i].collider.gameObject.tag;
                    //switch (tag)
                    //{
                    //    case "Unbreakable":break;
                    //    default:break;
                    //}

                    if (hits[i].collider.gameObject.CompareTag("Unbreakable"))
                    {
                        break;
                    }

                    if (hits[i].collider.gameObject.CompareTag("AI"))
                    {
                        hits[i].transform.GetComponent<FSM>().GetDamage(Damage * damageReduce / 2);
                        break;
                    }
                    if (hits[i].collider.gameObject.CompareTag("SkillItem"))
                    {
                        hits[i].transform.GetComponent<ItemHealth>().GetDamage(Damage * damageReduce / 2);
                        break;
                    }
                    if (hits[i].collider.gameObject.CompareTag("WallUnit"))
                    {
                        hits[i].transform.GetComponent<WallUnit>().GetDamage();
                        damageReduce--;
                    }
                    if (hits[i].collider.gameObject.CompareTag("BoardUnit"))
                    {
                        hits[i].transform.GetComponent<BoardUnit>().GetDamage(6);
                        damageReduce--;
                    }
                    

                }
            }
        }
        //换弹
        protected void Reload()
        {
            if (CurrentMaxAmmoCarried == 0 || CurrentAmmo == AmmoInMag + 1)
            {
                return;
            }
            isReload = true;
            //animator.SetTrigger(CurrentAmmo > 0 ? "Reload" : "ReloadAll");
            //animator.SetTrigger(CurrentAmmo > 0 ? "GunReload" : "GunReloadAll");
            animator.Play(CurrentAmmo > 0 ? "Reload" : "ReloadAll", 0, 0);
            animator.Play(CurrentAmmo > 0 ? "Reload" : "ReloadAll", 1, 0);
            audioPlay.clip = CurrentAmmo > 0 ? audioData.ReloadAudio : audioData.ReloadAllAudio;
            audioPlay.Play();

            fpc.allowRun = false;
        }
        protected virtual void ReloadEnd()
        {
            ////换弹
            //int loadnum = CurrentAmmo == 0 ? AmmoInMag - CurrentAmmo : AmmoInMag + 1 - CurrentAmmo;
            //CurrentAmmo += loadnum > CurrentMaxAmmoCarried ? CurrentMaxAmmoCarried : loadnum;
            //CurrentMaxAmmoCarried -= loadnum > CurrentMaxAmmoCarried ? CurrentMaxAmmoCarried : loadnum;

            //硬核换弹
            CurrentMaxAmmoCarried -= 30;
            CurrentAmmo = CurrentAmmo < 1 ? 30 : 31;

            isReload = false;
            fpc.allowRun = true;
        }
        //瞄准
        protected void Aim()
        {
            if (ToAiming)
            {
                fpc.allowRun = false;
                animator.SetBool("IsAim", true);
            }
            else
            {
                fpc.allowRun = true;
                animator.SetBool("IsAim", false);
            }
        }
        public void InAim()
        {
            Aiming = true;
        }
        public void OutAim()
        {
            Aiming = false;
            fpc.allowRun = true;
        }

        //切换武器
        protected abstract void SwitchWepong();
        public void SwitchEnd()
        {
            transform.parent.GetChild(switchNext).gameObject.SetActive(true);
            
            if(switchNext==0 || switchNext == 1)
                uiViewer.SwitchWeapon(switchNext);

            gameObject.SetActive(false);
        }


        void Update()
        {
            FireInput();

            //侧身移动
            if (peeking)
            {
                peekPos = Mathf.MoveTowards(peekPos, 0.5f * peek, 1.6f * Time.deltaTime);
                peekAng = Mathf.MoveTowardsAngle(peekAng, -15 * peek, 60f* Time.deltaTime);
                //peekPos = Mathf.Lerp(peekPos, 0.5f * peek, 1 * Time.deltaTime);
                //peekAng = Mathf.LerpAngle(peekAng, -15 * peek, 1 * Time.deltaTime);

                cameraRoot.transform.localPosition = new Vector3(peekPos, 1.6f, 0);
                transform.localEulerAngles = new Vector3(0, 0, peekAng);

                if (peekPos == 0.5f * peek && peekAng == -15 * peek)
                {
                    peeking = false;
                }
            }
            
            //瞄准移动
            if (isSight)
            {
                //float target = Mathf.MoveTowards(sightCurrent, sightUp ? 20 : 40, 1 * Time.deltaTime);
                sightCurrent = Mathf.MoveTowards(sightCurrent, sightUp ? 20 : 40, 60f * Time.deltaTime);
                fpc.cameraControl.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = sightCurrent;

                if (sightCurrent == (sightUp ? 20 : 40))
                    isSight = false;

                fpc.RotationSpeed = sightUp ? 0.8f : 2f;
            }
        }

        //武器输入
        private void FireInput()
        {
            //射击
            if (Input.GetMouseButton(0))
            {
                if (fpc._input.sprint)
                    fpc._input.sprint = false;
                else
                    DoAttack();
            }
            else
            {
                recoilAmmo = 0;//后坐力归零
            }
            //瞄准
            if (Input.GetMouseButtonDown(1))
            {
                //视野放大
                isSight = true;
                sightUp = true;
                sightCurrent = fpc.cameraControl.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView;

                ToAiming = true;
                Aim();
                aimSight.ChangeAimSight(true);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                //视野放大
                isSight = true;
                sightUp = false;
                sightCurrent = fpc.cameraControl.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView;


                ToAiming = false;
                Aim();
                aimSight.ChangeAimSight(false);
            }
            //切换武器
            if (Input.mouseScrollDelta.y != 0)
            {
                SwitchWepong();
            }
            //换弹
            if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }

            
            //近战
            if (Input.GetKeyDown(KeyCode.V))
            {
                switchNext = 2;
                fpc.armType = Arm.ArmType.attack;
                fpc.switchCurrent = switchCurrent;

                animator.SetTrigger("end");
            }
            //回复
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (fpc.HealConsume())
                {
                    switchNext = 2;
                    fpc.armType = Arm.ArmType.heal;
                    fpc.switchCurrent = switchCurrent;

                    animator.SetTrigger("end");
                }
            }
            //互动
            if (Input.GetKeyDown(KeyCode.F))
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
                RaycastHit hit;
                Physics.Raycast(ray,out hit, 1.5f);

                if (hit.collider)
                {
                    //补充弹药
                    if (hit.collider.CompareTag("SupplyAmmo"))
                    {
                        //CurrentMaxAmmoCarried += CurrentMaxAmmoCarried < MaxAmmoCarried ? 30 : 0;

                        CurrentMaxAmmoCarried += CurrentMaxAmmoCarried < MaxAmmoCarried ? 30:0;//硬核换弹模式

                        if(switchCurrent==0)
                            uiViewer.UpdateAmmoMainLeaft(CurrentMaxAmmoCarried);
                        else
                            uiViewer.UpdateAmmoSideLeaft(CurrentMaxAmmoCarried);
                    }

                    //补充回复
                    if (hit.collider.CompareTag("SupplyHeal"))
                    {
                        fpc.HealAdd();
                    }

                    //补充技能
                    if (hit.collider.CompareTag("SupplySkill"))
                    {
                        fpc.SkillAdd();
                    }
                }
            }
            //侧身
            if (Input.GetKeyDown(KeyCode.E))
            {
                peek = peek == 1 ? 0 : 1;
                //cameraRoot.transform.localPosition = new Vector3(0.5f, 1.6f, 0);
                //transform.localEulerAngles = new Vector3(0, 0, -15);

                peekPos = cameraRoot.transform.localPosition.x;
                peekAng = transform.localEulerAngles.z;

                peeking = true;
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                peek = peek == -1 ? 0 : -1;
                //cameraRoot.transform.localPosition = new Vector3(-0.5f, 1.6f, 0);
                //transform.localEulerAngles = new Vector3(0, 0, 15);

                peekPos = cameraRoot.transform.localPosition.x;
                peekAng = transform.localEulerAngles.z;

                peeking = true;
            }
        }

        [System.Serializable]
        public struct ReceilPosData
        {
            public Data[] datas;
            public int GetDir(int ammoCount)
            {
                int maxID = datas.Length - 1;
                int nextID = 0;
                Data dt;
                do
                {
                    dt = datas[nextID];
                    nextID++;
                }
                while (nextID <= maxID && ammoCount > dt.ammo);

                float random = Random.Range(0, 1f);
                if (random < dt.left)
                    return -1;
                else if (random < dt.left + dt.right)
                    return 1;
                return 0;
            }

            [System.Serializable]
            public struct Data
            {
                public int ammo;
                public float left;
                public float right;
            }
        }
    }
}