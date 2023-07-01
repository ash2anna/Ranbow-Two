using Cinemachine;
using StarterAssets;
using UnityEngine;

namespace Scripts.Weapon
{
    public abstract class Firearms : MonoBehaviour, IWeapon
    {
        //System
        public GameObject Player;//���
        private FirstPersonController fpc;//��һ�˳ƿ�����
        public Recoil recoil;//��������Ŀ¼
        public GameObject cameraRoot;//�����Ŀ¼
        public UIView uiViewer;//ui���
        public GameObject aimCamera;//��׼�����
        public AimSight aimSight;//��׼������


        public Transform MuzzlePoint;
        public Transform CasingPoint;

        public ParticleSystem MuzzlePartical;
        public ParticleSystem CasingPartical;

        public FirearmsAudio audioData;
        public AudioSource audioPlay;

        //״̬
        public float FireRate;
        public int AmmoInMag = 30;
        public int MaxAmmoCarried = 120;
        public int Damage=40;
        public int peek;
        private float peekPos;//����λ��
        private float peekAng;//����Ƕ�

        //��������
        protected int CurrentAmmo;//��ǰ��ҩ
        protected int CurrentMaxAmmoCarried;//��ǰʣ��
        protected float lastFireTime;
        [SerializeField]
        public int recoilAmmo;//����������
        public ReceilPosData receilPosData;
        //������
        protected Animator animator;

        //����
        protected bool Aiming;
        protected bool ToAiming;
        protected int switchNext;
        public int switchCurrent;
        protected bool isReload;
        private bool peeking;
        private bool isSight;
        private bool sightUp;
        private float sightCurrent;
        private float timer;//��ʱִ����


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

        //����
        public void DoAttack()
        {
            Shooting();
        }
        //���
        protected virtual void Shooting()
        {
            if (CurrentAmmo <= 0 || isReload || !IsAllowShooting()) return;

            CurrentAmmo -= 1;
            if (ToAiming) animator.Play("FireAim", 2, 0);
            else animator.Play("Fire", 2, 0);
            //����
            ShootRay();
            //������
            recoil.AddRecoil(recoilAmmo, receilPosData.GetDir(recoilAmmo));
            recoilAmmo++;
            //��Ƶ
            audioPlay.clip = audioData.ShootingAudio;
            audioPlay.Play();
            //ui
            uiViewer.UpdateAmmoMainCurrent(CurrentAmmo);
            //����
            lastFireTime = Time.time;
        }
        protected bool IsAllowShooting()
        {
            return Time.time - lastFireTime > 1 / FireRate;
        }
        //���-���߼��
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
        //����
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
            ////����
            //int loadnum = CurrentAmmo == 0 ? AmmoInMag - CurrentAmmo : AmmoInMag + 1 - CurrentAmmo;
            //CurrentAmmo += loadnum > CurrentMaxAmmoCarried ? CurrentMaxAmmoCarried : loadnum;
            //CurrentMaxAmmoCarried -= loadnum > CurrentMaxAmmoCarried ? CurrentMaxAmmoCarried : loadnum;

            //Ӳ�˻���
            CurrentMaxAmmoCarried -= 30;
            CurrentAmmo = CurrentAmmo < 1 ? 30 : 31;

            isReload = false;
            fpc.allowRun = true;
        }
        //��׼
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

        //�л�����
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

            //�����ƶ�
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
            
            //��׼�ƶ�
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

        //��������
        private void FireInput()
        {
            //���
            if (Input.GetMouseButton(0))
            {
                if (fpc._input.sprint)
                    fpc._input.sprint = false;
                else
                    DoAttack();
            }
            else
            {
                recoilAmmo = 0;//����������
            }
            //��׼
            if (Input.GetMouseButtonDown(1))
            {
                //��Ұ�Ŵ�
                isSight = true;
                sightUp = true;
                sightCurrent = fpc.cameraControl.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView;

                ToAiming = true;
                Aim();
                aimSight.ChangeAimSight(true);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                //��Ұ�Ŵ�
                isSight = true;
                sightUp = false;
                sightCurrent = fpc.cameraControl.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView;


                ToAiming = false;
                Aim();
                aimSight.ChangeAimSight(false);
            }
            //�л�����
            if (Input.mouseScrollDelta.y != 0)
            {
                SwitchWepong();
            }
            //����
            if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }

            
            //��ս
            if (Input.GetKeyDown(KeyCode.V))
            {
                switchNext = 2;
                fpc.armType = Arm.ArmType.attack;
                fpc.switchCurrent = switchCurrent;

                animator.SetTrigger("end");
            }
            //�ظ�
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
            //����
            if (Input.GetKeyDown(KeyCode.F))
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
                RaycastHit hit;
                Physics.Raycast(ray,out hit, 1.5f);

                if (hit.collider)
                {
                    //���䵯ҩ
                    if (hit.collider.CompareTag("SupplyAmmo"))
                    {
                        //CurrentMaxAmmoCarried += CurrentMaxAmmoCarried < MaxAmmoCarried ? 30 : 0;

                        CurrentMaxAmmoCarried += CurrentMaxAmmoCarried < MaxAmmoCarried ? 30:0;//Ӳ�˻���ģʽ

                        if(switchCurrent==0)
                            uiViewer.UpdateAmmoMainLeaft(CurrentMaxAmmoCarried);
                        else
                            uiViewer.UpdateAmmoSideLeaft(CurrentMaxAmmoCarried);
                    }

                    //����ظ�
                    if (hit.collider.CompareTag("SupplyHeal"))
                    {
                        fpc.HealAdd();
                    }

                    //���似��
                    if (hit.collider.CompareTag("SupplySkill"))
                    {
                        fpc.SkillAdd();
                    }
                }
            }
            //����
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