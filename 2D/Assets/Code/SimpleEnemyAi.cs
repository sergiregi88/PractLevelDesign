using System;
using UnityEngine;
public class SimpleEnemyAi:MonoBehaviour, ITakeDamage, IPlayerRespawnListener
{
    public float Speed;
    public float FireRate = 1;
    public Projectile Projectile;
    public GameObject DestroyEffect;
    public int PointsToGivePlayer;
    public AudioClip ShootSound;

    private CharacterController2D _controller;
    private Vector2 _direction;
    private Vector2 _startPosition;
    private float _canFireIn;

    public void Start()
    {
        _controller = GetComponent<CharacterController2D>();
        _direction = new Vector2(-1, 0);
        _startPosition = transform.position;
    }
    public void Update()
    {
        _controller.SetHorizontalForce(_direction.x * Speed);
        //if is collining to left or right
        // if is giong to left and is colliding left             or is going to right and is colliding right 
        if((_direction.x  < 0 && _controller.State.IsCollidingLeft) || ( _direction.x > 0 && _controller.State.IsCollidingRight ))
        {
            // reverse direction
            _direction = -_direction;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        if ((_canFireIn -= Time.deltaTime) > 0)
            return;
        var rayCast = Physics2D.Raycast(transform.position, _direction, 1 << LayerMask.NameToLayer("Player"));
        if (!rayCast)
            return;

        var projectile = (Projectile)Instantiate(Projectile, transform.position, transform.rotation);
        projectile.Initialize(gameObject, _direction, _controller.Velocity);
        _canFireIn = FireRate;
        if (ShootSound != null)
            AudioSource.PlayClipAtPoint(ShootSound, transform.position);


    }
    // When we take damage we want game object despear but not destroy because when de player response We want reset object to wherever he we when it started begin alive
    public void TakeDamage(int damage, GameObject instigator)
    {
        if (PointsToGivePlayer!=0)
        {
            var projectile = instigator.GetComponent<Projectile>();
            if(projectile!=null && projectile.Owner.GetComponent<Player>() != null)
            {
                GameManager.Instance.AddPoints(PointsToGivePlayer);
                FloatingText.Show(string.Format("+{0}", PointsToGivePlayer), "PointsStarText", new FromWorldPointTextPositioner(Camera.main, transform.position, 1.5f, 50));
            }
        }
        Instantiate(DestroyEffect,transform.position,transform.rotation);
        gameObject.SetActive(false);
    }

    public void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player)
    {
        _direction = new Vector2(-1, 0);
        transform.localScale = new Vector3(1, 1, 1);
        transform.position = _startPosition;
        gameObject.SetActive(true);
    }
}

