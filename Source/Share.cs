using System;
using MonoTouch.UIKit;
using TheokLibrary;
using System.IO;
using MonoTouch.Foundation;

namespace Metallagmena
{
	public static class Share
	{
		public static UIActionSheet shareSheet;
		
		public static void ShareScreen ()
		{


			string fbOk = "";
			string twOk = "";

			if (shareSheet == null) {
				shareSheet = new UIActionSheet ("ΜΟΙΡΑΣΟΥ ΤΟ", null, "Ακυρωση", null, "Στο Facebook" + fbOk, "Στο Twitter" + twOk, "Με Email");
				shareSheet.Style = UIActionSheetStyle.Automatic;
			} else {
				shareSheet = null;
			}
			
			shareSheet.Clicked += HandleShareSheetClicked;
			shareSheet.ShowInView (Global.NavigationController.View);			

			
		}

		static void HandleShareSheetClicked (object sender, UIButtonEventArgs e)
		{
			
			if (e.ButtonIndex == 0) {
					
				ShareOnFacebook ();

			} else if (e.ButtonIndex == 1) {
				#region share on twitter

				string url = "http://oblapps.com/gmo";
				string twitterMessage = Utils.ShortAppDesc + " #iPhone εφαρμογη by @ObliviusApps " + url;
					

				string capturedPhoto = Capture ();
				System.Threading.Thread.Sleep (1000);
				UIImage img = UIImage.FromFile (capturedPhoto);


				Twitter.JustTweet (twitterMessage, Global.NavigationController, img, url, delegate(bool ok) {

					File.Delete (capturedPhoto);
					if (ok) {
						Global.WriteSetting ("HasShared", "1");
						Global.IncreaseCounter ("GMO.Share");
					}
					
				});	
				
				
				#endregion
			} else if (e.ButtonIndex == 2) {
					
				//share with email
				string emailMessage = Utils.EmailSignature;
				
				string subject = Utils.ShortAppDesc;


				
				string capturedPhoto = Capture ();
				


				Global.SendEmail (subject, emailMessage, true, Global.NavigationController, delegate(bool obj) {

					File.Delete (capturedPhoto);
					if (obj) {
						Global.WriteSetting ("HasShared", "1");
						Global.IncreaseCounter ("GMO.Share");
					}

				}, delegate(MonoTouch.MessageUI.MFMailComposeViewController obj) {

					NSData data = NSData.FromFile (capturedPhoto);
					obj.AddAttachmentData (data, "image/png", "picture.png");					

				});
				

					
					
			} 
			shareSheet = null;				
		}
		
		private static void ShareOnFacebook ()
		{

			string text = Utils.ShortAppDesc + " - iPhone εφαρμογη by Oblivius Apps\r\rhttp://oblapps.com/gmo";
			string capturedPhoto = Capture ();
			Facebook.NewFacebookPost (Global.NavigationController, text, capturedPhoto, null, null, delegate(MonoTouch.Social.SLComposeViewControllerResult obj) {

				File.Delete (capturedPhoto);
				if (obj == MonoTouch.Social.SLComposeViewControllerResult.Done) {
					Global.WriteSetting ("HasShared", "1");
					Global.IncreaseCounter ("GMO.Share");
				}

			});

		}

		private static string Capture ()
		{
			string fName = DateTime.Now.ToString ("ddMMyyHHmmssfff") + ".png";
			return Global.ScreenCapture (Global.NavigationController.View, fName);
		}
		
		
	}
}

