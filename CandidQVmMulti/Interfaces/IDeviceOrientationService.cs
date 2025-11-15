#if ANDROID
using Android.Content.PM;
#endif
namespace CandidQVmMulti.Interfaces
{

    public interface IDeviceOrientationService
    {
#if ANDROID
        void LockOrientationPortrait(ScreenOrientation orientation = ScreenOrientation.Landscape);
        void UnlockOrientation();
#endif
    }

}
