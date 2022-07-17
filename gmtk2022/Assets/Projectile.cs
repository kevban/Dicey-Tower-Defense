using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public float destroyDist = 1f;
    public int attack = 5;
    public int type = 0;
    public int target = 0;
    public bool pierced = false;
    public Rigidbody2D rb;
    public Vector3 moveDir = Vector3.zero;
    /*
     * 0 is normal
     * 1 is crit
     * 2 is fireball
     */
    public Enemy enemy;
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }


    // Update is called once per frame
    void Update()
    {
        if (pierced == false) {
            moveDir = (enemy.transform.position - transform.position).normalized;
        }
        transform.position += moveDir * speed * Time.deltaTime;
        if (enemy!= null && Vector3.Distance(transform.position, enemy.transform.position) < destroyDist)
        {
            if (type == 2)
            {
                Instantiate(gameManager.explosionPrefab, transform.position, Quaternion.identity);
            }
            else if (target > 0)
            {
                chainEnemy(target);
            }

            if (type != 4)
            {
                enemy.TakeDamage(attack);
                Destroy(gameObject);
            }
            else
            {
                pierced = true;
                Invoke("DestroySelf", 4f);
            }

        }
    }

    private void DestroySelf() { 
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == 4 && collision.gameObject.tag == "enemy" && collision.gameObject.GetComponent<Enemy>().enemyHit == false) {
            print("hitted");
            collision.gameObject.GetComponent<Enemy>().HitByLaser();
            collision.gameObject.GetComponent<Enemy>().TakeDamage((int)(gameManager.attack * gameManager.pierceDmg));
        }
    }

    public void chainEnemy(int targets) {
        int remainingTargets = targets;
        List<Enemy> fireAt = new List<Enemy>();
        foreach (Enemy enemy in gameManager.enemyList)
        {
            if (enemy.isDead)
            {
                continue;
            }
            else
            {
                fireAt.Add(enemy);
            }
        }
        while (remainingTargets > 0)
        {
            int random = Random.Range(0, fireAt.Count);
            if (fireAt.Count <= 0)
            {
                remainingTargets = 0;
                break;
            }
            if (fireAt[random] != null && fireAt[random].health2 >= 0 && fireAt[random].inCamera)
            {
                GameObject projectileObject = Instantiate(gameManager.projectilePrefab, transform.position, Quaternion.identity);
                Projectile projectile = projectileObject.GetComponent<Projectile>();
                projectile.GetComponent<SpriteRenderer>().color = Color.blue;
                projectile.attack = (int) (gameManager.attack * gameManager.chainDmg);
                projectile.enemy = fireAt[random];
                remainingTargets--;
            }
            else
            {
                fireAt.RemoveAt(random);
            }
        }
    }
}
