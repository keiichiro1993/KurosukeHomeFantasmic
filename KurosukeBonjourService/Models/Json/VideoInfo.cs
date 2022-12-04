using System;

namespace KurosukeBonjourService.Models.Json
{
    public class VideoInfo
    {
        public VideoInfo() { }
        public VideoInfo(string name, TimeSpan length, string path)
        {
            Name = name;
            Length = length;
            Path = path;
        }

        public string Name { get; }
        public TimeSpan Length { get; }
        public string Path { get; }
    }
}
