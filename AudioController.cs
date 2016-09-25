
 
using UnityEngine;
using System.Xml;
using System.IO;
using System.Collections;
using System.Text;

public class AudioRecorder : MonoBehaviour {
    void Start() {
        AudioSource aud = GetComponent<AudioSource>();
        aud.clip = Microphone.Start("Built-in Microphone", true, 10, 44100);
        aud.Play();
    }
}

static public void SaveAudioClipToWav(AudioClip audioClip, string filename)
{
    FileStream fsWrite = File.Open(filename, FileMode.Create);

    BinaryWriter bw = new BinaryWriter(fsWrite);

    Byte[] header = { 82, 73, 70, 70, 22, 10, 4, 0, 87, 65, 86, 69, 102, 109, 116, 32 };
    bw.Write(header);

    Byte[] header2 = { 16, 0, 0, 0, 1, 0, 1, 0, 68, 172, 0, 0, 136, 88, 1, 0 };
    bw.Write(header2);

    Byte[] header3 = { 2, 0, 16, 0, 100, 97, 116, 97, 152, 9, 4, 0 };
    bw.Write(header3);

    float[] samples = new float[audioClip.samples];
    audioClip.GetData(samples, 0);
    int i = 0;

    while (i < audioClip.samples)
    {
        int sampleInt = (int)(32000.0 * samples[i++]);

        int msb = sampleInt / 256;
        int lsb = sampleInt - (msb * 256);

        bw.Write((Byte)lsb);
        bw.Write((Byte)msb);
    }

    fsWrite.Close();

}
 
// a very simplistic Audio upload and random name generator script
 
public class AudioUploader : MonoBehaviour
{
    void StartUpload()
    {
        StartCoroutine("UploadAudio");
    }
   
    IEnumerator UploadAudio()  
    {
        //making a dummy xml Audio file
        XmlDocument map = new XmlDocument();
        map.LoadXml("<Audio></Audio>");
       
        //converting the xml to bytes to be ready for upload
        byte[] AudioData =Encoding.UTF8.GetBytes(map.OuterXml);
       
        //generate a long random file name , to avoid duplicates and overwriting
        string fileName = Path.GetRandomFileName();
        fileName = fileName.Substring(0,6);
        fileName = fileName.ToUpper();
        fileName = fileName + ".xml";
       
        //if you save the generated name, you can make people be able to retrieve the uploaded file, without the needs of listings
        //just provide the Audio code name , and it will retrieve it just like a qrcode or something like that, please read below the method used to validate the upload,
        //that same method is used to retrieve the just uploaded file, and validate it
        //this method is similar to the one used by the popular game bike baron
        //this method saves you from the hassle of making complex server side back ends which enlists available Audios
        //this way you could enlist outstanding Audios just by posting the Audios code on a blog or forum, this way its easier to share, without the need of user accounts or install procedures
        WWWForm form = new WWWForm();
 
        print("form created ");
       
        form.AddField("action", "audio upload");
 
        form.AddField("file","file");
 
        form.AddBinaryData ( "file", AudioData, fileName,"audio/wav");
 
        print("binary data added ");
        //change the url to the url of the php file
        WWW w = new WWW("http://moodia.me/audio_upload.php", form);
        print("www created");
 
        yield return w;
        print("after yield w");
        if (w.error != null)
        {
            print("error");
            print ( w.error );    
        }
        else
        {
            //this part validates the upload, by waiting 5 seconds then trying to retrieve it from the web
            if(w.uploadProgress == 1  w.isDone)
            {
                yield return new WaitForSeconds(5);
                //change the url to the url of the folder you want it the Audios to be stored, the one you specified in the php file
                WWW w2 = new WWW("http://moodia.me/audio/" + fileName);
                yield return w2;
                if(w2.error != null)
                {
                    print("error 2");
                    print ( w2.error );  
                }
                else
                {
                    //then if the retrieval was successful, validate its content to ensure the Audio file integrity is intact
                    if(w2.text != null  w2.text != "")
                    {
                        if(w2.text.Contains("<Audio>")  w2.text.Contains("</Audio>"))
                        {
                            //and finally announce that everything went well
                            print ( "Audio File " + fileName + " Contents are: \n\n" + w2.text);
                            print ( "Finished Uploading Audio " + fileName);
                        }
                        else
                        {
                            print ( "Audio File " + fileName + " is Invalid");
                        }
                    }
                    else
                    {
                        print ( "Audio File " + fileName + " is Empty");
                    }
                }
            }      
        }
    }
   
    void OnGUI()
    {
        if(GUILayout.Button("Click me!"))
        {
            StartUpload();
        }
    }
}