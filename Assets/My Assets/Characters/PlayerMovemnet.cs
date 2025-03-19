using Unity.VisualScripting;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
// Kiểm tra xem hệ thống Input System có được bật không, nếu có thì import
#endif

[RequireComponent(typeof(CharacterController))] //Yêu cầu GameObject phải có CharacterController.
#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))] //Yêu cầu GameObject phải có PlayerInput.
#endif
public class PlayerMovemnet : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;// tốc độ di chuyển
    [SerializeField]
    private float sprintSpeed;// chạy
    [SerializeField]
    [Range(0f, 0.3f)] // khoảng xoay nhân vật theo hướng di chuyển
    private float rotationSmoothTime = 0.12f;// làm muọt xoay nhân vật theo thời gian.
    [SerializeField]
    private float speedChangeRate;// chỉnh tốc đọ di chuyển.
    [SerializeField]
    private float jumpHeight;// chiều cao nhảy
    [SerializeField]
    private float jumpTimeOut;// thời gian chờ để nhảy tiếp
    [SerializeField]
    private float fallTimeOut;// thời gian để kiểm tra nhân vật đang rơi
    [SerializeField]
    private float gravity;// trọng lực
    [SerializeField]
    private bool Grounded;// kiểm tra trạng thái tiếp đất.
    [SerializeField]
    private float groundedOffset;// kiểm tra khoảng cách với mặt đất
    [SerializeField]
    private float groundedRadius;// bán kính của hình cầu để kiểm tra chạm đất
    [SerializeField]
    private LayerMask GroundedLayers;//kiểm tra những obj có layer là ground.
    [SerializeField]
    private GameObject CinemachineCameraTG; // obj cần target camera
    [SerializeField]
    private float topClamp;//giới hạn quay lên
    [SerializeField]
    private float bottomClamp;// giới hạn quay xuống
    [SerializeField]
    private float cameraAngleOverride;// chỉnh góc cam  aim_default vào nhân vật.
    [SerializeField]
    private bool lockCameraPosition = false;// lock cam ko cho xoay.
    [SerializeField]
    private float airControl;
    [SerializeField]
    private Vector3 targetDirection1;


    //các biến mặc định của nhân vật

    // cinemachine
    private float _cinemachineTargetYaw1;// quay trái phải
    private float _cinemachineTargetPitch1;// quay lên xuống

    // player
    private float _speed;// tốc độ di chuyển mặc định
    private float _animationBlend;// value chuyển đổi giữa state này đến state khác theo tốc độ của player.
    private float _targetRotation = 0.0f;//góc xoay nhân vật theo hướng di chuyển.
    private float _rotationVelocity;// tốc độ xoay
    private float _verticalVelocity;// tốc độ nhảy và rơi.
    private float _terminalVelocity = 53.0f; // giới hạn tốc độ rơi.

    // timeout deltatime
    private float _jumpTimeoutDelta;// kiểm soát tránh spam nhảy lấy biến - time.deltatime >=0 mới cho nảy tiếp.
    private float _fallTimeoutDelta;// thời gian để chuyển sang trang thái rơi. (khi bước xuống cầu thang trong thật nhất)

    // animation IDs
    private int _animIDSpeed;// phát hiện trạng thái chạy và cập nhật giá trị animation theo.
    private int _animIDGrounded;// kiểm tra play trạng thái đứng yên và nhảy/rơi.
    private int _animIDJump;// play animation nhảy và idle.
    private int _animIDFreeFall;// play trạng thái rơi và tiếp đất.
    private int _animIDMotionSpeed;// điều chỉnh tốc độ của animation theo di chuyển của player.

#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif
    private Animator _animator;
    private CharacterController _controller;
    private InputSystem _input;//Dùng để nhận thông tin điều khiển từ người chơi (bàn phím, tay cầm, chuột).
    private GameObject _mainCamera;

    private const float _threshold = 0.01f;//Dùng để bỏ qua những chuyển động rất nhỏ, tránh lỗi rung.

    private bool _hasAnimator;// kiểm tra có animator không.

    private bool IsCurrentDeviceMouse // kiểm tra phải đang dùng phím và chuột
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";// điều kiện true đang dùng.
#else
				return false;
