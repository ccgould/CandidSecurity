using Android.Content.PM;
using CandidQVmMulti.Interfaces;

[assembly: Dependency(typeof(CandidQVmMulti.Platforms.Android.DeviceOrientationService))]
namespace CandidQVmMulti.Platforms.Android
{
    public class DeviceOrientationService : IDeviceOrientationService
    {
        public void LockOrientationPortrait(ScreenOrientation orientation = ScreenOrientation.Landscape)
        {
            var activity = Platform.CurrentActivity;
            activity.RequestedOrientation = orientation;
        }

        public void UnlockOrientation()
        {
            var activity = Platform.CurrentActivity;
            activity.RequestedOrientation = ScreenOrientation.Unspecified;


        }
    }
}
