using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WebSample : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private AudioSource audioSource;
    
    private InputField _inputField;

    private void Awake()
    {
        _inputField = GetComponent<InputField>();
    }

#region HelloWorld
    public void SendHelloWorld()
    {
        StartCoroutine(WebApiClient.Get<string>("test", (message) =>
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("요청 성공");
            sb.AppendLine($"message: {message}");
            Debug.Log(sb.ToString());
        }));
    }

#endregion
    
#region GetJson
    public void SendGetJson()
    {
        StartCoroutine(WebApiClient.Get<GetJsonData>("get-json", (data) =>
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("요청 성공");
            sb.AppendLine($"id: {data.id}");
            sb.AppendLine($"name: {data.name}");
            foreach (var tag in data.tags)
            {
                sb.AppendLine($"tag: {tag}");
            }
            Debug.Log(sb.ToString());
        }));
    }

#endregion
    
#region GetEchoText
    public void SendGetEchoText(string message)
    {
        string encodedMessage = UnityWebRequest.EscapeURL(message);
        StartCoroutine(WebApiClient.Get<GetEchoTextData>($"get-echo-text?message={encodedMessage}", (data) =>
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("요청 성공");
            sb.AppendLine($"received_message: {data.received_message}");
            Debug.Log(sb.ToString());
        }));
    }

#endregion
    
#region PostEchoJson
    public void PostEchoJson()
    {
        var request = new 
        {
            age = _inputField.Age,
            name = _inputField.Name
        };
        
        string jsonData = JsonConvert.SerializeObject(request);
        
        StartCoroutine(WebApiClient.Post<PostEchoTextData>(
            $"post-echo-json", 
            jsonData,
            (data) =>
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("요청 성공");
            sb.AppendLine($"age: {data.received_json.age}");
            sb.AppendLine($"name: {data.received_json.name}");
            Debug.Log(sb.ToString());
        }));
    }
    
#endregion
    
#region PostForm
    public void PostForm()
    {
        WWWForm form = new WWWForm();
        form.AddField("age", _inputField.Age);
        form.AddField("name", _inputField.Name);
        
        StartCoroutine(WebApiClient.PostForm<PostFormData>(
            "post-form", 
            form,
            (data)=>
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("요청 성공");
            sb.AppendLine($"age: {data.age}");
            sb.AppendLine($"name: {data.name}");
            Debug.Log(sb.ToString());
        }));
    }
    
#endregion

#region UploadImageFile
    public void UploadImage(Texture2D texture)
    {
        var bytes = texture.EncodeToPNG();
        StartCoroutine(WebApiClient.UploadFile<UploadFileData>(
            "upload-file", bytes, $"{texture.name}.png", "image/png", data =>
            {
                ApiData.LatestUploadFilename = data.filename;
                
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("요청 성공");
                sb.AppendLine($"name: {data.filename}");
                sb.AppendLine($"size: {data.filesize}");
                Debug.Log(sb.ToString());
            }));
    }
    
#endregion

#region UploadAudioFile
    public void UploadAudio(AudioClip audioClip)
    {
        byte[] audioData = WebApiUtility.ConvertToWav(audioClip);
        
        StartCoroutine(WebApiClient.UploadFile<UploadFileData>(
            "upload-file", audioData, $"{audioClip.name}.wav", "audio/wav", data =>
            {
                ApiData.LatestUploadFilename = data.filename;
                    
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("요청 성공");
                sb.AppendLine($"name: {data.filename}");
                sb.AppendLine($"size: {data.filesize}");
                Debug.Log(sb.ToString());
            }));
    }
    
#endregion

#region GetFile
    public void GetFile()
    {
        string filename = ApiData.LatestUploadFilename;

        if (string.IsNullOrEmpty(filename))
        {
            Debug.LogWarning("파일명이 설정되지 않았습니다.");
            return;
        }

        string extension = Path.GetExtension(filename).ToLower();

        if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
        {
            StartCoroutine(WebApiClient.DownloadFile<Sprite>(
                filename,
                sprite =>
                {
                    if (sprite != null)
                    {
                        image.sprite = sprite;
                    }
                }));
        }
        else if (extension == ".wav" || extension == ".mp3" || extension == ".ogg")
        {
            StartCoroutine(WebApiClient.DownloadFile<AudioClip>(
                filename,
                clip =>
                {
                    if (clip != null)
                    {
                        audioSource.resource = clip;
                        audioSource.Play();
                        Debug.Log($"{filename} is Playing");
                    }
                }));
        }
        else
        {
            Debug.LogWarning($"지원하지 않는 파일 형식: {extension}");
        }
    }
    
#endregion

}
