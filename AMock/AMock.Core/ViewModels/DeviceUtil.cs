using System;
using System.Collections.Generic;
using System.Text;

namespace AMock.Core.ViewModels
{
    static class DeviceUtil
    {
        // ATC
        const string ATC_IDENTIFY_SERVICE = "41444449-5445-4C42-4C45-4D4F44554C45";
        const string ATC_COMMUNICATION_SERVICE = "00035B03-58E6-07DD-021A-08123A000300";
        const string ATC_NOTIFY_CHARACTERISTIC = "00035B03-58E6-07DD-021A-08123A000301";
        const string ATC_WRITE_CHARACTERISTIC = "00035B03-58E6-07DD-021A-08123A000301";
        // UTC
        const string UTC_IDENTIFY_SERVICE = "41444449-5445-4C42-4C45-4D4F44554C45";
        const string UTC_COMMUNICATION_SERVICE = "00035B03-58E6-07DD-021A-08123A000300";
        const string UTC_NOTIFY_CHARACTERISTIC = "00035B03-58E6-07DD-021A-08123A000301";
        const string UTC_WRITE_CHARACTERISTIC = "00035B03-58E6-07DD-021A-08123A000301";
        // APC2
        const string APC2_0_COMMUNICATION_SERVICE = "0000A002-0000-1000-8000-00805F9B34FB";
        const string APC2_0_NOTIFY_CHARACTERISTIC = "0000C305-0000-1000-8000-00805F9B34FB";
        const string APC2_0_WRITE_CHARACTERISTIC = "0000C304-0000-1000-8000-00805F9B34FB";
        const string APC2_1_COMMUNICATION_SERVICE = "41444449-5445-4C42-4C45-4D4F44554C49";
        const string APC2_1_NOTIFY_CHARACTERISTIC = "41444449-5445-4C42-4C45-4D4F44554C44";
        const string APC2_1_WRITE_CHARACTERISTIC = "41444449-5445-4C42-4C45-4D4F44554C44";
        // PGC
        const string PGC_COMMUNICATION_SERVICE = "AF661820-D14A-4B21-90F8-54D58F8614F0";
        const string PGC_NOTIFY_CHARACTERISTIC = "1B6B9415-FF0D-47C2-9444-A5032F727B2D";
        const string PGC_WRITE_CHARACTERISTIC = "1B6B9415-FF0D-47C2-9444-A5032F727B2D";

        static readonly IDictionary<string, (DeviceCategory Category, string CommunicationUUID, string NotifyUUID, string WriteUUID)> s_uuids = new Dictionary<string, (DeviceCategory category, string, string, string)>()
        {
            ["2E19-036B-0"] = (DeviceCategory.ATC, ATC_COMMUNICATION_SERVICE, ATC_NOTIFY_CHARACTERISTIC, ATC_WRITE_CHARACTERISTIC),
            ["2E19-518B-0"] = (DeviceCategory.UTC, UTC_COMMUNICATION_SERVICE, UTC_NOTIFY_CHARACTERISTIC, UTC_WRITE_CHARACTERISTIC),
            ["2E19-02F9-0"] = (DeviceCategory.APC2, APC2_0_COMMUNICATION_SERVICE, APC2_0_NOTIFY_CHARACTERISTIC, APC2_0_WRITE_CHARACTERISTIC),
            ["2E19-02F9-1"] = (DeviceCategory.APC2, APC2_1_COMMUNICATION_SERVICE, APC2_1_NOTIFY_CHARACTERISTIC, APC2_1_WRITE_CHARACTERISTIC),
            ["045E-00CE-0"] = (DeviceCategory.APC2, APC2_0_COMMUNICATION_SERVICE, APC2_0_NOTIFY_CHARACTERISTIC, APC2_0_WRITE_CHARACTERISTIC),
            ["045E-00CE-1"] = (DeviceCategory.APC2, APC2_1_COMMUNICATION_SERVICE, APC2_1_NOTIFY_CHARACTERISTIC, APC2_1_WRITE_CHARACTERISTIC),
            ["2E19-02A0-0"] = (DeviceCategory.PGC, PGC_COMMUNICATION_SERVICE, PGC_NOTIFY_CHARACTERISTIC, PGC_WRITE_CHARACTERISTIC),
        };

        static string GetKey(short vid, short pid, byte mid)
            => $"{vid:X4}-{pid:X4}-{mid}";

        public static bool Valid(Guid uuid)
        {
            var uuidStr = uuid.ToString().ToUpper();
            return uuidStr == ATC_IDENTIFY_SERVICE || uuidStr == UTC_IDENTIFY_SERVICE;
        }

        public static bool Valid(short vid, short pid, byte mid)
        {
            var key = GetKey(vid, pid, mid);
            return s_uuids.ContainsKey(key);
        }

        public static DeviceArgs GetArgs(short vid, short pid, byte mid, string mac, short rssi)
        {
            var key = GetKey(vid, pid, mid);
            var (Category, CommunicationUUID, NotifyUUID, WriteUUID) = s_uuids[key];
            var category = Category;
            var communicationUUID = Guid.Parse(CommunicationUUID);
            var notifyUUID = Guid.Parse(NotifyUUID);
            var writeUUID = Guid.Parse(WriteUUID);
            return new DeviceArgs(category, vid, pid, mid, mac, communicationUUID, notifyUUID, writeUUID, rssi);
        }
    }
}
