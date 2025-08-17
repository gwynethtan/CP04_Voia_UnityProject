/*
 * Author: Hoo Ying Qi Praise and Tan Ting Yu Gwyneth
 * Date: 5/5/2025
 * A simple utility to convert Unity AudioClips to WAV files and back
 */

using System;
using System.IO;
using UnityEngine;

public static class WavUtility
{
    /// <summary>
    /// Header size
    /// </summary>
    const int HEADER_SIZE = 44;

    /// <summary>
    /// Converts a Unity AudioClip into a WAV byte array.
    /// </summary>
    /// <param name="clip">The AudioClip to convert.</param>
    /// <returns>Byte array representing the WAV file.</returns>
    public static byte[] FromAudioClip(AudioClip clip)
    {
        int sampleCount = clip.samples * clip.channels;
        int frequency = clip.frequency;
        int channels = clip.channels;
        float[] samples = new float[sampleCount];
        clip.GetData(samples, 0);
        return ConvertToWav(samples, channels, frequency);
    }

    /// <summary>
    /// Converts a WAV byte array into a Unity AudioClip.
    /// </summary>
    /// <param name="wavFileBytes">The byte array containing the WAV file.</param>
    /// <param name="clipName">Optional name for the created AudioClip (default: "wav").</param>
    /// <returns>AudioClip created from the WAV data.</returns>
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

    /// <summary>
    /// Converts a float array of audio samples to a 16-bit PCM byte array.
    /// </summary>
    /// <param name="data">The audio samples in float format (-1.0f to 1.0f).</param>
    /// <returns>Byte array representing 16-bit PCM audio data.</returns>
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

    /// <summary>
    /// Converts a 16-bit PCM byte array to a float array of audio samples.
    /// </summary>
    /// <param name="array">The byte array of 16-bit PCM audio data.</param>
    /// <returns>Float array representing the audio samples (-1.0f to 1.0f).</returns>
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

    /// <summary>
    /// Creates a WAV byte array from raw audio samples, channel count, and sample rate.
    /// </summary>
    /// <param name="samples">The audio samples in float format.</param>
    /// <param name="channels">Number of audio channels.</param>
    /// <param name="sampleRate">Sample rate in Hz.</param>
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

    /// <summary>
    /// Writes a WAV file header into a stream for the given AudioClip and data length.
    /// </summary>
    /// <param name="stream">The target stream to write the header into.</param>
    /// <param name="clip">The AudioClip providing metadata.</param>
    /// <param name="dataLength">The length of the audio data in bytes.</param>
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

