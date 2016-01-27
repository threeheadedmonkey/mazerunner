 using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using UnityEngine.UI;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
		// movement states
        public bool m_IsWalking;
		public bool m_IsSneaking;
		public bool m_IsRunning;
		public bool m_IsStandingStill;
        public bool isSafe;

		private Vector3 oldPosition;

		// movement speeds
        public float m_WalkSpeed;
        public float m_RunSpeed;
		public float m_SneakSpeed;
		public float m_JumpSpeed; //height
		public float speed;

		// character identities
		public bool m_IsRunner;
		public bool m_IsSwimmer;
		public bool m_IsClimber;
		public bool m_IsDefault;
		public float idChangeCoolDown;
		public float idActiveTime;
		public bool isIdChangePossible;

		private float lastIdChangeTime;

		public Slider distanceSlider;


        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
 
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);

			oldPosition = transform.position;
			m_IsDefault = true;
			isIdChangePossible = true;
            isSafe = false;
            lastIdChangeTime = Time.time - idChangeCoolDown;
       }


        // Update is called once per frame
        private void Update()
        {
            RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }


        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            //float speed;
            GetInput(out speed);


            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            // Spieler bewegt sich oder nicht
			if (!CheckMovement ()) {
				m_IsWalking = false;
				m_IsRunning = false;
				m_IsSneaking = false;
				m_IsStandingStill = true;
			} else {
				m_IsStandingStill = false;
			}

			CheckCoolDowns();

			if (isIdChangePossible)
				ChangeIndentity ();

            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);
        }


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


		private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running or sneaking or standing still

			m_IsWalking = !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey (KeyCode.LeftControl);
			m_IsRunning = Input.GetKey (KeyCode.LeftShift);
			m_IsSneaking = Input.GetKey (KeyCode.LeftControl);

#endif
            // set the desired speed to be walking or running
			speed = m_IsWalking ? m_WalkSpeed : m_IsSneaking ? m_SneakSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }


        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }

		private bool CheckMovement() {
			var displacement = transform.position - oldPosition;
			oldPosition = transform.position;

			return displacement.magnitude > 0.001;
		}


		// Identity Change and corresponding parameter changes
		private void ChangeIndentity(){
			if (Input.GetKeyDown ("1")) {
				Debug.Log ("Runner");
				m_IsRunner = true;
				m_IsClimber = false;
				m_IsSwimmer = false;
				m_IsDefault = false;

				m_RunSpeed = 12;
				m_JumpSpeed = 8;

				lastIdChangeTime = Time.time;


			} else if (Input.GetKeyDown ("2")) {
				Debug.Log ("Swimmer");
				m_IsRunner = false;
				m_IsClimber = false;
				m_IsSwimmer = true;
				m_IsDefault = false;

				m_RunSpeed = 8;
				m_JumpSpeed = 8;

				lastIdChangeTime = Time.time;


			} else if (Input.GetKeyDown ("3")) {
				Debug.Log ("Climber");
				m_IsRunner = false;
				m_IsClimber = true;
				m_IsSwimmer = false;
				m_IsDefault = false;

				m_RunSpeed = 8;
				m_JumpSpeed = 15;

				lastIdChangeTime = Time.time;

			}
		}

		private void CheckCoolDowns(){
			//Debug.Log (Time.time - lastIdChangeTime);
			if (m_IsDefault) {
				if (Time.time - lastIdChangeTime >= idChangeCoolDown) {
					isIdChangePossible = true;
				} else {
					isIdChangePossible = false;
				}
			} else if (!m_IsDefault) {
				isIdChangePossible = false;
				if (Time.time - lastIdChangeTime >= idActiveTime) {
					SetDefaultIdentity ();
				}
			
			}
		}

		private void SetDefaultIdentity() {
			m_IsRunner = false;
			m_IsSwimmer = false;
			m_IsClimber = false;
			m_IsDefault = true;

			m_RunSpeed = 10;
			m_JumpSpeed = 8;

        }

		public float GetRemainingIdTime() {
          float remainingIDTime = idActiveTime - (Time.time - lastIdChangeTime);
            if (remainingIDTime > 0) return remainingIDTime;
            else return 0;
		}

		public float GetRemainingCoolDownTime(){
            float remainingCooldown = idChangeCoolDown - (Time.time - lastIdChangeTime);
            if (remainingCooldown > 0) return remainingCooldown;
            else return 0;
    
		}

    }

}
