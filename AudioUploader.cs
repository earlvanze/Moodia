// a very simplistic Audio upload and random name generator script
 
public class AudioUploader : MonoBehaviour
{
    void StartUpload()
    {
        StartCoroutine("UploadAudio");
    }

    public static byte[] ReadFully(Stream input)
    {
        byte[] buffer = new byte[16*1024];
        using (MemoryStream ms = new MemoryStream())
        {
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }
   
    IEnumerator UploadAudio()
    {
        //converting the wav file to bytes to be ready for upload

        using (var fileStream = CreateEmpty(filepath)) {
            byte[] audioData = readFully(fileStream);
        }

        WWWForm form = new WWWForm();
 
        print("form created ");
       
        form.AddField("action", "audio upload");
 
        form.AddField("file","file");
 
        form.AddBinaryData ( "file", fileStream, filename,"audio/wav");
 
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
            if(w.uploadProgress == 1 || w.isDone)
            {
                yield return new WaitForSeconds(5);
                //change the url to the url of the folder you want it the Audios to be stored, the one you specified in the php file
                WWW w2 = new WWW("http://moodia.me/audio/" + SavWav.filename);
                yield return w2;
                if(w2.error != null)
                {
                    print("error 2");
                    print ( w2.error );  
                }
                else
                {
                    //then if the retrieval was successful, validate its content to ensure the Audio file integrity is intact
                    if(w2.text != null || w2.text != "")
                    {
                        if(w2.text.Contains("<Audio>"))
                        {
                            //and finally announce that everything went well
                            print ( "Audio File " + SavWav.filename + " Contents are: \n\n" + w2.text);
                            print ( "Finished Uploading Audio " + SavWav.filename);
                        }
                        else
                        {
                            print ( "Audio File " + SavWav.filename + " is Invalid");
                        }
                    }
                    else
                    {
                        print ( "Audio File " + SavWav.filename + " is Empty");
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