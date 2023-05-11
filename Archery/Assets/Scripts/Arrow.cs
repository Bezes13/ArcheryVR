using UnityEngine;

public class Arrow : MonoBehaviour
{
    private const float ArrowStingOffset = 0.05f;

    [SerializeField] private Transform stingTransform;
    [SerializeField] private Bow bow;

    private bool _isGrabbed;
    public bool isAttachedToBow;
    public bool fired;
    private ArrowFiredEvent _firedEvent;

    private Vector3 s;
    private Vector3 v;
    private Vector3 a;

    public Model model;
    public bool _shot = false;
    private Rigidbody rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var arrowStringPosition = bow.GetArrowStringPosition();
        if (fired)
        {
            FlyingArrow();
        }

        if (_isGrabbed && Vector3.Distance(stingTransform.position, arrowStringPosition) <= ArrowStingOffset)
        {
            isAttachedToBow = true;
        }

        if (isAttachedToBow)
        {
            transform.rotation = bow.transform.rotation;
            transform.Rotate(0, 0, -90);

            transform.position = arrowStringPosition + transform.position - stingTransform.position;
        }

        if (fired)
        {
            // update the rotation of the projectile during trajectory motion
        }
    }


    private void FlyingArrow()
    {
        v = Time.deltaTime * a + v;
        s = Time.deltaTime * v + s;
        a = new Vector3(0, -9.81f, 0);

        transform.position = s;
    }

    public void Grab()
    {
        _isGrabbed = true;
    }

    public void UnGrab()
    {
        _isGrabbed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        ShatterObject target = other.gameObject.GetComponent<ShatterObject>();

        if (target == null)
        {
            return;
        }


        Debug.LogWarning("heurica");
        Vector3 hit = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
        model.AddPoints(target.GetPoints());
        Debug.Log(model.GetPoints());
        v = new Vector3(0, 0, 0);
        a = new Vector3(0, 0, 0);
        _shot = false;
        fired = false;
    }


    public void FireArrow()
    {
        if (!bow.IsBowTensed() || !isAttachedToBow || _shot || fired)
        {
            return;
        }
        
        Debug.Log("Boom");
        isAttachedToBow = false;
        fired = true;
        _shot = true;
        Vector3 projectorvec = bow.GetArrowWoodPosition() - bow.GetArrowStringPosition();
        Vector3 projectorvecdir = projectorvec.normalized;
        Quaternion startRot = transform.rotation;
        _firedEvent = new ArrowFiredEvent()
        {
            Direction = projectorvecdir,
            StartRotation = startRot,
            Force = bow.GetBowForce()
        };
        // rigid.freezeRotation
        //rigid.AddForce(projectorvecdir * bow.GetBowForce() * 20f);
       // rigid.AddExplosionForce();
        a = projectorvecdir * bow.GetBowForce() * 20f;
        s = transform.position;
        v = a;

        FlyingArrow();
        bow.ResetSting();
    }
}