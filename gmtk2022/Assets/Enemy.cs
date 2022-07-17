using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool isDead = false;
    public int health2 = 10; // used for projectiles
    public int exp = 1;
    public float speed = 1f;
    public float moveOffset = 0f;
    public int curWaypoint = 0;
    public bool inCamera = false;
    public bool enemyHit = false;
    public bool boss = false;
    public GameManager gameManager;

    public Vector3 GetPosition() {
        return transform.position;
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        moveOffset = Random.Range(0f, 15f)/10f;

    }

    public void HitByLaser() { 
        enemyHit = true;
        Invoke("UnHitByLaser", 0.2f);
    }

    public void UnHitByLaser() {
        enemyHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.enemyTurn) {
            if (curWaypoint >= gameManager.wayPoints.Count) { 
                gameManager.GameOver();
            }
            else if (curWaypoint % 2 == 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(gameManager.wayPoints[curWaypoint].transform.position.x + moveOffset, transform.position.y, 0), speed * Time.deltaTime);
                if (transform.position == new Vector3(gameManager.wayPoints[curWaypoint].transform.position.x + moveOffset, transform.position.y, 0))
                {
                    curWaypoint++;
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, gameManager.wayPoints[curWaypoint].transform.position.y + moveOffset, 0), speed * Time.deltaTime);
                if (transform.position == new Vector3(transform.position.x, gameManager.wayPoints[curWaypoint].transform.position.y + moveOffset, 0))
                {
                    curWaypoint++;
                }
            }
        }
        
        
    }

    public void TakeDamage(int attack) {
        health2 -= attack;
        
        if (health2 <= 0)
        {
            isDead = true;
            gameManager.UpdateExp(exp);
            gameObject.SetActive(false);
            GetComponent<SpriteRenderer>().color = Color.red;
            if (boss) {
                gameManager.win = true;
                gameManager.GameOver();
            }
        }
        else {
            GetComponent<SpriteRenderer>().color = Color.red;
            Invoke("ResetFlash", 0.2f);
        }
    }

    public void ResetFlash() { 
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void OnBecameVisible()
    {
        inCamera = true;
    }
}
