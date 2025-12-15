using UnityEngine;
using System.Collections;
[System.Serializable]

public class VoiceLine
{
    public AudioClip clip;

    [TextArea(2, 4)]
    public string subtitle;
}
