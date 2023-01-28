using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AutomatedMoves : MonoBehaviour
{

    public PlayerController playerController;
    Vector3 target1;
    bool isRamp = false;
    private void Start() {
        playerController = transform.parent.GetComponent<PlayerController>();
        DOTween.SetTweensCapacity(7812, 125);
    }
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("enemy") && other.transform.parent.childCount > 0){
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        else if(other.CompareTag("cylinder") || other.CompareTag("spiral")){
            Destroy(gameObject);
        }
        else if(other.CompareTag("ramp") && !isRamp){
            isRamp = true;
            transform.DOLocalMoveY(4.5f, 1.2f).OnStepComplete(moveBack);
        }
        
    }
    private void moveBack() {
        transform.DOLocalMoveY(0f, 0.5f);
    }
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("finishArea")){
            playerController.finishArea = true;
        }
    }
}
