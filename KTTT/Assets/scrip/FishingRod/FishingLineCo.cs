using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FishingLineCo : MonoBehaviour
{
    //Objects that will interact with the rope
    public Transform whatTheRopeIsConnectedTo;
    public Transform whatIsHangingFromTheRope;

    //Line renderer used to display the rope
    private LineRenderer lineRenderer;

    //A list with all rope sections
    public List<Vector3> allRopeSections = new List<Vector3>();

    //Rope data
    private float ropeLength = 1f;
    [SerializeField] private float readyRopeLength = 1f;
    [SerializeField] private float minRopeLength = 1f;
    [SerializeField] private float maxRopeLength = 20f;
    //Mass of what the rope is carrying
    private float loadMass = 3f;
    //How fast we can add more/less rope
    float winchSpeed = 2f;

    [SerializeField] private bool isFloaterFishingRod = false;

    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private float ropeSegmentLeght = 0.25f;
    private int segmentCount = 20;
    private float lineWidth = 0.01f;


    //The joint we use to approximate the rope
    private SpringJoint springJoint;
    private Rigidbody rbWhatIsHangingFromTheRope;

    private Rigidbody rbCast;

    private bool isCastFishingRod = false;
    private Buoyancy buoyancyController = null;
    private bool isReadyFishinfRod = true;

    void Start()
    {
        ropeLength = readyRopeLength;

        rbWhatIsHangingFromTheRope = whatIsHangingFromTheRope.GetComponentInParent<Rigidbody>();

        springJoint = whatTheRopeIsConnectedTo.GetComponentInParent<SpringJoint>();
        springJoint.anchor = whatTheRopeIsConnectedTo.localPosition;
        springJoint.connectedAnchor = whatIsHangingFromTheRope.localPosition;

        buoyancyController = whatIsHangingFromTheRope.GetComponentInParent<Buoyancy>();
        rbCast = rbWhatIsHangingFromTheRope;

        if (isFloaterFishingRod)
        {
            rbCast = whatIsHangingFromTheRope.gameObject.GetComponentInParent<FishingLineCo>().
                whatIsHangingFromTheRope.gameObject.GetComponentInParent<Rigidbody>();

            buoyancyController = rbCast.gameObject.GetComponent<Buoyancy>();
        }

        //Init the line renderer we use to display the rope
        lineRenderer = GetComponentInParent<LineRenderer>();

        Vector3 ropeStartPoint = Vector3.zero;
        segmentCount = (int)(ropeLength * (1f / ropeSegmentLeght)) + 1;
        for (int i = 0; i < segmentCount; i++)
        {
            ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y += ropeSegmentLeght;
        }

        //Init the spring we use to approximate the rope from point a to b
        UpdateSpring();

        //Add the weight to what the rope is carrying
        rbWhatIsHangingFromTheRope.mass = loadMass;
    }

    void Update()
    {
        if(isReadyFishinfRod)
        {
            if (rbWhatIsHangingFromTheRope.velocity.magnitude > 5f)
                rbWhatIsHangingFromTheRope.drag = 0.5f;
            else
                rbWhatIsHangingFromTheRope.drag = 0f;
        }


        // Kiểm tra nếu phao dưới nước và đang quăng cần
        if (buoyancyController != null && buoyancyController.GetIsUnderWater() && isCastFishingRod)
        {
            isCastFishingRod = false;

            float distance = Vector3.Distance(whatTheRopeIsConnectedTo.position, whatIsHangingFromTheRope.position);
            Debug.Log(distance);

            // Cập nhật chiều dài dây câu nếu phao dưới nước
            ropeLength = distance + 1f;
            ropeLength = Mathf.Clamp(ropeLength, minRopeLength, maxRopeLength);

            UpdateSpring();
        }

        // Tiến hành cập nhật thông tin dây câu
        InitRope();
        DisplayRope();
    }


    private void FixedUpdate()
    {
        //Add more/less rope
        //UpdateWinch();

        Simulation();
    }

    #region Rope;
    private void InitRope()
    {


        int tempSegmentCount = (int)(ropeLength * (1f / ropeSegmentLeght)) + 1;
        if (tempSegmentCount > ropeSegments.Count)
        {
            Vector3 ropeStarPoint = ropeSegments[ropeSegments.Count - 1].posNow;
            segmentCount = tempSegmentCount;
            ropeStarPoint.y += ropeSegmentLeght;
            ropeSegments.Add(new RopeSegment(ropeStarPoint));
        }
        else if (tempSegmentCount < ropeSegments.Count)
        {
            segmentCount = tempSegmentCount;
            ropeSegments.RemoveAt(ropeSegments.Count - 1);
        }
    }
    private void Simulation()
    {
        Vector3 forceGravity = new Vector3(0f, -1f, 0f);

        for (int i = 1; i < ropeSegments.Count; i++)
        {
            RopeSegment currentSegment = ropeSegments[i];
            Vector3 velocity = currentSegment.posNow - currentSegment.posOld;
            currentSegment.posOld = currentSegment.posNow;

            RaycastHit hit;
            if (Physics.Raycast(currentSegment.posNow, -Vector3.up, out hit, 0.1f))
            {
                if (hit.collider != null)
                {
                    velocity = Vector3.zero;
                    forceGravity.y = 0f;
                }
            }
            currentSegment.posNow += velocity;
            currentSegment.posNow += forceGravity * Time.fixedDeltaTime;
            ropeSegments[i] = currentSegment;
        }

        for (int i = 0; i < 20; i++)
        {
            ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        RopeSegment firstSegment = ropeSegments[0];
        firstSegment.posNow = whatTheRopeIsConnectedTo.position;
        ropeSegments[0] = firstSegment;

        RopeSegment endSegment = ropeSegments[ropeSegments.Count - 1];
        endSegment.posNow = whatIsHangingFromTheRope.position;
        ropeSegments[ropeSegments.Count - 1] = endSegment;

        for (int i = 0; i < ropeSegments.Count - 1; i++)
        {
            RopeSegment firsSeg = ropeSegments[i];
            RopeSegment secondSeg = ropeSegments[i + 1];

            float dist = (firsSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - ropeSegmentLeght);
            Vector3 changeDir = Vector3.zero;

            if (dist > ropeSegmentLeght)
            {
                changeDir = (firsSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < ropeSegmentLeght)
            {
                changeDir = (secondSeg.posNow - firsSeg.posNow).normalized;
            }

            Vector3 changeAmount = changeDir * error;

            if (i != 0)
            {
                firsSeg.posNow -= changeAmount * 0.5f;
                ropeSegments[i] = firsSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                ropeSegments[i + 1] = secondSeg;
            }
        }
    }

    //Display the rope with a line renderer
    private void DisplayRope()
    {
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositons = new Vector3[ropeSegments.Count];
        for (int i = 0; i < ropeSegments.Count; i++)
        {
            ropePositons[i] = ropeSegments[i].posNow;
        }

        lineRenderer.positionCount = ropePositons.Length;
        lineRenderer.SetPositions(ropePositons);
    }


    //Update the spring constant and the length of the spring
    private void UpdateSpring()
    {
        //Someone said you could set this to infinity to avoid bounce, but it doesnt work
        //kRope = float.inf

        //
        //The mass of the rope
        //
        //Density of the wire (stainless steel) kg/m3
        float density = 7750f;
        //The radius of the wire
        float radius = 0.02f;

        float volume = Mathf.PI * radius * radius * ropeLength;

        float ropeMass = volume * density;

        //Add what the rope is carrying
        ropeMass += loadMass;


        //
        //The spring constant (has to recalculate if the rope length is changing)
        //
        //The force from the rope F = rope_mass * g, which is how much the top rope segment will carry
        float ropeForce = ropeMass * 9.81f;

        //Use the spring equation to calculate F = k * x should balance this force, 
        //where x is how much the top rope segment should stretch, such as 0.01m

        //Is about 146000
        //float kRope = ropeForce / 0.01f;

        float kRope = 1000f;

        //print(ropeMass);

        //Add the value to the spring
        springJoint.spring = kRope * 1.0f;
        springJoint.damper = kRope * 0.05f;

        //Update length of the rope
        springJoint.maxDistance = ropeLength;
    }

    #endregion


    public void CastFishingRod(float percentCast, Vector3 vec)
    {
        if (isCastFishingRod || buoyancyController == null || buoyancyController.GetIsUnderWater())
            return; // Không quăng cần nếu phao dưới nước hoặc cần đã quăng

        float castPower = 5000f;
        float currenCastPower = castPower * percentCast / 100f;
        float maxCastLength = 50f;

        ropeLength = maxCastLength * percentCast / 100f;
        ropeLength = Mathf.Clamp(ropeLength, minRopeLength, maxCastLength);

        InitRope();
        UpdateSpring();

        rbCast.AddForce((vec) * currenCastPower);

        isCastFishingRod = true;
    }

    public bool GetIsReady()
    {
        if (!isCastFishingRod)
        {
            if (ropeLength + rbWhatIsHangingFromTheRope.gameObject.GetComponent<FishingLineCo>().GetRopeLength()
                <= readyRopeLength)
                return true;
            return false;
        }
        if (ropeLength <= readyRopeLength)
            return true;
        return false;
    }

    public void SetIsReadyFishinfRod(bool isReady)
    {
        isReadyFishinfRod = isReady;
    }

    #region Floater

    public float GetRopeLength()
    {
        return ropeLength;
    }

    public void SetFishingRodFloaterDepth(float val)
    {
        ropeLength = readyRopeLength - val;
        InitRope();
        UpdateSpring();
    }
    //Add more/less rope
    public void UpdateWinch(bool invert)
    {
        bool hasChangedRope = false;

        //More rope
        if (invert && ropeLength < maxRopeLength)
        {
            ropeLength += winchSpeed * Time.deltaTime;

            InitRope();

            //!!!!!!
            rbWhatIsHangingFromTheRope.WakeUp();

            hasChangedRope = true;
        }
        else if (!invert && ropeLength > minRopeLength)
        {
            ropeLength -= winchSpeed * Time.deltaTime;

            InitRope();

            //!!!!!!
            rbWhatIsHangingFromTheRope.WakeUp();

            hasChangedRope = true;
        }


        if (hasChangedRope)
        {
            ropeLength = Mathf.Clamp(ropeLength, minRopeLength, maxRopeLength);

            //Need to recalculate the k-value because it depends on the length of the rope
            UpdateSpring();
        }
    }
    public void UpdateFloaterDepth(bool isUp)
    {
        bool hasChangedRope = false;

        //More rope
        if (isUp && ropeLength < maxRopeLength)
        {
            ropeLength += winchSpeed * Time.deltaTime;

            InitRope();

            //!!!!!!
            rbWhatIsHangingFromTheRope.WakeUp();

            hasChangedRope = true;
        }
        else if (!isUp && ropeLength > minRopeLength)
        {
            ropeLength -= winchSpeed * Time.deltaTime;

            InitRope();

            //!!!!!!
            rbWhatIsHangingFromTheRope.WakeUp();

            hasChangedRope = true;
        }


        if (hasChangedRope)
        {
            ropeLength = Mathf.Clamp(ropeLength, minRopeLength, maxRopeLength);

            //Need to recalculate the k-value because it depends on the length of the rope
            UpdateSpring();
        }
    }
    #endregion
    public struct RopeSegment
    {
        public Vector3 posNow;
        public Vector3 posOld;

        public RopeSegment(Vector3 pos)
        {
            posNow = pos;
            posOld = pos;
        }
    }
}