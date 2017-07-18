using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0]")]
	public partial class GlobalSettingsNetworkObject : NetworkObject
	{
		public const int IDENTITY = 5;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		private int _numberofplayers;
		public event FieldEvent<int> numberofplayersChanged;
		public InterpolateUnknown numberofplayersInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public int numberofplayers
		{
			get { return _numberofplayers; }
			set
			{
				// Don't do anything if the value is the same
				if (_numberofplayers == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_numberofplayers = value;
				hasDirtyFields = true;
			}
		}

		public void SetnumberofplayersDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_numberofplayers(ulong timestep)
		{
			if (numberofplayersChanged != null) numberofplayersChanged(_numberofplayers, timestep);
			if (fieldAltered != null) fieldAltered("numberofplayers", _numberofplayers, timestep);
		}

		protected override void OwnershipChanged()
		{
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			numberofplayersInterpolation.current = numberofplayersInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _numberofplayers);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_numberofplayers = UnityObjectMapper.Instance.Map<int>(payload);
			numberofplayersInterpolation.current = _numberofplayers;
			numberofplayersInterpolation.target = _numberofplayers;
			RunChange_numberofplayers(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _numberofplayers);

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (numberofplayersInterpolation.Enabled)
				{
					numberofplayersInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					numberofplayersInterpolation.Timestep = timestep;
				}
				else
				{
					_numberofplayers = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_numberofplayers(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (numberofplayersInterpolation.Enabled && !numberofplayersInterpolation.current.Near(numberofplayersInterpolation.target, 0.0015f))
			{
				_numberofplayers = (int)numberofplayersInterpolation.Interpolate();
				RunChange_numberofplayers(numberofplayersInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public GlobalSettingsNetworkObject() : base() { Initialize(); }
		public GlobalSettingsNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public GlobalSettingsNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}