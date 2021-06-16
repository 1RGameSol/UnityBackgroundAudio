using System;
using System.Collections;
using System.IO;

using BackgroundAudio;
using BackgroundAudio.Base;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    [SerializeField]
    private BackgroundAudioImplementation _androidBackgroundAudio;
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Slider _seekBar;

    private void Awake()
    {
        _androidBackgroundAudio = BackgroundAudioManager.NewInstance();
        // Debug.LogError("Instance is : " + UnityMainThreadDispatcher.Instance);
        _androidBackgroundAudio.OnAudioStarted += () => UnityMainThreadDispatcher.Instance.Enqueue(() => _seekBar.maxValue = _androidBackgroundAudio.GetDuration());
        _toggle.onValueChanged.AddListener(SetLooping);
    }

    public void Play()
    {
#if UNITY_EDITOR
        GetAudioFileURI(s => StartCoroutine(LoadFile(s)));
#else
        GetAudioFileURI(s => _androidBackgroundAudio.Play(s));
#endif
    }

    private void GetAudioFileURI(Action<string> callback)
    {
#if UNITY_IOS // IOS can read from StreamingAssets
        //var filePath = Path.Combine(Application.streamingAssetsPath, "SampleAudio.mp3");
        var filePath = Path.Combine(Application.streamingAssetsPath, "1-Circulation.flac");
        callback?.Invoke(filePath);
        return;

#elif UNITY_ANDROID // Android can't
        //var persistantPath = Path.Combine(Application.persistentDataPath, "SampleAudio.mp3");
        //var filePath = Path.Combine(Application.streamingAssetsPath, "SampleAudio.mp3");
        var persistantPath = Path.Combine(Application.persistentDataPath, "6-Sleep.flac");
        var filePath = Path.Combine(Application.streamingAssetsPath, "6-Sleep.flac");

        Debug.LogError($"PersistantPath: {persistantPath}");
        if (File.Exists(persistantPath))
        {
            Debug.LogError("Exists");
            callback?.Invoke(persistantPath);
            return;
        }

        Debug.LogError($"StreamingPath: {filePath}");

        var req = new UnityWebRequest(filePath, UnityWebRequest.kHttpVerbGET, new DownloadHandlerFile(persistantPath), null);
        var asyncOp = req.SendWebRequest();
        asyncOp.completed += op =>
        {
            Debug.LogError("The callback is called");
            callback?.Invoke(persistantPath);
        };
#endif
    }

    private IEnumerator LoadFile(string fullpath)
    {
        Debug.LogError("LOADING CLIP " + fullpath);

        if (!System.IO.File.Exists(fullpath))
        {
            Debug.LogError("DIDN'T EXIST: " + fullpath);
            yield break;
        }

        AudioClip temp = null;

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + fullpath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }
            if (www.isNetworkError || !string.IsNullOrEmpty(www.error))
            {
                Debug.LogError(www.error);
            }
            else
            {
                temp = DownloadHandlerAudioClip.GetContent(www);
                Debug.LogError("WE have th clip now playing it");
                _androidBackgroundAudio.Play(temp);
            }
        }

    }

    private void Update()
    {
        if (_androidBackgroundAudio.IsPlaying())
        {
            _seekBar.value = _androidBackgroundAudio.GetCurrentPosition();
        }
    }

    public void Pause()
    {
        _androidBackgroundAudio.Pause();
    }

    public void Resume()
    {
        _androidBackgroundAudio.Resume();
    }

    public void SeekFwd()
    {
        _androidBackgroundAudio.Seek(5f);
    }

    public void SeekBwd()
    {
        _androidBackgroundAudio.Seek(-5f);
    }

    public void SetLooping(bool value)
    {
        _androidBackgroundAudio.SetLoop(value);
    }
}