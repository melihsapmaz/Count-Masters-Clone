using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class EnemyController : MonoBehaviour {
    [SerializeField] GameObject Enemy;
    [Range(0f, 20f)][SerializeField] private float DistanceFactor, Radius;
    public Transform enemyTransform;
    public bool attack;
    public TextMeshProUGUI _enemyCountText;

    private void Start() {
        EnemyCreator(30);
        _enemyCountText.text = "30";
    }
    private void Update() {
        Attack();
        if (transform.childCount == 0) {
            StopAttacking();
        }
    }

    private void EnemyCreator(int numberOfEnemy) {
        for (int i = 0; i < numberOfEnemy; i++) {
            var enemy = Instantiate(Enemy, transform.position, Quaternion.identity, transform);
        }
        PutEnemiesInPosition();
    }
    private void PutEnemiesInPosition() {
        for (int i = 1; i < this.transform.childCount; i++) {
            var x = DistanceFactor * Mathf.Sqrt(i) * Mathf.Cos(i * Radius);
            var z = DistanceFactor * Mathf.Sqrt(i) * Mathf.Sin(i * Radius);
            var NewPos = new Vector3(x, -0.7f, z);
            transform.GetChild(i).DOLocalMove(NewPos, 0.5f).SetEase(Ease.OutBack);
        }
    }

    private void Attack() {
        if (attack && transform.childCount > 1) {
            var enemyDirection = enemyTransform.position - transform.position;
            for (int i = 0; i < transform.childCount; i++) {
                transform.GetChild(i).rotation = Quaternion.Slerp(transform.GetChild(i).rotation, Quaternion.LookRotation(enemyDirection, Vector3.up),
                    Time.deltaTime * 3f);
                if (enemyTransform.childCount > 1) {
                    var distance = enemyTransform.GetChild(1).position - transform.GetChild(i).position;
                    if (distance.magnitude < 10f) {
                        transform.GetChild(i).position = Vector3.Lerp(transform.GetChild(i).position,
                        enemyTransform.GetChild(1).position, Time.deltaTime * 2f);
                    }
                }
            }
        }
    }
    public void AttackThem(Transform enemyForce) {
        enemyTransform = enemyForce;
        attack = true;

        for (int i = 1; i < transform.childCount; i++) {
            transform.GetChild(i).GetComponent<Animator>().SetBool("Running", true);
        }
    }

    public void StopAttacking() {
        //PlayerManager.PlayerManagerInstance.gameState =  attack = false;

        for (int i = 1; i < transform.childCount; i++) {
            transform.GetChild(i).GetComponent<Animator>().SetBool("Running", false);
        }

    }
}
