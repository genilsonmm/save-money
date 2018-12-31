
using Android.App;
using Android.Widget;
using SaveMoney.Droid.Util;
using SaveMoney.Util;

[assembly: Xamarin.Forms.Dependency(typeof(ToastMessage))]
namespace SaveMoney.Droid.Util
{
    public class ToastMessage : IToastMessage
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}