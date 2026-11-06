using UnityEngine;
using UnityEngine.EventSystems;

// Lightweight, sound-only UI feedback. Added automatically to buttons by
// UISoundInstaller at runtime, so it carries no clips itself — it just asks
// its owning installer to play the hover / click sound. Pointer events still
// reach any other handlers on the same object (e.g. an upgrade card's hover-grow),
// so this never fights existing animation.
public class UISound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    UISoundInstaller owner;
    public void Bind(UISoundInstaller o) => owner = o;

    public void OnPointerEnter(PointerEventData e) { if (owner != null) owner.PlayHover(); }
    public void OnPointerClick(PointerEventData e) { if (owner != null) owner.PlayClick(); }
}
