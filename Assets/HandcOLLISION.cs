using UnityEngine;
using System.Collections;

public class HandcOLLISION : MonoBehaviour {

	void OnCollisionEnter(Collider collider)
    {
        Debug.Log("Collided with hand");
    }
}
