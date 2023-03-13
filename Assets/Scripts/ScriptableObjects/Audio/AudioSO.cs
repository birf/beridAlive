
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioTracks
{
    public AUDIOCLIPS audioClipName;
    public AudioClip audioClip;
}

[CreateAssetMenu(fileName = "AudioClips", menuName = "Scriptable/Audio")]
public class AudioSO : ScriptableObject
{
    public List<AudioTracks> audioTracks;

    public AudioClip GetAudioClip(AUDIOCLIPS clipName)
    {
        for (int i = 0; i < audioTracks.Count; i++)
        {
            if (clipName == audioTracks[i].audioClipName)
            {
                return audioTracks[i].audioClip;
            }
        }
        return null;

    }

}
