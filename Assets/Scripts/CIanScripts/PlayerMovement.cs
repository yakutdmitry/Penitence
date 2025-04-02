using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MovementSpeed;
    public float groundDrag;
    public Transform orientation;

    public float jumpForce;
    public float jumpCooldown;
    public float AirMultiplier;
    public bool ReadyToJump;

    public Rigidbody rb;
    Vector3 Movedirection;

    float HorizontalInput;
    float VerticalInput;
    public float PlayerHeight;
    public LayerMask IsGround;
    bool grounded;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    // Health and Damage
    public float health = 100f;  // Player's health
    public float maxHealth = 100f; // Maximum health (you can adjust this)
    public float invincibilityTime = 1f; // Time after taking damage where player is invincible
    private bool isInvincible = false;

    // Reference to HealthManager (UI)
    public TMPro.TextMeshProUGUI healthText;  // Reference to the TextMeshPro component for UI

    private SceneManagerCustom sceneManager;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        MyInput();
        SpeedControl();

        // Check for grounded status
        grounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, IsGround);

        if (grounded)
            rb.drag = groundDrag;
        else rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && ReadyToJump && grounded)
        {
            ReadyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {

        Movedirection = orientation.forward * VerticalInput + orientation.right * HorizontalInput;
        if (grounded)
            rb.AddForce(Movedirection.normalized * MovementSpeed * 10f, ForceMode.Force);
        else if (!grounded)
            rb.AddForce(Movedirection.normalized * 10f * AirMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 FlatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (FlatVel.magnitude > MovementSpeed)
        {
            Vector3 limitedVel = FlatVel.normalized * MovementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        ReadyToJump = true;
    }

    // Method for taking damage
    public void TakeDamage(float damage)
    {
        if (!isInvincible)
        {
            health -= damage;
            health = Mathf.Clamp(health, 0f, maxHealth);  // Ensure health doesn't go negative
            Debug.Log("Player took damage: " + damage + ". Health remaining: " + health);

            // Update health UI
            UpdateHealthUI();

            if (health <= 0)
            {
                Die();
            }
            else
            {
                // Apply temporary invincibility after taking damage
                StartCoroutine(InvincibilityFrames());
            }
        }
    }

    // Coroutine to handle invincibility after taking damage
    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    // Method for handling player death
    private void Die()
    {
        // Handle death (e.g., show game over screen, respawn, etc.)
        Debug.Log("Player died!");
        // Reload the scene
        sceneManager = FindObjectOfType<SceneManagerCustom>();
        if (sceneManager != null)
        {
            sceneManager.ReloadLevel();
        }
        else
        {
            Debug.LogError("SceneManager is missing!");
        }



    }

    // Method to update health UI
    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + health.ToString("F0");  // Format as integer (no decimals)
        }
        else
        {
            Debug.LogError("HealthText UI element is missing!");
        }
    }
}
