#if PVR_CCK_WORLDS
using PVR.PSharp;
using UnityEngine.UI;


namespace AudioLink
{
    public class GlobalToggle : PSharpBehaviour
    {
        [PSharpSynced(SyncType.Manual)]
        private bool syncedValue;
        private bool deserializing;
        private Toggle toggle;
        private PSharpPlayer localPlayer;

        private void Start()
        {
            toggle = transform.GetComponent<Toggle>();
            localPlayer = PSharpPlayer.LocalPlayer;
            syncedValue = toggle.isOn;
            deserializing = false;

            if (IsOwner)
                Sync("syncedValue");
        }

        public override void OnDeserialization()
        {
            if (!enabled) return;

            deserializing = true;
            toggle.isOn = syncedValue;
            deserializing = false;
        }

        public void ToggleUpdate()
        {
            if (!enabled) return;
            
            if (toggle == null)
                return;
            if (deserializing)
                return;
            if (!IsOwner)
                PSharpNetworking.SetOwner(localPlayer, gameObject);

            syncedValue = toggle.isOn;
            Sync("syncedValue");
        }
    }
}
#endif
