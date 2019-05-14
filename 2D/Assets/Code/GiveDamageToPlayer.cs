using UnityEngine;

class GiveDamageToPlayer :MonoBehaviour
{
    public int DamageToGive = 10;
    private Vector2
        _lastPosition,
        _velocity;
    public void LateUpdate()
    {
        _velocity = (_lastPosition - (Vector2)transform.position) / Time.deltaTime;
        _lastPosition = transform.position;

    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player == null)
            return;
        player.TakeDamage(DamageToGive,gameObject);
        var controller = player.GetComponent<CharacterController2D>();
        var totalVelocity = controller.Velocity + _velocity;
        controller.SetForce(
            new Vector2(
                -1 * Mathf.Sign(totalVelocity.x) * Mathf.Clamp(Mathf.Abs(totalVelocity.x) * 6, 10, 40),
                -1 * Mathf.Sign(totalVelocity.y) * Mathf.Clamp(Mathf.Abs(totalVelocity.y)*6,5,30)
            )
         );

    }
}

