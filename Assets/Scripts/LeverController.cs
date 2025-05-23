using UnityEngine;

public class LeverController : MonoBehaviour
{
    public Sprite leverOn;
    public Sprite leverOff;
    public GameObject[] connectedPlatforms; // Bağlı platformlar

    private bool isOn = false;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateLeverVisual();
    }

    public void ToggleLever()
    {
        isOn = !isOn;
        UpdateLeverVisual();

        foreach (GameObject platform in connectedPlatforms)
        {
            platform.GetComponent<RotatingPlatform>()?.ToggleRotation();
        }
    }

    private void UpdateLeverVisual()
    {
        sr.sprite = isOn ? leverOn : leverOff;
    }
}