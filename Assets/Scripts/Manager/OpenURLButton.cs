using UnityEngine;

/// <summary>
/// Drop on any Button and wire its OnClick → OpenURLButton.Open().
/// Set the URL in the Inspector. Use for a "Wishlist on Steam" button on the
/// Victory / Game Over screens (and the main menu).
/// </summary>
public class OpenURLButton : MonoBehaviour
{
    [Tooltip("e.g. your Steam store page once it exists.")]
    public string url = "https://store.steampowered.com/";

    public void Open()
    {
        if (!string.IsNullOrEmpty(url)) Application.OpenURL(url);
    }
}
