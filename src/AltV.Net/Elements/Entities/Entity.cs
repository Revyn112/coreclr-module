using System;
using System.Collections.Concurrent;
using AltV.Net.Data;
using AltV.Net.Native;

namespace AltV.Net.Elements.Entities
{
    public abstract class Entity : IInternalEntity, IEntity
    {
        public static ushort GetId(IntPtr entityPointer) => AltVNative.Entity.Entity_GetID(entityPointer);
        public static EntityType GetType(IntPtr entityPointer) => AltVNative.Entity.BaseObject_GetType(entityPointer);

        private readonly ConcurrentDictionary<string, object> data = new ConcurrentDictionary<string, object>();

        //private Position position = Position.Zero;

        //private Rotation rotation = Rotation.Zero;

        public IntPtr NativePointer { get; }
        public bool Exists { get; set; }

        public ushort Id { get; }
        public EntityType Type { get; }

        protected Entity(IntPtr nativePointer, EntityType type, ushort id)
        {
            NativePointer = nativePointer;
            Id = id;
            Type = type;
            Exists = true;
        }

        public void SetData(string key, object value)
        {
            data[key] = value;
        }

        public bool GetData<T>(string key, out T result)
        {
            if (!data.TryGetValue(key, out var value))
            {
                result = default;
                return false;
            }

            if (!(value is T cast))
            {
                result = default;
                return false;
            }

            result = cast;
            return true;
        }

        //"Has" needs to do the same calculations Get has to do so consider using Get always
        public bool Has(string key)
        {
            return data.ContainsKey(key);
        }

        protected void CheckExistence()
        {
            if (Exists)
            {
                return;
            }

            throw new EntityDeletedException(this);
        }

        /*public Position PositionRef for later performance optimization
        {
            get
            {
                AltVNative.Entity.Entity_GetPositionRef(NativePointer, ref position);
                return position;
            }
            set
            {
                position = value;
                AltVNative.Entity.Entity_SetPositionRef(NativePointer, ref value);
            }
        }*/

        /*public Rotation RotationRef for later performance optimization
        {
            get
            {
                AltVNative.Entity.Entity_GetRotationRef(NativePointer, ref rotation);
                return rotation;
            }
            set
            {
                rotation = value;
                AltVNative.Entity.Entity_SetRotationRef(NativePointer, ref value);
            }
        }*/

        public Position Position
        {
            get => !Exists ? Position.Zero : AltVNative.Entity.Entity_GetPosition(NativePointer);
            set
            {
                if (Exists)
                {
                    AltVNative.Entity.Entity_SetPosition(NativePointer, value);
                }
            }
        }

        public Rotation Rotation
        {
            get => !Exists ? Rotation.Zero : AltVNative.Entity.Entity_GetRotation(NativePointer);
            set
            {
                if (Exists)
                {
                    AltVNative.Entity.Entity_SetRotation(NativePointer, value);
                }
            }
        }

        public ushort Dimension
        {
            get => !Exists ? default : AltVNative.Entity.Entity_GetDimension(NativePointer);
            set
            {
                if (Exists)
                {
                    AltVNative.Entity.Entity_SetDimension(NativePointer, value);
                }
            }
        }

        public void SetPosition(float x, float y, float z)
        {
            if (Exists)
            {
                AltVNative.Entity.Entity_SetPositionXYZ(NativePointer, x, y, z);
            }
        }

        public void SetRotation(float roll, float pitch, float yaw)
        {
            if (Exists)
            {
                AltVNative.Entity.Entity_SetRotationRPY(NativePointer, roll, pitch, yaw);
            }
        }

        public void SetMetaData(string key, object value)
        {
            if (Exists)
            {
                var mValue = MValue.CreateFromObject(value) ?? MValue.Nil;
                AltVNative.Entity.Entity_SetMetaData(NativePointer, key, ref mValue);
            }
        }

        public bool GetMetaData<T>(string key, out T result)
        {
            if (!Exists)
            {
                result = default;
                return false;
            }
            var mValue = MValue.Nil;
            AltVNative.Entity.Entity_GetMetaData(NativePointer, key, ref mValue);
            if (!(mValue.ToObject() is T cast))
            {
                result = default;
                return false;
            }

            result = cast;
            return true;
        }

        public void SetSyncedMetaData(string key, object value)
        {
            if (Exists)
            {
                var mValue = MValue.CreateFromObject(value) ?? MValue.Nil;
                AltVNative.Entity.Entity_SetSyncedMetaData(NativePointer, key, ref mValue);
            }
        }

        public bool GetSyncedMetaData<T>(string key, out T result)
        {
            if (!Exists)
            {
                result = default;
                return false;
            }
            var mValue = MValue.Nil;
            AltVNative.Entity.Entity_GetSyncedMetaData(NativePointer, key, ref mValue);
            if (!(mValue.ToObject() is T cast))
            {
                result = default;
                return false;
            }

            result = cast;
            return true;
        }

        public void Remove()
        {
            Alt.RemoveEntity(this);
        }
    }
}