using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
  #region [SerializeField] public Transform TargetDestinaton { get; set; }

  /// <summary>
  /// Defines target destination of elevator.
  /// </summary>
  [field: Tooltip("Defines target destination of elevator.")]
  [field: SerializeField]
  public Transform TargetDestinaton { get; set; }

  #endregion

  #region [SerializeField] public float Speed { get; set; }

  /// <summary>
  /// Defines speed of elevator movement.
  /// </summary>
  [field: Tooltip("Defines strength scale for move")]
  [field: SerializeField]
  public float Speed { get; set; }

  #endregion

  [SerializeField] public bool IsActive;


  bool IsMovingToFirstPosition;
  Vector3 _initialPosition { get; set; }

  void Awake()
  {
   _initialPosition = transform.position;
  }

  // Update is called once per frame
  void Update()
  {
    if (IsActive)
    {
      StartElevator();
    }
  }

  void StartElevator()
  {
    if (!IsMovingToFirstPosition && Vector2.Distance(_initialPosition, transform.position) >= Vector2.Distance(_initialPosition, TargetDestinaton.position)) 
    {
      IsMovingToFirstPosition = true;
    }
    else if(IsMovingToFirstPosition && Vector2.Distance(TargetDestinaton.position, transform.position) >= Vector2.Distance(TargetDestinaton.position, _initialPosition))
    {
      IsMovingToFirstPosition = false;
      IsActive = false;
      return;
    }

    if(IsMovingToFirstPosition)
    {
      transform.position = Vector2.MoveTowards(transform.position, _initialPosition, Speed);
    }
    else
    {
      transform.position = Vector2.MoveTowards(transform.position, TargetDestinaton.position, Speed);
    }
  }
}
