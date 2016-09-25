using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour {

    public GameObject nodePrefab;

	// Use this for initialization
	void Start () {

        for (int i = 0; i < 500; i++)
        {

            // Pick a random spawn position

            float x = Random.Range(-3, 3);
            float y = Random.Range(-5, 5);
            float z = Random.Range(-3, 3);
            Vector3 pos = new Vector3(x, y, z);

            Instantiate(nodePrefab, pos, Quaternion.identity);

        }

    }

    // Update is called once per frame
    void Update () {
	    
        

	}
}
