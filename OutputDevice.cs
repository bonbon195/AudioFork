using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioFork
{
    internal class OutputDevice
    {
        public MMDevice mDevice;
        public DirectSoundOut waveOut;
        public BufferedWaveProvider provider;

        public OutputDevice(MMDevice mDevice, DirectSoundOut waveOut, BufferedWaveProvider provider)
        {
            this.mDevice = mDevice;
            this.waveOut = waveOut;
            this.provider = provider;
        }

    }
}