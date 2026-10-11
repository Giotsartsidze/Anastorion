using UnityEngine;

/// <summary>
/// Tells the persistent MusicManager which track to play when this scene loads.
/// Put ONE on a plain (non-persistent) GameObject in each scene — NOT on the
/// MusicManager itself (that object persists and its duplicate self-destroys).
///   MainMenu  → Track = Menu
///   SampleScene → Track = Gameplay
/// </summary>
public class SceneMusic : MonoBehaviour
{
    public enum Track { Menu, Gameplay }
    public Track track = Track.Gameplay;

    void Start()
    {
        if (MusicManager.Instance == null) return;
        if (track == Track.Menu) MusicManager.Instance.PlayMenu();
        else MusicManager.Instance.PlayDefault();
    }
}
