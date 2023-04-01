using UnityEngine;

public class CameraController : MonoBehaviour
{
  public Transform followTransform;

  // Update is called once per frame
  void LateUpdate()
  {
    this.transform.position = new Vector3(followTransform.position.x, followTransform.position.y, this.transform.position.z);
  }
}
