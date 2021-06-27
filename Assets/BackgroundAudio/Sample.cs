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

    public AudioClip a;

    private void Awake()
    {
        _androidBackgroundAudio = BackgroundAudioManager.NewInstance();
        _androidBackgroundAudio.OnAudioStarted += () => UnityMainThreadDispatcher.Instance.Enqueue(() => _seekBar.maxValue = _androidBackgroundAudio.GetDuration());
        _toggle.onValueChanged.AddListener(SetLooping);
    }

    public void Play()
    {
#if UNITY_EDITOR
        GetAudioFileURI(s => StartCoroutine(LoadAudioInEditor(s)));
#else
        GetAudioFileURI(s => _androidBackgroundAudio.Play(s));
#endif
    }

    private void GetAudioFileURI(Action<string> callback)
    {
        string audioName = "SampleAudio.mp3";
        var streamingFilePath = Path.Combine(Application.streamingAssetsPath, audioName);
        var persistantPath = Path.Combine(Application.persistentDataPath, audioName);

#if UNITY_EDITOR
        callback?.Invoke(streamingFilePath); //persistantPath
        return;
#elif UNITY_IOS // IOS can read from StreamingAssets
        callback?.Invoke(filePath);
        return;
#elif UNITY_ANDROID // Android can't get files from streaming assets since its moved to persistant path
        Debug.LogError($"PersistantPath: {persistantPath}");
        if (File.Exists(persistantPath))
        {
            Debug.LogError("Exists");
            callback?.Invoke(persistantPath);
            return;
        }

        Debug.LogError($"StreamingPath: {streamingFilePath}");
        if (File.Exists(streamingFilePath))
            Debug.LogError("streaming assets path exists");
        UnityWebRequest req = new UnityWebRequest(streamingFilePath, UnityWebRequest.kHttpVerbGET, new DownloadHandlerFile(persistantPath), null);
        UnityWebRequestAsyncOperation asyncOp = req.SendWebRequest();
        asyncOp.completed += op =>
        {
            Debug.LogError("The callback is called");
            callback?.Invoke(persistantPath);
        };
#endif
    }

    private IEnumerator LoadAudioInEditor(string audioPath)
    {
        Debug.LogError("LOADING CLIP " + audioPath);

        if (!System.IO.File.Exists(audioPath))
        {
            Debug.LogError("DIDN'T EXIST: " + audioPath);
            yield break;
        }

        AudioClip audioClipFile = null;

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + audioPath, AudioType.WAV))
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
                audioClipFile = DownloadHandlerAudioClip.GetContent(www);
                Debug.LogError("WE have th clip now playing it");
                _androidBackgroundAudio.Play(audioClipFile);
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