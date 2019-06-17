using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battlefield_BitStream.Core.Engine
{
    public class _3DEngine//part of this code is borrowed from WarrantyVoider(https://github.com/zeroKilo)
    {
        public SharpDX.Direct3D11.Device device { get; private set; }
        public DeviceContext context { get; private set; }
        public SwapChain swapChain { get; private set; }
        private PictureBox p { get; set; }
        public _3DEngine()
        {
            p = new PictureBox();
        }
        public void InitDevice()
        {
            SharpDX.Direct3D11.Device dev;
            SwapChain swap;
            SharpDX.Direct3D11.Device.CreateWithSwapChain(
                DriverType.Hardware,
                DeviceCreationFlags.None,
                new[] 
                {
                    FeatureLevel.Level_11_1,
                    FeatureLevel.Level_11_0,
                    FeatureLevel.Level_10_1,
                    FeatureLevel.Level_10_0,
                },
                new SwapChainDescription()
                {
                    ModeDescription =
                        new ModeDescription(
                            p.ClientSize.Width,
                            p.ClientSize.Height,
                            new Rational(60, 1),
                            Format.R8G8B8A8_UNorm
                        ),
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = Usage.BackBuffer | Usage.RenderTargetOutput,
                    BufferCount = 1,
                    Flags = SwapChainFlags.AllowModeSwitch,
                    IsWindowed = true,
                    OutputHandle = p.Handle,
                    SwapEffect = SwapEffect.Discard,
                },
                out dev, out swap
            );
            device = dev;
            swapChain = swap;
        }
    }
}