using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider; // Referència al Slider UI
    public float maxHealth = 100f; // Vida màxima
    private float currentHealth; // Vida actual
    public float damageAmount = 10f; // Quantitat de vida que es perd quan no es trenca un cub
    public float regenRate = 5f; // Quantitat de vida que es regenera per segon
    public TextMeshProUGUI gameOverString;
    public PauseMenuScript pauseMenuScript;

    private bool OneTimeGameOver = true;

    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    void Update()
    {
        if (currentHealth > 0)
        {
            RegenerateHealth();
        }
        else
        {
            GameOver();
        }
    }

    public void TakeDamage()
    {
        currentHealth -= damageAmount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        healthSlider.value = currentHealth;
    }

    private void RegenerateHealth()
    {
        currentHealth += regenRate * Time.deltaTime;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthSlider.value = currentHealth;
    }

    private void GameOver()
    {
        if (OneTimeGameOver)
        {
            OneTimeGameOver = false;
            Debug.Log("Game Over");
            gameOverString.text = "GAME OVER";

            // Trigger the pause menu with game over state
            pauseMenuScript.TriggerGameOver();
        }
    }
}
