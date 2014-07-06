using System;
using System.Linq;
using TheokLibrary;

namespace Metallagmena
{
	public static class Utils
	{
		
		public static string FacebookAppClientId = "527204330656440";
		public static string ShortAppDesc = "\"Κατάλογος Μεταλλαγμένων Προϊόντων\"";
		public static string EmailSignature = @"
			<a href='http://oblapps.com/gmo'><img src='http://www.oblapps.com/gmo/images/icon.png' border='0'></a><BR><BR>
			<font color=gray><B><a href='http://oblapps.com/gmo'>Κατάλογος Μεταλλαγμένων Προϊόντων</a></b> by <B><a href='http://www.oblapps.com/'>Oblivius Apps</a></b><BR><BR>" +
			@"<B><font color=black>""Εφαρμογη iPhone""</B></font><BR>";		
		

		public static string TextFilter {
			get {
				return Global.ReadSetting ("TextFilter", "");	
			}
			set {
				Global.WriteSetting ("TextFilter", value);	
			}
		}


		public static int OrderBy {
			get {
				return int.Parse (Global.ReadSetting ("OrderBy", "3"));	
			}
			set {
				Global.WriteSetting ("OrderBy", value.ToString ());	
			}
		}

		public static int CategoryFilter {
			get {
				return int.Parse (Global.ReadSetting ("CategoryFilter", "0"));	
			}
			set {
				Global.WriteSetting ("CategoryFilter", value.ToString ());	
			}
		}

		public static int RatingFilter {
			get {
				return int.Parse (Global.ReadSetting ("RatingFilter", "0"));	
			}
			set {
				Global.WriteSetting ("RatingFilter", value.ToString ());	
			}
		}

		public static string GetMacAddress ()
		{
			string macVal;
			macVal = (from i in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces () 
			        where i.Id.Equals ("en0") 
			        orderby i.Id ascending 
			        select string.Join (":", i.GetPhysicalAddress ().GetAddressBytes ().Select (x => x.ToString ("X")))).FirstOrDefault (); 			
			
			return macVal;
			
		}
		
	}
}

