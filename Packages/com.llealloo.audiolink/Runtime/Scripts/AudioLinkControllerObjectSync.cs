using UnityEngine;
using PVR.PSharp;
using PVR.CCK.Worlds.Components;

namespace AudioLink
{
	public class AudioLinkControllerObjectSync : PSharpBehaviour
	{

		[SerializeField] private PVR_Pickup _pickup;
		[SerializeField] private PVR_Pickup _pickup2;
		[PSharpSynced] private Vector3 _position;
		[PSharpSynced] private Quaternion _rotation;

		private void Awake()
		{

			_position = transform.position;
			_rotation = transform.rotation;
		}

		public override void OnNetworkReady()
		{
			if (!IsObjectOwner)
			{
				transform.position = _position;
				transform.rotation = _rotation;
			}
		}

		private void FixedUpdate()
		{
			if (IsObjectOwner)
			{
				_position = transform.position;
				_rotation = transform.rotation;
				return;
			}

			transform.position = Vector3.Lerp(transform.position, _position, Time.deltaTime * 10);
			transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, Time.deltaTime * 10);
		}

		public override void OnOwnershipTransfer(PSharpPlayer newOwner)
		{
			if (newOwner.PlayerID != PSharpPlayer.LocalPlayer.PlayerID)
			{
				_pickup.Drop();
				_pickup2.Drop();
			}
		}
	}
}