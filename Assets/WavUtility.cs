using System;
using System.IO;
using UnityEngine;

public static class WavUtility
{
    const int HEADER_SIZE = 44;

    public static byte[] FromAudioClip(AudioClip clip)
    {
        int sampleCount = clip.samples * clip.channels;
        int frequency = clip.frequency;
        int channels = clip.channels;
        float[] samples = new float[sampleCount];
        clip.GetData(samples, 0);
        return ConvertToWav(samples, channels, frequency);
    }


    public static AudioClip ToAudioClip(byte[] wavFileBytes, string clipName = "wav")
    {
        using (MemoryStream stream = new MemoryStream(wavFileBytes))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            // Skip header
            reader.BaseStream.Position = 22;
            ushort channels = reader.ReadUInt16();
            int sampleRate = reader.ReadInt32();

            reader.BaseStream.Position = 40;
            int dataSize = reader.ReadInt32();

            byte[] data = reader.ReadBytes(dataSize);
            float[] samples = ConvertByteToFloat(data);

            AudioClip audioClip = AudioClip.Create(clipName, samples.Length / channels, channels, sampleRate, false);
            audioClip.SetData(samples, 0);
            return audioClip;
        }
    }

    private static byte[] ConvertAudioClipDataToInt16ByteArray(float[] data)
    {
        MemoryStream dataStream = new MemoryStream();

        for (int i = 0; i < data.Length; i++)
        {
            short value = (short)(data[i] * short.MaxValue);
            dataStream.Write(BitConverter.GetBytes(value), 0, 2);
        }

        return dataStream.ToArray();
    }

    private static float[] ConvertByteToFloat(byte[] array)
    {
        int floatCount = array.Length / 2;
        float[] floatArr = new float[floatCount];

        for (int i = 0; i < floatCount; i++)
        {
            short value = BitConverter.ToInt16(array, i * 2);
            floatArr[i] = value / 32768.0f;
        }

        return floatArr;
    }

    private static byte[] ConvertToWav(float[] samples, int channels, int sampleRate)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            int byteRate = sampleRate * channels * 2;
            int dataSize = samples.Length * 2;

            // RIFF header
            stream.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"), 0, 4);
            stream.Write(BitConverter.GetBytes(36 + dataSize), 0, 4);
            stream.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"), 0, 4);
            stream.Write(System.Text.Encoding.ASCII.GetBytes("fmt "), 0, 4);
            stream.Write(BitConverter.GetBytes(16), 0, 4);
            stream.Write(BitConverter.GetBytes((short)1), 0, 2);
            stream.Write(BitConverter.GetBytes((short)channels), 0, 2);
            stream.Write(BitConverter.GetBytes(sampleRate), 0, 4);
            stream.Write(BitConverter.GetBytes(byteRate), 0, 4);
            stream.Write(BitConverter.GetBytes((short)(channels * 2)), 0, 2);
            stream.Write(BitConverter.GetBytes((short)16), 0, 2);

            // data chunk
            stream.Write(System.Text.Encoding.ASCII.GetBytes("data"), 0, 4);
            stream.Write(BitConverter.GetBytes(dataSize), 0, 4);

            // samples
            Int16[] intData = new Int16[samples.Length];
            byte[] bytesData = new byte[samples.Length * 2];

            for (int i = 0; i < samples.Length; i++)
            {
                float clamped = Mathf.Clamp(samples[i], -1f, 1f);
                intData[i] = (short)(clamped * short.MaxValue);
                byte[] byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }

            stream.Write(bytesData, 0, bytesData.Length);

            // 👇 RETURN before disposing
            return stream.ToArray();
        }
    }


    private static void WriteHeader(Stream stream, AudioClip clip, int dataLength)
    {
        int sampleRate = clip.frequency;
        int channels = clip.channels;
        int byteRate = sampleRate * channels * 2;

        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(dataLength + HEADER_SIZE - 8);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
            writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((ushort)1); // PCM
            writer.Write((ushort)channels);
            writer.Write(sampleRate);
            writer.Write(byteRate);
            writer.Write((ushort)(channels * 2));
            writer.Write((ushort)16);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(dataLength);
        }
    }
}

