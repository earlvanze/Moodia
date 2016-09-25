using UnityEngine;
using System.Collections;

public class SpawnBalls : MonoBehaviour {
  public GameObject BallPrefab;
  public float delayInterval = .15f; // seconds
  public int BallLimit = 100;
  public Vector3 BallSize = new Vector3(0.1f, 0.1f, 0.1f);

   public static bool isIndexUp;

  private IEnumerator _spawnCoroutine;

  void Awake () {
    _spawnCoroutine = AddBallWithDelay(BallPrefab);
        isIndexUp = false;
  }

  public void StartBalls(){
        Debug.Log("FINGER IS UP");
        isIndexUp = true;

    }

  public void StopBalls(){
        Debug.Log("FINGER IS NOT UP");
        isIndexUp = false;
        
  }

  private IEnumerator AddBallWithDelay (GameObject prefab) {
    while (true) {
      addBall(prefab);
      yield return new WaitForSeconds(delayInterval);
    }
  }

  private void addBall (GameObject prefab) {
    if (transform.childCount > BallLimit) removeBalls(BallLimit / 10);
    GameObject go = GameObject.Instantiate(prefab);
    go.transform.parent = transform;
    go.transform.localPosition = Vector3.zero;
    go.transform.localScale = BallSize;
    Rigidbody rb = go.GetComponent<Rigidbody>();
    rb.AddForce(Random.value * 3, -Random.value * 13, Random.value * 3, ForceMode.Impulse);
  }

  private void removeBalls (int count) {
    if (count > transform.childCount) count = transform.childCount;
    for (int b = 0; b < count; b++) {
      Destroy(transform.GetChild(b).gameObject);
    }
  }
}
