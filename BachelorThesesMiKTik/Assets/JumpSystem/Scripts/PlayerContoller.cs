using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class PlayerContoller : MonoBehaviour
{
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

  private void OnEnable()
  {
    _move = _inputActions.Player.Move;
    _jump = _inputActions.Player.Jump;

    _move.Enable();
    _jump.Enable();
  }

  private void OnDisable()
  {
    _move.Disable();
    _jump.Enable();
  }

  // Start is called before the first frame update
  void Start()
  {
  }

  // Update is called once per frame
  void FixedUpdate()
  {
    Move();
    CheckGrounded();
    Jump();
  }

  public void Jump()
  {
    if (_jump.ReadValue<float>() > 0 && IsGrounded)
    {
      Vector2 up = Vector2.up * JumpPower;
      _rigidbody.velocity += up;
    }
  }

  void Move()
  {
    Vector2 moveDir = _move.ReadValue<Vector2>();
    _rigidbody.velocity = new Vector2(moveDir.x, moveDir.y) * 2f;
  }

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

