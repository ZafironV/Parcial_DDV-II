using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    public GameObject projectilePrefab;
    public Transform shootPoint;
    private float lastShootTime;
    public float shootCooldown = 1f;
    public float projectileSpeed = 10f;
    public GameManager gameManager;
    public float projectileDestroyDelay = 5f;
    public int keyCount = 0;
    public Animator anim;
    public CameraShake cameraShake;
    public AudioSource audioSource;
    public AudioClip keyPickupSound;
    public AudioClip lifePickupSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && Time.time - lastShootTime >= shootCooldown)
        {
            Shoot();
            lastShootTime = Time.time;
        }

        anim.SetFloat("horizontal", movement.x);
        anim.SetFloat("vertical", movement.y);
    }

    void FixedUpdate()
    {
        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void Shoot()
    {
        Vector2 direction = movement.normalized;

        if (direction.magnitude > 0)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.velocity = direction * projectileSpeed;
            Destroy(projectile, projectileDestroyDelay);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            gameManager.EnemyTouchLink();
            Destroy(collision.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyProjectile"))
        {
            gameManager.EnemyTouchLink();
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Key"))
        {
            keyCount++;
            cameraShake.StartShake();
            audioSource.PlayOneShot(keyPickupSound);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Lifes"))
        {
            gameManager.IncreaseLinkLives();
            audioSource.PlayOneShot(lifePickupSound);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Escape"))
        {
            if (keyCount >= 4)
            {
                SceneManager.LoadScene("TE_Cinematic");
            }
            else
            {
                SceneManager.LoadScene("WIN");
            }
        }
    }
}