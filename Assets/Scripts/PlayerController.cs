using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public GameObject explosionEffect;
    public GameObject boosterFlame;
    public GameObject borderParent;
    public float thrustForce = 1f;
    public float maxSpeed = 5f;
    public AudioClip explosionSound;
    public AudioClip restartSound;

    public InputAction moveForward;
    public InputAction lookPosition;

    public UIDocument uIDocument;
    private Label scoreText;
    private Label highScoreTitle;
    private Label[] highScoreLabels = new Label[5];
    private VisualElement backGround;

    private float elapsedTime = 0f;
    private float score = 0f;
    public float scoreMultiplier = 10f;

    private Button restartButton;
    Rigidbody2D rb;

    void Start()
    {
        moveForward.Enable();
        lookPosition.Enable();

        var root = uIDocument.rootVisualElement;

        scoreText = root.Q<Label>("ScoreLabel");

        // High score labels
        highScoreTitle = root.Q<Label>("HighScoreTitle");
        backGround = root.Q<VisualElement>("BackGround");
        for (int i = 0; i < highScoreLabels.Length; i++)
        {
            highScoreLabels[i] = root.Q<Label>("HighScore_" + i);
        }

        // Hide high score section at start
        SetHighScoreVisibility(false);

        restartButton = root.Q<Button>("RestartButton");
        restartButton.style.display = DisplayStyle.None;
        restartButton.clicked += ReloadScene;

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Score settings
        elapsedTime += Time.deltaTime;
        score = Mathf.FloorToInt(elapsedTime * scoreMultiplier);
        scoreText.text = "Score: " + score;

        if (moveForward.IsPressed())
        {
            // Calculate mouse direction
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(lookPosition.ReadValue<Vector2>());
            Vector2 direction = (mousePos - transform.position).normalized;

            // Move player in direction of mouse
            transform.up = direction;
            rb.AddForce(direction * thrustForce);
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        // Fire sprite when mouse is pressed and released
        if (moveForward.WasPressedThisFrame())
        {
            boosterFlame.SetActive(true);
        }
        else if (moveForward.WasReleasedThisFrame())
        {
            boosterFlame.SetActive(false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position);
        
        Instantiate(explosionEffect, transform.position, transform.rotation);
        borderParent.SetActive(false);

        // Save and show high scores
        int finalScore = Mathf.FloorToInt(score);
        ScoreManager.Instance.SaveScore(finalScore);
        ShowHighScores();

        restartButton.style.display = DisplayStyle.Flex;
        Destroy(gameObject);
    }

    void ShowHighScores()
    {
        List<int> scores = ScoreManager.Instance.LoadAllScores();

        string[] suffixes = { "st", "nd", "rd", "th", "th" };

        for (int i = 0; i < highScoreLabels.Length; i++)
        {
            if (highScoreLabels[i] == null) continue;

            if (i < scores.Count && scores[i] > 0)
                highScoreLabels[i].text = (i + 1) + suffixes[i] + "  —  " + scores[i];
            else
                highScoreLabels[i].text = (i + 1) + suffixes[i] + "  —  ---";
        }

        SetHighScoreVisibility(true);
    }

    void SetHighScoreVisibility(bool visible)
    {
        var display = visible ? DisplayStyle.Flex : DisplayStyle.None;

    if (backGround != null)
        backGround.style.display = display;

    if (highScoreTitle != null)
        highScoreTitle.style.display = display;

    foreach (var label in highScoreLabels)
        if (label != null)
            label.style.display = display;
    }

    void ReloadScene()
{
    AudioSource.PlayClipAtPoint(restartSound, Camera.main.transform.position);
    ScoreManager.Instance.LoadSceneDelayed(
        SceneManager.GetActiveScene().name, 
        restartSound.length
    );
}
}