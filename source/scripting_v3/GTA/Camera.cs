using System;
using GTA.Math;
using GTA.Native;

namespace GTA
{
	public sealed class Camera : PoolObject, IEquatable<Camera>, ISpatial
	{
		#region Fields
		internal static readonly string[] _shakeNames = {
			"HAND_SHAKE",
			"SMALL_EXPLOSION_SHAKE",
			"MEDIUM_EXPLOSION_SHAKE",
			"LARGE_EXPLOSION_SHAKE",
			"JOLT_SHAKE",
			"VIBRATE_SHAKE",
			"ROAD_VIBRATION_SHAKE",
			"DRUNK_SHAKE",
			"SKY_DIVING_SHAKE",
			"FAMILY5_DRUG_TRIP_SHAKE",
			"DEATH_FAIL_IN_EFFECT_SHAKE"
		};
		#endregion

		public Camera(int handle) : base(handle)
		{
		}

		/// <summary>
		/// Gets the memory address of this <see cref="Camera"/>.
		/// </summary>
		public IntPtr MemoryAddress => SHVDN.NativeMemory.GetCameraAddress(Handle);

		private IntPtr MatrixAddress
		{
			get
			{
				IntPtr address = MemoryAddress;
				if(address == IntPtr.Zero)
				{
					return IntPtr.Zero;
				}
				return (SHVDN.NativeMemory.ReadByte(address + 0x220) & 1) == 0 ? address + 0x30 : address + 0x110;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Camera"/> is currently being rendered.
		/// </summary>
		/// <value>
		///   <c>true</c> if this <see cref="Camera"/> is active; otherwise, <c>false</c>.
		/// </value>
		public bool IsActive
		{
			get => Function.Call<bool>(Hash.IS_CAM_ACTIVE, Handle);
			set => Function.Call(Hash.SET_CAM_ACTIVE, Handle, value);
		}

		/// <summary>
		/// Gets the matrix of this <see cref="Camera"/>.
		/// </summary>
		public Matrix Matrix
		{
			get
			{
				IntPtr address = MatrixAddress;
				return address == IntPtr.Zero ? new Matrix() : new Matrix(SHVDN.NativeMemory.ReadMatrix(address));
			}
		}

		/// <summary>
		/// Gets or sets the position of this <see cref="Camera"/>.
		/// </summary>
		public Vector3 Position
		{
			get => Function.Call<Vector3>(Hash.GET_CAM_COORD, Handle);
			set => Function.Call(Hash.SET_CAM_COORD, Handle, value.X, value.Y, value.Z);
		}

		/// <summary>
		/// Gets or sets the rotation of this <see cref="Camera"/>.
		/// </summary>
		/// <value>
		/// The yaw, pitch and roll rotations measured in degrees.
		/// </value>
		public Vector3 Rotation
		{
			get => Function.Call<Vector3>(Hash.GET_CAM_ROT, Handle, 2);
			set => Function.Call(Hash.SET_CAM_ROT, Handle, value.X, value.Y, value.Z, 2);
		}

		/// <summary>
		/// Gets or sets the direction this <see cref="Camera"/> is pointing in.
		/// </summary>
		public Vector3 Direction
		{
			get => ForwardVector;
			set
			{
				value.Normalize();
				Vector3 vector1 = new Vector3(value.X, value.Y, 0f);
				Vector3 vector2 = new Vector3(value.Z, vector1.Length(), 0f);
				Vector3 vector3 = Vector3.Normalize(vector2);
				Rotation = new Vector3((float)(System.Math.Atan2(vector3.X, vector3.Y) * 57.295779513082323), Rotation.Y, (float)(System.Math.Atan2(value.X, value.Y) * -57.295779513082323));
			}
		}

		/// <summary>
		/// Gets the up vector of this <see cref="Camera"/>.
		/// </summary>
		public Vector3 UpVector
		{
			get
			{
				IntPtr address = MatrixAddress;
				if (address == IntPtr.Zero)
				{
					return Vector3.RelativeTop;
				}
				return new Vector3(SHVDN.NativeMemory.ReadVector3(address + 0x20));
			}
		}

		/// <summary>
		/// Gets the right vector of this <see cref="Camera"/>.
		/// </summary>
		public Vector3 RightVector
		{
			get
			{
				IntPtr address = MatrixAddress;
				if(address == IntPtr.Zero)
				{
					return Vector3.RelativeRight;
				}
				return new Vector3(SHVDN.NativeMemory.ReadVector3(address));
			}
		}

		/// <summary>
		/// Gets the forward vector of this <see cref="Camera"/>, see also <seealso cref="Direction"/>.
		/// </summary>
		public Vector3 ForwardVector
		{
			get
			{
				IntPtr address = MatrixAddress;
				if(address == IntPtr.Zero)
				{
					return Vector3.RelativeFront;
				}
				return new Vector3(SHVDN.NativeMemory.ReadVector3(address + 0x10));
			}
		}

		/// <summary>
		/// Gets the position in world coords of an offset relative to this <see cref="Camera"/>
		/// </summary>
		/// <param name="offset">The offset from this <see cref="Camera"/>.</param>
		public Vector3 GetOffsetPosition(Vector3 offset)
		{
			return Matrix.TransformPoint(offset);
		}

		/// <summary>
		/// Gets the relative offset of this <see cref="Camera"/> from a world coords position
		/// </summary>
		/// <param name="worldCoords">The world coords.</param>
		public Vector3 GetPositionOffset(Vector3 worldCoords)
		{
			return Matrix.InverseTransformPoint(worldCoords);
		}

		/// <summary>
		/// Gets or sets the far clip of this <see cref="Camera"/>.
		/// </summary>
		public float FarClip
		{
			get => Function.Call<float>(Hash.GET_CAM_FAR_CLIP, Handle);
			set => Function.Call(Hash.SET_CAM_FAR_CLIP, Handle, value);
		}

		/// <summary>
		/// Gets or sets the near clip of this <see cref="Camera"/>.
		/// </summary>
		public float NearClip
		{
			get => Function.Call<float>(Hash.GET_CAM_NEAR_CLIP, Handle);
			set => Function.Call(Hash.SET_CAM_NEAR_CLIP, Handle, value);
		}

		/// <summary>
		/// Gets or sets the field of view of this <see cref="Camera"/>.
		/// </summary>
		public float FieldOfView
		{
			get => Function.Call<float>(Hash.GET_CAM_FOV, Handle);
			set => Function.Call(Hash.SET_CAM_FOV, Handle, value);
		}

		/// <summary>
		/// Gets or sets the far depth of field of this <see cref="Camera"/>.
		/// </summary>
		public float FarDepthOfField
		{
			get => Function.Call<float>(Hash.GET_CAM_FAR_DOF, Handle);
			set => Function.Call(Hash.SET_CAM_FAR_DOF, Handle, value);
		}

		/// <summary>
		/// Sets the near depth of field for this <see cref="Camera"/>.
		/// </summary>
		public float NearDepthOfField
		{
			set => Function.Call(Hash.SET_CAM_NEAR_DOF, Handle, value);
		}
		
		/// <summary>
		/// Sets the strenght of the motion blur for this <see cref="Camera"/>
		/// </summary>
		public float MotionBlurStrength
		{
			set => Function.Call(Hash.SET_CAM_MOTION_BLUR_STRENGTH, Handle, value);
		}

		/// <summary>
		/// Sets the depth of field strength for this <see cref="Camera"/>.
		/// </summary>
		public float DepthOfFieldStrength
		{
			set => Function.Call(Hash.SET_CAM_DOF_STRENGTH, Handle, value);
		}

		/// <summary>
		/// Shakes this <see cref="Camera"/>.
		/// </summary>
		/// <param name="shakeType">Type of the shake to apply.</param>
		/// <param name="amplitude">The amplitude of the shaking.</param>
		public void Shake(CameraShake shakeType, float amplitude)
		{
			Function.Call(Hash.SHAKE_CAM, Handle, _shakeNames[(int)shakeType], amplitude);
		}
		/// <summary>
		/// Stops shaking this <see cref="Camera"/>.
		/// </summary>
		public void StopShaking()
		{
			Function.Call(Hash.STOP_CAM_SHAKING, Handle, true);
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Camera"/> is shaking.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Camera"/> is shaking; otherwise, <c>false</c>.
		/// </value>
		public bool IsShaking => Function.Call<bool>(Hash.IS_CAM_SHAKING, Handle);

		/// <summary>
		/// Sets the shake amplitude for this <see cref="Camera"/>.
		/// </summary>
		public float ShakeAmplitude
		{
			set => Function.Call(Hash.SET_CAM_SHAKE_AMPLITUDE, Handle, value);
		}

		/// <summary>
		/// Points this <see cref="Camera"/> at a specified <see cref="Entity"/>.
		/// </summary>
		/// <param name="target">The <see cref="Entity"/> to point at.</param>
		/// <param name="offset">The offset from the <paramref name="target"/> to point at.</param>
		public void PointAt(Entity target, Vector3 offset = default(Vector3))
		{
			Function.Call(Hash.POINT_CAM_AT_ENTITY, Handle, target.Handle, offset.X, offset.Y, offset.Z, true);
		}
		/// <summary>
		/// Points this <see cref="Camera"/> at a specified <see cref="PedBone"/>.
		/// </summary>
		/// <param name="target">The <see cref="PedBone"/> to point at.</param>
		/// <param name="offset">The offset from the <paramref name="target"/> to point at</param>
		public void PointAt(PedBone target, Vector3 offset = default(Vector3))
		{
			Function.Call(Hash.POINT_CAM_AT_PED_BONE, Handle, target.Owner.Handle, target, offset.X, offset.Y, offset.Z, true);
		}
		/// <summary>
		/// Points this <see cref="Camera"/> at a specified position.
		/// </summary>
		/// <param name="target">The position to point at.</param>
		public void PointAt(Vector3 target)
		{
			Function.Call(Hash.POINT_CAM_AT_COORD, Handle, target.X, target.Y, target.Z);
		}

		/// <summary>
		/// Stops this <see cref="Camera"/> pointing at a specific target.
		/// </summary>
		public void StopPointing()
		{
			Function.Call(Hash.STOP_CAM_POINTING, Handle);
		}

		/// <summary>
		/// Moves the to camera from this <see cref="Camera"/> position to the to cameras Poisition.
		/// </summary>
		public void InterpTo(Camera to, int duration, int easePosition, int easeRotation)
		{
			Function.Call(Hash.SET_CAM_ACTIVE_WITH_INTERP, to.Handle, Handle, duration, easePosition, easeRotation);
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Camera"/> is interpolating.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Camera"/> is interpolating; otherwise, <c>false</c>.
		/// </value>
		public bool IsInterpolating => Function.Call<bool>(Hash.IS_CAM_INTERPOLATING, Handle);

		/// <summary>
		/// Attaches this <see cref="Camera"/> to a specific <see cref="Entity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="Entity"/> to attach to.</param>
		/// <param name="offset">The offset from the <paramref name="entity"/> to attach to.</param>
		public void AttachTo(Entity entity, Vector3 offset)
		{
			Function.Call(Hash.ATTACH_CAM_TO_ENTITY, Handle, entity.Handle, offset.X, offset.Y, offset.Z, true);
		}
		/// <summary>
		/// Attaches this <see cref="Camera"/> to a specific <see cref="PedBone"/>.
		/// </summary>
		/// <param name="pedBone">The <see cref="PedBone"/> to attach to.</param>
		/// <param name="offset">The offset from the <paramref name="pedBone"/> to attach to.</param>
		public void AttachTo(PedBone pedBone, Vector3 offset)
		{
			Function.Call(Hash.ATTACH_CAM_TO_PED_BONE, Handle, pedBone.Owner.Handle, pedBone, offset.X, offset.Y, offset.Z, true);
		}

		/// <summary>
		/// Detaches this <see cref="Camera"/> from any <see cref="Entity"/> or <see cref="PedBone"/> it may be attached to.
		/// </summary>
		public void Detach()
		{
			Function.Call(Hash.DETACH_CAM, Handle);
		}

		/// <summary>
		/// Destroys this <see cref="Camera"/>.
		/// </summary>
		public override void Delete()
		{
			Function.Call(Hash.DESTROY_CAM, Handle, 0);
		}

		public override bool Exists()
		{
			return Function.Call<bool>(Hash.DOES_CAM_EXIST, Handle);
		}

		public bool Equals(Camera camera)
		{
			return !(camera is null) && Handle == camera.Handle;
		}
		public override bool Equals(object camera)
		{
			return !(camera is null) && camera.GetType() == GetType() && Equals((Camera)camera);
		}

		public sealed override int GetHashCode()
		{
			return Handle;
		}

		public static bool operator ==(Camera left, Camera right)
		{
			return left is null ? right is null : left.Equals(right);
		}
		public static bool operator !=(Camera left, Camera right)
		{
			return !(left == right);
		}
	}
}