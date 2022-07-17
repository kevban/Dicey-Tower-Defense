using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour
{
    // Start is called before the first frame update
    GameManager gameManager;
    public Collider2D collider;
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.audioManager.Play("fireballboom");
        transform.localScale = new Vector3(1.5f + gameManager.fireballArea, 1.5f + gameManager.fireballArea, 0); 
        Invoke("DestroySelf", 0.3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "enemy")
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage((int)(gameManager.attack * gameManager.fireballDmg));
            print("hit");
        }
    }

    private void DestroyCollider() { 
        Destroy(collider);
    }

    private void DestroySelf() { 
        Destroy(gameObject);
    }
}
