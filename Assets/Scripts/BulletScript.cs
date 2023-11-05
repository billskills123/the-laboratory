using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BulletScript : MonoBehaviour {
    [Header("Bullet Settings")]
    [SerializeField] private float bulletSpeed;
    [SerializeField] private ParticleSystem bulletParticle;
    [SerializeField] private GameObject bulletTrail;
    private Vector3 mousePos;
    private Rigidbody2D rb;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Vector3 direction = mousePos - transform.position;
        Vector3 rotation = transform.position - mousePos;

        rb.velocity = new Vector2(direction.x, direction.y).normalized * bulletSpeed;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);

        Destroy(gameObject, 2.5f); //Destroys the bullets after a delay
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        bulletTrail.GetComponent<TrailRenderer>().enabled = false;
        StartCoroutine(BulletDeath());
    }

    private IEnumerator BulletDeath() {
        bulletParticle.Play();
        yield return new WaitUntil(() => bulletParticle.isPlaying == false);
        Destroy(gameObject);
    }
}