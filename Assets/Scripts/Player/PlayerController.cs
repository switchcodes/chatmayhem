using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// Hey!
    /// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
    /// Right now it only contains movement and jumping, but it should be pretty easy to expand... I may even do it myself
    /// if there's enough interest. You can play and compete for best times here: https://tarodev.itch.io/
    /// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/GqeHHnhHpz
    /// </summary>
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        // Public for external hooks
        public Vector3 Velocity { get; private set; }
        public FrameInput Input { get; private set; }
        public bool JumpingThisFrame { get; private set; }
        public bool LandingThisFrame { get; private set; }
        public Vector3 RawMovement { get; private set; }
        public bool Grounded => _colDown;

        public GameObject playerModel;
        public StudioEventEmitter fireballEMitter;

        private Vector3 _lastPosition;
        private float _currentHorizontalSpeed, _currentVerticalSpeed;

        // This is horrible, but for some reason colliders are not fully established when update starts...
        private bool _active;
        
        private TrailRenderer trailRenderer;
        public GameObject fireBall;
        void Awake() => Invoke(nameof(Activate), 0.5f);
        void Activate() => _active = true;

        private void Start()
        {
            fireballEMitter = GetComponentInChildren<StudioEventEmitter>();
            _camera = Camera.main;
            trailRenderer = GetComponentInChildren<TrailRenderer>();
            if (trailRenderer != null)
            {
                trailRenderer.enabled = false;
            }
           
        }

        private void Update()
        {
            if (!_active) return;
            // Calculate velocity
            var position = transform.position;
            Velocity = (position - _lastPosition) / Time.deltaTime;
            _lastPosition = position;

            RunCollisionChecks();
            
            if (!currentlyDashing)
            {
                CalculateWalk(); // Horizontal movement
                CalculateJumpApex(); // Affects fall speed, so calculate before gravity
                CalculateGravity(); // Vertical movement
                CalculateJump(); // Possibly overrides vertical
            }
            CalculateDash();
            Fire();

            MoveCharacter(); // Actually perform the axis movement

            UpdateAnimator();
        }

        #region Animations

        private void UpdateAnimator()
        {
            var grounded = false;
            var walking = false;
            switch (_currentVerticalSpeed)
            {
                case > 0:
                    Debug.Log("going up");
                    animator.SetBool("Jump Start", true);
                    animator.SetBool("Jump Down", false);
                    break;
                case < -0:
                {
                    // check if we're close to the ground
                    Physics.Raycast(transform.position, Vector3.down, out var hit, Mathf.Infinity, groundLayer);
                    Debug.Log(hit.distance);
                    if (hit.distance <= 0.5f)
                    {
                        Debug.Log("going down");
                        animator.SetBool("Jump Start", false);
                        animator.SetBool("Jump Down", true);
                    }

                    break;
                }
                default:
                    grounded = true;
                    break;
            }

            switch (_currentHorizontalSpeed)
            {
                case > 0:
                {
                    if (playerModel.transform.localScale.x < 0)
                    {
                        var scale = playerModel.transform.localScale;
                        scale.x *= -1;
                        playerModel.transform.localScale = scale;
                    }

                    walking = true;
                    break;
                }
                case < 0:
                {
                    if (playerModel.transform.localScale.x > 0)
                    {
                        var scale = playerModel.transform.localScale;
                        scale.x *= -1;
                        playerModel.transform.localScale = scale;
                    }
                    walking = true;
                    break;
                }
                case 0:
                {
                    walking = false;
                    break;
                }
            }
            
            animator.SetBool("IsDashing", currentlyDashing);

            animator.SetBool("OnGround", grounded);
            animator.SetBool("IsWalking", walking);
        }

        #endregion


        #region Gather Input

        [SerializeField] Animator animator;

        public void OnMove(InputAction.CallbackContext context)
        {
            var frameInput = Input;
            frameInput.X = context.ReadValue<Vector2>().x;
            Input = frameInput;
            Debug.Log("OnMove");
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            var frameInput = Input;
            frameInput.JumpDown = context.started;
            frameInput.JumpUp = context.canceled;
            if (frameInput.JumpDown)
            {
                _lastJumpPressed = Time.time;
            }
            Input = frameInput;
        }
        
        public void OnDash(InputAction.CallbackContext context)
        {
            var frameInput = Input;
            frameInput.DashUp = context.ReadValueAsButton();
            Debug.Log(frameInput.DashUp);
            Input = frameInput;
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            var frameInput = Input;
            frameInput.Sprint = context.ReadValueAsButton();
            Debug.Log(frameInput.Sprint);
            Input = frameInput;
        }
        
        public void OnFire(InputAction.CallbackContext context)
        {
            var frameInput = Input;
            frameInput.FireDown = context.ReadValueAsButton();
            Debug.Log(frameInput.FireDown);
            Input = frameInput;
        }

        #endregion

        #region Collisions

        [Header("COLLISION")] [SerializeField] private Bounds characterBounds;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private int detectorCount = 3;
        [SerializeField] private float detectionRayLength = 0.1f;

        [SerializeField] [Range(0.1f, 0.3f)]
        private float rayBuffer = 0.1f; // Prevents side detectors hitting the ground

        private RayRange _raysUp, _raysRight, _raysDown, _raysLeft;
        private bool _colUp, _colRight, _colDown, _colLeft;

        private float _timeLeftGrounded;

        // We use these raycast checks for pre-collision information
        private void RunCollisionChecks()
        {
            // Generate ray ranges. 
            CalculateRayRanged();

            // Ground
            LandingThisFrame = false;
            var groundedCheck = RunDetection(_raysDown);
            if (_colDown && !groundedCheck) _timeLeftGrounded = Time.time; // Only trigger when first leaving
            else if (!_colDown && groundedCheck)
            {
                _coyoteUsable = true; // Only trigger when first touching
                LandingThisFrame = true;
            }

            _colDown = groundedCheck;

            // The rest
            _colUp = RunDetection(_raysUp);
            _colLeft = RunDetection(_raysLeft);
            _colRight = RunDetection(_raysRight);

            bool RunDetection(RayRange range)
            {
                return EvaluateRayPositions(range)
                    .Any(point => Physics2D.Raycast(point, range.Dir, detectionRayLength, groundLayer));
            }
        }

        private void CalculateRayRanged()
        {
            // This is crying out for some kind of refactor. 
            var b = new Bounds(transform.position + characterBounds.center, characterBounds.size);

            _raysDown = new RayRange(b.min.x + rayBuffer, b.min.y, b.max.x - rayBuffer, b.min.y, Vector2.down);
            _raysUp = new RayRange(b.min.x + rayBuffer, b.max.y, b.max.x - rayBuffer, b.max.y, Vector2.up);
            _raysLeft = new RayRange(b.min.x, b.min.y + rayBuffer, b.min.x, b.max.y - rayBuffer, Vector2.left);
            _raysRight = new RayRange(b.max.x, b.min.y + rayBuffer, b.max.x, b.max.y - rayBuffer, Vector2.right);
        }


        private IEnumerable<Vector2> EvaluateRayPositions(RayRange range)
        {
            for (var i = 0; i < detectorCount; i++)
            {
                var t = (float)i / (detectorCount - 1);
                yield return Vector2.Lerp(range.Start, range.End, t);
            }
        }

        private void OnDrawGizmos()
        {
            // Bounds
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + characterBounds.center, characterBounds.size);

            // Rays
            if (!Application.isPlaying)
            {
                CalculateRayRanged();
                Gizmos.color = Color.blue;
                foreach (var range in new List<RayRange> { _raysUp, _raysRight, _raysDown, _raysLeft })
                {
                    foreach (var point in EvaluateRayPositions(range))
                    {
                        Gizmos.DrawRay(point, range.Dir * detectionRayLength);
                    }
                }
            }

            if (!Application.isPlaying) return;

            // Draw the future position. Handy for visualizing gravity
            Gizmos.color = Color.red;
            var move = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed) * Time.deltaTime;
            Gizmos.DrawWireCube(transform.position + characterBounds.center + move, characterBounds.size);
        }

        #endregion


        #region Walk

        [Header("WALKING")] [SerializeField] private float acceleration = 90;
        [SerializeField] private float moveClamp = 13;
        [SerializeField] private float deAcceleration = 60f;
        [SerializeField] private float apexBonus = 2;
        [SerializeField] private float sprintMultiplier = 1.5f;

        private void CalculateWalk()
        {
            if (Input.X != 0)
            {
                // Set horizontal move speed
                _currentHorizontalSpeed += Input.X * acceleration * Time.deltaTime;

                // clamped by max frame movement
                var maxFrameMovement = moveClamp * (Input.Sprint ? sprintMultiplier : 1f);
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -maxFrameMovement, maxFrameMovement);

                // Apply bonus at the apex of a jump
                var bonus = Mathf.Sign(Input.X) * apexBonus * _apexPoint;
                _currentHorizontalSpeed += bonus * Time.deltaTime;
            }
            else
            {
                // No input. Let's slow the character down
                _currentHorizontalSpeed =
                    Mathf.MoveTowards(_currentHorizontalSpeed, 0, deAcceleration * Time.deltaTime);
            }

            if (_currentHorizontalSpeed > 0 && _colRight || _currentHorizontalSpeed < 0 && _colLeft)
            {
                // Don't walk through walls
                _currentHorizontalSpeed = 0;
            }
        }

        #endregion

        #region Gravity

        [Header("GRAVITY")] [SerializeField] private float fallClamp = -40f;
        [SerializeField] private float minFallSpeed = 80f;
        [SerializeField] private float maxFallSpeed = 120f;
        private float _fallSpeed;

        private void CalculateGravity()
        {
            if (_colDown)
            {
                // Move out of the ground
                if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
            }
            else
            {
                // Add downward force while ascending if we ended the jump early
                var fallSpeed = _endedJumpEarly && _currentVerticalSpeed > 0
                    ? _fallSpeed * jumpEndEarlyGravityModifier
                    : _fallSpeed;

                // Fall
                _currentVerticalSpeed -= fallSpeed * Time.deltaTime;

                // Clamp
                if (_currentVerticalSpeed < fallClamp) _currentVerticalSpeed = fallClamp;
            }
        }

        #endregion

        #region Jump

        [Header("JUMPING")] [SerializeField] private float jumpHeight = 30;
        [SerializeField] private float jumpApexThreshold = 10f;
        [SerializeField] private float coyoteTimeThreshold = 0.1f;
        [SerializeField] private float jumpBuffer = 0.1f;
        [SerializeField] private float jumpEndEarlyGravityModifier = 3;
        private bool _coyoteUsable;
        private bool _endedJumpEarly = true;
        private float _apexPoint; // Becomes 1 at the apex of a jump
        private float _lastJumpPressed;
        private bool CanUseCoyote => _coyoteUsable && !_colDown && _timeLeftGrounded + coyoteTimeThreshold > Time.time;
        private bool HasBufferedJump => _colDown && _lastJumpPressed + jumpBuffer > Time.time;

        private void CalculateJumpApex()
        {
            if (!_colDown)
            {
                // Gets stronger the closer to the top of the jump
                _apexPoint = Mathf.InverseLerp(jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
                _fallSpeed = Mathf.Lerp(minFallSpeed, maxFallSpeed, _apexPoint);
            }
            else
            {
                _apexPoint = 0;
            }
        }

        private void CalculateJump()
        {
            // Jump if: grounded or within coyote threshold || sufficient jump buffer
            if (Input.JumpDown && CanUseCoyote || HasBufferedJump)
            {
                _currentVerticalSpeed = jumpHeight;
                _endedJumpEarly = false;
                _coyoteUsable = false;
                _timeLeftGrounded = float.MinValue;
                JumpingThisFrame = true;
            }
            else
            {
                JumpingThisFrame = false;
            }

            // End the jump early if button released
            if (!_colDown && Input.JumpUp && !_endedJumpEarly && Velocity.y > 0)
            {
                // _currentVerticalSpeed = 0;
                _endedJumpEarly = true;
            }

            if (!_colUp) return;
            if (_currentVerticalSpeed > 0) _currentVerticalSpeed = 0;
        }

        #endregion
        
        [Header("DASH")] 
        [SerializeField]private float dashPower = 3f;

        private bool currentlyDashing;

        private bool dashedAlready;
        private float currentDashTime = 1f;
        [SerializeField]private float maxDashTime = 1f;
        public bool infiniteDash;
        private void CalculateDash()
        {
            currentDashTime -= Time.deltaTime;
            if (currentlyDashing && currentDashTime <= 0)
            {
                currentlyDashing = false;
                if (trailRenderer != null)
                {
                    trailRenderer.enabled = false;
                }
            }
            if (Grounded)
            {
                dashedAlready = false;
            }
            if ((!infiniteDash && dashedAlready) || !Input.DashUp)
            {
                return;
            }
            Debug.Log("start dashing");
            currentDashTime = maxDashTime;
            currentlyDashing = true;
            if (trailRenderer != null)
            {
                trailRenderer.enabled = true;
            }

            Vector3 mousePos = Mouse.current.position.value;
            mousePos.z = playerModel.transform.position.z - _camera.transform.position.z;
            var worldPos = _camera.ScreenToWorldPoint(mousePos);
            var dir = (worldPos - transform.position).normalized * dashPower;
            _currentHorizontalSpeed = dir.x;
            _currentVerticalSpeed = dir.y;
            Debug.Log(dir);
            dashedAlready = true;
        }
        
        [Header("FIRE")] 
        [SerializeField]private float fireBallSpeed = 5f;

        [SerializeField] private float fireBallMaxCooldown = 0.5f;
        [SerializeField] private Transform fireSpawnPoint;
        private float fireBallCurrentCooldown;
        private void Fire()
        {
            fireBallCurrentCooldown -= Time.deltaTime;
            if (!Input.FireDown || fireBallCurrentCooldown>0)
            {
                return;
            }

            if (fireballEMitter != null)
            {
                fireballEMitter.Play();
            }
            animator.SetTrigger("Fire");
            fireBallCurrentCooldown = fireBallMaxCooldown;
        }

        public void FirePhase2()
        {
            Vector3 mousePos = Mouse.current.position.value;
            mousePos.z = playerModel.transform.position.z - _camera.transform.position.z;
            var worldPos = _camera.ScreenToWorldPoint(mousePos);
            var dir = (worldPos - transform.position).normalized;
            var ball= Instantiate(fireBall, fireSpawnPoint.position, Quaternion.identity);
            ball.GetComponent<FireBall>().ShootFireBall(dir, fireBallSpeed);
        }

        #region Move

        [Header("MOVE")]
        [SerializeField, Tooltip("Raising this value increases collision accuracy at the cost of performance.")]
        private int freeColliderIterations = 10;

        private Camera _camera;

        // We cast our bounds before moving to avoid future collisions
        private void MoveCharacter()
        {
            var pos = transform.position + characterBounds.center;
            RawMovement = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed); // Used externally
            var move = RawMovement * Time.deltaTime;
            var furthestPoint = pos + move;

            // check furthest movement. If nothing hit, move and don't do extra checks
            var hit = Physics2D.OverlapBox(furthestPoint, characterBounds.size, 0, groundLayer);
            if (!hit || currentlyDashing)
            {
                transform.position += move;
                return;
            }

            // otherwise increment away from current pos; see what closest position we can move to
            var positionToMoveTo = transform.position;
            for (var i = 1; i < freeColliderIterations; i++)
            {
                // increment to check all but furthestPoint - we did that already
                var t = (float)i / freeColliderIterations;
                var posToTry = Vector2.Lerp(pos, furthestPoint, t);

                if (Physics2D.OverlapBox(posToTry, characterBounds.size, 0, groundLayer))
                {
                    transform.position = positionToMoveTo;

                    // We've landed on a corner or hit our head on a ledge. Nudge the player gently
                    if (i != 1) return;
                    if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
                    var transform1 = transform;
                    var position = transform1.position;
                    var dir = position - hit.transform.position;
                    position += dir.normalized * move.magnitude;
                    transform1.position = position;

                    return;
                }

                positionToMoveTo = posToTry;
            }
        }

        #endregion
        
        public void IncreaseSpeed()
        {
            dashPower += 2;
        }

        public void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("XRocket"))
            {
                GetComponent<Health>().GetDamaged();
            }
        }
    }
   
}