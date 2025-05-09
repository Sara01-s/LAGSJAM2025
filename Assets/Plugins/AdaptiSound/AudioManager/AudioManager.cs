using UnityEngine;
using UnityEngine.Audio;
using System.IO;
using System.Collections.Generic;
using System.Collections;

namespace AdaptiSound
{

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    

    [Header("Audio Directories")]                                           // Directories
    [Tooltip("Assign BGM Directorie in Assets/Resources folder")]
    [SerializeField] private string BGM_dir = "";    
    [Tooltip("Assign ABGS Directorie in Assets/Resources folder")]
    [SerializeField] private string ABGM_dir = "";   
    [Tooltip("Assign BGS Directorie in Assets/Resources folder")]
    [SerializeField] private string BGS_dir = "";                    
    
    [Header("Extensions")]                                                  // Extensions
    [SerializeField] private bool OGG;
    [SerializeField] private bool WAV;
    [SerializeField] private bool MP3;

    [Header("Audio Buses")]                                                 // Audio Buses
    [SerializeField] public AudioMixerGroup bgm_bus;
    [SerializeField] public AudioMixerGroup abgm_bus;
    [SerializeField] public AudioMixerGroup bgs_bus;

    [Header("Debug")]
    [SerializeField] private bool Debugging;
    private static bool can_debug;


    private string[] fileExtensions = new string[] {};

    private GameObject bgm_container;                                       // BGM Audio Container
    private Dictionary<string, AudioClip> BGM_audio_clips;                  // BGM Audio Clips Dictionary
    private Dictionary<string, Adapti_AudioSource> BGM_audio_sources;       // BGM Audio Sources Dictionary

    private GameObject abgm_container;                                      // ABGM Audio Container
    private Dictionary<string, GameObject> ABGM_audio_prefabs;              // ABGM Audio Prefabs Dictionary
    private Dictionary<string, GameObject> ABGM_audio_sources;              // ABGM Audio Sources Dictionary

    private GameObject bgs_container;                                       // BGS Audio Container
    private Dictionary<string, AudioClip> BGS_audio_clips;                  // BGS Audio Clips Dictionary
    private Dictionary<string, Adapti_AudioSource> BGS_audio_sources;       // BGS Audio Sources Dictionary


    [HideInInspector] public string current_playback;                       // Current Playback
    [HideInInspector] public string current_bgs_playback;


    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {

        can_debug = Debugging;

        bgm_container = GameObject.Find("BGM");
        abgm_container = GameObject.Find("ABGM");
        bgs_container = GameObject.Find("BGS");

        // EXTENSIONS //
        fileExtensions = set_extensions();

        BGM_audio_clips = new Dictionary<string, AudioClip>();
        BGM_audio_sources = new Dictionary<string, Adapti_AudioSource>();
        ABGM_audio_prefabs = new Dictionary<string, GameObject>();
        ABGM_audio_sources = new Dictionary<string, GameObject>();
        BGS_audio_clips = new Dictionary<string, AudioClip>();
        BGS_audio_sources = new Dictionary<string, Adapti_AudioSource>();

        file_browser();
    }


    public void playMusic(string track_name, float volume = 1.0f, float fade_in = 0.5f, float fade_out = 0.5f, int loop_index = 0)
    {
        Adapti_AudioSource track = null;
        GameObject abgm_track = null;

        GameObject container = get_container(track_name);
        if (container == null)
        {
            print("warning", "Track not found");
            return;
        }

        // BGM //
        if (container == bgm_container)
        {
            track = add_bgm_track(track_name);

            if (current_playback == track_name)
            {
                print("warning", "Track alredy playing");
                return;
            }

            // Stop Current Playback //
            if (current_playback != "")
            {
                stop_current_playback(fade_out);
            }

            // Play Audio //
            if (fade_in != 0.0f)
            {
                track.StartFade(fade_in, volume, true);
            }
            else
            {
                track.StartFade(fade_in, volume, true);
                track.audio_source.volume = volume;
            }
            track.audio_source.Play();
            current_playback = track_name;
            return;
        }


        // ABGM //
        if (container == abgm_container)
        {
            abgm_track = add_abgm_track(track_name);
            AdaptiNode abgm_component = abgm_track.GetComponent<AdaptiNode>();
            //Debug.Log(abgm_component);
            
            if (current_playback == track_name)
            {
                print("warning", "Track alredy playing");
                return;
            }

            // Stop Current Playback //
            if (current_playback != "")
            {
                stop_current_playback(fade_out);
            }

            // Play Audio //
            abgm_component.bus = abgm_bus;
            abgm_component.on_play(fade_in, volume, true, loop_index);
            current_playback = track_name;
            return;
        }

    }

