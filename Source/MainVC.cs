using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using TheokLibrary;
using MonoTouch.CoreLocation;
using System.Text;
using System.Collections.Generic;
using System.IO;
using MonoTouch.MapKit;
using System.Text.RegularExpressions;
using System.Linq;
using System.Runtime.Serialization;
using MonoTouch.Dialog;

namespace Metallagmena
{

	public class MainVC: UIViewController2
	{
		UIButton btnShare;
		UIButton btnAction;
		UISearchBar searchBar;
		GlassButton btnSortFilter;
		GlassButton btnReset;
		UITableView table;
		ProductsDataSource dataSource;
		ProductsTableDelegate tableDelegate;
		ViewAnimator animator;
		TappableImageView panel;
		UILabel lblProduct;
		UILabel lblCategory;
		UILabel lblCompany;
		UITextView txtLegend;
		UIImageView imgCheck;
		MyActionSheet sheetActions;
		SettingsViewController settingsVC;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Global.SetStatusBarHidden (true, false);

			if (Global.ReadSetting ("FirstOpen", "1") == "1") {
				Global.IncreaseCounter ("GMO.FirstOpen", () => Global.WriteSetting ("FirstOpen", "0"));
			}


			bool tall = UIDevice.CurrentDevice.IsTall ();
			
			View.BackgroundColor = UIColor.FromPatternImage (ImageUtils.FromBundle2 ("images/main.jpg"));

			#region init controls

			btnAction = UIButton.FromType (UIButtonType.InfoLight);
			btnAction.Frame = new RectangleF (5, View.Bounds.Height - 35 - 100, 30, 30);
			View.AddSubview (btnAction);
			
			btnShare = UIButton.FromType (UIButtonType.Custom);
			btnShare.Frame = new RectangleF (View.Frame.Width - 45, 2, 35, 35);
			btnShare.SetBackgroundImage (ImageUtils.FromBundle2 ("images/shout.png"), UIControlState.Normal);
			btnShare.TouchUpInside += delegate {
				Shout ();
			};
			View.AddSubview (btnShare);

			searchBar = new UISearchBar ();

			searchBar.Opaque=true;
			searchBar.SearchBarStyle = UISearchBarStyle.Minimal;


			searchBar.Text = Utils.TextFilter;
			searchBar.Frame = new RectangleF (8, btnShare.Frame.Bottom + 2, View.Bounds.Width - 16, 40);
			searchBar.BackgroundColor = UIColor.Clear;
			View.AddSubview (searchBar);

			searchBar.SearchButtonClicked += delegate {
				searchBar.EndEditing (true);
				Utils.TextFilter = searchBar.Text.ToLower ();
				btnReset.Enabled = true;
				Data.Reset ();
				table.ReloadData ();
			};
			
			searchBar.TextChanged += delegate {
				
				if (String.IsNullOrWhiteSpace (searchBar.Text) || searchBar.Text.Trim ().Length > 1) {
					Utils.TextFilter = searchBar.Text.ToLower ();

					btnReset.Enabled = true;

					Data.Reset ();
					table.ReloadData ();
				}
				
			};


			table = new UITableView (new RectangleF (8, searchBar.Frame.Bottom - 2, View.Bounds.Width - 16, View.Bounds.Height - searchBar.Frame.Bottom - 35 - 100), UITableViewStyle.Grouped);
			table.Opaque = true;
			table.BackgroundColor = UIColor.Clear;
			table.BackgroundView = null;
			dataSource = new ProductsDataSource (this);
			tableDelegate = new ProductsTableDelegate (this);
			table.DataSource = dataSource;
			table.Delegate = tableDelegate;
			table.ReloadData ();
			View.AddSubview (table);

			UIButton btnLogo = new UIButton (UIButtonType.Custom);
			btnLogo.Frame = new RectangleF (0, 0, 120, 35);
			btnLogo.TouchUpInside += delegate {
				TheokLibrary.WebViewController.OpenUrl (this, "http://oblapps.com");
			};
			View.AddSubview (btnLogo);


			btnSortFilter = new GlassButton (new RectangleF (86, View.Bounds.Height - 33 - 100, 100, 28));
			btnSortFilter.Font = UIFont.SystemFontOfSize (UIFont.SmallSystemFontSize);
			btnSortFilter.SetTitle ("Προσαρμογη", UIControlState.Normal);
			btnSortFilter.Tapped += btnSettingsTapped;
			btnSortFilter.Enabled = true;
			View.AddSubview (btnSortFilter);

			btnReset = new GlassButton (btnSortFilter.Frame.NextTo (100, 28, 10));
			btnReset.NormalColor = UIColor.Red;
			btnReset.Font = UIFont.SystemFontOfSize (UIFont.SmallSystemFontSize);
			btnReset.SetTitle ("Επαναφορα", UIControlState.Normal);
			btnReset.Tapped += btnResetTapped;
			btnReset.Enabled = false;
			View.AddSubview (btnReset);



