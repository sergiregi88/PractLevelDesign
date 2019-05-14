using System;
using UnityEngine;

public class PathedProjectile : MonoBehaviour, ITakeDamage
{
    private Transform _destination;
    private float _speed;
    public AudioClip DestroySound;

    public GameObject DestroyEffect;
    public int PointsToGivePlayer;


    public void Initialize(Transform destination,float speed)
    {
        _destination = destination;
        _speed = speed;
    }
    public  void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _destination.position, Time.deltaTime * _speed);
        var distanceSquared = (_destination.transform.position - transform.position).sqrMagnitude;
        if (distanceSquared > .1f * .1f)
            return;
        if (DestroyEffect != null)
            Instantiate(DestroyEffect, transform.position, transform.rotation);
        if (DestroySound != null)
            AudioSource.PlayClipAtPoint(DestroySound, transform.position);
        Destroy(gameObject);
      
    }

    public void TakeDamage(int damage, GameObject instigator)
    {
        if (DestroyEffect != null)
            Instantiate(DestroyEffect, transform.position, transform.rotation);
        Destroy(gameObject);
        var projectile = instigator.GetComponent<Projectile>();
        if(projectile!=null && projectile.Owner.GetComponent<Player>()!=null && PointsToGivePlayer!=0)
        {
            GameManager.Instance.AddPoints(PointsToGivePlayer);
            FloatingText.Show(string.Format("+{0}", PointsToGivePlayer), "PointsStarText", new FromWorldPointTextPositioner(Camera.main, transform.position, 1.5f, 50));
        }

    }
}