    private void stop_current_playback(float fade_out)
    {
        Adapti_AudioSource current_track = null;
        GameObject abgm_current_track = null;

        if (BGM_audio_sources.ContainsKey(current_playback))
        {
            current_track = BGM_audio_sources[current_playback];
            if (fade_out != 0.0f)
            {
                current_track.StartFade(fade_out, 0.0f, false);
            }
            else
            {
                current_track.audio_source.Stop();
            }
        }
        if (ABGM_audio_sources.ContainsKey(current_playback))
        {
            abgm_current_track = GameObject.Find(current_playback + "(Clone)");
            AdaptiNode adaptinode = abgm_current_track.GetComponent<AdaptiNode>();

            if (fade_out != 0.0f)
            {
                adaptinode.on_stop(fade_out);
            }
            else
            {
                adaptinode.on_stop();
            }
        }
    }



    public void resetMusic(float fade_out = 0.0f, float fade_in = 0.0f)
    {   
        print("log", "resetMusic not available yet");
    }


    public void stopMusic(bool can_fade = false, float fade_out = 1.5f)
    {
        if (current_playback != "")
        {
            GameObject container = get_container(current_playback);

            if (container != abgm_container)
            {
                Adapti_AudioSource track = get_bgm_track(current_playback);
                if (can_fade)
                {
                    track.StartFade(fade_out, 0.0f, false);
                }
                else
                {
                    track.audio_source.Stop();
                }
            }
            else
            {
                AdaptiNode adaptinode = get_abgm_track(current_playback);
                if (can_fade == false)
                {
                    fade_out = 0.0f;
                }
                adaptinode.on_stop(fade_out);
            }

            current_playback = "";
            return;
        }
        else
        {
            print("log", "No track playing");
            return;
        }
    }



    public void toOutro(string track_name, bool can_fade = false, float fade_out = 0.5f, float fade_in = 0.5f)
    {
        if (!can_fade)
        {
            fade_out = 0.0f;
            fade_in = 0.0f;
        }

        if(!ABGM_audio_prefabs.ContainsKey(track_name))
        {
            print("warning", "Track not found");
            return;
        }

        if (current_playback != track_name)
        {
            print("warning", "Track is not playing");
            return;
        }

        GameObject track = add_abgm_track(track_name);
        AdaptiNode abgm_component = track.GetComponent<AdaptiNode>();

        abgm_component.on_outro(fade_out, fade_in);
    }


    public void setSequence(string track_name)
    {
        if (current_playback == "")
        {
            print("warning", "No current track to assign sequence");
            return;
        }

        if(!ABGM_audio_prefabs.ContainsKey(current_playback))
        {
            print("warning", "Current track is not ABGM, set secuence only be set in ABGM tracks");
            return;
        }

        GameObject track = add_abgm_track(current_playback);
        AdaptiNode abgm_component = track.GetComponent<AdaptiNode>();

        abgm_component.set_sequence(track_name);
    }



    public void changeLoop(string track_name, int loop_index, bool can_fade = false, float fade_out = 0.5f, float fade_in = 0.5f)
    {
        if (can_fade == false)
        {
            fade_out = 0.0f;
            fade_in = 0.0f;
        }

        if(!ABGM_audio_prefabs.ContainsKey(track_name))
        {
            print("warning", "Track not found");
            return;
        }

        if (current_playback != track_name)
        {
            print("warning", "Track is not playing");
            return;
        }

        GameObject track = add_abgm_track(track_name);
        AdaptiNode abgm_component = track.GetComponent<AdaptiNode>();

        abgm_component.change_loop(loop_index, fade_out, fade_in);
    }


    public void muteAllLayer(string track_name, bool mute_state, float fade_time = 1.5f)
    {
        if(!ABGM_audio_prefabs.ContainsKey(track_name))
        {
            print("warning", "Track not found");
            return;
        }

        if (current_playback != track_name)
        {
            print("warning", "Track is not playing");
            return;
        }

        GameObject track = add_abgm_track(track_name);
        AdaptiNode abgm_component = track.GetComponent<AdaptiNode>();

        abgm_component.mute_all_layer(fade_time, mute_state);
    }


    public void muteLayer(string track_name, int layer, bool mute_state, float fade_time = 1.5f)
    {

        if(!ABGM_audio_prefabs.ContainsKey(track_name))
        {
            print("warning", "Track not found");
            return;
        }

        if (current_playback != track_name)
        {
            print("warning", "Track is not playing");
            return;
        }

        GameObject track = add_abgm_track(track_name);
        AdaptiNode abgm_component = track.GetComponent<AdaptiNode>();

        abgm_component.mute_layer(layer, fade_time, mute_state);

    }



