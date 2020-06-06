using UnityEngine;


public class CameraFollow : MonoBehaviour
{
  private Transform target;
  private Vector3 offset = new Vector3(0, 0, -10);

  void Start() {
    GetPlayerTransform();
  }

  void LateUpdate () {
    transform.position = target.position + offset;
  }

  // Get the Player game object by Tag, if possible:
  // update the target transform information
  // otherwise, log an error message
  void GetPlayerTransform () {
    GameObject player = GameObject.FindWithTag("Player");
    if (player != null) {
      target = player.transform;
    } else {
      Debug.Log("Player object not found");
    }
  }
}
