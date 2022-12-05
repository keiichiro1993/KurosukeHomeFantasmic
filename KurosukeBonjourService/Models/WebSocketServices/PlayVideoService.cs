using KurosukeBonjourService.Models.BonjourEventArgs;
using System;
using System.Text.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace KurosukeBonjourService.Models.WebSocketServices
{
    public class PlayVideoService : WebSocketBehavior
    {
        public enum PlayVideoServiceStatus { Play, Pause }

        public event EventHandler<PlayVideoEventArgs> PlayVideoRequested;

        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
            if (PlayVideoRequested != null && !string.IsNullOrEmpty(e.Data))
            {
                var deserializedData = JsonSerializer.Deserialize<PlayVideoEventArgs>(e.Data);
                PlayVideoRequested(this, deserializedData);
            }
        }
    }
}
