/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/14 10:30:51
 */

using Google.Protobuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace XMLib
{
    /// <summary>
    /// DataUtility
    /// </summary>
    public static class DataUtility
    {
        #region json

        public static JsonSerializerSettings jsonSetting;

        static DataUtility()
        {
            jsonSetting = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
            };
        }

        public static string ToJson<T>(T obj)
        {
            string json = JsonConvert.SerializeObject(obj, jsonSetting);
            return json;
        }

        public static T FromJson<T>(string json)
        {
            T obj = JsonConvert.DeserializeObject<T>(json, jsonSetting);
            return obj;
        }

        public static object FromJson(string json)
        {
            object obj = JsonConvert.DeserializeObject(json, jsonSetting);
            return obj;
        }

        public static byte[] ToJsonBytes<T>(T obj)
        {
            string json = ToJson(obj);
            return Encoding.UTF8.GetBytes(json);
        }

        public static T FromJsonBytes<T>(byte[] jsonBytes)
        {
            string json = Encoding.UTF8.GetString(jsonBytes);
            return FromJson<T>(json);
        }

        public static string ToJson(object obj, Type type)
        {
            string json = JsonConvert.SerializeObject(obj, type, jsonSetting);
            return json;
        }

        public static object FromJson(string json, Type type)
        {
            object obj = JsonConvert.DeserializeObject(json, type, jsonSetting);
            return obj;
        }

        #endregion json

        #region proto

        public static T FromProto<T>(byte[] buf) where T : IMessage<T>, new()
        {
            T obj = new T();
            obj.MergeFrom(buf);
            return obj;
        }

        public static byte[] ToProto<T>(T obj) where T : IMessage<T>
        {
            byte[] outBuf = obj.ToByteArray();
            return outBuf;
        }

        public static string ProtoToJson<T>(byte[] buf) where T : IMessage<T>, new()
        {
            T obj = FromProto<T>(buf);
            JsonFormatter formatter = JsonFormatter.Default;
            return formatter.Format(obj);
        }

        #endregion proto

        #region zip

        public static byte[] Zip(byte[] buf)
        {
            byte[] outBuf = null;
            using (MemoryStream outStream = new MemoryStream())
            {
                using (MemoryStream inStream = new MemoryStream(buf))
                {
                    SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();

                    //写入长度
                    outStream.Write(BitConverter.GetBytes(inStream.Length), 0, 8);

                    //写入属性
                    coder.WriteCoderProperties(outStream);

                    //压缩
                    coder.Code(inStream, outStream, inStream.Length, -1, null);

                    outStream.Flush();
                    outBuf = outStream.ToArray();
                }
            }

            return outBuf;
        }

        public static byte[] Unzip(byte[] buf)
        {
            byte[] outBuf = null;
            using (MemoryStream outStream = new MemoryStream())
            {
                using (MemoryStream inStream = new MemoryStream(buf))
                {
                    SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();

                    //读取长度
                    byte[] fileLengthBytes = new byte[8];
                    inStream.Read(fileLengthBytes, 0, 8);
                    long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

                    //读取属性
                    byte[] properties = new byte[5];
                    inStream.Read(properties, 0, 5);

                    //设置属性
                    coder.SetDecoderProperties(properties);

                    //解压
                    coder.Code(inStream, outStream, inStream.Length, fileLength, null);

                    outStream.Flush();
                    outBuf = outStream.ToArray();
                }
            }

            return outBuf;
        }

        public static byte[] StringZip(string str)
        {
            byte[] buf = Encoding.UTF8.GetBytes(str);
            byte[] outBuf = Zip(buf);
            return outBuf;
        }

        public static string StringUnzip(byte[] buf)
        {
            byte[] outBuf = Unzip(buf);
            string outStr = Encoding.UTF8.GetString(outBuf);
            return outStr;
        }

        #endregion zip
    }
}