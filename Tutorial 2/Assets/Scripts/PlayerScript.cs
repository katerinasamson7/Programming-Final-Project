using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{

    private Rigidbody2D rd2d;
    private int scoreValue = 0;
    private int count;
    private int lives;
    private AudioSource musicSource;
    private SpriteRenderer spriteRenderer;

    public Text score;
    public Text winText;
    public Text livesText;
    public Text CountdownText;
    public Text restartText;

    public AudioClip musicClip;
    public AudioClip coinPickup;
    public AudioClip jumpSound;

    public float speed;
    public float jumpForce;
    public float startTime = 60.0f;

    private float startSpeed;

    public float powerUpTimer;
    public float startPowerUpTimer;

    public Animator animator;

    private bool restart;

    public bool isGrounded;
    public bool facingRight;
    public bool isSpedUp;

    bool isLevel2;

    public Transform startMarker;

    // Start is called before the first frame update
    void Start()
    {
        rd2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        score.text = scoreValue.ToString();
        winText.text = "";
        restartText.text = "";
        lives = 3;
        Time.timeScale = 1f;
        startSpeed = 5;
        startPowerUpTimer = powerUpTimer;
        SetCountText();
        musicSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (Input.GetKey("escape"))
        {
            Debug.Log("quit");
            Application.Quit();
        }

        float hozMovement = Input.GetAxis("Horizontal");
        float verMovement = Input.GetAxis("Vertical");
        rd2d.AddForce(new Vector2(hozMovement * speed, verMovement * speed));
        SetCountText();

        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        Vector2 movement = new Vector2(moveHorizontal, 0);

        if (moveHorizontal >= 0.1 && facingRight || moveHorizontal <= -0.01 && !facingRight)
        {
            Flip();
        }

        if (isGrounded == true && Input.GetKeyDown(KeyCode.W))
        {
            animator.SetTrigger("Jump");
        }

        if (moveHorizontal == 0)
        {
            animator.SetBool("isRunning", false);
        }
        else
        {
            animator.SetBool("isRunning", true);
        }

    }

    void SetCountText()
    {

        if (scoreValue >= 4 && !isLevel2)
        {

            transform.position = new Vector2(startMarker.position.x, startMarker.position.y);
            lives = 3;
            scoreValue = 0;
            isLevel2 = true;
        }
        if (scoreValue >= 8 && isLevel2 == true)
        {
            winText.text = "You Won! Game Created by Katerina Samson";
            musicSource.PlayOneShot(musicClip, 0.5f);
        }

        livesText.text = "Lives: " + lives.ToString();

        if (lives <= 0)
        {
            winText.text = "You Lose!";
            Destroy(gameObject);
            restart = true;
        }

        CountdownText.text = startTime.ToString("F0");
        if (startTime <= 0)
        {
            restartText.text = "Time's up, you lose! Press R to restart";
            Destroy(gameObject);
        }
    }

    private void Update()
    {

        if (lives <= 0)
        {
            winText.text = "You Lose! Press R to restart";
            restart = true;
        }

        startTime -= Time.deltaTime;

        if (startTime <= 0)
        {
            restartText.text = "Time's up, you lose! Press R to restart";
            restart = true;
        }

        if (restart)
        {
            Time.timeScale = 0f;
            if (Input.GetKeyDown(KeyCode.R))
            {
                Destroy(this.gameObject);
                restart = false;
                SceneManager.LoadScene("SampleScene");
            }
        }

        startPowerUpTimer -= Time.deltaTime;

        if (startPowerUpTimer <= 0)
        {
            isSpedUp = false;
        }
        PowerUpCheck();
    }

    void PowerUpCheck()
    {
        if (!isSpedUp)
        {
            speed = startSpeed;
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            if (other.GetComponent<Powerup>().type == "ExtraSpeed")
            {
                isSpedUp = true;
                startPowerUpTimer = powerUpTimer;
                speed += 10;
            }
            other.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Coin")
        {
            scoreValue += 1;
            score.text = scoreValue.ToString();
            musicSource.PlayOneShot(coinPickup);
            Destroy(collision.collider.gameObject);
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {

            collision.gameObject.SetActive(false);
            lives = lives - 1;
            SetCountText();
        }

    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            isGrounded = true;
            if (Input.GetKey(KeyCode.W))
            {
                rd2d.AddForce(new Vector2(0, 3), ForceMode2D.Impulse);
                musicSource.PlayOneShot(jumpSound);
                isGrounded = false;
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 characterScale = transform.localScale;
        characterScale.x *= -1;
        transform.localScale = characterScale;
    }

}
