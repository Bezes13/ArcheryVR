using UnityEngine;

public class Arrow : MonoBehaviour
{
    private const float ArrowStingOffset = 0.15f;
    [SerializeField] private Transform stingTransform;
    [SerializeField] private Bow bow;
    
    public bool isAttachedToBow;
    public bool fired;
    
    private bool _isGrabbed;
    private Model _model;
    private Rigidbody _rigid;
    private Vector3 _lastVel;

    private void Start()
    {
        _rigid = GetComponent<Rigidbody>();
        bow = FindObjectOfType<Bow>();
        _model = FindObjectOfType<Model>();
        Physics.IgnoreCollision(bow.GetComponent<Collider>(), GetComponent<Collider>());
        Physics.IgnoreCollision(FindObjectOfType<Sting>().GetComponent<Collider>(),
            GetComponent<Collider>());
    }

    private void Update()
    {
        var arrowStringPosition = bow.GetArrowStringPosition();
        if (fired)
        {
            _rigid.freezeRotation = false;
            _rigid.velocity = Time.deltaTime * _model.GetWind() + _rigid.velocity;
        }

        if (_isGrabbed && Vector3.Distance(stingTransform.position, arrowStringPosition) <= ArrowStingOffset)
        {
            isAttachedToBow = true;
        }

        if (!isAttachedToBow) return;
        transform.rotation = bow.transform.rotation;
        transform.Rotate(0, 0, -90);

        transform.position = arrowStringPosition + transform.position - stingTransform.position;
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
        var target = other.gameObject.GetComponent<ShatterObject>();

        if (target == null)
        {
            return;
        }

        _model.AddPoints(target.GetPoints());
        fired = false;
    }


    public void FireArrow()
    {
        if (!bow.IsBowTensed() || !isAttachedToBow || fired)
        {
            return;
        }

        Debug.Log("Boom");
        isAttachedToBow = false;
        fired = true;
        var projectorVec = bow.GetArrowWoodPosition() - bow.GetArrowStringPosition();
        var projectorVecNormalized = projectorVec.normalized;
        _rigid.freezeRotation = false;
        _rigid.useGravity = true;
        _rigid.isKinematic = false;
        _rigid.AddForce( projectorVecNormalized * bow.GetBowForce() * 20f, ForceMode.VelocityChange );
		_rigid.AddTorque(  projectorVecNormalized * 20 );
        bow.ResetSting();
    }
}