
using UnityEngine;


class BackgroundParallax:MonoBehaviour
{
    public Transform[] Backgrounds;
    public float ParallaxScale;
    public float ParallaxReductionFactor;
    public float Smoogthing;

    private Vector3 _lastPosition;

    public void Start()
    {
        _lastPosition = transform.position;

    }
    public void Update()
    {
        var parallax = (_lastPosition.x - transform.position.x) * ParallaxScale;
        for(var i = 0; i < Backgrounds.Length; i++)
        {
            var backgroundTargetPosition = Backgrounds[i].position.x + parallax * (i * ParallaxReductionFactor + 1);
            Backgrounds[i].position = Vector3.Lerp(
                Backgrounds[i].position, 
                new Vector3(backgroundTargetPosition, Backgrounds[i].position.y, Backgrounds[i].position.z),
                Smoogthing*Time.deltaTime);


        }
        _lastPosition = transform.position;
    }
}

