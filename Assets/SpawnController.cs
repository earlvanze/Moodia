using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour {

    public GameObject nodePrefab;
    public GameObject bgNodePrefab;

    public float xRangeMin, xRangeMax, yRangeMin, yRangeMax, zRangeMin, zRangeMax;
    public int nodeAmount;

    // Control all of the node's rotations
    public Transform target;
    public float RotationSpeed = 100f;
    public float OrbitDegrees = 1f;

    // Use this for initialization
    void Start () {


        // Interactible nodes
        for (int i = 0; i < nodeAmount; i++)
        {
            // Pick a random spawn position
            float x = Random.Range(xRangeMin, xRangeMax);
            float y = Random.Range(yRangeMin, yRangeMax);
            float z = Random.Range(zRangeMin, zRangeMax);
            Vector3 pos = new Vector3(x, y, z);

            GameObject node = Instantiate(nodePrefab, pos, Quaternion.identity) as GameObject; ;
            node.transform.parent = gameObject.transform;
        }
        

    }

    // Update is called once per frame
    void Update () {

        if (!SpawnBalls.isIndexUp)
        {
            transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
            transform.RotateAround(target.position, Vector3.up, OrbitDegrees);
        }
        

    }
}
