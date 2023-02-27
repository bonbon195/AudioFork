using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AudioFork
{
    public partial class MainWindow : Window
    {
        Audio audio;
        public MainWindow()
        {
            InitializeComponent();
            audio = new Audio();
            var allDevices = audio.GetAllDevices();
            foreach ( var device in allDevices )
            {
                inputDevicesListBox.Items.Add( device );
            }

        }


        private void inputDevicesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDevice = inputDevicesListBox.SelectedItem as MMDevice;
            audio.ChooseCaptureDevice(selectedDevice);
            var newAvailableOutputDevices = audio.GetAllDevices();
            if (audio.outputDevices.Exists(dev => dev.mDevice.ID == selectedDevice.ID))
            {

                audio.RemoveOutputDevice(selectedDevice);
            }
            newAvailableOutputDevices.RemoveAll(dev => dev.ID == selectedDevice.ID);
            outputDevicesListBox.Items.Clear();
            newAvailableOutputDevices.ForEach(dev =>
            {
                outputDevicesListBox.Items.Add(dev);
            });

        }

        private void outputDevicesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var removedItem in e.RemovedItems)
            {
                audio.RemoveOutputDevice(removedItem as MMDevice);
            }
            foreach (var addedItem in e.AddedItems)
            {
                audio.AddOutputDevice(addedItem as MMDevice);
            }
        }

        private void inputButton_Click(object sender, RoutedEventArgs e)
        {
            if (audio.waveIn != null)
            {
                if (inputButton.Content.ToString() == "Начать" & audio.outputDevices.Count > 0)
                {
                    audio.StartCapturing();
                    inputButton.Content = "Остановить";
                    inputDevicesListBox.IsEnabled = outputDevicesListBox.IsEnabled = false;

                }
                else if(inputButton.Content.ToString() == "Остановить")
                {
                    audio.StopCapturing();
                    inputButton.Content = "Начать";

                    inputDevicesListBox.IsEnabled = outputDevicesListBox.IsEnabled = true;

                }
            }
        }
    }
}
