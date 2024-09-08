#if PVR_CCK_WORLDS
using PVR.PSharp;

using UnityEngine.UI;


namespace AudioLink
{

    public class GlobalSlider : PSharpBehaviour
    {
        [PSharpSynced(SyncType.Manual)]
        private float syncedValue;
        private bool deserializing;
        private Slider slider;
        private PSharpPlayer localPlayer;

        private void Start()
        {
            slider = transform.GetComponent<Slider>();
            localPlayer = PSharpPlayer.LocalPlayer;
            syncedValue = slider.value;
            deserializing = false;

            if (PSharpNetworking.IsOwner(localPlayer, gameObject))
                Sync("syncedValue");
        }

        public override void OnDeserialization()
        {
            if (!enabled) return;

            deserializing = true;
            slider.value = syncedValue;
            deserializing = false;
        }

        public void SlideUpdate()
        {
            if (!enabled) return;
            
            if (slider == null)
                return;
            if (deserializing)
                return;
            if (!PSharpNetworking.IsOwner(localPlayer, gameObject))
                PSharpNetworking.SetOwner(localPlayer, gameObject);

            syncedValue = slider.value;
            Sync("syncedValue");
        }
    }
}
#endif
