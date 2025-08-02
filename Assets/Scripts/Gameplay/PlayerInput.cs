using TMPro;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    private Player player;
    private float horizontal, vertical;
    private Vector2 lookTarget;

    private int nukeCounter = 0;
    public TMP_Text counterText;
    private const int maxNukes = 3;
    private PickupNuke pickupNuke;
    private PickupShot pickupShot;

    [Header("Rapid Fire Pickup Info")]
    private bool rapidFireActive = false;
    private float rapidFireEndTime = 0f;
    private float rapidFireRate = 0.1f;
    private float lastRapidFireTime = 0f;

    [Header("Scatter Shot Pickup Info")]
    private bool scatterShotActive = false;
    private float scatterShotDuration = 0f;

    void Start()
    {
        player = GetComponent<Player>();

        pickupShot = GetComponent<PickupShot>();


        // Auto-find the counter text if not assigned
        if (counterText == null)
        {
            counterText = FindFirstObjectByType<TMP_Text>();

            // If still null, look for it in the UI Canvas
            if (counterText == null)
            {
                Canvas canvas = FindFirstObjectByType<Canvas>();
                if (canvas != null)
                {
                    counterText = canvas.GetComponentInChildren<TMP_Text>();
                }
            }
            UpdateCounterDisplay();
        }
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        lookTarget = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            player.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseNuke();
        }

        // Rapid fire with right click (hold)
        if (rapidFireActive && Input.GetMouseButton(1))
        {
            if (Time.time >= lastRapidFireTime + rapidFireRate)
            {
                player.Shoot();
                lastRapidFireTime = Time.time;
            }
        }
        else if (Input.GetMouseButtonDown(1) && pickupShot != null)
        {

        }

        // Check if rapid fire ended
        if (rapidFireActive && Time.time >= rapidFireEndTime)
        {
            rapidFireActive = false;

            // Notify UIManager to stop indicator
            UIManager uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null)
            {
                uiManager.StopRapidFireIndicator();
            }

            Debug.Log("$Rapid Fire ended!");
        }

        //scatter shot pick up
        if (scatterShotActive)
        {
            player.scatterShoot();
            scatterShotDuration -= Time.deltaTime;
            if (scatterShotDuration <= 0)
            {
                scatterShotActive = false;
            }
        }
    }

    private void FixedUpdate()
    {
        player.Move(new Vector2(horizontal, vertical), lookTarget);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Nuke") && collision.gameObject.activeSelf)
        {
            if (nukeCounter < maxNukes)
            {
                collision.gameObject.SetActive(false);
                nukeCounter = Mathf.Clamp(nukeCounter + 1, 0, maxNukes);
                UpdateCounterDisplay();
                Debug.Log($"Nuke picked up! Count: {nukeCounter}/{maxNukes}");
            }
            else
            {
                Debug.Log("Nuke inventory full! Cannot pick up more nukes.");
            }
        }
    }
    private void UpdateCounterDisplay()
    {
        if (counterText != null)
        {
            counterText.text = $": {nukeCounter}/{maxNukes}";
        }
    }

    private void UseNuke()
    {
        if (nukeCounter > 0)
        {
            nukeCounter -= 1;
            PickupNuke.DestroyAllEnemies();
            UpdateCounterDisplay();

            Debug.Log($"Nuke used! Remaining: {nukeCounter}");

        }
        else
        {
            Debug.Log($"No nukes available to use!");
        }
    }


    public void ActivateRapidFire(float duration, float fireRate)
    {
        rapidFireActive = true;
        rapidFireEndTime = Time.time + duration;
        rapidFireRate = fireRate;

        // Notify UIManager to start indicator
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.StartRapidFireIndicator(duration);
        }

        Debug.Log($"Rapid Fire activated for {duration} seconds!");
    }

    //activate the scatter shoot
    public void ActivateScatterShot(float duration)
    {
        scatterShotActive = true;
        scatterShotDuration = duration;

        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.StartScatterShotIndicator(duration);
        }
    }
}
