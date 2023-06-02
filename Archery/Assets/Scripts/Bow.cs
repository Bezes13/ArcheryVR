using UnityEngine;

/// <summary>
/// This class handles the Behaviour of the Bow
/// </summary>
public class Bow : MonoBehaviour
{
    private const float MaxStingRange = 0.0738f;
    private const float MinStingValue = 0.01075402f;

    private Animator _animator;

    [SerializeField] private Transform arrowStringPosition;
    [SerializeField] private Transform arrowWoodPosition;
    [SerializeField] private ArrowSpawner spawner;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clip;
    private static readonly int Bow1 = Animator.StringToHash("bow");

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
        var localPosition = arrowStringPosition.localPosition;
        localPosition =
            new Vector3(0, Mathf.Max(-MaxStingRange, localPosition.y), 0);
        arrowStringPosition.localPosition = localPosition;
        var value = GetStringPosition(localPosition.y);
        _animator.SetFloat(Bow1, value);
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
            Mathf.Min(1.0f, (arrowStringPosition.localPosition.y + MinStingValue) / -(MaxStingRange - MinStingValue)));
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
        float[] animationValues =
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
        if (y >= animationValues[0])
        {
            return 0;
        }

        for (var i = 0; i < animationValues.Length - 1; i++)
        {
            if (!(y > animationValues[i])) continue;
            var f = Helper(animationValues[i], animationValues[i + 1], i);

            return f / 18.0f;
        }

        return 11f / 18f;

        float Helper(float lower, float higher, float time)
        {
            return time + (lower - y) / (lower - higher);
        }
    }
}