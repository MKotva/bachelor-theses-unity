using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerContoller : MonoBehaviour
{
  #region [SerializeField] public float MovePower { get; set; }

  /// <summary>
  /// Defines strength scale for move.
  /// </summary>
  [field: Tooltip("Defines strength scale for move")]
  [field: SerializeField]
  public float MovePower { get; set; }

  #endregion

  #region [SerializeField] public float JumpPower { get; set; }

  /// <summary>
  /// Defines strength scale for jump
  /// </summary>
  [field: Tooltip("Defines strength scale for jump")]
  [field: SerializeField]
  public float JumpPower { get; set; }

  #endregion

  #region [SerializeField] public bool IsGrounded { get; private set; }

  /// <summary>
  /// Defines strength scale for jump
  /// </summary>
  [field: Tooltip("Defines strength scale for jump")]
  [field: SerializeField]
  public bool IsGrounded { get; private set; }

  #endregion

  private PlayerInputActions _inputActions;
  private InputAction _jump;
  private InputAction _move;
  private Rigidbody2D _rigidbody;

  [SerializeField]
  private float _groundCheckDistance;

  private void Awake()
  {
    _inputActions = new PlayerInputActions();
    _rigidbody = GetComponent<Rigidbody2D>();
    Assert.IsNotNull(_rigidbody);
  }

  public void OnEnable()
  {
    _move = _inputActions.Player.Move;
    _jump = _inputActions.Player.Jump;

    _move.Enable();
    _jump.Enable();
  }

  public void OnDisable()
  {
    _move.Disable();
    _jump.Disable();
  }

  // Start is called before the first frame update
  void Start()
  {
   
  }

  // Update is called once per frame
  private void FixedUpdate()
  {
    Move();
    CheckGrounded();
  }

  /// <summary>
  /// This method is invoked by input system.
  /// If this method is invoked and player stands on the ground, player jumps.
  /// </summary>
  public void OnJump()
  {
    if (IsGrounded)
    {
      Vector2 opposite = Vector2.up * ( JumpPower );
      _rigidbody.AddForce(opposite);
    }
  }


  void Move()
  {
    Vector2 moveDir = _move.ReadValue<Vector2>();
    _rigidbody.AddForce(moveDir * MovePower);
  }

  /// <summary>
  /// Checks if player stands on the box.
  /// </summary>
  void CheckGrounded()
  {
    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _groundCheckDistance, LayerMask.GetMask("Box"));
    if (hit.collider != null)
      IsGrounded = true;
    else
      IsGrounded = false;
  }


  private void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _groundCheckDistance);
  }
}

