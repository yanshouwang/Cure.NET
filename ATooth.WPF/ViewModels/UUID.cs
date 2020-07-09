using System;
using System.Collections.Generic;
using System.Text;

namespace ATooth.WPF.ViewModels
{
    static class UUID
    {
        // ATC
        public static Guid ATC_CUSTOM_SERVICE = Guid.Parse("41444449-5445-4C42-4C45-4D4F44554C45");
        public static Guid ATC_COMMUNICATION_SERVICE = Guid.Parse("00035B03-58E6-07DD-021A-08123A000300");
        public static Guid ATC_NOTIFY_CHARACTERISTIC = Guid.Parse("00035B03-58E6-07DD-021A-08123A000301");
        public static Guid ATC_WRITE_CHARACTERISTIC = Guid.Parse("00035B03-58E6-07DD-021A-08123A000301");
        // UTC
        public static Guid UTC_CUSTOM_SERVICE = Guid.Parse("41444449-5445-4C42-4C45-4D4F44554C45");
        public static Guid UTC_COMMUNICATION_SERVICE = Guid.Parse("00035B03-58E6-07DD-021A-08123A000300");
        public static Guid UTC_NOTIFY_CHARACTERISTIC = Guid.Parse("00035B03-58E6-07DD-021A-08123A000301");
        public static Guid UTC_WRITE_CHARACTERISTIC = Guid.Parse("00035B03-58E6-07DD-021A-08123A000301");
        // APC2
        public static Guid APC2_0_COMMUNICATION_SERVICE = Guid.Parse("0000A002-0000-1000-8000-00805F9B34FB");
        public static Guid APC2_0_NOTIFY_CHARACTERISTIC = Guid.Parse("0000C305-0000-1000-8000-00805F9B34FB");
        public static Guid APC2_0_WRITE_CHARACTERISTIC = Guid.Parse("0000C304-0000-1000-8000-00805F9B34FB");
        public static Guid APC2_1_COMMUNICATION_SERVICE => Guid.Parse("41444449-5445-4C42-4C45-4D4F44554C49");
        public static Guid APC2_1_NOTIFY_CHARACTERISTIC = Guid.Parse("41444449-5445-4C42-4C45-4D4F44554C44");
        public static Guid APC2_1_WRITE_CHARACTERISTIC = Guid.Parse("41444449-5445-4C42-4C45-4D4F44554C44");
        // PGC
        public static Guid PGC_COMMUNICATION_SERVICE = Guid.Parse("AF661820-D14A-4B21-90F8-54D58F8614F0");
        public static Guid PGC_NOTIFY_CHARACTERISTIC = Guid.Parse("1B6B9415-FF0D-47C2-9444-A5032F727B2D");
        public static Guid PGC_WRITE_CHARACTERISTIC = Guid.Parse("1B6B9415-FF0D-47C2-9444-A5032F727B2D");

        public static Guid GetCommunicationId(short vid, short pid, byte mid)
        {
            if (vid == 0x2E19 && pid == 0x036B && mid == 0)
            {
                return ATC_COMMUNICATION_SERVICE;
            }
            else if (vid == 0x2E19 && pid == 0x518B && mid == 0)
            {
                return UTC_COMMUNICATION_SERVICE;
            }
            else if (vid == 0x2E19 && pid == 0x02F9 && mid == 0)
            {
                return APC2_0_COMMUNICATION_SERVICE;
            }
            else if (vid == 0x2E19 && pid == 0x02F9 && mid == 1)
            {
                return APC2_1_COMMUNICATION_SERVICE;
            }
            else if (vid == 0x045E && pid == 0x00CE && mid == 0)
            {
                return APC2_0_COMMUNICATION_SERVICE;
            }
            else if (vid == 0x045E && pid == 0x00CE && mid == 1)
            {
                return APC2_1_COMMUNICATION_SERVICE;
            }
            else if (vid == 0x2E19 && pid == 0x02A0 && mid == 0)
            {
                return PGC_COMMUNICATION_SERVICE;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
        public static Guid GetNotityId(short vid, short pid, byte mid)
        {
            if (vid == 0x2E19 && pid == 0x036B && mid == 0)
            {
                return ATC_NOTIFY_CHARACTERISTIC;
            }
            else if (vid == 0x2E19 && pid == 0x518B && mid == 0)
            {
                return UTC_NOTIFY_CHARACTERISTIC;
            }
            else if (vid == 0x2E19 && pid == 0x02F9 && mid == 0)
            {
                return APC2_0_NOTIFY_CHARACTERISTIC;
            }
            else if (vid == 0x2E19 && pid == 0x02F9 && mid == 1)
            {
                return APC2_1_NOTIFY_CHARACTERISTIC;
            }
            else if (vid == 0x045E && pid == 0x00CE && mid == 0)
            {
                return APC2_0_NOTIFY_CHARACTERISTIC;
            }
            else if (vid == 0x045E && pid == 0x00CE && mid == 1)
            {
                return APC2_1_NOTIFY_CHARACTERISTIC;
            }
            else if (vid == 0x2E19 && pid == 0x02A0 && mid == 0)
            {
                return PGC_NOTIFY_CHARACTERISTIC;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
        public static Guid GetWriteId(short vid, short pid, byte mid)
        {
            if (vid == 0x2E19 && pid == 0x036B && mid == 0)
            {
                return ATC_WRITE_CHARACTERISTIC;
            }
            else if (vid == 0x2E19 && pid == 0x518B && mid == 0)
            {
                return UTC_WRITE_CHARACTERISTIC;
            }
            else if (vid == 0x2E19 && pid == 0x02F9 && mid == 0)
            {
                return APC2_0_WRITE_CHARACTERISTIC;
            }
            else if (vid == 0x2E19 && pid == 0x02F9 && mid == 1)
            {
                return APC2_1_WRITE_CHARACTERISTIC;
            }
            else if (vid == 0x045E && pid == 0x00CE && mid == 0)
            {
                return APC2_0_WRITE_CHARACTERISTIC;
            }
            else if (vid == 0x045E && pid == 0x00CE && mid == 1)
            {
                return APC2_1_WRITE_CHARACTERISTIC;
            }
            else if (vid == 0x2E19 && pid == 0x02A0 && mid == 0)
            {
                return PGC_WRITE_CHARACTERISTIC;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
