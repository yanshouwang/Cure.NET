using CoreBluetooth;
using Foundation;
using Screw.Core.LE;
using System;
using System.Collections.Generic;

namespace Screw.iOS.LE
{
    static class Util
    {
        public static IList<Advertisement> GetAdvertisements(NSDictionary data)
        {
            var advertisements = new List<Advertisement>();
            foreach (NSString key in data.Keys)
            {
                var type = GetType(key);
                switch (type)
                {
                    case 0x07:
                        {
                            var array = (NSArray)data.ObjectForKey(key);
                            for (nuint i = 0; i < array.Count; i++)
                            {
                                var value = array.GetItem<NSData>(i).ToArray();
                                var item = new Advertisement(type, value);
                                advertisements.Add(item);
                            }
                            break;
                        }
                    case 0x09:
                        {
                            var localName = (NSString)data.ObjectForKey(key);
                            var value = NSData.FromString(localName).ToArray();
                            var item = new Advertisement(type, value);
                            advertisements.Add(item);
                            break;
                        }
                    case 0x0A:
                        {
                            var level = (NSNumber)data.ObjectForKey(key);
                            var value = new[] { level.ByteValue };
                            var item = new Advertisement(type, value);
                            advertisements.Add(item);
                            break;
                        }
                    case 0x16:
                        {
                            var datas = (NSDictionary)data.ObjectForKey(key);
                            foreach (CBUUID key1 in datas.Keys)
                            {
                                var data1 = (NSData)datas.ObjectForKey(key1);
                                var array1 = key1.Data.ToArray();
                                var array2 = data1.ToArray();
                                var value = new byte[array1.Length + array2.Length];
                                Array.Copy(array1, 0, value, 0, array1.Length);
                                Array.Copy(array2, 0, value, array1.Length, array2.Length);
                                var item = new Advertisement(type, value);
                                advertisements.Add(item);
                            }
                            break;
                        }
                    case 0xAA:
                        {
                            var connectable = (NSNumber)data.ObjectForKey(key);
                            var value = new[] { connectable.ByteValue };
                            var item = new Advertisement(type, value);
                            advertisements.Add(item);
                            break;
                        }
                    case 0xFF:
                        {
                            var data1 = (NSData)data.ObjectForKey(key);
                            var value = data1.ToArray();
                            var item = new Advertisement(type, value);
                            advertisements.Add(item);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            return advertisements;
        }

        static byte GetType(NSString keyStr)
        {
            byte key;
            if (keyStr == CBAdvertisement.DataServiceUUIDsKey)
            {
                key = 0x07;
            }
            else if (keyStr == CBAdvertisement.DataLocalNameKey)
            {
                key = 0x09;
            }
            else if (keyStr == CBAdvertisement.DataTxPowerLevelKey)
            {
                key = 0x0A;
            }
            else if (keyStr == CBAdvertisement.DataServiceDataKey)
            {
                key = 0x16;
            }
            else if (keyStr == CBAdvertisement.IsConnectable)
            {
                key = 0xAA;
            }
            else if (keyStr == CBAdvertisement.DataManufacturerDataKey)
            {
                key = 0xFF;
            }
            else
            {
                key = 0x00;
            }
            return key;
        }
    }
}