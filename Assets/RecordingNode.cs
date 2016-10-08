using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		isRecording = false;
        recordingButtonPressed = false;
        rend = GetComponent<Renderer>();
        rend.enabled = true;

		//Check if there is at least one microphone connected  
		if(Microphone.devices.Length <= 0)  
		{  
			//Throw a warning message at the console if there isn't  
			Debug.LogWarning("Microphone not connected!");  
		}  
		else //At least one microphone is present  
		{  
			//Set 'micConnected' to true  
			micConnected = true;  

			//Get the default microphone recording capabilities  
			Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);  

			//According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...  
			if(minFreq == 0 && maxFreq == 0)  
			{  
				//...meaning 44100 Hz can be used as the recording sampling rate  
				maxFreq = 44100;  
			}  

			//Get the attached AudioSource component  
			goAudioSource = this.GetComponent<AudioSource>();  
		}  

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

		//If there is a microphone  
		if(micConnected)  
		{  
			//If the audio from any microphone isn't being captured  
			if(!Microphone.IsRecording(null))  
			{  
				//Case the 'Record' button gets pressed  
				if(GUI.Button(new Rect(Screen.width/2-100, Screen.height/2-25, 200, 50), "Record"))  
				{  
					//Start recording and store the audio captured from the microphone at the AudioClip in the AudioSource  
					goAudioSource.clip = Microphone.Start(null, true, 30, maxFreq);
				}  
			}  
			else //Recording is in progress  
			{  
				//Case the 'Stop and Play' button gets pressed  
				if(GUI.Button(new Rect(Screen.width/2-100, Screen.height/2-25, 200, 50), "Stop and Play!"))  
				{  
					Microphone.End(null); //Stop the audio recording
					goAudioSource.Play(); //Playback the recorded audio
				}
				recordingSaved = Save(goAudioSource.clip);	// Save the audio clip - returns true on success
				GUI.Label(new Rect(Screen.width/2-100, Screen.height/2+25, 200, 50), "Recording in progress...");  
			}
		}  
		else // No microphone  
		{  
			//Print a red "Microphone not connected!" message at the center of the screen  
			GUI.contentColor = Color.red;  
			GUI.Label(new Rect(Screen.width/2-100, Screen.height/2-25, 200, 50), "Microphone not connected!");  
		}
    }


	public static string filepath;
	public static string filename;

	const int HEADER_SIZE = 44;

	public static bool Save(AudioClip clip) {
		//generate a long random file name , to avoid duplicates and overwriting
		filename = Path.GetRandomFileName();
		filename = filename.Substring(0,6);
		filename = filename.ToUpper();
		filename = filename + ".wav";

		//if you save the generated name, you can make people be able to retrieve the uploaded file, without the needs of listings
		//just provide the Audio code name , and it will retrieve it just like a qrcode or something like that, please read below the method used to validate the upload,
		//that same method is used to retrieve the just uploaded file, and validate it
		//this method is similar to the one used by the popular game bike baron
		//this method saves you from the hassle of making complex server side back ends which enlists available Audios
		//this way you could enlist outstanding Audios just by posting the Audios code on a blog or forum, this way its easier to share, without the need of user accounts or install procedures

		if (!filename.ToLower().EndsWith(".wav")) {
			filename += ".wav";
		}

		filepath = Path.Combine(Application.persistentDataPath, filename);

		Debug.Log(filepath);

		// Make sure directory exists if user is saving to sub dir.
		Directory.CreateDirectory(Path.GetDirectoryName(filepath));

		using (var fileStream = CreateEmpty(filepath)) {

			ConvertAndWrite(fileStream, clip);

			WriteHeader(fileStream, clip);
		}

		return true; // TODO: return false if there's a failure saving the file
	}

	public static AudioClip TrimSilence(AudioClip clip, float min) {
		var samples = new float[clip.samples];

		clip.GetData(samples, 0);

		return TrimSilence(new List<float>(samples), min, clip.channels, clip.frequency);
	}

	public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz) {
		return TrimSilence(samples, min, channels, hz, false, false);
	}

	public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool _3D, bool stream) {
		int i;

		for (i=0; i<samples.Count; i++) {
			if (Mathf.Abs(samples[i]) > min) {
				break;
			}
		}

		samples.RemoveRange(0, i);

		for (i=samples.Count - 1; i>0; i--) {
			if (Mathf.Abs(samples[i]) > min) {
				break;
			}
		}

		samples.RemoveRange(i, samples.Count - i);

		var clip = AudioClip.Create("TempClip", samples.Count, channels, hz, _3D, stream);

		clip.SetData(samples.ToArray(), 0);

		return clip;
	}

	static FileStream CreateEmpty(string filepath) {
		var fileStream = new FileStream(filepath, FileMode.Create);
		byte emptyByte = new byte();

		for(int i = 0; i < HEADER_SIZE; i++) //preparing the header
		{
			fileStream.WriteByte(emptyByte);
		}

		return fileStream;
	}

	static void ConvertAndWrite(FileStream fileStream, AudioClip clip) {

		var samples = new float[clip.samples];

		clip.GetData(samples, 0);

		Int16[] intData = new Int16[samples.Length];
		//converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

		Byte[] bytesData = new Byte[samples.Length * 2];
		//bytesData array is twice the size of
		//dataSource array because a float converted in Int16 is 2 bytes.

		float rescaleFactor = 32767;

		for (int i = 0; i<samples.Length; i++) {
			intData[i] = (short) (samples[i] * rescaleFactor);
			Byte[] byteArr = new Byte[2];
			byteArr = BitConverter.GetBytes(intData[i]);
			byteArr.CopyTo(bytesData, i * 2);
		}

		fileStream.Write(bytesData, 0, bytesData.Length);
	}

	static void WriteHeader(FileStream fileStream, AudioClip clip) {

		var hz = clip.frequency;
		var channels = clip.channels;
		var samples = clip.samples;

		fileStream.Seek(0, SeekOrigin.Begin);

		Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
		fileStream.Write(riff, 0, 4);

		Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
		fileStream.Write(chunkSize, 0, 4);

		Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
		fileStream.Write(wave, 0, 4);

		Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
		fileStream.Write(fmt, 0, 4);

		Byte[] subChunk1 = BitConverter.GetBytes(16);
		fileStream.Write(subChunk1, 0, 4);

		UInt16 two = 2;
		UInt16 one = 1;

		Byte[] audioFormat = BitConverter.GetBytes(one);
		fileStream.Write(audioFormat, 0, 2);

		Byte[] numChannels = BitConverter.GetBytes(channels);
		fileStream.Write(numChannels, 0, 2);

		Byte[] sampleRate = BitConverter.GetBytes(hz);
		fileStream.Write(sampleRate, 0, 4);

		Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
		fileStream.Write(byteRate, 0, 4);

		UInt16 blockAlign = (ushort) (channels * 2);
		fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

		UInt16 bps = 16;
		Byte[] bitsPerSample = BitConverter.GetBytes(bps);
		fileStream.Write(bitsPerSample, 0, 2);

		Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
		fileStream.Write(datastring, 0, 4);

		Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
		fileStream.Write(subChunk2, 0, 4);

		fileStream.Close();
	}
}
