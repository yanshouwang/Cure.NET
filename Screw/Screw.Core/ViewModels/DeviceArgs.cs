using System;
using System.Collections.Generic;
using System.Text;

namespace Screw.Core.ViewModels
{
    class DeviceArgs
    {
        public DeviceCategory Category { get; }
        public short RSSI { get; set; }
        public short VId { get; }
        public short PId { get; }
        public byte MId { get; }
        public string MAC { get; }
        public Guid CommunicatonUUID { get; }
        public Guid NotifyUUID { get; }
        public Guid WriteUUID { get; }

        public DeviceArgs(DeviceCategory category, short rssi, short vid, short pid, byte mid, string mac, Guid communicationUUID, Guid notifyUUID, Guid writeUUID)
        {
            Category = category;
            RSSI = rssi;
            VId = vid;
            PId = pid;
            MId = mid;
            MAC = mac;
            CommunicatonUUID = communicationUUID;
            NotifyUUID = notifyUUID;
            WriteUUID = writeUUID;
        }
    }
}
