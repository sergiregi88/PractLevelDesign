using UnityEngine;

class HealthBar :MonoBehaviour
{
    public Player Player;
    public Transform ForegroundSpite;
    public SpriteRenderer ForegroundRenderer;
    // color unity express by 1 to 0 every color because we divide the color /225f converts
    public Color MaxHealthColor = new Color(255 / 255f, 63 / 225f, 63 / 225f);
    public Color MinHealthColor = new Color(64 / 225f, 137 / 255f, 255 / 255f);

    public void Update()
    {
            var healthPercent = Player.Health / (float) Player.MaxHealth;
            ForegroundSpite.localScale = new Vector3(healthPercent, 1, 1);
            ForegroundRenderer.color = Color.Lerp(MaxHealthColor, MinHealthColor, healthPercent);
    }

}

