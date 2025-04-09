using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStats : Stats
{
    //[SerializeField] private float maxHealth = 100f;
    //[SerializeField] private float currentHealth;
    //[SerializeField] private float attackPower = 10f;

    //[SerializeField] private Slider eSlider;
    //private void Start()
    //{
    //    currentHealth = maxHealth;
    //    if (eSlider != null)
    //    {
    //        eSlider.maxValue = maxHealth;
    //        eSlider.value = currentHealth;
    //    }
    //}
    //public void TakeDamage(float damage)
    //{
    //    if (currentHealth <= 0) return;

    //    currentHealth = Mathf.Max(0, currentHealth - damage);
    //    UpdateHealthBar();

    //    // Nếu máu của enemy hết, thực hiện hành động chết
    //    if (currentHealth == 0)
    //    {
    //        Die();
    //    }
    //}

    //// Hàm chết khi enemy hết máu
    //private void Die()
    //{
    //    // Có thể thêm các hành động khi enemy chết, ví dụ như chơi animation chết, rơi loot, v.v.
    //    Debug.Log("Enemy died!");
    //    // Có thể thêm ảnh hưởng mạng lưới Photon ở đây nếu là game multiplayer.
    //    // PhotonNetwork.Destroy(gameObject); // Nếu muốn xóa enemy khỏi mạng
    //}

    //// Hàm cập nhật thanh máu của enemy
    //private void UpdateHealthBar()
    //{
    //    if (eSlider != null)
    //    {
    //        eSlider.value = currentHealth;
    //    }
    //}

    //// Hàm để lấy thông tin máu hiện tại
    //public float GetCurrentHealth()
    //{
    //    return currentHealth;
    //}
}
