using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0,0,0]")]
	public partial class CharSquareNetworkObject : NetworkObject
	{
		public const int IDENTITY = 1;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		private Vector2 _position;
		public event FieldEvent<Vector2> positionChanged;
		public InterpolateVector2 positionInterpolation = new InterpolateVector2() { LerpT = 0f, Enabled = false };
		public Vector2 position
		{
			get { return _position; }
			set
			{
				// Don't do anything if the value is the same
				if (_position == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_position = value;
				hasDirtyFields = true;
			}
		}

		public void SetpositionDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_position(ulong timestep)
		{
			if (positionChanged != null) positionChanged(_position, timestep);
			if (fieldAltered != null) fieldAltered("position", _position, timestep);
		}
		private int _localPlayerId;
		public event FieldEvent<int> localPlayerIdChanged;
		public InterpolateUnknown localPlayerIdInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public int localPlayerId
		{
			get { return _localPlayerId; }
			set
			{
				// Don't do anything if the value is the same
				if (_localPlayerId == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_localPlayerId = value;
				hasDirtyFields = true;
			}
		}

		public void SetlocalPlayerIdDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_localPlayerId(ulong timestep)
		{
			if (localPlayerIdChanged != null) localPlayerIdChanged(_localPlayerId, timestep);
			if (fieldAltered != null) fieldAltered("localPlayerId", _localPlayerId, timestep);
		}
		private int _networkPlayerId;
		public event FieldEvent<int> networkPlayerIdChanged;
		public InterpolateUnknown networkPlayerIdInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public int networkPlayerId
		{
			get { return _networkPlayerId; }
			set
			{
				// Don't do anything if the value is the same
				if (_networkPlayerId == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x4;
				_networkPlayerId = value;
				hasDirtyFields = true;
			}
		}

		public void SetnetworkPlayerIdDirty()
		{
			_dirtyFields[0] |= 0x4;
			hasDirtyFields = true;
		}

		private void RunChange_networkPlayerId(ulong timestep)
		{
			if (networkPlayerIdChanged != null) networkPlayerIdChanged(_networkPlayerId, timestep);
			if (fieldAltered != null) fieldAltered("networkPlayerId", _networkPlayerId, timestep);
		}

		protected override void OwnershipChanged()
		{
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			positionInterpolation.current = positionInterpolation.target;
			localPlayerIdInterpolation.current = localPlayerIdInterpolation.target;
			networkPlayerIdInterpolation.current = networkPlayerIdInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _position);
			UnityObjectMapper.Instance.MapBytes(data, _localPlayerId);
			UnityObjectMapper.Instance.MapBytes(data, _networkPlayerId);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_position = UnityObjectMapper.Instance.Map<Vector2>(payload);
			positionInterpolation.current = _position;
			positionInterpolation.target = _position;
			RunChange_position(timestep);
			_localPlayerId = UnityObjectMapper.Instance.Map<int>(payload);
			localPlayerIdInterpolation.current = _localPlayerId;
			localPlayerIdInterpolation.target = _localPlayerId;
			RunChange_localPlayerId(timestep);
			_networkPlayerId = UnityObjectMapper.Instance.Map<int>(payload);
			networkPlayerIdInterpolation.current = _networkPlayerId;
			networkPlayerIdInterpolation.target = _networkPlayerId;
			RunChange_networkPlayerId(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _position);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _localPlayerId);
			if ((0x4 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _networkPlayerId);

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
				if (positionInterpolation.Enabled)
				{
					positionInterpolation.target = UnityObjectMapper.Instance.Map<Vector2>(data);
					positionInterpolation.Timestep = timestep;
				}
				else
				{
					_position = UnityObjectMapper.Instance.Map<Vector2>(data);
					RunChange_position(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (localPlayerIdInterpolation.Enabled)
				{
					localPlayerIdInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					localPlayerIdInterpolation.Timestep = timestep;
				}
				else
				{
					_localPlayerId = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_localPlayerId(timestep);
				}
			}
			if ((0x4 & readDirtyFlags[0]) != 0)
			{
				if (networkPlayerIdInterpolation.Enabled)
				{
					networkPlayerIdInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					networkPlayerIdInterpolation.Timestep = timestep;
				}
				else
				{
					_networkPlayerId = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_networkPlayerId(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (positionInterpolation.Enabled && !positionInterpolation.current.Near(positionInterpolation.target, 0.0015f))
			{
				_position = (Vector2)positionInterpolation.Interpolate();
				RunChange_position(positionInterpolation.Timestep);
			}
			if (localPlayerIdInterpolation.Enabled && !localPlayerIdInterpolation.current.Near(localPlayerIdInterpolation.target, 0.0015f))
			{
				_localPlayerId = (int)localPlayerIdInterpolation.Interpolate();
				RunChange_localPlayerId(localPlayerIdInterpolation.Timestep);
			}
			if (networkPlayerIdInterpolation.Enabled && !networkPlayerIdInterpolation.current.Near(networkPlayerIdInterpolation.target, 0.0015f))
			{
				_networkPlayerId = (int)networkPlayerIdInterpolation.Interpolate();
				RunChange_networkPlayerId(networkPlayerIdInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public CharSquareNetworkObject() : base() { Initialize(); }
		public CharSquareNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public CharSquareNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}