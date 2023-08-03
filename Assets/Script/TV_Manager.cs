using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using TMPro;
using UnityEngine.EventSystems;
public class TV_Manager : MonoBehaviour
{
    [Header("Video Player")]
    [SerializeField]
    private VideoPlayer videoPlayer;
    [SerializeField]
    private List<VideoClip> Videos, intialLoading;
    private int VideoCount;

    
    [SerializeField]
    private Button playButton, pauseButton, seekForwardButton, seekBackwardButton, video1, video2, video3, OnOff, playBtn, repeatBtn;
    [SerializeField]

    
    private GameObject GalleryParent, Gallery, VideoListVertical, PlayPausePreBack_panel, RepeatPanel, Blankscreen;

    [Header("Sprites")]
    [SerializeField]
    private List<Sprite> OnOffSprite, play_pause_Sprite;

    private bool isPlaying = false;
    private bool IsControlONOFF, IsControlPlay_Pause, IsTimerControl, IsControlPlayback, IsReachLoop;


    [SerializeField]
    private TMP_Text helloText;
    public float timer;
    private const string tvTag = "TV";

    private void OnEnable()
    {
        /* UI Buttons OnClick controls*/
        playButton.onClick.AddListener(delegate { PlayVideo(); });
        repeatBtn.onClick.AddListener(delegate { RepeatVideo(); });
        seekForwardButton.onClick.AddListener(delegate { SeekForward(); });
        seekBackwardButton.onClick.AddListener(delegate { SeekBackward(); });
        video1.onClick.AddListener(delegate { VideoSelection(0); });
        video2.onClick.AddListener(delegate { VideoSelection(1); });
        video3.onClick.AddListener(delegate { VideoSelection(2); });
        OnOff.onClick.AddListener(delegate { SwitchOn(); });
        Gallery.GetComponent<Button>().onClick.AddListener(delegate { IsControlPlay_Pause = false; });
        videoPlayer.loopPointReached += OnVideoEnd;

    }

    private void Init()              /* Just enable and disable method with the help of INVOKE method*/
    {
        helloText.gameObject.SetActive(false);
        Gallery.SetActive(true);
    }
    public void SwitchOn()            /* Switch On TV and change the sprites */
    {
        IsControlONOFF = !IsControlONOFF;
        IsControlPlayback = true;
        IsControlPlay_Pause = false;
        RepeatPanel.SetActive(false);
        Invoke("EnableScript", 0f);
        PlayPausePreBack_panel.SetActive(false);
        videoPlayer.time = 0f;
 
        if (IsControlONOFF)
        {

            CancelInvoke("Init");
            Gallery.SetActive(!IsControlONOFF);
            helloText.gameObject.SetActive(false);
        }

        OnOff.GetComponent<Image>().sprite = IsControlONOFF ? OnOffSprite[0] : OnOffSprite[1];
        videoPlayer.clip = intialLoading[0];
        GalleryParent.SetActive(IsControlONOFF);
        VideoListVertical.SetActive(!IsControlONOFF);

    }

    public void PlayVideo()              /* UI Buttons OnClick controls*/
    {

        IsControlPlay_Pause = !IsControlPlay_Pause;
        IsControlPlayback = false;
        IsReachLoop = false;
        Invoke("EnableScript", 0f);
        playBtn.GetComponent<Image>().sprite = IsControlPlay_Pause ? play_pause_Sprite[0] : play_pause_Sprite[1];
        if (IsControlPlay_Pause)
        {
            videoPlayer.clip = Videos[VideoCount];
            videoPlayer.Play();
            isPlaying = true;
            timer = 0;
        }
        else
        {
            videoPlayer.Pause();
            isPlaying = false;
            timer = 0;
        }

    }
    public void RepeatVideo()            /* Repeat the played video  */
    {
        IsControlPlay_Pause = false;
        PlayVideo();

    }
    private void Update()                /* Touch the TV screen and hide or enable the PlayPausePreBack_panel  */
    {
        TimerControl();
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.gameObject.CompareTag(tvTag))

                        {

                            if (videoPlayer.isPaused || videoPlayer.isPlaying)
                            {

                                if (!IsControlPlayback && !IsReachLoop)
                                {
                                    IsTimerControl = true;
                                    PlayPausePreBack_panel.SetActive(true);

                                }
                            }
                            else
                            {
                                PlayPausePreBack_panel.SetActive(false);

                            }
                        }
                    }

                }
            }
        }

    }

    public void TimerControl()             /* Time controller for control the Play,pause,forward,backward panels */
    {
        if (IsTimerControl)
        {
            timer += Time.deltaTime;
        }
        if (timer > 5)
        {
            timer = 0f;
            IsTimerControl = false;
            PlayPausePreBack_panel.SetActive(false);

        }
    }
    public void SeekForward()                /* seek forward the video for 5sec */
    {
        videoPlayer.time += 5f;
        timer = 0;
    }

    public void SeekBackward()                /* seek backward the video for 5sec*/
    {
        videoPlayer.time -= 5f;
        timer = 0;
    }
    public void VideoSelection(int value)       /* Select the videos and get the ID value */
    {
        VideoCount = value;
        PlayVideo();
    }

    private void OnVideoEnd(VideoPlayer vp)      /* Once Video ended Call this method for hiding some gameobjects  */
    {
        if (IsControlPlayback)
        {
            isPlaying = false;
            helloText.gameObject.SetActive(true);
            Invoke("Init", 1);
        }
        else
        {
            vp.time = 0.0f;
            IsReachLoop = true;
            Blankscreen.SetActive(true);
            PlayPausePreBack_panel.SetActive(false);
            RepeatPanel.SetActive(true);

        }
    }

    private void EnableScript()               /* Initially 1to2 frames blocking for previous Renderer texture can't display in screen */
    {

        Blankscreen.SetActive(true);
        Invoke("DisableScript", 0.5f);
    }

    private void DisableScript()
    {
        Blankscreen.SetActive(false);
    }
}
