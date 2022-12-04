using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
