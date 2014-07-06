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
	public class Product
	{
		public string ProductName;
		public string Company;
		public int Category;
		public int Rating;

		public Product (string productName, string company, int category, int rating)
		{
			this.ProductName = productName;
			this.Company = company;
			this.Category = category;
			this.Rating = rating;
		}

		public UIColor Color {
			get {

				return Data.GetRatingColor (Rating);
			}
		}

		public Lookup CategoryObject {
			get {
				return Data.Categories.Find (c => c.ID == this.Category);
			}
		}
		public Lookup RatingObject {
			get {
				return Data.Ratings.Find (c => c.ID == this.Rating);
			}
		}


	}

	public class Lookup
	{
		public int ID;
		public string DESC;

		public Lookup (int id, string desc)
		{
			this.ID = id;
			this.DESC = desc;
		}
	}

	public class Data
	{

		public static UIColor GetRatingColor (int rating)
		{

			switch (rating) {
			case 0:
				return UIColor.FromRGB (200, 0, 0);
			case 1:
				return UIColor.Red;
			case 2:
				return UIColor.Orange;
			case 3:
				return UIColor.FromRGB (0, 200, 0);
			case 4:
				return UIColor.Green;
			default:
				return UIColor.White;
				
			}
		}

		public static string GetRatingDesc (int rating)
		{
			
			if (rating == 0)
				return "SOS\r\rΜΕΤΑΛΛΑΓΜΕΝΟ ΠΡΟΙΟΝ";
			else if (rating == 1)
				return "ΚΟΚΚΙΝΟ\r\rTα προϊόντα αυτά προέρχονται ή είναι πιθανό να προέρχονται από ζώα που έχουν τραφεί µε µεταλλαγµένους οργανισμούς. Ειναι επισης προϊόντα εταιριών που έχουν δώσει ασαφείς απαντήσεις και προϊόντα εταιριών που δεν έχουν απαντήσει στο σχετικό ερωτηματολόγιο.";
			else if (rating == 2)
				return "ΠΟΡΤΟΚΑΛΙ\r\rΓια τα προϊόντα αυτά οι εταιρίες παραγωγής ήδη αναζητούν τρόπους για να εξασφαλίσουν ζωοτροφές απαλλαγμένες από µεταλλαγµένους οργανισμούς και έχουν δεσμευτεί εγγράφως ότι στο άμεσο μέλλον θα “καθαρίσουν” το σύνολο των ζωοτροφών από µεταλλαγµένα συστατικά.";
			else if (rating == 3)
				return "ΠΡΑΣΙΝΟ\r\rΓια τα προϊόντα αυτά οι εταιρίες παραγωγής εγγυώνται ότι προέρχονται από ζώα που δεν έχουν τραφεί µε µεταλλαγµένους οργανισμούς.";
			else if (rating == 4)
				return "ΠΡΑΣΙΝΟ ΜΕ ΤΟΝΟ\r\rΓια τα προϊόντα αυτά η Greenpeace έχει πραγματοποιήσει έλεγχο των πιστοποιητικών που διαθέτουν οι παραγωγοί από τους προμηθευτές τους για τη χρήση µη µεταλλαγµένων οργανισμών στις ζωοτροφές.";
			return "";

		}

		public static List<Lookup> Categories = new List<Lookup> () {
			new Lookup(1,	"Αλλαντικα"),
			new Lookup(2,	"Αυγά - Κοτοπουλα"),
			new Lookup(3,	"Γαλακτοκομικα"),
			new Lookup(4,	"Γιαουρτια – Επιδορπια"),
			new Lookup(5,	"Ζυμαρικα και σαλτσες ζυμαρικων"),
			new Lookup(6,	"Ιχθυοτροφεια"),
			new Lookup(7,	"Κονσερβες"),
			new Lookup(8,	"Παγωτα"),
			new Lookup(9,	"Σαλατες – Σαλτσες – Σουπες"),
			new Lookup(10,	"Σπορελαια – Μαργαρινες"),
			new Lookup(11,	"Τσιπς – Σνακς"),
			new Lookup(12,	"Ψαρια")
		};
		public static List<Lookup> Ratings = new List<Lookup> () {
			new Lookup(0,	"SOS"),
			new Lookup(1,	"ΚΟΚΚΙΝΟ"),
			new Lookup(2,	"ΠΟΡΤΟΚΑΛΙ"),
			new Lookup(3,	"ΠΡΑΣΙΝΟ"),
			new Lookup(4,	"ΠΡΑΣΙΝΟ ΜΕ ΤΟΝΟ")
		};

		private static List<Product> initialProducts;
		private static List<Product> currentProducts;



		public static void Reset ()
		{
			currentProducts = null;
		}

		public static List<Product> Products {
			get {
				if (initialProducts == null) {

					initialProducts = new List<Product> ();

					#region create database from CSV file
					string csvFile = Path.Combine (Environment.CurrentDirectory, "Foods.csv");
					string[] lines = System.IO.File.ReadAllLines (csvFile);

					for (int i=1; i<lines.Length; i++) {

						string[] tokens = lines [i].Split (';');


						string productName = tokens [0];
						string company = tokens [1];
						int category = int.Parse (tokens [2]);
						int rating = int.Parse (tokens [3]);

//						if (rating < 3) {
//							productName = "??";
//							company = "??";
//						}

						initialProducts.Add (new Product (productName, company, category, rating));

					}

					#endregion

				} 

				if (currentProducts == null) {
					currentProducts = initialProducts;
					ApplyFilterAndSort (ref currentProducts);
				} 

				return currentProducts;

			}
		}

		private static void ApplyFilterAndSort (ref List<Product> list)
		{

			if (!string.IsNullOrWhiteSpace (Utils.TextFilter)) {
				list = list.FindAll (p => p.ProductName.ToLower ().Contains (Utils.TextFilter) || p.CategoryObject.DESC.ToLower ().Contains (Utils.TextFilter));
			}
			
			if (Utils.CategoryFilter != 0) {
				list = list.FindAll (p => p.Category == Utils.CategoryFilter);
			}
			
			if (Utils.RatingFilter != 0) {
				list = list.FindAll (p => p.Rating == Utils.RatingFilter - 1);
			}

			switch (Utils.OrderBy) {
			case 0:
				list = list.OrderBy (p => p.ProductName).ToList ();
				break;
			case 1:
				list = list.OrderBy (p => p.Company).ToList ();
				break;
			case 2:
				list = list.OrderBy (p => p.Category).ToList ();
				break;
			case 3:
				list = list.OrderByDescending (p => p.Rating).ToList ();
				break;
			}


		}



	}
}

