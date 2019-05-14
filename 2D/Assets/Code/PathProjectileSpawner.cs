using UnityEngine;

public class PathProjectileSpawner : MonoBehaviour
{
    public Transform Destination;
    public PathedProjectile Projectile;

    public GameObject SpawnEffect;
    public float Speed;
    public float FireRate;
    public AudioClip SpawnProjectileSound;
    public Animator Animator;

    private float _nextShotInSeconds;

    public void Start()
    {
        _nextShotInSeconds = FireRate;
    }
    public void Update()
    {
        if ((_nextShotInSeconds -= Time.deltaTime) > 0)
            return;
        _nextShotInSeconds = FireRate;
        var projectile = (PathedProjectile)Instantiate(Projectile, transform.position, transform.rotation);
        projectile.Initialize(Destination, Speed);
        if (SpawnEffect != null)
            Instantiate(SpawnEffect, transform.position, transform.rotation);
        if (SpawnProjectileSound != null)
            AudioSource.PlayClipAtPoint(SpawnProjectileSound, transform.position);
        if (Animator != null)
            Debug.Log("ssss");
            Animator.SetTrigger("Fire");


    }


}

