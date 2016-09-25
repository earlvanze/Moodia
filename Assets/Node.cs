using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {

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
    
    void Start()
    {
        animator = GetComponent<Animator>();
        isPlayingMessage = false;
        audio = GetComponent<AudioSource>();

        rend = GetComponent<Renderer>();
        rend.enabled = true;
    }

    void Update()
    {

        if(!SpawnBalls.isIndexUp)
        {
            transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
            transform.RotateAround(target.position, Vector3.up, OrbitDegrees);
        }
                
    }
    
    void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.tag == "IndexCollider" && SpawnBalls.isIndexUp && !audio.isPlaying)
        {
            StartCoroutine(PlayUntilFinished());
        } else if (!audio.isPlaying && collision.collider.tag != "Node")
        {
            audio.clip = collisionChimes[Random.Range(0, 6)];
            audio.Play();
        }
        
    }

    IEnumerator PlayUntilFinished()
    {
        audio.clip = voiceMessages[0];
        animator.SetBool("isAudioMessagePlaying", true);
        audio.Play();
        isPlayingMessage = true;
        rend.sharedMaterial = materials[1];
        yield return new WaitForSeconds(audio.clip.length);
        animator.SetBool("isAudioMessagePlaying", false);
    }


}