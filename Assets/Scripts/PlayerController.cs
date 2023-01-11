using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] private float forwardSpeed, touchThreshold, horizontalMoveMultiplier, horizontalSpeed;
    [SerializeField] private GameObject groundObj;
    private float _deltaPosX, _groundBoundsX, _playerBoundsX;
    private Rigidbody rb;
    private Animator anim;
    private Touch _touch;
    private Vector3 _horizontalMove;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        anim.SetBool("Running", true);
        _groundBoundsX = groundObj.GetComponent<Renderer>().bounds.size.x;
        _playerBoundsX = GetComponent<BoxCollider>().bounds.size.x;
    }

    private void Update() {
        MovePlayerForward();
        MovePlayerLeftAndRight();
    }
    private void MovePlayerForward() {
        var pos = transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, forwardSpeed * Time.deltaTime);
        rb.MovePosition(pos);
    }
    private void MovePlayerLeftAndRight() {
        if (Input.touchCount > 0) {
            _touch = Input.GetTouch(0);
            switch (_touch.phase) {
                case TouchPhase.Moved:
                    _deltaPosX = _touch.deltaPosition.x;
                    if (_deltaPosX > touchThreshold) {     //Move to right
                        _horizontalMove = new Vector3(transform.position.x + _deltaPosX / Screen.width * horizontalMoveMultiplier, transform.position.y, transform.position.z);
                        transform.position = Vector3.Lerp(transform.position, _horizontalMove, horizontalSpeed);
                        var limitX = transform.position;
                        limitX.x = Mathf.Clamp(transform.position.x, -_groundBoundsX / 2 + _playerBoundsX / 2, _groundBoundsX / 2 - _playerBoundsX / 2);
                        transform.position = limitX;
                    } else if (_deltaPosX < -touchThreshold) {
                        _horizontalMove = new Vector3(transform.position.x + _deltaPosX / Screen.width * horizontalMoveMultiplier, transform.position.y, transform.position.z);
                        transform.position = Vector3.Lerp(transform.position, _horizontalMove, horizontalSpeed);
                        var limitX = transform.position;
                        limitX.x = Mathf.Clamp(transform.position.x, -_groundBoundsX / 2 + _playerBoundsX / 2, _groundBoundsX / 2 - _playerBoundsX / 2);
                        transform.position = limitX;
                    }
                    break;
            }
        }
    }
}
