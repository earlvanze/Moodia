using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {

    private int id;
        
    public Transform target;
    public float RotationSpeed = 100f;
    public float OrbitDegrees = 1f;

    void Start()
    {
        gameObject.transform.rotation = Random.rotation;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
        transform.RotateAround(target.position, Vector3.up, OrbitDegrees); 
    }
}
