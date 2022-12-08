using System;

namespace KurosukeBonjourService.Models.BonjourEventArgs
{

    public enum ConnectionStatus { Connected, Disconnected, Connecting }
    public class ConnectionStatusEventArgs : EventArgs
    {
        public ConnectionStatus Status { get; set; }
        public string StatusMessage { get; set; }
    }
}
