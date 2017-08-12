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
		private int _numberOfPlayers;
		public event FieldEvent<int> numberOfPlayersChanged;
		public InterpolateUnknown numberOfPlayersInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public int numberOfPlayers
		{
			get { return _numberOfPlayers; }
			set
			{
				// Don't do anything if the value is the same
				if (_numberOfPlayers == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_numberOfPlayers = value;
				hasDirtyFields = true;
			}
		}

		public void SetnumberOfPlayersDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_numberOfPlayers(ulong timestep)
		{
			if (numberOfPlayersChanged != null) numberOfPlayersChanged(_numberOfPlayers, timestep);
			if (fieldAltered != null) fieldAltered("numberOfPlayers", _numberOfPlayers, timestep);
		}

		protected override void OwnershipChanged()
		{
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			numberOfPlayersInterpolation.current = numberOfPlayersInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _numberOfPlayers);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_numberOfPlayers = UnityObjectMapper.Instance.Map<int>(payload);
			numberOfPlayersInterpolation.current = _numberOfPlayers;
			numberOfPlayersInterpolation.target = _numberOfPlayers;
			RunChange_numberOfPlayers(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _numberOfPlayers);

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
				if (numberOfPlayersInterpolation.Enabled)
				{
					numberOfPlayersInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					numberOfPlayersInterpolation.Timestep = timestep;
				}
				else
				{
					_numberOfPlayers = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_numberOfPlayers(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (numberOfPlayersInterpolation.Enabled && !numberOfPlayersInterpolation.current.Near(numberOfPlayersInterpolation.target, 0.0015f))
			{
				_numberOfPlayers = (int)numberOfPlayersInterpolation.Interpolate();
				RunChange_numberOfPlayers(numberOfPlayersInterpolation.Timestep);
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