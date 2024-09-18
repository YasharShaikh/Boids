using System;
using UnityEngine;

public class Flock : MonoBehaviour
{

    [Header("Spawn Units")]
    [SerializeField] FlockUnit UnitPrefab;
    [SerializeField] int flockSize;
    [SerializeField] Vector3 spawnBounds;

    [Header("Speed Setup")]
    [Range(0, 10)]
    [SerializeField] float _minSpeed;
    public float minSpeed { get { return _minSpeed; } }
    [Range(0, 10)]
    [SerializeField] float _maxSpeed;
    public float maxSpeed { get { return _maxSpeed; } }



    [Header("Detection Distance")]
    [SerializeField] float _cohesionDistance;
    public float cohesionDisntace { get { return _cohesionDistance; } }
    [SerializeField] float _avoidanceDistance;
    public float avoidanceDisntace { get { return _avoidanceDistance; } }
    [SerializeField] float _alignmentDistance;
    public float alignemntDisntace { get { return _alignmentDistance; } }

    [SerializeField] float _boundstDistance;
    public float boundsDistance { get { return boundsDistance; } }



    [Header("Behaviour Weights")]
    [SerializeField] float _cohesionWeights;
    public float cohesionWeights { get { return _cohesionWeights; } }
    [SerializeField] float _avoidanceWeights;
    public float avoidanceWeights { get { return _avoidanceWeights; } }
    [SerializeField] float _alignmentWeights;
    public float alignemntWeights { get { return _alignmentWeights; } }

    [SerializeField] float _boundstWeights;
    public float boundsWeights { get { return boundsWeights; } }

    public FlockUnit[] allUnits { get; set; }



    // Start is called before the first frame update
    void Start()
    {
        GenerateUnits();
    }


    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < allUnits.Length; i++)
        {
            allUnits[i].MoveUnit();
        }
    }


    private void GenerateUnits()
    {
        allUnits = new FlockUnit[flockSize];
        for (int i = 0; i < flockSize; i++)
        {

            var randomVector = UnityEngine.Random.insideUnitSphere;
            randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y * spawnBounds.y, randomVector.z * spawnBounds.z);
            var spawmPosition = transform.position + randomVector;
            var spawnRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            allUnits[i] = Instantiate(UnitPrefab, spawmPosition, spawnRotation);
            allUnits[i].gameObject.name = "Fish:" + i;
            allUnits[i].AssignFlock(this);
            allUnits[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));
        }


    }
}
