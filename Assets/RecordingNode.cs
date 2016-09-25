using UnityEngine;
using System.Collections;

public class RecordingNode : MonoBehaviour {

    private int id;

    Animator animator;

    public Material[] materials;


    public Transform target;
    public float RotationSpeed = 100f;
    public float OrbitDegrees = 1f;

    public AudioClip[] collisionChimes;
    public AudioClip[] voiceMessages;

    private bool isPlayingMessage;

    private AudioSource audio;
    private float audioCoolDown;
    private Renderer rend;

    public static bool recordingButtonPressed = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        isPlayingMessage = false;
        audio = GetComponent<AudioSource>();
        recordingButtonPressed = false;
        rend = GetComponent<Renderer>();
        rend.enabled = true;

    }

    void Update()
    {

        if (!SpawnBalls.isIndexUp)
        {
            transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
            transform.RotateAround(target.position, Vector3.up, OrbitDegrees);
        }
        

    }

    void OnCollisionEnter(Collision collision)
    {
        /*if(!audio.isPlaying)
        {
            audio.clip = collisionChimes[Random.Range(0, 6)];
            audio.Play();
        }*/

        if (collision.collider.tag == "IndexCollider")
        {
            recordingButtonPressed = true;
            StartCoroutine(BeginRecording());
        }

    }

    IEnumerator BeginRecording()
    {
        audio.clip = voiceMessages[0];
        animator.SetBool("isAudioMessagePlaying", true);
        isPlayingMessage = true;
        yield return new WaitForSeconds(10);
        animator.SetBool("isAudioMessagePlaying", false);
    }


}
