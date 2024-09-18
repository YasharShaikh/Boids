using System.Collections.Generic;
using UnityEngine;

public class FlockUnit : MonoBehaviour
{
    [SerializeField] private float smoothDamp; // lower the value the closer can get to cohesian vector, thus rotating faster
    [SerializeField] float FOV;


    List<FlockUnit> cohesionNeighbour = new List<FlockUnit>();
    List<FlockUnit> alignmentNeighbour = new List<FlockUnit>();
    List<FlockUnit> avoidanceNeighbour = new List<FlockUnit>();

    Flock assignedFlock;
    Vector3 currentVelocity;
    float speed;

    public Transform myTransform { get; set; }



    void Awake()
    {
        myTransform = transform;
    }
    void Start()
    {
        //var cohesionVector = CalculateCohesionVector(); // cohesian refers to how closely the other gameobjects are to each other in this reference
        //var alignmentVector = CalculateAlignmentVector();
    }

    public void InitializeSpeed(float speed)
    {
        this.speed = speed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, assignedFlock.cohesionDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, assignedFlock.avoidanceDistance);

    }

    public void MoveUnit()
    {
        FindNeighbour();
        CalculateSpeed();
        var cohesionVector = CalculateCohesionVector() * assignedFlock.cohesionWeights;
        var avoidanceVector = CalculateAvoidanceVector() * assignedFlock.avoidanceWeights;
        var aligementVector = CalculateAlignmentVector() * assignedFlock.alignemntWeights;
        var boundsVector = CalculateBoundsVector() * assignedFlock.boundsWeights;

        var moveVector = cohesionVector + avoidanceVector + aligementVector + boundsVector;
        moveVector = Vector3.SmoothDamp(myTransform.forward, moveVector, ref currentVelocity, smoothDamp);
        moveVector = moveVector.normalized * speed;
        if (moveVector == Vector3.zero)
            moveVector = transform.forward;

        myTransform.forward = moveVector;
        myTransform.position += moveVector * Time.deltaTime;

    }


    private void CalculateSpeed()
    {

        if (cohesionNeighbour.Count == 0)
            return;
        speed = 0;
        for (int i = 0; i < cohesionNeighbour.Count; i++)
        {
            speed += cohesionNeighbour[i].speed;
        }

        speed /= cohesionNeighbour.Count;
        speed = Mathf.Clamp(speed, assignedFlock.minSpeed, assignedFlock.maxSpeed);
    }


    private Vector3 CalculateAvoidanceVector()
    {
        var avoidanceVector = Vector3.zero;
        if (avoidanceNeighbour.Count == 0)
            return Vector3.zero;
        int neighboursInFOV = 0;
        for (int i = 0; i < avoidanceNeighbour.Count; i++)
        {
            if (IsInFOV(avoidanceNeighbour[i].myTransform.position))
            {
                neighboursInFOV++;
                avoidanceVector += myTransform.position - avoidanceNeighbour[i].myTransform.position;
            }
        }

        avoidanceVector /= neighboursInFOV;
        avoidanceVector = avoidanceVector.normalized;
        return avoidanceVector;
    }

    private Vector3 CalculateCohesionVector()
    {
        var cohesionVector = Vector3.zero;

        if (cohesionNeighbour.Count == 0)
            return Vector3.zero;

        int neighboursInFOV = 0;
        for (int i = 0; i < cohesionNeighbour.Count; i++)
        {
            if (IsInFOV(cohesionNeighbour[i].myTransform.position))
            {
                neighboursInFOV++;
                cohesionVector += cohesionNeighbour[i].myTransform.position;
            }
        }
        cohesionVector /= neighboursInFOV;
        cohesionVector -= myTransform.position;
        cohesionVector = cohesionVector.normalized;
        //cohesionVector.Normalize();
        return cohesionVector;
    }
    private Vector3 CalculateAlignmentVector()
    {
        var aligementVector = myTransform.forward;
        if (alignmentNeighbour.Count == 0)
            return myTransform.forward;
        int neighboursInFOV = 0;
        for (int i = 0; i < alignmentNeighbour.Count; i++)
        {
            if (IsInFOV(alignmentNeighbour[i].myTransform.position))
            {
                neighboursInFOV++;
                aligementVector += alignmentNeighbour[i].myTransform.forward;
            }
        }

        aligementVector /= neighboursInFOV;
        aligementVector = aligementVector.normalized;
        return aligementVector;

    }



    private Vector3 CalculateBoundsVector()
    {
        var offsetToCenter = assignedFlock.transform.position - myTransform.position;
        bool isNearCenter = offsetToCenter.magnitude >= assignedFlock.boundsDistance * 0.9f;
        return isNearCenter ? offsetToCenter.normalized : Vector3.zero;
    }

    private bool IsInFOV(Vector3 position)
    {

        return Vector3.Angle(myTransform.forward, position - myTransform.position) <= FOV;
        //float angle = Vector3.Angle(myTransform.forward, position - myTransform.position);
        //return angle < FOV && Vector3.Distance(position, myTransform.position) <= assignedFlock.cohesionDisntace;
    }

    public void AssignFlock(Flock flock)
    {
        assignedFlock = flock;
    }
    private void FindNeighbour()
    {
        cohesionNeighbour.Clear();
        alignmentNeighbour.Clear(); // Also clear the alignment neighbors list
        avoidanceNeighbour.Clear();
        var allUnits = assignedFlock.allUnits;
        for (int i = 0; i < allUnits.Length; i++)
        {
            var currentUnit = allUnits[i];
            if (currentUnit != null && currentUnit != this) // Ensure the unit doesn't add itself
            {
                float currentNeighbourDistanceSqrt = Vector3.SqrMagnitude(currentUnit.transform.position - myTransform.position);
                if (currentNeighbourDistanceSqrt <= assignedFlock.cohesionDistance * assignedFlock.cohesionDistance)
                {
                    cohesionNeighbour.Add(currentUnit);
                }
                if (currentNeighbourDistanceSqrt <= assignedFlock.alignemntDistance * assignedFlock.alignemntDistance)
                {
                    alignmentNeighbour.Add(currentUnit); // Add to both lists
                }
                if (currentNeighbourDistanceSqrt <= assignedFlock.avoidanceDistance * assignedFlock.avoidanceDistance)
                {
                    avoidanceNeighbour.Add(currentUnit);
                }

            }
        }

    }

}