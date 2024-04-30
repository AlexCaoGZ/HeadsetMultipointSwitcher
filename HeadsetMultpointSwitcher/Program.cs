using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Windows.Forms;
using System.Drawing;
namespace HeadsetMultipointSwitcher
{
    internal class Program
    {
        static NotifyIcon trayIcon;
        static CoreAudioController controller = new AudioSwitcher.AudioApi.CoreAudio.CoreAudioController();

        static void Main(string[] args)
        {
            var task = System.Threading.Tasks.Task.Factory.StartNew(() => setTrayIcon());
            task.Wait();
        }

        /*
         * void setTrayIcon()
         * IN   : none
         * OUT  : none
         * INFO : set tray icon
         */
        private static void setTrayIcon()
        {
            trayIcon = new NotifyIcon
            {
                Text = "Headset Multipoint Switcher",
                Icon = new Icon(@"../../icon.ico"),
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Switch",changeDevice),
                    new MenuItem("Exit", trayIconExit)
                }),
            };
            trayIcon.DoubleClick += new System.EventHandler(changeDevice);
            trayIcon.Visible = true;
            //114514 does not matter, it was deprecated, but ShowBalloonTip() still require it. 
            trayIcon.ShowBalloonTip(114514, "Information", "Headset Multipoint Switcher Started.", ToolTipIcon.Info);
            Application.Run();
        }

        /*
         * void trayIconExit()
         * IN   : none
         * OUT  : none
         * INFO : when user click Exit in trayIcon's menu
         */
        static void trayIconExit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        /*
         * void changeDevice()
         * IN   : none
         * OUT  : none
         * INFO : Get defaul device, switch to other device, wait 2s, back to defaul device
         */
        static void changeDevice(object sender, EventArgs e)
        {
            trayIcon.ShowBalloonTip(114514, "Information", "Switching", ToolTipIcon.Info);
            IEnumerable<CoreAudioDevice> devices = new CoreAudioController().GetPlaybackDevices();

            if (devices.Count() == 1)
            {
                trayIcon.ShowBalloonTip(114514, "Error", "Code99:Only 1 device found.", ToolTipIcon.Info);
                return;
            }

            CoreAudioDevice defaulDevice = new CoreAudioController().GetDefaultDevice(DeviceType.Playback, Role.Multimedia);
            foreach (CoreAudioDevice device in devices)
            {
                if (device != defaulDevice)
                {
                    controller.SetDefaultDevice(device);
                    break;
                }
            }
            Thread.Sleep(2000);
            controller.SetDefaultDevice(defaulDevice);

            //hope can work
            devices = null;
            defaulDevice = null;
        }
    }
}
