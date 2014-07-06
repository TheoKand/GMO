
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using TheokLibrary;
using MonoTouch.Dialog;

namespace Metallagmena
{
	public partial class SettingsViewController : UIViewController
	{


		public string[] Fields = new string[4] {"Προϊον","Εταιρια","Κατηγορια","Σημανση"};
		public UITableView table;

		MainVC parent;
		GlassButton btnOK;
		TableDataSource tableDataSource;
		TableDelegate tableDelegate;

		public SettingsViewController (MainVC p)
		{
			this.parent = p;
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.BackgroundColor = UIColor.FromPatternImage (ImageUtils.FromBundle2 ("images/background.jpg"));

			btnOK = new GlassButton (View.Frame.CenterHorz (5, 150, 40));
			btnOK.SetTitle ("Επιστροφη", UIControlState.Normal);
			btnOK.Tapped += delegate(GlassButton obj) {
				DismissViewController (true, null);
				parent.Refresh ();
			};
			btnOK.Enabled = true;
			View.AddSubview (btnOK);


			table = new UITableView (new RectangleF (0, 50, View.Bounds.Width, View.Bounds.Height - 45), UITableViewStyle.Grouped);
			table.BackgroundView = null;
			tableDataSource = new TableDataSource (this);
			tableDelegate = new TableDelegate (this);
			table.DataSource = tableDataSource;
			table.Delegate = tableDelegate;
			table.ReloadData ();
			View.AddSubview (table);

			// Perform any additional setup after loading the view, typically from a nib.
		}
	}


	public class TableDataSource : UITableViewDataSource
	{
		SettingsViewController parent;
		
		public TableDataSource (SettingsViewController p)
		{
			this.parent = p;
		}
		
		public override int NumberOfSections (UITableView tableView)
		{
			return 3;
		}
		
		public override string TitleForHeader (UITableView tableView, int section)
		{
			if (section == 0)
				return "Ταξινομηση κατα...";
			else if (section == 1)
				return "Φιλτρο Κατηγοριας";
			else
				return "Φιλτρο Σημανσης";
		}
		
		public override int RowsInSection (UITableView tableView, int section)
		{
			if (section == 0)
				return parent.Fields.Length;
			else if (section == 1)
				return Data.Categories.Count + 1;
			else
				return Data.Ratings.Count + 1;
		}
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = new UITableViewCell (UITableViewCellStyle.Default, null);

			string text = "";

			if (indexPath.Section == 0) {
				text = parent.Fields [indexPath.Row];

				if (Utils.OrderBy == indexPath.Row)
					cell.Accessory = UITableViewCellAccessory.Checkmark;
				else
					cell.Accessory = UITableViewCellAccessory.None;


			} else if (indexPath.Section == 1) {
				if (indexPath.Row == 0)
					text = "ΟΛΕΣ";
				else
					text = Data.Categories [indexPath.Row - 1].DESC;

				if (Utils.CategoryFilter == indexPath.Row)
					cell.Accessory = UITableViewCellAccessory.Checkmark;
				else
					cell.Accessory = UITableViewCellAccessory.None;


			} else {
				if (indexPath.Row == 0)
					text = "ΟΛΕΣ";
				else
					text = Data.Ratings [indexPath.Row - 1].DESC;

				if (Utils.RatingFilter == indexPath.Row)
					cell.Accessory = UITableViewCellAccessory.Checkmark;
				else
					cell.Accessory = UITableViewCellAccessory.None;

				cell.BackgroundColor = Data.GetRatingColor (indexPath.Row - 1);

			}


			cell.TextLabel.Text = text;
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;



			return cell;				
			
			
		}

	}
	
	class TableDelegate : UITableViewDelegate
	{
		SettingsViewController parent;
		
		public TableDelegate (SettingsViewController OrderViewController)
		{
			this.parent = OrderViewController;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Section == 0) {

				UITableViewCell prevCell = tableView.CellAt (NSIndexPath.FromRowSection (Utils.OrderBy, indexPath.Section));
				if (prevCell != null)
					prevCell.Accessory = UITableViewCellAccessory.None;

				tableView.CellAt (indexPath).Accessory = UITableViewCellAccessory.Checkmark;
				Utils.OrderBy = indexPath.Row;

			} else if (indexPath.Section == 1) {

				UITableViewCell prevCell = tableView.CellAt (NSIndexPath.FromRowSection (Utils.CategoryFilter, indexPath.Section));
				if (prevCell != null)
					prevCell.Accessory = UITableViewCellAccessory.None;

				tableView.CellAt (indexPath).Accessory = UITableViewCellAccessory.Checkmark;
				Utils.CategoryFilter = indexPath.Row;


			} else {

				UITableViewCell prevCell = tableView.CellAt (NSIndexPath.FromRowSection (Utils.RatingFilter, indexPath.Section));
				if (prevCell != null)
					prevCell.Accessory = UITableViewCellAccessory.None;

				tableView.CellAt (indexPath).Accessory = UITableViewCellAccessory.Checkmark;
				Utils.RatingFilter = indexPath.Row;


			}

		}

		public override UIView GetViewForHeader (UITableView tableView, int section)
		{
			string title;

			if (section == 0)
				title = "  Ταξινομηση κατα...";
			else if (section == 1)
				title = "  Φιλτρο Κατηγοριας";
			else
				title = "  Φιλτρο Σημανσης";


			UILabel lbl = Global.Label (title, true, new RectangleF (20, 0, 400, 40), UIColor.White, UIColor.DarkGray, 15);
			return lbl;
		}

		
	}	
}

