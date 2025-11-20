using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class HUDController : MonoBehaviour
{
    public TMP_Text healthText;   

    public GameObject deathPanel;
    public Transform player;
    public Slider healthSlider;


    public void SetHealth(int max, int current)
    {
        healthText.text = $"Health: {current} / {max}";
        healthSlider.maxValue = max;
        healthSlider.value = current;
    }

    public void ShowDeathScreen()
    {
        Debug.Log("death screenis shown");
        deathPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }


}