    public void stopAll(string tracks_category = "ALL", float fade_time = 0.5f)
    {
        if (tracks_category == "BGM" || tracks_category == "ABGM")
        {
            foreach (AdaptiNode tracks in abgm_container.GetComponentsInChildren<AdaptiNode>())
            {
                tracks.on_stop(fade_time);
            }

            foreach (Adapti_AudioSource tracks in bgm_container.GetComponents<Adapti_AudioSource>())
            {
                if (fade_time != 0.0f)
                {
                    tracks.StartFade(fade_time, 0.0f, false);
                }
                else
                {
                    tracks.audio_source.Stop();
                }
            }

            current_playback = "";
        } 
        if (tracks_category == "BGS")
        {
            foreach (Adapti_AudioSource tracks in bgs_container.GetComponents<Adapti_AudioSource>())
            {
                if (fade_time != 0.0f)
                {
                    tracks.StartFade(fade_time, 0.0f, false);
                }
                else
                {
                    tracks.audio_source.Stop();
                }
            }
            current_bgs_playback = "";
        }
        else
        {
            foreach (AdaptiNode tracks in abgm_container.GetComponentsInChildren<AdaptiNode>())
            {
                tracks.on_stop(fade_time);
            }

            foreach (Adapti_AudioSource tracks in bgm_container.GetComponents<Adapti_AudioSource>())
            {
                if (fade_time != 0.0f)
                {
                    tracks.StartFade(fade_time, 0.0f, false);
                }
                else
                {
                    tracks.audio_source.Stop();
                }
            }

            foreach (Adapti_AudioSource tracks in bgs_container.GetComponents<Adapti_AudioSource>())
            {
                if (fade_time != 0.0f)
                {
                    tracks.StartFade(fade_time, 0.0f, false);
                }
                else
                {
                    tracks.audio_source.Stop();
                }
            }
            current_playback = "";
            current_bgs_playback = "";
        }
    }


    public void playSound(string track_name, float volume = 1.0f, float fade_in = 0.5f, float fade_out = 0.5f)
    {
        Adapti_AudioSource track = null;
        //GameObject abgm_track = null;

        GameObject container = get_container(track_name);
        if (container == null)
        {
            print("warning", "Track not found");
            return;
        }

        // BGS //
        if (container == bgs_container)
        {
            track = add_bgs_track(track_name);

            if (current_bgs_playback == track_name)
            {
                print("warning", "Track alredy playing");
                return;
            }

            // Stop Current Playback //
            if (current_bgs_playback != "")
            {
                stop_current_playback(fade_out);
            }

            // Play Audio //
            if (fade_in != 0.0f)
            {
                track.StartFade(fade_in, volume, true);
            }
            else
            {
                track.StartFade(fade_in, volume, true);
                track.audio_source.volume = volume;
            }
            track.audio_source.Play();
            current_bgs_playback = track_name;
            return;
        }
    }



    private Adapti_AudioSource add_bgm_track(string track_name)
    {
        if (!BGM_audio_sources.ContainsKey(track_name))
        {
            Adapti_AudioSource bgm_audio = bgm_container.AddComponent<Adapti_AudioSource>();
            bgm_audio.audio_source.clip = BGM_audio_clips[track_name];                  
            BGM_audio_sources[track_name] = bgm_audio;
            return bgm_audio;
        }
        else
        {
            Adapti_AudioSource bgm_audio = BGM_audio_sources[track_name];
            return bgm_audio;
        }
    }


    private GameObject add_abgm_track(string track_name)
    {
        if (!ABGM_audio_sources.ContainsKey(track_name))
        {
            GameObject track = Instantiate(ABGM_audio_prefabs[track_name], abgm_container.transform);
            ABGM_audio_sources[track_name] = track;
            return track;
        }
        else
        {
            GameObject track = GameObject.Find(track_name + "(Clone)");
            return track;
        }
    }


    // BGS //
    private Adapti_AudioSource add_bgs_track(string track_name)
    {
        if (!BGS_audio_sources.ContainsKey(track_name))
        {
            Adapti_AudioSource bgs_audio = bgs_container.AddComponent<Adapti_AudioSource>();
            bgs_audio.audio_source.clip = BGS_audio_clips[track_name];                  
            BGS_audio_sources[track_name] = bgs_audio;
            return bgs_audio;
        }
        else
        {
            Adapti_AudioSource bgs_audio = BGS_audio_sources[track_name];
            return bgs_audio;
        }
    }

    

    private void add_clip_dictionary(AudioClip audioClip, string type)
    {
        if ( type == "BGM")
        {
            BGM_audio_clips[audioClip.name] = audioClip;
        }
        if ( type == "BGS")
        {
            BGS_audio_clips[audioClip.name] = audioClip;
        }
        
    }

    private void add_prefab_dictionary(GameObject abgmPrefab)
    {
        ABGM_audio_prefabs[abgmPrefab.name] = abgmPrefab;
    }


