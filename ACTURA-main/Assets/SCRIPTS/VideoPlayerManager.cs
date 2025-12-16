using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

[System.Serializable]
public class ButtonVideoMapping
{
    [Header("Button and Video Clip")]
    public Button button;
    public VideoClip videoClip;
    [Tooltip("Should this video loop when playing?")]
    public bool loopVideo = false;
}

public class VideoPlayerManager : MonoBehaviour
{
    [Header("Single Video Player Setup")]
    [Tooltip("The GameObject that contains the VideoPlayer component")]
    public GameObject videoPlayerGameObject;

    [Header("Button-Video Clip Mappings")]
    [SerializeField] private ButtonVideoMapping[] buttonVideoMappings = new ButtonVideoMapping[4];

    [Header("Video Player Settings")]
    [SerializeField] private bool showVideoPlayerWhenPlaying = true;
    [SerializeField] private bool hideVideoPlayerWhenNotPlaying = false;

    [Header("Auto Play Settings")]
    [SerializeField] private bool autoPlaySequence = true;
    [SerializeField] private float delayBetweenVideos = 0.5f; // Delay between videos in seconds
    [SerializeField] private bool startAutoPlayOnStart = true;

    [Header("Audio Settings")]
    [SerializeField] private float defaultVolume = 1.0f;

    private VideoPlayer videoPlayer;
    private int currentVideoIndex = 0;
    private bool isAutoPlaying = false;
    private Coroutine autoPlayCoroutine;

    private void Start()
    {
        // Find the VideoPlayer component
        FindVideoPlayerComponent();

        // Initialize all button listeners
        SetupButtonListeners();

        // Initialize video player
        InitializeVideoPlayer();

        // Start auto play if enabled
        if (startAutoPlayOnStart && autoPlaySequence)
        {
            StartAutoPlay();
        }
    }

    private void FindVideoPlayerComponent()
    {
        if (videoPlayerGameObject != null)
        {
            videoPlayer = videoPlayerGameObject.GetComponent<VideoPlayer>();

            if (videoPlayer == null)
            {
                Debug.LogError($"No VideoPlayer component found on GameObject '{videoPlayerGameObject.name}'!");
            }
            else
            {
                Debug.Log($"Found VideoPlayer component on '{videoPlayerGameObject.name}'");
            }
        }
        else
        {
            Debug.LogError("Video Player GameObject is not assigned!");
        }
    }

    private void SetupButtonListeners()
    {
        for (int i = 0; i < buttonVideoMappings.Length; i++)
        {
            if (buttonVideoMappings[i].button != null)
            {
                int index = i; // Capture the index for the lambda
                buttonVideoMappings[i].button.onClick.AddListener(() => PlayVideoManually(index));
                Debug.Log($"Button listener added for index {i}");
            }
            else
            {
                Debug.LogWarning($"Button at index {i} is not assigned!");
            }
        }
    }

    private void InitializeVideoPlayer()
    {
        if (videoPlayer != null)
        {
            // Set up video player properties
            videoPlayer.playOnAwake = false;
            videoPlayer.SetDirectAudioVolume(0, defaultVolume);

            // Subscribe to video events
            videoPlayer.loopPointReached += OnVideoFinished;

            // Initially hide if configured
            if (hideVideoPlayerWhenNotPlaying && videoPlayerGameObject != null)
            {
                videoPlayerGameObject.SetActive(false);
            }
        }
    }

    // Manual button play (stops auto play temporarily)
    public void PlayVideoManually(int buttonIndex)
    {
        StopAutoPlay();
        PlayVideo(buttonIndex);
    }

    // Internal play method
    private void PlayVideo(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= buttonVideoMappings.Length)
        {
            Debug.LogError($"Invalid button index: {buttonIndex}");
            return;
        }

        if (videoPlayer == null)
        {
            Debug.LogError("Video Player is not found!");
            return;
        }

        var selectedMapping = buttonVideoMappings[buttonIndex];

