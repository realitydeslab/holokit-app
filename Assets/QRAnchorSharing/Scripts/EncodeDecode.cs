using System;
using System.Collections.Generic;
using UnityEngine;

namespace QRFoundation {

    public class EncodeDecode 
    {
        public static void EncodePose(Pose pose, float width, int id, string meta, out string serialized)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(pose.position.x));
            bytes.AddRange(BitConverter.GetBytes(pose.position.y));
            bytes.AddRange(BitConverter.GetBytes(pose.position.z));
            bytes.AddRange(BitConverter.GetBytes(pose.rotation.x));
            bytes.AddRange(BitConverter.GetBytes(pose.rotation.y));
            bytes.AddRange(BitConverter.GetBytes(pose.rotation.z));
            bytes.AddRange(BitConverter.GetBytes(pose.rotation.w));
            bytes.AddRange(BitConverter.GetBytes(width));
            bytes.AddRange(BitConverter.GetBytes(id));

            serialized = Convert.ToBase64String(bytes.ToArray());
            serialized += meta;
        }

        public static void DecodePose(string serialized, out Pose pose, out float width, out int id, out string rest)
        {
            int codeTotalBytes = 4 * 9;
            int characters = Mathf.CeilToInt(codeTotalBytes / 3f) * 4;

            byte[] decodedBytes = Convert.FromBase64String(serialized.Substring(0, characters));
            int i = 0;
            pose = new Pose();
            pose.position.x = BitConverter.ToSingle(decodedBytes, (i++) * 4);
            pose.position.y = BitConverter.ToSingle(decodedBytes, (i++) * 4);
            pose.position.z = BitConverter.ToSingle(decodedBytes, (i++) * 4);
            pose.rotation.x = BitConverter.ToSingle(decodedBytes, (i++) * 4);
            pose.rotation.y = BitConverter.ToSingle(decodedBytes, (i++) * 4);
            pose.rotation.z = BitConverter.ToSingle(decodedBytes, (i++) * 4);
            pose.rotation.w = BitConverter.ToSingle(decodedBytes, (i++) * 4);
            width = BitConverter.ToSingle(decodedBytes, (i++) * 4);
            id = BitConverter.ToInt32(decodedBytes, (i++) * 4);
            if (characters >= serialized.Length)
            {
                rest = "";
            }
            else
            {
                rest = serialized.Substring(characters);
            }
        }
    }
}
