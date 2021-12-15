using ProtoBuf;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace RedRedJiang.Unity
{
    /// <summary>
    /// 二进制 序列化 和 反序列化
    /// </summary>
    public class BinarySystem : Singleton<BinarySystem>, IBinary
    {
        private const string mBytesName = ".bytes";

        #region 正常的序列化 和反序列化
        public T GetBinaryObject<T>(string _Path) where T : class
        {
            if (!File.Exists(_Path + mBytesName))
            {
                UnityEngine.Debug.Log(_Path + mBytesName);
                return null;
            }
            T t = default(T);

            FileStream fs = new FileStream(_Path + mBytesName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            BinaryFormatter bf = new BinaryFormatter();
            t = (T)bf.Deserialize(fs);
            fs.Dispose();
            return t;
        }

        public void SaveBinaryObject<T>(string _Path, T _class) where T : class
        {
            if (string.IsNullOrEmpty(_Path) || _class == null)
            {
                return;
            }

            FileStream fs = new FileStream(_Path + mBytesName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(fs, _class);

            fs.Dispose();
        }

        #endregion

        #region Proto的序列化和反序列化

        public void Serialize<T>(string _Path, T _Class)
        {
            if (string.IsNullOrEmpty(_Path) || null == _Class)
            {
                return;
            }

            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, _Class);
                bytes = new byte[ms.Position];
                var fullBytes = ms.GetBuffer();
                Array.Copy(fullBytes, bytes, bytes.Length);
            }

            FileStream fs = new FileStream(_Path + mBytesName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs.Write(bytes, 0, bytes.Length);
            fs.Dispose();
        }

        public T Deserialize<T>(string _path) where T : class
        {
            if (null == _path)
            {
                return null;
            }
            
            using (FileStream fs = File.OpenRead(_path + mBytesName))
            {
                T result = ProtoBuf.Serializer.Deserialize<T>(fs);
                return result;
            }
        }

        public void SerializeOfArchive<T>(string key, T _Class)
        {
            if (string.IsNullOrEmpty(key) || null == _Class)
            {
                return;
            }

            ArchivesObject ao = new ArchivesObject(key);

            ArchivesSystem.Instance.AddArchives(key, ao);

            Serialize<T>(ArchivesSystem.Instance.DicArchivesOfNameMap[key].ToString(), _Class);
        }

        public T DeserializeOfArchive<T>(string _pathKey) where T : class
        {
            string path = ArchivesSystem.Instance.GetArchivesPath(_pathKey);

            if(string.IsNullOrEmpty(path))
            {
                return null;
            }

            return Deserialize<T>(path);
        }

        public T DeseriallizsOfArchiveConstData<T>(string _NameKey)
        {
            if(string.IsNullOrEmpty(_NameKey))
            {
                return default(T);
            }

            if (!ResourcesSystem.Instance.IsOnce) { ResourcesSystem.Instance.Init(); }

            UnityEngine.Object obj = ResourcesSystem.Instance.ResObj<UnityEngine.Object>(_NameKey, false);

            if (!obj) { return default(T); }

            TextAsset te = UnityEngine.Object.Instantiate(obj) as TextAsset;

            if(te == null) { return default(T); }

            Stream stream = BytesToStream(te.bytes);

            if(stream == null) { return default(T); }

            T tiy = Serializer.Deserialize<T>(stream);

            if(tiy == null) { return default(T); }

            return tiy;
        }
        #endregion

        #region Stream与Byte[]之间的转换

        private Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        private byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        #endregion
    }
}