        if (selectedMapping.videoClip == null)
        {
            Debug.LogError($"Video Clip at index {buttonIndex} is not assigned!");
            return;
        }

        Debug.Log($"Playing video {buttonIndex + 1}: {selectedMapping.videoClip.name}");

        // Stop current video if playing
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }

        // Show the video player GameObject
        if (showVideoPlayerWhenPlaying && videoPlayerGameObject != null)
        {
            videoPlayerGameObject.SetActive(true);
        }

        // Set the new video clip
        videoPlayer.clip = selectedMapping.videoClip;

        // For auto play, don't loop individual videos (let sequence handle it)
        videoPlayer.isLooping = isAutoPlaying ? false : selectedMapping.loopVideo;

        // Start from the beginning
        videoPlayer.time = 0;

        // Update current index
        currentVideoIndex = buttonIndex;

        // Play the video immediately
        videoPlayer.Play();
    }

    // Called when a video finishes
    private void OnVideoFinished(VideoPlayer vp)
    {
        if (isAutoPlaying)
        {
            // Move to next video in sequence
            StartCoroutine(PlayNextVideoAfterDelay());
        }
    }

    private IEnumerator PlayNextVideoAfterDelay()
    {
        yield return new WaitForSeconds(delayBetweenVideos);

        // Move to next video
        currentVideoIndex = (currentVideoIndex + 1) % buttonVideoMappings.Length;

        // Play next video
        PlayVideo(currentVideoIndex);
    }

    // Auto play controls
    public void StartAutoPlay()
    {
        if (!isAutoPlaying)
        {
            isAutoPlaying = true;
            currentVideoIndex = 0; // Start from first video
            PlayVideo(currentVideoIndex);
            Debug.Log("Auto play sequence started");
        }
    }

    public void StopAutoPlay()
    {
        if (isAutoPlaying)
        {
            isAutoPlaying = false;
            if (autoPlayCoroutine != null)
            {
                StopCoroutine(autoPlayCoroutine);
                autoPlayCoroutine = null;
            }
            Debug.Log("Auto play sequence stopped");
        }
    }

    public void ToggleAutoPlay()
    {
        if (isAutoPlaying)
        {
            StopAutoPlay();
        }
        else
        {
            StartAutoPlay();
        }
    }

    // Original methods (updated)
    public void StopVideo()
    {
        StopAutoPlay();

        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();

            // Hide the GameObject if configured
            if (hideVideoPlayerWhenNotPlaying && videoPlayerGameObject != null)
            {
                videoPlayerGameObject.SetActive(false);
            }
        }
    }

    public void PauseVideo()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
    }

    public void ResumeVideo()
    {
        if (videoPlayer != null && !videoPlayer.isPlaying)
        {
            videoPlayer.Play();
        }
    }

    public void SetVolume(float volume)
    {
        defaultVolume = Mathf.Clamp01(volume);
        if (videoPlayer != null)
        {
            videoPlayer.SetDirectAudioVolume(0, defaultVolume);
        }
    }

    public bool IsVideoPlaying()
    {
        return videoPlayer != null && videoPlayer.isPlaying;
    }

    public bool IsAutoPlaying()
    {
        return isAutoPlaying;
    }

    public int GetCurrentVideoIndex()
    {
        return currentVideoIndex;
    }

    public float GetVideoProgress()
    {
        if (videoPlayer != null && videoPlayer.clip != null)
        {
            return (float)(videoPlayer.time / videoPlayer.clip.length);
        }
        return 0f;
    }

    private void OnDestroy()
    {
        // Clean up video events
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }

        // Stop coroutines
        if (autoPlayCoroutine != null)
        {
            StopCoroutine(autoPlayCoroutine);
        }

        // Clean up button listeners
        foreach (var mapping in buttonVideoMappings)
        {
            if (mapping.button != null)
            {
                mapping.button.onClick.RemoveAllListeners();
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Ensure we always have 4 mappings
        if (buttonVideoMappings.Length != 4)
        {
            System.Array.Resize(ref buttonVideoMappings, 4);
        }
    }
#endif
}