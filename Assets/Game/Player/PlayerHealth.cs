using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    [field: SerializeField] public float maxHealth { get; private set; }
    [SerializeField] private Slider healthSlider;
    private float health;

    public void Damage(float damage) {
        health = Mathf.Clamp(health - damage, 0, maxHealth);
        if (health == 0) Die();
        UpdateHealthSlider();
    }
    public void Die() {
        print("Player is dead");
    }

    private void UpdateHealthSlider() {
        healthSlider.value = health / maxHealth;
    }

    private void Start() {
        health = maxHealth;
    }
}