#endif
        }
    }


    private void Awake()
    {
        if (_mainCamera == null)// check null cam
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");// tìm obj có tag "main camera".
        }

    }

    private void Start()
    {
        _cinemachineTargetYaw1 = CinemachineCameraTG.transform.rotation.eulerAngles.y;// giá trị xoay quanh trục y.
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<InputSystem>();
        _hasAnimator = TryGetComponent(out _animator);

#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
#endif


        AssignAnimationIDs();
        // reset our timeouts on start
        _jumpTimeoutDelta = jumpTimeOut;
        _fallTimeoutDelta = fallTimeOut;
    }

    private void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);
        JumpAndGravity();
        GroundedCheck();
        Move();

    }
    private void LateUpdate() // xử lý sau hàm update và fix update
                              // hoạt động khi nhân vật di chuyển xong trong hàm update sau đó late mới xử lý cam di chuyển theo.
    {
        CameraRotation();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");          // chuyển đổi kiểu string thành int tối ưu hoá hơn.
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void GroundedCheck() // kiểm tra va chạm mặt đất
    {
        Vector3 SpherePosion = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        // lấy vị trí theo player
        // (transform.position.y-groundedoffset kiểm tra trên bề mặt gồ ghề luôn nằm dưới chân)

        Grounded = Physics.CheckSphere(SpherePosion, groundedRadius, GroundedLayers, QueryTriggerInteraction.Ignore);
        // tạo hình cầu check
        // querytriggeraction.ignore không kiểm tra bỏ qua các collider trigger.

        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded); // bật trạng thái chạm đất.
        }
    }

    private void CameraRotation() // xoay cam
    {
        if (_input.look.sqrMagnitude >= _threshold && !lockCameraPosition)
        // bình phương hướng di chuyển của cam lớn hơn bằng threshold tránh lỗi rung khi input quá nhỏ.
        // và kiểm tra cam không bị lock.
        {
            // chỉnh tốc độ camera theo bộ điều khiển chuột(1.0f) và tay cầm(time.deltatime).
            // dùng chuột giá trị giữ nguyên nền = 1.0f còn dùng tay cầm cần độ mượt nên dùng deltatime
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw1 += _input.look.x * deltaTimeMultiplier;// cập nhật góc xoay theo trục x xoay ngang
            _cinemachineTargetPitch1 += _input.look.y * deltaTimeMultiplier;// cập nhật góc xoay theo trục y xoay dọc
        }
        // giới hạng góc xoay min và max
        _cinemachineTargetYaw1 = ClampAngle(_cinemachineTargetYaw1, float.MinValue, float.MaxValue);// từ -360 đến 360
        _cinemachineTargetPitch1 = ClampAngle(_cinemachineTargetPitch1, bottomClamp, topClamp);// giới hạn bottomclamp qua topclamp

        //cập nhật camera vào obj target
        CinemachineCameraTG.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch1 + cameraAngleOverride, _cinemachineTargetYaw1, 0f);

    }

    private void Move()
    {
        // nhận biết input chạy và đi bộ
        float targetspeed = _input.sprint ? sprintSpeed : moveSpeed;

        if (_input.move == Vector2.zero) targetspeed = 0.0f;// không có input vào set tốc độ =0.0f để giúp nhân vật dừng tự nhiên.

        float currenHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        // cập nhật value vào X(trái/phải) và Z(tiến/lùi) chỉ khi player trên mặt đất dùng magnitube để cập nhật value vào velocity.
        float speedOfset = 0.1f; // tránh giật lag.
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
        // điều kiện true láy value cập nhật vào bộ điều khiển tay cầm còn lại lấy 1.0f vào keyboardmouse.


        if (currenHorizontalSpeed > targetspeed + speedOfset || currenHorizontalSpeed < targetspeed - speedOfset)
        // nếu tốc độ nhỏ hơn targetspeed thì cho tiếp tục tăng và nếu tốc độ lớn hơn thì giảm lại đến giá trị gần bé hơn bằng target.
        {
            _speed = Mathf.Lerp(currenHorizontalSpeed, targetspeed * inputMagnitude, Time.deltaTime * speedChangeRate);
            //  Mathf.Lerp(A, B, T) giúp dịch chuyển dần giá trị từ A đến B theo tỷ lệ T. (lerp tự động chạy từ A->B và ngừng)
            // clamp thù phải kiểm tra thủ công.
            // A là tốc độ gốc.
            // B tốc độ muốn đạt được
            // T là giá trị tăng dần theo thời gian của A đến B.

            _speed = Mathf.Round(_speed * 1000f) / 1000f;
            // làm tròn 3 chữ số giúp hiển thị gọn hơn dễ nhìn, tránh sai số.
        }
        else
        {
            _speed = targetspeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetspeed, Time.deltaTime * speedChangeRate);
        // chỉnh tốc độ của animation theo tốc độ của nhân vật. (làm mượt animation)
        if (_animationBlend < 0.01f) _animationBlend = 0f;
        // tránh giá trị quá nhỏ làm animation bị ảnh hưởng.

        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
        // tránh lỗi di chuyển chéo bị tốc độ lớn hơn đi thẳng.
        // trục Y trong 2d là tiến/lùi còn X là trái/phải. 

        if (_input.move != Vector2.zero)
        // đang di chuyển
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            // dùng atan2 để tính góc giữa 2 vector và dùng rad2deg đổi qua đơn vị Độ (xoay trên mặt phẳng).
            //, cộng thêm cam xoay theo trục y để xoay nhân vật.(xoay trong không gian theo trục y)
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime);
            // xoay theo trục y đến góc target với một tốc độ xoay theo thời gian.
            transform.rotation = Quaternion.Euler(0f, rotation, 0f);
            //cập nhật góc xoay của nhân vật sang trái và sang phải.

        }
         targetDirection1 = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        // cập nhật hướng di chuyển về phía trước theo góc xoay của camera . 
        _controller.Move(targetDirection1.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        // điều khiển player
        //  _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) code này để di chuyển trên ground
        // new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime); code này điều khiển nhảy.
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            // set state speed có giá trị là animationblend (theo tốc độ của nhân vật). 
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            // set state motion có giá trị là inputmagnitude (bộ điều khiển).
        }


    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            _fallTimeoutDelta = fallTimeOut;
            // trả về thời gian để vào trạng thái rơi (kiểm tra)

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
                // tắt các trạng thái nhảy và rơi
            }
            if (_verticalVelocity < 0f) _verticalVelocity = -2f;
            // đặt cho player 1 vận tốc rơi nhất định tránh lỗi rơi xuyên sàn ở độ cao quá cao.

            if (_input.jump && _jumpTimeoutDelta <= 0f)
            //nhận nhảy và hết thời gian đợi nhảy tiêp
            {
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                // tính toán nhảy và nhân với trừ 2 để tính chính xác nhất với công thức gốc.
                // gravity luôn âm nên cần nhân với số âm để căn bậc luôn dương.
                if (_hasAnimator) _animator.SetBool(_animIDJump, true);
                // bật trạng thái nhảy.
            }
            if (_jumpTimeoutDelta >= 0f) _jumpTimeoutDelta -= Time.deltaTime;
            // thời gian để có thẻ nhảy tiếp nếu lớn hơn 0 thì không cho nhảy và trừ dần về 0 mới nhảy tiếp.

        }
        else
        {
            _jumpTimeoutDelta = jumpTimeOut;
            // kiểm tra trả về trạng thái nhay.

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
                // nếu thời gian rơi lớn 0 thì không cho rơi.
            }
            else
            {
                if (_hasAnimator)
                    _animator.SetBool(_animIDFreeFall, true);
                //bật rơi tự do.
                _controller.Move(AirVelocity());
                _input.jump = false;
            }

            _input.jump = false;
            // đang rơi không cho nhảy tiếp

        }
        
        if (_verticalVelocity < _terminalVelocity) _verticalVelocity += gravity * Time.deltaTime;
        // nếu vận tốc rơi hiện tại bé hơn vận tốc max thì cộng thêm trọng lực theo thời gian.

    }

    private Vector3 AirVelocity()
    {
       return (((transform.forward * Mathf.Abs(_input.move.y)) +(transform.right * Mathf.Abs(_input.move.x)))).normalized *(airControl/100f) ;
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    // dùng static khi hàm chỉ của class và không liên quan đến obj mà chỉ tính toán cơ bản.
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
        // giới hạn góc quay cam chính xác cập nhật vào camrotation.
    }
    private void OnDrawGizmosSelected() // tạo điểm check 1 tính năng
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        // màu xanh có độ trong suốt 35%
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        // đỏ 35%.
        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;
        // chạm đất xanh ngược lại đỏ.
        Gizmos.DrawSphere(
               new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
               groundedRadius);
        //tạo hình cầu có vị trí vector3 của player chỉnh theo trục y và chỉnh được bán kính để check.

    }

    //private void OnFootstep(AnimationEvent animationEvent)
    //{
    //    if (animationEvent.animatorClipInfo.weight > 0.5f)
    //    {
    //        if (FootstepAudioClips.Length > 0)
    //        {
    //            var index = Random.Range(0, FootstepAudioClips.Length);
    //            AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
    //        }
    //    }
    //}

    //private void OnLand(AnimationEvent animationEvent)
    //{
    //    if (animationEvent.animatorClipInfo.weight > 0.5f)
    //    {
    //        AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
    //    }
    //}
}