			panel = new TappableImageView (ImageUtils.FromBundle2 ("images/panel"));
			panel.Frame = panel.Frame.Move (20, 76);
			panel.Tapped = PanelTapped;
			View.AddSubview (panel);		

			lblProduct = Global.Label ("", true, new RectangleF (panel.Frame.Left + 15, panel.Frame.Top + (tall ? 30 : 10), 250, 80), UIColor.White, UIColor.Black, 19);
			lblProduct.LineBreakMode = UILineBreakMode.WordWrap;
			lblProduct.Lines = 3;
			View.AddSubview (lblProduct);

			lblCompany = Global.Label ("", false, lblProduct.Frame.Below (View.Frame.Width - 90, tall ? 30 : 25, 0), UIColor.LightGray, UIColor.Black, 17);
			View.AddSubview (lblCompany);

			lblCategory = Global.Label ("", false, lblCompany.Frame.Below (View.Frame.Width - 90, tall ? 30 : 25, 0), UIColor.White, 16);
			View.AddSubview (lblCategory);

			txtLegend = new UITextView (lblCategory.Frame.Below (View.Frame.Width - 90, View.Frame.Height - (tall ? 360 : 280), 8).Move (7, tall ? 45 : 0));
			txtLegend.ClipsToBounds = true;
			txtLegend.Editable = false;
			txtLegend.Font = UIFont.SystemFontOfSize (14);
			txtLegend.Layer.CornerRadius = 13.0f;
			txtLegend.ScrollEnabled = true;

			View.AddSubview (txtLegend);

			imgCheck = new UIImageView (ImageUtils.FromBundle2 ("images/check"));
			imgCheck.Frame = imgCheck.Frame.Reposition (txtLegend.Frame.Right - imgCheck.Frame.Width, txtLegend.Frame.Top - imgCheck.Frame.Height / 2);
			imgCheck.Alpha = 0;
			View.AddSubview (imgCheck);

			TappableImageView vote = new TappableImageView (ImageUtils.FromBundle2 ("images/vote.png"));
			vote.Frame = new RectangleF (0, View.Bounds.Height - 103, 320, 103);
			vote.Tapped = delegate(object sender, EventArgs e) {
				Global.OpenLink (Global.NavigationController, "itms://itunes.apple.com/gr/app/id623625700");
			};
			View.AddSubview (vote);

			sheetActions = new MyActionSheet ();
			sheetActions.parent = this;
			sheetActions.Frame = new RectangleF (0, UIScreen.MainScreen.Bounds.Height, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
			View.AddSubview (sheetActions);			
			View.BringSubviewToFront (sheetActions);

			#endregion




			#region animator
			animator = new ViewAnimator ();
			animator.AnimationDuration = 0.4f;
			animator.AddToVisibleGroup (table, ViewAnimator.WhereToMove.Bottom);
			animator.AddToVisibleGroup (btnReset, ViewAnimator.WhereToMove.Bottom);
			animator.AddToVisibleGroup (btnSortFilter, ViewAnimator.WhereToMove.Bottom);
			animator.AddToVisibleGroup (searchBar, ViewAnimator.WhereToMove.Bottom);
			animator.AddToVisibleGroup (btnAction, ViewAnimator.WhereToMove.Bottom);
			animator.AddToVisibleGroup (vote, ViewAnimator.WhereToMove.Bottom);


			animator.AddToHiddenGroup (panel, ViewAnimator.WhereToMove.Bottom);
			animator.AddToHiddenGroup (lblProduct, ViewAnimator.WhereToMove.Bottom);
			animator.AddToHiddenGroup (lblCompany, ViewAnimator.WhereToMove.Bottom);
			animator.AddToHiddenGroup (lblCategory, ViewAnimator.WhereToMove.Bottom);
			animator.AddToHiddenGroup (txtLegend, ViewAnimator.WhereToMove.Bottom);


			#endregion

			UpdateResetButton ();
			
			#region events
			btnAction.TouchUpInside += delegate {
				Menu ();
			};
			#endregion


			#region temp code



			#endregion
			//InputBox.Instance.ShowMessage ("Έχουν αφαιρεθεί όλα τα προϊόντα με αρνητική σήμανση! Εκπρόσωποι απο διάφορες εταιρίες τροφίμων μας ενημέρωσαν πως η λίστα αυτή, παρότι κυκλοφορεί ευρέως στο διαδίκτυο από όπου την πήραμε και εμείς,  περιέχει αρκετές ανακρίβειες και δεν είναι πρόσφατα επικαιροποιημένη.");

			
		}


