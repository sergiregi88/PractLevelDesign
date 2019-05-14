using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour ,ITakeDamage
{
    // say if sprite is facing right true or left false
    private bool _isFacingRight;
    // controller Character
    private CharacterController2D _controller;
    // normalized Speed Horitzontal
    private float _normalizedHorizontalSpeed;
    // MaxSpeed of Player
    public float MaxSpeed = 8;
    // determin how the player velociti can hange
    public float SpeedAccelerationOnGround = 10f;
    public float SpeedAccelerationInAir = 5f;
    public bool IsDead { get; private set; }
    public AudioClip PlayerHitSound;
    public AudioClip PlayerShootSound;
    public AudioClip PlayerHealthSound;
    public Animator Animator;
    

    // Heald and Damage
    public int MaxHealth = 100;
    public int Health { get; private set; }

    public GameObject OuchEffect;
    // projectile
    public Projectile Projectile;
    public float FireRate;
    // track if player fire yet
    private float _canFireIn;
    public Transform ProjectileFireLocation;
    public GameObject FireProjectileEffect;




    // Use this for initialization
    void Awake() {
      
        

        //get controller from component
        _controller = GetComponent<CharacterController2D>();
        // is facing right wil be true if component x > 0 and will be false if component x < 0 
        _isFacingRight = transform.localScale.x > 0;
        Health = MaxHealth;

    }

    // Update is called once per frame
    void Update()
    {
        //projectile
        _canFireIn -= Time.deltaTime;
        //-------------
        if (!IsDead)
        HandleInput();

        var movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;
        if (IsDead)
            _controller.SetHorizontalForce(0);
        else
            _controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x, _normalizedHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor));
        //Animator.SetBool("IsGrounded", _controller.State.IsGrounded);
        //Animator.SetBool("IsDead", IsDead);
        //Animator.SetFloat("Speed",Mathf.Abs(_controller.Velocity.x)/MaxSpeed);
    }
    public void Kill()
    {
        _controller.HandleCollisions = false;
        GetComponent<Collider2D>().enabled = false;
        IsDead = true;
        Health=0;
        _controller.SetForce(new Vector2(0, 20));

    }
    public void RespawnAt(Transform spawnPoint)
    {
        if (!_isFacingRight)
            Flip();
        
        IsDead = false;
        _controller.HandleCollisions = true;
        GetComponent<Collider2D>().enabled = true;
        transform.position = spawnPoint.position;
        Health = MaxHealth;

    }
    public void TakeDamage(int damage,GameObject instigator)
    {

        AudioSource.PlayClipAtPoint(PlayerHitSound, transform.position);
        FloatingText.Show(string.Format("-{0}", damage), "PlayerTakeDamage", new FromWorldPointTextPositioner(Camera.main, transform.position, 2f, 60f));
       if(OuchEffect!=null)
        Instantiate(OuchEffect, transform.position, transform.rotation);
        Health -= damage;
        if (Health <= 0)
       LevelManager.Instance.KillPlayer();
    }
    public void GiveHealth(int health, GameObject instigator)
    {
        AudioSource.PlayClipAtPoint(PlayerHealthSound, transform.position);
        FloatingText.Show(string.Format("+{0}!", health), "PlayerGotHealthText", new FromWorldPointTextPositioner(Camera.main, transform.position, 2f, 50));
    
        Health = Mathf.Min(Health + health, MaxHealth);
    }
    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            _normalizedHorizontalSpeed = 1;
            if (!_isFacingRight)
                Flip();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _normalizedHorizontalSpeed = -1;
                if (_isFacingRight)
                Flip();
        }
        else 
        {
            _normalizedHorizontalSpeed = 0;
            

        }
        if(_controller.CanJump && Input.GetKeyDown(KeyCode.W))
        {
            _controller.Jump();
        }
       
        if (Input.GetMouseButtonDown(0))
            FireProjectile();
        _canFireIn -= Time.deltaTime;
       // ell te variable privada _canFireIN  x saver si pot dispara 

    }
    //projectile
    private void FireProjectile()
    {
        if (_canFireIn > 0)
            return;
        if (FireProjectileEffect != null)
        {
           var effect=(GameObject) Instantiate(FireProjectileEffect, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
            effect.transform.parent = transform;
        }
        var direction = _isFacingRight? Vector2.right:-Vector2.right;
        var projecile = (Projectile)Instantiate(Projectile, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
        projecile.Initialize(gameObject, direction, _controller.Velocity);
       
        _canFireIn = FireRate;
        AudioSource.PlayClipAtPoint(PlayerShootSound, transform.position);
      //  Animator.SetTrigger("Fire");
    }
    //-------------
    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _isFacingRight = transform.localScale.x > 0;
 
    }
    public void FinishLevel()
    {
        enabled = false;
        _controller.enabled = false;
       GetComponent<Collider2D>().enabled = false;
    }
    
}