using System;
using System.IO;

namespace VoiceToForm.PlatformInterfaces
{
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);
        string GetRootFolder();
        bool Exists(string fileName);
        string[] GetFileNamesByExtension(string path, string extension);
        void WriteAllText(string text, string path);
        string ReadAllText(string path);
        void WriteStream(Stream stream, string path);
        Stream ReadStream(string path);
        void WriteAllBytes(byte[] bytes, string path);
        Byte[] ReadAllBytes(string path);
        void DeleteFile(string fileName);
    }
}