		public void PanelTapped (object sender, EventArgs e)
		{
			Toggle ();
		}

		public void Toggle ()
		{
			Toggle (null);
		}

		private int counter;
		public void Toggle (Product prd)
		{
			imgCheck.Alpha = 0;

			if (prd != null) {

				lblProduct.Text = prd.ProductName;
				lblCompany.Text = prd.Company;
				lblCategory.Text = prd.CategoryObject.DESC;
				txtLegend.BackgroundColor = Data.GetRatingColor (prd.Rating);
				txtLegend.Text = Data.GetRatingDesc (prd.Rating);

				if (prd.Rating == 4) {
					ExecuteLater.Later (100, delegate {
						FadeHelper.Instance.AnimationDuration = 1.0f;
						FadeHelper.Instance.FadeView ("showCheck", imgCheck, false, null, false);
					});
				}

				counter++;

				bool showShare = false;

				if (Global.ReadSetting ("HasShared", "0") == "0")
					showShare = true;
				else  
					showShare = ((counter % 4) == 0);
				
				if (showShare) {
					
					ExecuteLater.Later (1000, "sharefunky", delegate() {
						View.FunkyMessage ("Share--->", UIColor.White, btnShare.Frame.MoveAndChange (-135, 0, 130, 40), 30);	
						
					});				
				}


			} 

			animator.NewToggle (Global.Animation, ViewAnimator.WhereToMove.Right, ViewAnimator.WhereToMove.Right);

		}

		void btnResetTapped (GlassButton obj)
		{
			Utils.CategoryFilter = 0;
			Utils.RatingFilter = 0;
			Utils.TextFilter = "";
			Data.Reset ();
			settingsVC.table.ReloadData ();
			table.ReloadData ();

			btnReset.Enabled = false;
		}


		void btnSettingsTapped (GlassButton obj)
		{
		
			if (settingsVC == null) 
				settingsVC = new SettingsViewController (this);

			Global.NavigationController.PresentViewController (settingsVC, true, null);

		}

		public void Refresh ()
		{
			UpdateResetButton ();
			Data.Reset ();
			table.ReloadData ();
			if (table.DataSource.RowsInSection (table, 0) > 0)
				table.ScrollToRow (NSIndexPath.FromRowSection (0, 0), UITableViewScrollPosition.Top, false);

		}

		private void UpdateResetButton ()
		{

			if (Utils.RatingFilter != 0 || Utils.TextFilter != "" || Utils.CategoryFilter != 0) 
				btnReset.Enabled = true;

		}


		private void Shout ()
		{
			Share.ShareScreen ();
		}
		
		private void Menu ()
		{
			sheetActions.ShowInView (View);
		

		}
		

		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			sheetActions.Hide (true);
			Global.NavigationController.SetNavigationBarHidden (true, true);
			
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);






		}

		public class ProductsDataSource : UITableViewDataSource
		{
			MainVC parent;

			public ProductsDataSource (MainVC p)
			{
				this.parent = p;
			}
			
			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
			}



			public override int RowsInSection (UITableView tableView, int section)
			{
				return Data.Products.Count;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{

				Product prd = Data.Products [indexPath.Row];

				var cell = new UITableViewCell (UITableViewCellStyle.Default, null);
				cell.BackgroundColor =  UIColor.Clear;
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;

				UILabel lblProduct = Global.Label ("", true, new RectangleF (14, 0, tableView.Frame.Width - 84, 25), UIColor.Black, UIColor.DarkGray, 16);	
				lblProduct.TextColor = UIColor.Black;
				lblProduct.AdjustsFontSizeToFitWidth = true;
				lblProduct.Text = prd.ProductName;
				cell.AddSubview (lblProduct);

				UILabel lblCompany = Global.Label ("", false, new RectangleF (14, 18, 120, 20), UIColor.White, UIColor.DarkGray, 12);
				lblCompany.Text = prd.Company;
				cell.AddSubview (lblCompany);					

				UILabel lblCategory = Global.Label ("", false, new RectangleF (lblCompany.Frame.Right, 18, tableView.Frame.Width - lblCompany.Frame.Right - 40, 20), UIColor.Black, 12);
				lblCategory.Text = prd.CategoryObject.DESC;
				cell.AddSubview (lblCategory);					

				cell.BackgroundView = new UIImageView (ImageUtils.FromBundle2 ("images/cell" + prd.Rating + ".png"));


				return cell;				
				
				
			}
			
			
		}

		class ProductsTableDelegate : UITableViewDelegate
		{
			MainVC parent;

			public ProductsTableDelegate (MainVC parent)
			{
				this.parent = parent;
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				Product prd = Data.Products [indexPath.Row];

				parent.Toggle (prd);

			}
			

			
			
		}		
		
	
	
		
	}
}

