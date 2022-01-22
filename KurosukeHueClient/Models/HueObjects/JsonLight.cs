using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHueClient.Models.HueObjects
{
    public class JsonLight
    {
        public JsonLight() { }
        public JsonLight(Q42.HueApi.Light light)
        {
            Id = light.Id;
            State = new JsonState(light.State);
        }
        public string Id { get; set; }
        public JsonState State { get; set; }
    }
}
