using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AudioFork
{
    internal class Audio
    {
        public List<OutputDevice> outputDevices = new List<OutputDevice>();
        public MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
        public WasapiLoopbackCapture waveIn;
        public void StartCapturing()
        {
            try
            {
                Console.WriteLine(outputDevices.Count);
                outputDevices.ForEach(outputDevice =>
                {
                    PlayStream(outputDevice);
                });

                if (waveIn.CaptureState == CaptureState.Starting)
                {
                    Console.WriteLine("Starting audio stream...");
                }
                waveIn.StartRecording();

            }
            catch
            {
                Console.WriteLine("ПРОИЗОШЁЛ КРИНЖ");
            }



        }

        public void PlayStream(OutputDevice outputDevice)
        {
            Console.WriteLine("OUTPUT    " + outputDevice.mDevice.FriendlyName);

            outputDevice.waveOut.Play();
        }

        public void StopCapturing()
        {
            waveIn.StopRecording();
            outputDevices.ForEach(outputDevice =>
            {
                outputDevice.waveOut.Stop();
                outputDevice.waveOut.Dispose();
            });

        }

        public void DisposeCaptureDevice()
        {
            waveIn.Dispose();
        }

        private void RecordingStopped(object sender, StoppedEventArgs args)
        {
            foreach (var outputDevice in outputDevices)
            {
                Console.WriteLine("OUTPUT    " + outputDevice.mDevice.FriendlyName);
                outputDevice.waveOut.Stop();
            }

        }

        private void OnDataAvailable(object sender, WaveInEventArgs args)
        {
            outputDevices.ForEach((outputDevice) =>
            {
                outputDevice.provider.AddSamples(args.Buffer, 0, args.BytesRecorded);
            });

        }

        public void ChooseCaptureDevice(MMDevice device)
        {
            //if (waveIn == null)
            //{
            //    waveIn = new WasapiLoopbackCapture(device);
            //}
            //else {
            //    waveIn = new WasapiLoopbackCapture(device);
            //    var newOutputDevices = new List<OutputDevice>();
            //    foreach (var outputDevice in outputDevices)
            //    {
            //        outputDevice.waveOut.Dispose();
            //        outputDevice.provider.ClearBuffer();
            //        outputDevice.provider = new BufferedWaveProvider(waveIn.WaveFormat);
            //        outputDevice.waveOut.Init(outputDevice.provider);
            //        newOutputDevices.Add(outputDevice);
            //    }
            //    outputDevices = newOutputDevices;

            //}
            //defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);

            waveIn = new WasapiLoopbackCapture(device);

            waveIn.DataAvailable += OnDataAvailable;
            waveIn.RecordingStopped += RecordingStopped;


        }
        public void RemoveOutputDevice(MMDevice device)
        {
            //if (outputDevices.Exists(dev => dev.mDevice.ID == device.ID))
            //{
            //    var outputDevice = outputDevices.Find(dev => dev.mDevice.ID == device.ID);
            //    outputDevice.mDevice.Dispose();
            //    outputDevice.waveOut.Dispose();
            //    outputDevice.provider.ClearBuffer();
            //    outputDevices.Remove(outputDevice);
            //}

            outputDevices.RemoveAll(dev => dev.mDevice.ID == device.ID);

        }

        public void AddOutputDevice(MMDevice device)
        {
            var provider = new BufferedWaveProvider(waveIn.WaveFormat);
            provider.DiscardOnBufferOverflow = true;

            var waveOut = new DirectSoundOut(Guid.Parse(device.ID.Substring(18, 36)));
            waveOut.Init(provider);

            var outputDevice = new OutputDevice(device, waveOut, provider);

            outputDevices.Add(outputDevice);

        }

        public List<MMDevice> GetAllDevices()
        {
            return enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).ToList();
        }

    }

}