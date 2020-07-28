using Prism.Navigation;
using Screw.Core.LE;
using Screw.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Screw.Core.ViewModels
{
    class WatcherViewModel : BaseViewModel
    {
        IAdapter Adapter { get; }
        public IList<DeviceViewModel> Devices { get; }

        public WatcherViewModel(INavigationService navigationService, ILEService leService)
            : base(navigationService)
        {
            Adapter = leService.GetAdapter();
            Devices = new ObservableCollection<DeviceViewModel>();

            Adapter.DeviceScanned += OnDeviceScanned;
        }

        private void OnDeviceScanned(object sender, DeviceEventArgs e)
        {
            var nameSection = e.Advertisements.FirstOrDefault(i => i.Type == 0x09);
            if (nameSection == null)
                return;
            var uuidSection = e.Advertisements.FirstOrDefault(i => i.Type == 0x07);
            var vendorSection = e.Advertisements.FirstOrDefault(i => i.Type == 0xFF);
            if (uuidSection == null && vendorSection == null)
                return;
            short vid;
            short pid;
            byte mid;
            string mac;
            if (uuidSection != null)
            {
                var uuidValue = uuidSection.Value;
                Array.Reverse(uuidValue);
                var uuidStr = BitConverter.ToString(uuidValue).Replace("-", string.Empty);
                var uuid = Guid.Parse(uuidStr);
                if (!DeviceUtil.Valid(uuid))
                    return;
                var nameValue = nameSection.Value;
                var mixed = Encoding.ASCII.GetString(nameValue);
                var pattern = @"(?<LETTER>[ACQRab])(?<NUMBER>\d{3})(?<MAC>(?i)[A-F|0-9]{4})";
                var match = Regex.Match(mixed, pattern);
                if (!match.Success)
                    return;
                vid = 0x2E19;
                mid = 0;
                var letter = match.Groups["LETTER"].Value.ToUpper();
                var number = match.Groups["NUMBER"].Value;
                // 兼容老版本
                if (letter.Equals("A", StringComparison.OrdinalIgnoreCase) || letter.Equals("C", StringComparison.OrdinalIgnoreCase))
                {
                    pid = 0x036B;
                    //name = letter == "A" ? $"Additel {number}" : $"ConST {number}";
                }
                else
                {
                    var firstByte = Encoding.ASCII.GetBytes(letter)[0];
                    //var higher = firstByte.GetHigher();
                    var lower = firstByte.GetLower();
                    pid = (short)(lower == 1 ? 0x036B : 0x518B);
                    //name = higher == 5 ? $"ConST {number}" : $"Additel {number}";
                }
                mac = match.Groups["MAC"].Value;
            }
            else
            {
                var vendorValue = vendorSection.Value;
                if (vendorValue.Length < 11)
                    return;
                // TODO: 优化此处逻辑 BitConverter.ToInt16(vendorValue, i);
                // VId
                var vidValue = new byte[2];
                Array.Copy(vendorValue, 0, vidValue, 0, vidValue.Length);
                vid = BitConverter.ToInt16(vidValue, 0);
                if (vid != 0x2E19 && vid != 0x045E)
                    return;
                // PId
                var pidValue = new byte[2];
                Array.Copy(vendorValue, 2, pidValue, 0, pidValue.Length);
                pid = BitConverter.ToInt16(pidValue, 0);
                // MId
                var midValue = new byte[1];
                Array.Copy(vendorValue, 4, midValue, 0, midValue.Length);
                mid = midValue[0];
                if (!DeviceUtil.Valid(vid, pid, mid))
                    return;
                // MAC
                var macArray = new byte[6];
                Array.Copy(vendorValue, 5, macArray, 0, macArray.Length);
                mac = BitConverter.ToString(macArray).Replace('-', ':');
            }
            var deviceModel = Devices.FirstOrDefault(i => i.MAC == mac);
            if (deviceModel != null)
            {
                deviceModel.RSSI = e.RSSI;
            }
            else
            {
                var deviceArgs = DeviceUtil.GetArgs(e.RSSI, vid, pid, mid, mac);
                deviceModel = new DeviceViewModel(NavigationService, deviceArgs);
                Devices.Add(deviceModel);
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            Adapter.StartScan();
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            Adapter.StopScan();
        }
    }
}
