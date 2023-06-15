using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Rigidbody2D myRb;
    [SerializeField] float moveSpeed = 1f;
    void Start()
    {
        myRb = this.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        myRb.velocity = new Vector2(moveSpeed, myRb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Platform")){
            transform.localScale = new Vector3(myRb.velocity.x * -1 , 1f);
            moveSpeed *= -1;
        }
    }
}