    public void removeTrack(string track_name)
    {
        GameObject container = get_container(track_name);
        
        if (container == bgm_container)
        {
            if (!BGM_audio_sources.ContainsKey(track_name))
            {
                return;
            }
            BGM_audio_sources[track_name].removeTrack();
            BGM_audio_sources.Remove(track_name);
            current_playback = "";
        }

        if (container == abgm_container)
        {
            if (!ABGM_audio_sources.ContainsKey(track_name))
            {
                return;
            }
            AdaptiNode track = ABGM_audio_sources[track_name].GetComponent<AdaptiNode>();
            track.removeTrack();
            Destroy(ABGM_audio_sources[track_name]);
            ABGM_audio_sources.Remove(track_name);
            current_playback = "";
        }

        if (container == bgs_container)
        {
            if (!BGS_audio_sources.ContainsKey(track_name))
            {
                return;
            }
            BGS_audio_sources[track_name].removeTrack();
            BGS_audio_sources.Remove(track_name);
            current_bgs_playback = "";
        }
    }


    public Adapti_AudioSource get_bgm_track(string track_name)
    {
        GameObject container = get_container(track_name);

        if (container == bgm_container)
        {
            Adapti_AudioSource track = BGM_audio_sources[track_name];
            return track;
        }
        else
        {
            return null;
        }
    }

    public AdaptiNode get_abgm_track(string track_name)
    {
        GameObject container = get_container(track_name);

        if (container == abgm_container)
        {
            GameObject track = GameObject.Find(track_name + "(Clone)");
            if (track == null)
            {
                print("warning", "Track is not in the scene");
                return null;
            }
            AdaptiNode abgm_component = track.GetComponent<AdaptiNode>();
            return abgm_component;
        }
        else
        {
            return null;
        }
    }


    public Adapti_AudioSource get_bgs_track(string track_name)
    {
        GameObject container = get_container(track_name);

        if (container == bgs_container)
        {
            Adapti_AudioSource track = BGS_audio_sources[track_name];
            return track;
        }
        else
        {
            return null;
        }
    }


    private GameObject get_container(string track_name)
    {
        if (BGM_audio_clips.ContainsKey(track_name))
        {
            return bgm_container;
        }
        if (ABGM_audio_prefabs.ContainsKey(track_name))
        {
            return abgm_container;
        }
        if (BGS_audio_clips.ContainsKey(track_name))
        {
            return bgs_container;
        }
        else
        {
            return null;
        }
    }


    // File Browser //
    void file_browser()
    {
        // BGM //
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>(BGM_dir);

        if (audioClips.Length == 0)
        {
            print("log", "No files in BGM Directory");
        }

        foreach (AudioClip audioClip in audioClips)
        {
            add_clip_dictionary(audioClip, "BGM");
        }

        // ABGM //
        GameObject[] prefabs = Resources.LoadAll<GameObject>(ABGM_dir);

        if (prefabs.Length == 0)
        {
            print("log", "No files in ABGM Directory");
        }

        foreach (GameObject file in prefabs)
        {
            add_prefab_dictionary(file);
        }

        // BGS //
        AudioClip[] bgs_audioClips = Resources.LoadAll<AudioClip>(BGS_dir);

        if (bgs_audioClips.Length == 0)
        {
            print("log", "No files in BGS Directory");
        }

        foreach (AudioClip audioClip in bgs_audioClips)
        {
            add_clip_dictionary(audioClip, "BGS");
        }

    }

    // Audio Files Extensions //
    private string[] set_extensions()
    {
        string[] extensions = new string[] {};

        if (OGG)
        {

            string[] new_ext = new string[extensions.Length + 1];
            for (int i = 0; i < extensions.Length; i++)
            {
                new_ext[i] = extensions[i];
            }
            new_ext[extensions.Length] = "ogg";

            extensions = new_ext;
        }

        if (WAV)
        {
            string[] new_ext = new string[extensions.Length + 1];
            for (int i = 0; i < extensions.Length; i++)
            {
                new_ext[i] = extensions[i];
            }
            new_ext[extensions.Length] = "wav";

            extensions = new_ext;
        }

        if (MP3)
        {
            string[] new_ext = new string[extensions.Length + 1];
            for (int i = 0; i < extensions.Length; i++)
            {
                new_ext[i] = extensions[i];
            }
            new_ext[extensions.Length] = "mp3";

            extensions = new_ext;
        }
        
        /*foreach (string ext in extensions)
        {
            Debug.Log(ext);
        }*/

        return extensions;
    }



    // DEBUG //
    public static void print(string type, string message)
    {
        if (can_debug)
        {
            if (type == "error")
            {
                Debug.LogError(message);
            }
            if (type == "log")
            {
                Debug.Log(message);
            }
            if (type == "warning")
            {
                Debug.LogWarning(message);
            }
        }
    }
}

}

