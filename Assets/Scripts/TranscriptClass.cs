using System.Collections.Generic;

[System.Serializable]
public class Alternative
{
    public string transcript;
    public float confidence;
}

[System.Serializable]
public class Result
{
    public List<Alternative> alternatives;
    public string resultEndTime;
    public string languageCode;
}

[System.Serializable]
public class TranscriptionResponse
{
    public List<Result> results;
    public string totalBilledTime;
    public string requestId;
}
