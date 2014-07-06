
using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using TheokLibrary;
using MonoTouch.Dialog;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Metallagmena
{
	public class MyActionSheet : SuperActionSheetWithBanners
	{

		public override void AddButtons ()
		{
			bool tall = UIDevice.CurrentDevice.IsTall ();
			AddSpace (tall ? 20 : 8);

			AddButton ("Πληροφορίες", UIColor.LightGray, false);
			AddButton ("Κριτικές", UIColor.LightGray, false);
			
			AddSpace (tall ? 24 : 0);
			AddButton ("Σχετικά με εμάς", UIColor.Gray, false);

			AddSpace (tall ? 24 : 0);
			
			AddButton ("Επιστροφή", UIColor.Red, false);


			PrepareBanners ();

		}

		public override void HandleBtnTouchUpInside (object sender, EventArgs e)
		{
			string title = (sender as UIButton).Title (UIControlState.Normal);
			
			if (title == "Πληροφορίες") {
				
				WebViewController.OpenUrl (parent, "http://oblapps.com/gmo/about.html");
				
			} else if (title == "Κριτικές") {
				
				WebViewController.OpenUrl (parent, "http://oblapps.com/gmo/ReviewsFeed.aspx", true);
				
			} else if (title == "Σχετικά με εμάς") {
				
				WebViewController.OpenUrl (parent, "http://obliviusapps.wordpress.com/hireus");
			} else {
				Hide (false);
			}
		}

		
	}
}

