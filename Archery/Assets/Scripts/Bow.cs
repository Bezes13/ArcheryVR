using UnityEngine;

public class Bow : MonoBehaviour
{
    private const float MaxStingRange = 0.0738f;
    private const float MinStingValue = 0.01075402f;

    private readonly float[] _animationValues =
    {
        -0.01075402f,
        -0.01096038f,
        -0.01180249f,
        -0.01364458f,
        -0.01671125f,
        -0.02091927f,
        -0.02597514f,
        -0.03288396f,
        -0.04263402f,
        -0.0542632f,
        -0.06590831f,
        -0.07389119f
    };

    private Animator _animator;

    [SerializeField] private Transform arrowStringPosition;
    [SerializeField] private Transform arrowWoodPosition;
    [SerializeField] private ArrowSpawner spawner;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clip;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        arrowStringPosition.localPosition =
            new Vector3(0, Mathf.Max(-MaxStingRange, arrowStringPosition.localPosition.y), 0);
        var value = GetStringPosition(arrowStringPosition.localPosition.y);
        _animator.SetFloat("bow", value);
    }

    public Vector3 GetArrowStringPosition()
    {
        arrowStringPosition.localPosition = new Vector3(0, arrowStringPosition.localPosition.y, 0);
        return arrowStringPosition.position;
    }

    public Vector3 GetArrowWoodPosition()
    {
        return arrowWoodPosition.position;
    }

    public bool IsBowTensed()
    {
        return arrowStringPosition.localPosition.y < 0;
    }

    public float GetBowForce()
    {
        return Mathf.Max(0,
            Mathf.Min(1.0f, (arrowStringPosition.localPosition.y + MaxStingRange) / -(MaxStingRange - MaxStingRange)));
    }

    public void ResetSting()
    {
        arrowStringPosition.localPosition = new Vector3(0, -MinStingValue, 0);
    }

    public void ShotFired()
    {
        _animator.SetTrigger("shot");
        audioSource.clip = clip;
        audioSource.Play();
        foreach (var (arrow, _) in spawner.arrows)
        {
            if (arrow.isAttachedToBow)
            {
                arrow.FireArrow();
            }
        }
    }

    private float GetStringPosition(float y)
    {
        if (y >= _animationValues[0])
        {
            return 0;
        }

        for (var i = 0; i < _animationValues.Length - 1; i++)
        {
            if (!(y > _animationValues[i])) continue;
            var f = Helper(_animationValues[i], _animationValues[i + 1], i);

            return f / 18.0f;
        }

        return 11f / 18f;

        float Helper(float lower, float higher, float time)
        {
            return time + (lower - y) / (lower - higher);
        }
    }
}