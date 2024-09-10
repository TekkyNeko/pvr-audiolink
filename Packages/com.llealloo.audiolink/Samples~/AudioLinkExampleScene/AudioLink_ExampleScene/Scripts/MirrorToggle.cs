
using UnityEngine;

#if PVR_CCK_WORLDS
using PVR.PSharp;

namespace AudioLink
{
    public class MirrorToggle : PSharpBehaviour
    {

        public GameObject mirror;

        void Start()
        {
            
        }

        public override void OnInteract()
        {
            bool toggle = !mirror.activeSelf;
            mirror.SetActive(toggle);
            SetInteractText("Mirror is " + (string)((toggle == true) ? "ON" : "OFF") + " (local)");
        }

    }
}

#else
namespace AudioLink
{
    public class MirrorToggle : MonoBehaviour { }
}
#endif