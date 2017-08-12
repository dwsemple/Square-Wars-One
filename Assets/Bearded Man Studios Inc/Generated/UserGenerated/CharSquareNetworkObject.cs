using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0,0]")]
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
		private int _playerId;
		public event FieldEvent<int> playerIdChanged;
		public InterpolateUnknown playerIdInterpolation = new InterpolateUnknown() { LerpT = 0f, Enabled = false };
		public int playerId
		{
			get { return _playerId; }
			set
			{
				// Don't do anything if the value is the same
				if (_playerId == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_playerId = value;
				hasDirtyFields = true;
			}
		}

		public void SetplayerIdDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_playerId(ulong timestep)
		{
			if (playerIdChanged != null) playerIdChanged(_playerId, timestep);
			if (fieldAltered != null) fieldAltered("playerId", _playerId, timestep);
		}

		protected override void OwnershipChanged()
		{
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			positionInterpolation.current = positionInterpolation.target;
			playerIdInterpolation.current = playerIdInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _position);
			UnityObjectMapper.Instance.MapBytes(data, _playerId);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_position = UnityObjectMapper.Instance.Map<Vector2>(payload);
			positionInterpolation.current = _position;
			positionInterpolation.target = _position;
			RunChange_position(timestep);
			_playerId = UnityObjectMapper.Instance.Map<int>(payload);
			playerIdInterpolation.current = _playerId;
			playerIdInterpolation.target = _playerId;
			RunChange_playerId(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _position);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _playerId);

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
				if (playerIdInterpolation.Enabled)
				{
					playerIdInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					playerIdInterpolation.Timestep = timestep;
				}
				else
				{
					_playerId = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_playerId(timestep);
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
			if (playerIdInterpolation.Enabled && !playerIdInterpolation.current.Near(playerIdInterpolation.target, 0.0015f))
			{
				_playerId = (int)playerIdInterpolation.Interpolate();
				RunChange_playerId(playerIdInterpolation.Timestep);
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