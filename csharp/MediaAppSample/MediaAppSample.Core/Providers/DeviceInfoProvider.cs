using System;
using Windows.Graphics.Display;
using Windows.System.Profile;
using Windows.UI.Xaml;

namespace Contoso.Core.Providers
{
    public partial class PlatformCoreBase
    {
        /// <summary>
        /// Gets access to the device info adapter implement of the platform currently executing.
        /// </summary>
        public DeviceInfoProvider DeviceInfo
        {
            get { return this.GetAdapter<DeviceInfoProvider>(); }
            protected set { this.Register<DeviceInfoProvider>(value); }
        }
    }

    /// <summary>
    /// Base class providing access to the device information for the executing platform.
    /// </summary>
    public sealed class DeviceInfoProvider : ProviderBase
    {
        internal DeviceInfoProvider()
        {
        }

        #region Properties

        /// <summary>
        /// Gets the unique identifier of the device.
        /// </summary>
        public string DeviceUniqueId
        {
            get
            {
                //var token = HardwareIdentification.GetPackageSpecificToken(null);
                //var hardwareId = token.Id;
                //var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);
                //byte[] bytes = new byte[hardwareId.Length];
                //dataReader.ReadBytes(bytes);
                //return BitConverter.ToString(bytes);

                // TODO return DeviceUniqueID
                return null;
            }
        }

        /// <summary>
        /// Gets the device's physical width in pixels.
        /// </summary>
        public double DevicePhysicalPixelsWidth
        {
            get { return this.GetResolution(this.DeviceLogicalPixelsWidth, DisplayInformation.GetForCurrentView().ResolutionScale); }
        }

        /// <summary>
        /// Gets the device's physical height in pixels.
        /// </summary>
        public double DevicePhysicalPixelsHeight
        {
            get { return this.GetResolution(this.DeviceLogicalPixelsHeight, DisplayInformation.GetForCurrentView().ResolutionScale); }
        }

        /// <summary>
        /// Gets the device's logical width in pixels.
        /// </summary>
        public double DeviceLogicalPixelsWidth
        {
            get
            {
                return Window.Current.Bounds.Width;
            }
        }

        /// <summary>
        /// Gets the device's logical height in pixels.
        /// </summary>
        public double DeviceLogicalPixelsHeight
        {
            get
            {
                return Window.Current.Bounds.Height;
            }
        }

        private double GetResolution(double pixels, ResolutionScale rs)
        {
            return pixels * (int)rs;
        }

        #endregion
    }
}