using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour, IGetDamage
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 2.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		// cinemachine
		private float _cinemachineTargetPitch;
		private Recoil recoil;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;


	
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		public StarterAssetsInputs _input;
		public GameObject _mainCamera;
		public GameObject cameraControl;

		private const float _threshold = 0.01f;

		private bool IsCurrentDeviceMouse
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
				return _playerInput.currentControlScheme == "KeyboardMouse";
				#else
				return false;
				#endif
			}
		}



		//动画
		private Animator anima0;
		private Animator anima1;
		private Animator anima2;
		public bool allowRun=true;
		public Arm.ArmType armType;
		public int switchCurrent;

		//进度
		private float Fprogress;
		//翻越
		public bool canClimb;
		public Vector3 climbTargetPosition;
		//UI
		public UIView uiViewer;
		public int health;
		//特殊技能
		private int skillChoice;
		private int currentSkill;
		private int[] skillNum;
		private int[] skillMax;
		private bool finishSkill;
		//回复数量
		public int healNum;
		public int healMax;

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}

			//数组初始化
			skillNum = new int[4];
			skillMax = new int[4];
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
			//recoil = GetComponent<Recoil>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
			_playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;


			//anima0 = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
			anima1 = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>();
			anima2 = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Animator>();


			//初始化设置
			health = 50;
			healNum = 3;
			healMax = 5;
			skillNum[0] = 4;
			skillNum[1] = 2;
			skillMax[0] = 5;
			skillMax[1] = 4;
		}

		private void Update()
		{
			JumpAndGravity();
			GroundedCheck();
			Move();

			PlayerInput();
		}

		private void LateUpdate()
		{
			CameraRotation();
		}

		public void GetDamage(int damage)
        {
			health -= damage;
			

			if (health <= 0)
            {

            }
			else if (health > 100)
            {
				health = 100;
            }

			uiViewer.UpdateHealth(health);
		}

		//输入
        private void PlayerInput()
        {
			//互动
			if (Input.GetKeyDown(KeyCode.F))
            {
				Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.CompareTag("Wall"))
                    {
                        hit.transform.GetComponent<Wall>().Strength();
                    }
                    if (hit.collider.CompareTag("Board"))
                    {
                        if (hit.transform.GetComponent<Board>().hp <= 0)
                        {
                            Vector3 direction = transform.position - hit.transform.position;
                            float dot = Vector3.Dot(direction.normalized, hit.transform.forward);
                            hit.transform.GetComponent<Board>().ReBuild(dot > 0 ? true : false);
                        }

                    }
                }
            }

			//翻越
			if (Input.GetKeyDown(KeyCode.Z))
			{
				Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
				LayerMask mask = 1 << 16;

				if (Physics.Raycast(ray, out RaycastHit hit, 1.5f, mask))
				{
					Vector3 direction = transform.position - hit.transform.position;
					float dot = Vector3.Dot(direction.normalized, transform.forward);
					Vector3 windowPos = new Vector3(hit.transform.position.x, 0, hit.transform.position.z);
					transform.position = hit.transform.parent.position + (dot < 0 ? transform.forward * 3 : -transform.forward * 3);
				}
			}

			//布置特殊技能
            if (Input.GetMouseButtonDown(2))
            {
				finishSkill = false;
                if (!finishSkill)
                {
					if (skillNum[currentSkill] <= 0)
						return;

					switch (currentSkill)
					{
						case 0: SetEDD(); break;
						case 1: SetWifi(); break;
						default: break;
					}
				}
                
            }
			else if (Input.GetMouseButtonUp(2))
			{
				finishSkill = true;
            }

			//切换技能
            if (Input.GetKeyDown(KeyCode.Tab))
            {
				uiViewer.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
			else if (Input.GetKeyUp(KeyCode.Tab))
            {
				currentSkill = skillChoice;
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;

				uiViewer.UpdateSkillCurrent(currentSkill);
				uiViewer.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
			}

		}

		//设置动画器
		private void SetAnimaBool(string boolname,bool b)
        {
            try
            {
				//anima0.SetBool(boolname, b);
				anima1.SetBool(boolname, b);
				//anima2.SetBool(boolname, b);
			}
            catch { }
			try
			{
				anima2.SetBool(boolname, b);
			}
			catch { }
		}

		//回复-消耗
		public bool HealConsume()
        {
			if (healNum <= 0)
				return false;
			else
            {
				healNum--;
				uiViewer.UpdateHealNum(healNum);
				return true;
			}
        }
		//回复-补充
		public bool HealAdd()
		{
			if (healNum < healMax)
			{
				healNum++;
				uiViewer.UpdateHealNum(healNum);
				return true;
			}
			else
			{
				return false;
			}
		}

		//技能-补充
		public void SkillAdd()
        {
			for(int i = 0; i < 2; i++)
            {
                if (skillNum[i] < skillMax[i])
                {
					skillNum[i]++;
					uiViewer.UpdateSkillNum(i,skillNum[i]);
                }
            }
        }
		//技能-切换
		public void SwitchSkill(int i)
        {
			skillChoice = i;
			
			uiViewer.UpdateSkillChoice(i);
        }
		//技能-使用
		private void SetWifi()
        {
			//减少技能使用次数
			skillNum[currentSkill]--;
			uiViewer.UpdateSkillNum(currentSkill, skillNum[currentSkill]);

			GetComponent<SetSkill>().CreatWifi();
		}
		private void SetEDD()
		{
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
			RaycastHit hit;
			LayerMask mask = ~((1 << 15) | (1 << 16));
			if(Physics.Raycast(ray, out hit, 200, mask))
            {
				//减少技能使用次数
				skillNum[currentSkill]--;
				uiViewer.UpdateSkillNum(currentSkill, skillNum[currentSkill]);

				//放置技能
				Vector3 direction = transform.position - hit.transform.position;
				float dot = Vector3.Dot(direction.normalized, hit.transform.forward);
				Vector3 front = dot > 0 ? hit.transform.forward : -hit.transform.forward;

				Vector3 left = hit.transform.parent.position - hit.transform.position;
				dot = Vector3.Dot(left.normalized, hit.transform.right);

				GetComponent<SetSkill>().CreatEDD(hit.collider.tag, hit.transform, hit.point.y, front, dot > 0 ? true : false);

				
			}
		}


		/// <summary>
		/// Unity Control,	修改Move()
		/// </summary>
		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);				

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
				
			}
		}

		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint && allowRun && _input.move.y > 0 ? SprintSpeed : MoveSpeed;
            

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero)
			{
				targetSpeed = 0.0f;
				SetAnimaBool("IsWalk", false);
			}
			else
			{
				SetAnimaBool("IsWalk", true);
			}
			if (_input.sprint && allowRun && _input.move.y > 0)
			{
				SetAnimaBool("IsRun", true);
			}
			else
			{
				SetAnimaBool("IsRun", false);
			}

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				// if we are not grounded, do not jump
				_input.jump = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
}