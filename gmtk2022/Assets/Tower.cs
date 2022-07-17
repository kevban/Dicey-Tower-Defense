using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public GameManager gameManager;
    public List<Enemy> fireAt = new List<Enemy>();
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire(int projectiles) { 
        StartCoroutine(FireProjectile(projectiles));
    }
    
    private IEnumerator FireProjectile(int projectiles){
        int projectilesToShoot = projectiles;
        foreach (Enemy enemy in gameManager.enemyList) {
            if (enemy.isDead)
            {
                continue;
            }
            else if (Vector3.Distance(transform.position, enemy.GetPosition()) < gameManager.range) { 
                fireAt.Add(enemy);
            }
        }
        while (projectilesToShoot > 0) { 
            int random = Random.Range(0, fireAt.Count);
            if (fireAt.Count <= 0) {
                projectilesToShoot = 0;
                break;
            }
            if (fireAt[random] != null && fireAt[random].health2 >= 0 && fireAt[random].inCamera)
            {
                gameManager.CreateProjectile(transform.position, fireAt[random]);
                projectilesToShoot--;
                yield return new WaitForSeconds((1.8f / projectiles));
            }
            else {
                fireAt.RemoveAt(random);
            }
        }
        fireAt.Clear();
    }
}
