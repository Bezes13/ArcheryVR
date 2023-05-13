using UnityEngine;

public class Bow : MonoBehaviour
{
    private const float MaxStingRange = 0.0738f;

    private Animator _animator;
    private bool isArrowAttached;

    [SerializeField] private Transform arrowStringPosition;
    [SerializeField] private Transform arrowWoodPosition;
    [SerializeField] private ArrowSpawner spawner;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clip;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame

    void Update()
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

    public void attachArrow()
    {
        isArrowAttached = true;
    }

    public bool IsBowTensed()
    {
        return arrowStringPosition.localPosition.y < 0;
    }

    public float GetBowForce()
    {
        return Mathf.Max(0, Mathf.Min(1.0f, (arrowStringPosition.localPosition.y + 0.01075402f) / -(MaxStingRange - 0.01075402f)));
    }

    public void ResetSting()
    {
        arrowStringPosition.localPosition = new Vector3(0, -0.01075402f, 0);
    }

    public void ShotFired()
    {
        _animator.SetTrigger("shot");
        audioSource.clip = clip;
        audioSource.Play();
        foreach (var item in spawner.arrows)
        {
            if(item.Item1.isAttachedToBow){
                item.Item1.FireArrow();
            }
        }
    }

    private static float GetStringPosition(float y)
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

        for (int i = 0; i < animationValues.Length - 1; i++)
        {
            if (y > animationValues[i])
            {
                var f = helper(animationValues[i], animationValues[i + 1], i);

                return (f / 18.0f);
            }
        }

        return 11f / 18f;

        float helper(float lower, float higher, float time)
        {
            return time + (lower - y) / (lower - higher);
        }
    }
}