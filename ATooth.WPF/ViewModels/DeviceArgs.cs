using System;
using System.Collections.Generic;
using System.Text;

namespace ATooth.WPF.ViewModels
{
    class DeviceArgs
    {
        public DeviceCategory Category { get; }
        public ulong Address { get; }
        public short RSSI { get; set; }
        public short VId { get; }
        public short PId { get; }
        public byte MId { get; }
        public string MAC { get; }
        public Guid CommunicatonUUID { get; }
        public Guid NotifyUUID { get; }
        public Guid WriteUUID { get; }

        public DeviceArgs(DeviceCategory category, ulong address, short rssi, short vid, short pid, byte mid, string mac, Guid communicationUUID, Guid notifyUUID, Guid writeUUID)
        {
            Category = category;
            Address = address;
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
