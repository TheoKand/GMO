using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using TheokLibrary;

namespace Metallagmena
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		MainVC mainVC;

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			#region twitter initialize
			TwitterAccount.OAuthConfig = new OAuthConfig () {
				ConsumerKey = "wu2XtNIqnGxFXacBa9b7Q",
				ConsumerSecret = "tYUGE52t5ByFM8GQYh74r4gA7lHVHz1qrofb6ehk44",
				RequestTokenUrl = "https://api.twitter.com/oauth/request_token", 
				AuthorizeUrl = "https://api.twitter.com/oauth/authorize",			
				AccessTokenUrl = "https://api.twitter.com/oauth/access_token", 
				Callback = ""
			};			
			#endregion				

			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			mainVC = new MainVC ();
			Global.NavigationController = new UINavigationController (mainVC);
			Global.NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
			Global.NavigationController.SetNavigationBarHidden (true, false);
			Global.NavigationController.SetToolbarHidden (true, false);			
			window.RootViewController = Global.NavigationController;
			
			
			// make the window visible
			window.MakeKeyAndVisible ();
			
			
			
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations (UIApplication application, UIWindow forWindow)
		{
			return UIInterfaceOrientationMask.Portrait | UIInterfaceOrientationMask.PortraitUpsideDown;
		}

		public override void OnActivated (UIApplication application)
		{
			ExecuteLater.Later (500, false, () => ObliviusWS ());
			ExecuteLater.Later (500, false, () => MyActionSheet.Prepare ("metallagmena"));
		}
		
		private bool RateOk = false;
		private void ObliviusWS ()
		{
			try {
				if (Oblivius.Instance == null) {
					
					Oblivius.Instantiate ("Metallagmena", "ObliviusApps", "619112269", "http://oblapps.com/gmo/itunes");			
				} 
				
				Oblivius.Instance.AppUsageCounter ++;
				DateTime lastObliviusCheckTime = DateTime.ParseExact (Global.ReadSetting ("lastObliviusCheckTime", DateTime.MinValue.ToString ("ddMMyyyyHHmm")), "ddMMyyyyHHmm", null);
				if ((DateTime.Now - lastObliviusCheckTime).TotalMinutes < 10)
					return;
				
				Global.WriteSetting ("lastObliviusCheckTime", DateTime.Now.ToString ("ddMMyyyyHHmm"));
				
				if (Oblivius.Ready && Oblivius.Instance.AppUsageCounter > 1) {
					if (!Oblivius.Instance.ShowVersionMessageIfNew ()) {
						if (!Oblivius.Instance.ShowPopupMessageIfNew (Global.NavigationController)) {

							if (!RateOk && Oblivius.Instance.DaysSinceThisVersionInstall >= 2) {
								RateOk = true;
								Oblivius.Instance.AskForReviewIfRequired ();
							} 
						}
						
					}
					
				}
			} catch {
				Global.PopNetworkActive (true);	
			}
			
			
		}	
	}
}

