using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{

  #region [SerializeField] public Transform FirstPosition { get; set; }

  /// <summary>
  /// Defines how much should elevator move on axis from actual position.
  /// </summary>
  [field: Tooltip("Defines strength scale for move")]
  [field: SerializeField]
  public Transform FirstPosition { get; set; }

  #endregion

  #region [SerializeField] public Transform SecondPosition { get; set; }

  /// <summary>
  /// Defines how much should elevator move on axis from actual position.
  /// </summary>
  [field: Tooltip("Defines strength scale for move")]
  [field: SerializeField]
  public Transform SecondPosition { get; set; }

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
    if(transform.position.x >= SecondPosition.position.x && transform.position.y >= SecondPosition.position.y) 
    {
      IsMovingToFirstPosition = true;
    }
    else if(transform.position.x <= FirstPosition.position.x && transform.position.y <= FirstPosition.position.y)
    {
      IsMovingToFirstPosition = false;
      IsActive = false;
      return;
    }

    if(IsMovingToFirstPosition)
    {
      transform.position = Vector2.MoveTowards(transform.position, FirstPosition.position, Speed);
    }
    else
    {
      transform.position = Vector2.MoveTowards(transform.position, SecondPosition.position, Speed);
    }
  }
}
