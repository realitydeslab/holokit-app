using UnityEngine;
using UnityEngine.VFX;

public class ScreenEffectController : MonoBehaviour
{
    private VisualEffect vfx;

    private void Start()
    {
        vfx = GetComponent<VisualEffect>();
    }
    
    public void OnHit()
    {

    }
}
