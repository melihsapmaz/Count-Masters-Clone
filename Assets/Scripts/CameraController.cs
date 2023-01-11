using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] private GameObject follow;
    private float _speed = 5f;
    private Vector3 _offset, _target;

    private void Start() {
        _offset = follow.transform.position - transform.position;
    }
    private void LateUpdate() {
        FollowPlayer();
    }

    private void FollowPlayer() {
        _target = follow.transform.position - _offset;
        _target.x = 0;
        transform.position = Vector3.MoveTowards(transform.position, _target, _speed * Time.smoothDeltaTime);
    }
}
