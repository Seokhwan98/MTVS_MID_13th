using Newtonsoft.Json;

public class ApiResponse<T>
{
    public T data;
    public string error;
    public string message;
    public bool status;
    
    public static ApiResponse<T> Parse(string jsonText)
    {
        return JsonConvert.DeserializeObject<ApiResponse<T>>(jsonText);
    }
}

public class ApiData
{
    public static string LatestUploadFilename;
}

/// <summary>
/// get-json용 데이터 구조
/// </summary>
public class GetJsonData
{
    public int id;
    public string name;
    public string[] tags;
}

/// <summary>
/// get-echo-text용 데이터 구조
/// </summary>
public class GetEchoTextData
{
    public string received_message;
}

/// <summary>
/// post-echo-text용 데이터 구조
/// </summary>
public class PostEchoTextData
{
    public class Info
    {
        public int age;
        public string name;
    }
    
    public Info received_json;
}

/// <summary>
/// post-form-data용 데이터 구조
/// </summary>
public class PostFormData
{
    public int age;
    public string name;
}

public class UploadFileData
{
    public string filename;
    public int filesize;
}

