using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class PlayerController : MonoBehaviour {
    [SerializeField] private float forwardSpeed, touchThreshold, horizontalMoveMultiplier, horizontalSpeed;
    [SerializeField] private GameObject groundObj, human;
    private float _deltaPosX, _groundBoundsX, _playerBoundsX, _playerBoundsY;
    private int _totalHumanCount = 1;
    private Rigidbody rb;
    private Touch _touch;
    private Vector3 _horizontalMove;
    [Range(0f, 20f)][SerializeField] private float DistanceFactor, Radius;
    public Transform enemy;
    string text;
    public bool attack, isFinished = false, isStarted = false;
    private int numOfEnemy, numOfPlayer, currentStackCount = 0, last;
    public TextMeshProUGUI _playerCountText;
    [SerializeField] private List<int> towerCountList = new List<int>();
    List<Transform> playersList = new List<Transform>();
    public GameObject camera;
    public bool finishArea = false;

    public Canvas canvas;

    private void Start() {
        forwardSpeed = 0;
        rb = GetComponent<Rigidbody>();
        groundObj = GameObject.FindGameObjectWithTag("ground");
        _groundBoundsX = groundObj.GetComponent<Renderer>().bounds.size.x;
        _playerBoundsY = transform.GetChild(1).GetComponent<CapsuleCollider>().bounds.size.y;
        _playerBoundsX = transform.GetChild(1).GetComponent<CapsuleCollider>().bounds.size.x;
        transform.GetChild(1).GetComponent<Animator>().SetBool("DynIdle", true);

    }

    private void Update() {
        //MovePlayerForward();
        //MovePlayerLeftAndRight();   
        Attack();
        var textt = transform.childCount-1;
        _playerCountText.text = textt.ToString();
        if(finishArea){
            forwardSpeed = 0;
            for(int i = 1; i < transform.childCount - 1; i++){
                transform.GetChild(i).GetComponent<Animator>().SetBool("DynIdle", true);
            }
        }
    }
    private void MovePlayerForward() {
        transform.Translate(transform.forward * forwardSpeed * Time.deltaTime);

    }
    private void MovePlayerLeftAndRight() {
        if (Input.touchCount > 0) {
            if(!isStarted){
                forwardSpeed = 5;
                canvas.gameObject.transform.GetChild(4).gameObject.SetActive(false);
                canvas.gameObject.transform.GetChild(5).gameObject.SetActive(false);
                canvas.gameObject.transform.GetChild(6).gameObject.SetActive(false);
                canvas.gameObject.transform.GetChild(7).gameObject.SetActive(false);
                canvas.gameObject.transform.GetChild(8).gameObject.SetActive(false);
                canvas.gameObject.transform.GetChild(9).gameObject.SetActive(false);
                canvas.gameObject.transform.GetChild(10).gameObject.SetActive(false);
                canvas.gameObject.transform.GetChild(11).gameObject.SetActive(false);
                isStarted = true;
                transform.GetChild(1).GetComponent<Animator>().SetBool("Running", true);
            }
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
    private void InstantiatePlayers(int humanIncreaseAmount) {
        for (int i = 0; i < humanIncreaseAmount; i++) {
            Instantiate(human, transform.position, Quaternion.identity, transform);
        }
        _totalHumanCount = transform.childCount;
        MoveAutomatedCharacters();
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("gate")) {
            other.transform.GetComponent<BoxCollider>().enabled = false;
            if (other.gameObject.name.ToString()[0].Equals('L')) {
                text = other.transform.parent.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>().text.ToString();
            } else if (other.gameObject.name.ToString()[0].Equals('R')) {
                text = other.transform.parent.GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>().text.ToString();
            }
            if (text[0].Equals('x')) {
                text = text.Substring(1);
                InstantiatePlayers(_totalHumanCount * int.Parse(text) - _totalHumanCount);
            } else {
                InstantiatePlayers(int.Parse(text));
            }
        } else if (other.CompareTag("EnemyParent")) {
            enemy = other.transform;
            attack = true;
            forwardSpeed = 2f;
            enemy.GetChild(1).GetComponent<EnemyController>().AttackThem(transform);
            StartCoroutine(UpdateEnemyAndPlayerCount());
        }
        else if(other.CompareTag("Finish")){
            isFinished = true;
            forwardSpeed = 0;
            for (int i = 1; i < transform.childCount; i++) {
                transform.GetChild(i).GetComponent<Animator>().SetBool("DynIdle", true);
            }
            //GameOverStack();
            transform.GetChild(0).gameObject.SetActive(false);
            FillTowerList();
            StartCoroutine(EndGame());
        }
    }
    public void MoveAutomatedCharacters() {
        for (int i = 1; i < transform.childCount; i++) {
            var x = DistanceFactor * Mathf.Sqrt(i) * Mathf.Cos(i * Radius);
            var z = DistanceFactor * Mathf.Sqrt(i) * Mathf.Sin(i * Radius);
            var NewPos = new Vector3(x, 0, z);
            transform.GetChild(i).DOLocalMove(NewPos, 0.5f).SetEase(Ease.OutBack);
            transform.GetChild(i).GetComponent<Animator>().SetBool("Running", true);
        }
    }

    private void Attack() {
        if (attack) {
            var enemyDirection = new Vector3(enemy.position.x, transform.position.y, enemy.position.z) - transform.position;
            for (int i = 1; i < transform.childCount; i++) {
                transform.GetChild(i).rotation =
                    Quaternion.Slerp(transform.GetChild(i).rotation, Quaternion.LookRotation(enemyDirection, Vector3.up), Time.deltaTime * 3f);
            }
            if (enemy.GetChild(1).childCount > 1) {
                for (int i = 1; i < transform.childCount; i++) {
                    var Distance = enemy.GetChild(1).GetChild(0).position - transform.GetChild(i).position;
                    if (Distance.magnitude < 10f) {
                        transform.GetChild(i).position = Vector3.Lerp(transform.GetChild(i).position,
                            new Vector3(enemy.GetChild(1).GetChild(0).position.x, transform.GetChild(i).position.y,
                                enemy.GetChild(1).GetChild(0).position.z), Time.deltaTime * 1f);
                    }
                }
            } else {
                attack = false;
                forwardSpeed = 5f;
                MoveAutomatedCharacters();
                for (int i = 1; i < transform.childCount; i++){
                    transform.GetChild(i).rotation = Quaternion.identity;
                }
                enemy.gameObject.SetActive(false);
            }
            if (transform.childCount == 1) {
                enemy.GetChild(1).GetComponent<EnemyController>().StopAttacking();
                gameObject.SetActive(false);
                canvas.gameObject.transform.GetChild(12).gameObject.SetActive(true);
            }
        } else {
            MovePlayerForward();
            MovePlayerLeftAndRight();
        }
    }

    IEnumerator UpdateEnemyAndPlayerCount() {
        numOfEnemy = enemy.GetChild(1).childCount - 1;
        numOfPlayer = transform.childCount - 1;
        while (numOfEnemy > 0 && numOfPlayer > 0) {
            numOfPlayer--;
            numOfEnemy--;
            enemy.transform.GetChild(1).GetComponent<EnemyController>()._enemyCountText.text = numOfEnemy.ToString();
            _playerCountText.text = numOfPlayer.ToString();
            yield return null;
        }
        if (numOfEnemy == 0) {
            for (int i = 0; i < transform.childCount; i++) {
                transform.GetChild(i).rotation = Quaternion.identity;
            }
        }
    }
    void FillTowerList()
    {
        var playerAmount = transform.childCount-1;
        var maxStack = (int)Mathf.Sqrt(playerAmount - 1)+1;
        last = maxStack * 2;
        for (int i = 1; i <= maxStack; i++)
        {
            if (playerAmount < i)
            {
                break;
            }
            playerAmount -= i*2;
            towerCountList.Add(i);
            towerCountList.Add(i);
        }
        towerCountList.Add(playerAmount);
    }
    IEnumerator EndGame(){
        yield return new WaitForSecondsRealtime(0.1f);
        var count = transform.childCount - 1;
        foreach (Transform child in transform){
            playersList.Add(child);
        }
        foreach (int towerHumanCount in towerCountList){
            for(int i = towerHumanCount; i > 0; i--){
                if(count >= 0 && !playersList[count].CompareTag("textt"))
                    if(towerHumanCount % 2 != 0){
                        if(i % 2 == 0)
                            playersList[count].transform.DOMove(new Vector3(_playerBoundsX*i/2, last * _playerBoundsY - 3f, transform.position.z), 1f).OnComplete(throwHuman);
                        else if(i % 2 != 0)
                            playersList[count].transform.DOMove(new Vector3(_playerBoundsX*-i/2, last * _playerBoundsY - 3f, transform.position.z), 1f).OnComplete(throwHuman);
                    }
                    else{
                        if(i % 2 == 0)
                            playersList[count].transform.DOMove(new Vector3(_playerBoundsX*i/2 - _playerBoundsX/2, last * _playerBoundsY - 3f, transform.position.z), 1f).OnComplete(throwHuman);
                        else if(i % 2 != 0)
                            playersList[count].transform.DOMove(new Vector3(_playerBoundsX*-i/2 - _playerBoundsX/2, last * _playerBoundsY - 3f, transform.position.z), 1f).OnComplete(throwHuman);
                    }
                count--;
            }
            last--;
        }
    }
    private void throwHuman(){
        foreach (var item in playersList)
        {
            if(!item.CompareTag("textt")){
                forwardSpeed = 10;
                StartCoroutine(moveFinish(item));
                camera.GetComponent<CameraController>().isFinish = true;
            }
        }
    }
    IEnumerator moveFinish(Transform go){
        yield return new WaitForSeconds(3);
        go.GetComponent<CapsuleCollider>().isTrigger = false;
        go.GetComponent<Rigidbody>().isKinematic = false;
        go.GetComponent<Animator>().SetBool("Running", true);
    }
}
