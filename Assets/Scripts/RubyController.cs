using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    
    public int maxHealth = 5;
    
    public static int counter = 0;
    public int counter2 = 0;

    public bool endCheck;
    
    private BoxCollider2D box;

    public GameObject projectilePrefab;
    public GameObject gameOver;
    public GameObject Rubb;

    public Text winText;

    public AudioClip throwSound;
    public AudioClip hitSound;
    
    public int health { get { return currentHealth; }}
    int currentHealth;
    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    public AudioSource BGM;
    public AudioSource loseMusic;
    public AudioSource winMusic;
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    
    AudioSource audioSource;

    public GameObject Health;
    public GameObject Hurt;

    public static int cogs;
    public Text cogAmount;
    
    // Start is called before the first frame update
    void Start()
    {

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameOver.SetActive(false);
        Rubb.SetActive(true);
        loseMusic.Stop();
        winMusic.Stop();
        box = GetComponent<BoxCollider2D>();
        cogs = 4;
        cogAmount.text = "Cogs: " + cogs;
        
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        if (ScoreScript.scoreValue >= 4)
        {
            winText.gameObject.SetActive(true);
            endCheck = true;
        } 
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        
        if(Input.GetKeyDown(KeyCode.C) && cogs > 0)
        {
            Launch();
        }

        if(sceneName == "Scene 2" && ScoreScript.scoreValue >= 4 && counter == 0)
        {
            counter++;
            winMusic.Play();
            BGM.Stop();
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (endCheck == true)
            {
                ScoreScript.scoreValue = 0;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }

                if(ScoreScript.scoreValue >= 4)
                {
                    ScoreScript.scoreValue = 0;
                    cogs = 4;
                    winText.gameObject.SetActive(false);
                    SceneManager.LoadScene("Scene 2");
                }
            }
        }
    }
    
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
            GameObject projectileObject2 = Instantiate(Hurt, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(hitSound);
            
        }

        

        if (amount > 0)
        {
            GameObject projectileObject2 = Instantiate(Health, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if(currentHealth == 0){
            BGM.Stop();
            box.GetComponent<Collider2D>().enabled = false;
            loseMusic.Play();
            gameOver.SetActive(true);
            speed = 0;
            endCheck = true;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ammo"))
        {
            cogs = cogs +4;
            cogAmount.text = "Cogs: " + cogs.ToString();
            other.gameObject.SetActive(false);
        }
    }
    
    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        
        PlaySound(throwSound);
        cogs--;
        cogAmount.text = "Cogs: " + cogs;
    } 
    
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}