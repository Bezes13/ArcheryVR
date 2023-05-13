using UnityEngine;

public class Arrow : MonoBehaviour
{
     [SerializeField] private const float ArrowStingOffset = 0.15f;
    [SerializeField] private Transform stingTransform;
    [SerializeField] private Bow bow;

    private bool _isGrabbed;
    public bool isAttachedToBow;
    public bool fired;
    private ArrowFiredEvent _firedEvent;

    private Vector3 s;
    private Vector3 v;
    private Vector3 a;

    private Model model;
    public bool _shot = false;
    private Rigidbody rigid;
    private Vector3 lastVel;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        bow =  Object.FindObjectOfType<Bow>();
        model =  Object.FindObjectOfType<Model>();
        Physics.IgnoreCollision(bow.GetComponent<Collider>(), this.GetComponent<Collider>());
        Physics.IgnoreCollision(Object.FindObjectOfType<Sting>().GetComponent<Collider>(), this.GetComponent<Collider>());
    }

    private void Update()
    {
        var arrowStringPosition = bow.GetArrowStringPosition();
        if (fired)
        {
            //var rot = lastVel-rigid.velocity;
            //transform.Rotate(new Vector3(rot.x, 0, rot.z));
            FlyingArrow();
            rigid.freezeRotation = false;
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
        rigid.velocity = Time.deltaTime * model.GetWind() + rigid.velocity;
        s = Time.deltaTime * v + s;
        a = new Vector3(0, -9.81f, 0);

       // transform.position = s;
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

        Vector3 hit = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
        model.AddPoints(target.GetPoints());
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
        rigid.freezeRotation = true;
        rigid.velocity = projectorvecdir * bow.GetBowForce() * 20f;
        //lastVel = projectorvecdir * bow.GetBowForce() * 20f;
        //rigid.acceleration = model.GetWind();
       // rigid.AddExplosionForce();
        a = projectorvecdir * bow.GetBowForce() * 20f;
        s = transform.position;
        v = a;
        
        FlyingArrow();
        bow.ResetSting();
    }
}