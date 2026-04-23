using UnityEngine;
using UnityEngine.SceneManagement;

public class GuardChase : MonoBehaviour
{
    public Transform player;

    public float speed = 10f;
    public float catchDistance = 2f;

    public float spawnDistanceBehind = 10f;
    public float fadeDuration = 1.5f;
    public float spawnDelay = 5f;

    public float speedMultiplier = 1.2f;
    public float increaseInterval = 5f;

    private bool hasSpawned = false;
    private bool canSpawn = false;
    private float fadeTimer = 0f;
    private float spawnTimer = 0f;
    private float speedTimer = 0f;

    private Renderer[] renderers;
    private Rigidbody rb;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        rb = GetComponent<Rigidbody>();

        SetUpRigidbody();
        SetVisible(false);
    }

    void SetUpRigidbody()
    {
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        if (player == null) return;

        HandleSpawnDelay();

        if (!canSpawn) return;

        if (!hasSpawned)
        {
            SpawnBehindPlayer();
            return;
        }

        speedTimer += Time.deltaTime;
        if (speedTimer >= increaseInterval)
        {
            speed *= speedMultiplier;
            speedTimer = 0f;
        }

        HandleFadeIn();
        RotateTowardsPlayer();
        CheckCatch(); // This checks the distance every frame
    }

    void FixedUpdate()
    {
        if (player == null || !hasSpawned || !canSpawn) return;
        ChasePlayer();
    }

    void HandleSpawnDelay()
    {
        if (canSpawn) return;
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnDelay) canSpawn = true;
    }

    void SpawnBehindPlayer()
    {
        Vector3 spawnPos = player.position - player.forward * spawnDistanceBehind;
        rb.position = spawnPos;
        rb.linearVelocity = Vector3.zero;
        Physics.SyncTransforms();
        hasSpawned = true;
        SetVisible(true);
        SetAlpha(0f);
    }

    void HandleFadeIn()
    {
        if (fadeTimer < fadeDuration)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            SetAlpha(alpha);
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - rb.position).normalized;
        Vector3 newPosition = rb.position + direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    void RotateTowardsPlayer()
    {
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;
        if (lookDir != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = lookRotation * Quaternion.Euler(0f, 180f, 0f);
        }
    }

    void CheckCatch()
    {
        float distance = Vector3.Distance(rb.position, player.position);

        if (distance < catchDistance)
        {
            // Call the counter logic on the GameManager
            RestartGame();
        }
    }

    void SetAlpha(float alpha)
    {
        foreach (Renderer r in renderers)
        {
            foreach (Material mat in r.materials)
            {
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;
            }
        }
    }

    void SetVisible(bool visible)
    {
        foreach (Renderer r in renderers) r.enabled = visible;
    }

    // THE FIXED RESTART LOGIC
    void RestartGame()
    {
        if (gameStartEscape.instance != null)
        {
            // This triggers the save and the reload
            gameStartEscape.instance.RestartRun();
        }
        else
        {
            // Fallback if the script isn't found
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}