#if PVR_CCK_WORLDS
using PVR.PSharp;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace AudioLink
{
    [RequireComponent(typeof(Collider))]
#if PVR_CCK_WORLDS
    public class AudioLinkZone : PSharpBehaviour
#else
    public class AudioLinkZone : MonoBehaviour
#endif
    {
        [Header("Targets")]
        public AudioLink audioLink;
        [FormerlySerializedAs("audioSource")]
        public AudioSource targetAudioSource;

        [Header("Settings")]
        public bool disableSource = true;
        public bool enableTarget = true;

#if PVR_CCK_WORLDS
        public override void OnPlayerTriggerEnter(PSharpPlayer player)
        {
            if (player.IsNull || !player.IsLocal) return;
#else
        private void OnTriggerEnter(Collider player)
        {
            if (!player.gameObject.CompareTag("Player")) return;
#endif
            if (disableSource && audioLink.audioSource != null) audioLink.audioSource.gameObject.SetActive(false);

            audioLink.audioSource = targetAudioSource;

            if (enableTarget && audioLink.audioSource != null) audioLink.audioSource.gameObject.SetActive(true);
        }
    }
}
