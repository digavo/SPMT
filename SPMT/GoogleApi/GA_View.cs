using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPMT
{
    class GA_View
    {
        private string DOTNET2HTML;

        public void dynmap_show(WebBrowser webB)
        {

            string m1, m2;

            DOTNET2HTML = "";
            DOTNET2HTML += "< !DOCTYPE html >" +
 "< html >" + "< head >" +
 @"< meta name = ""viewport"" content = ""initial-scale=1.0, user-scalable=no"" >" +
@"< meta charset = ""utf-8"" >" +
"< title > + Directions service </ title >" +
"< style > # map {height: 100 %;} html, body {height: 100%;margin: 0;padding: 0;}" +
"# floating-panel {position: absolute;top: 10px;left: 25%;z-index: 5; background-color: #fff;padding: 5px;border: 1px solid #999;text-align: center;" +
"font-family: 'Roboto','sans-serif'; line-height: 30px;padding-left: 10px;}</style>" +
"</head>" +
"<body>" +

 "</div>" + @"<div id=""map"">" + "</div>" + "<script>" +
"function initMap() {" +
"var directionsService = new google.maps.DirectionsService;" +
"var directionsDisplay = new google.maps.DirectionsRenderer;" +
"var map = new google.maps.Map(document.getElementById('map'), {" +
"zoom: 7," +
"center: { lat: 51.11, lng: 17.03}" +
"});" +
"directionsDisplay.setMap(map);" +
"var onChangeHandler = function() {" +
"calculateAndDisplayRoute(directionsService, directionsDisplay);" +
"};" +

"document.getElementById('start').addEventListener('change', onChangeHandler);" +
"document.getElementById('via').addEventListener('change', onChangeHandler);" +
"document.getElementById('via2').addEventListener('change', onChangeHandler);" +
"document.getElementById('end').addEventListener('change', onChangeHandler);" +
"}"+

"function calculateAndDisplayRoute(directionsService, directionsDisplay) {" +
"directionsService.route({" +
"origin: document.getElementById('start').value," +
"destination: document.getElementById('end').value," +
"waypoints: [{ location: document.getElementById('via').value }, { location: document.getElementById('via2').value }]," +
"travelMode: 'DRIVING'}," +
"function(response, status) {" +
"if (status === 'OK'){directionsDisplay.setDirections(response);}" +
"else{window.alert('Directions request failed due to ' + status);}" +
"});}" +
"</script>" +
@"<script async defer src = ""https://maps.googleapis.com/maps/api/js?key=AIzaSyC7Jv088sHc_qsjUtrPk5NpG4fqEYCK_ZQ&callback=initMap"">" +
"</script>" +
"</body>" +
"</html>";

            SetWebBrowserVersion(11001); // musi byc
            File.WriteAllText(@"HTMLPage1.html", DOTNET2HTML.ToString());
            //webB.Url = new Uri(String.Format(@"HTMLPage1.html"));
            string curDir = Directory.GetCurrentDirectory();
            webB.Url = new Uri(String.Format("file:///{0}/HTMLPage1.html", curDir));
        }



        /*
         * 
        private string AP_KEY;
        private string[] znaczniki;

        public void staticshowMAP(WebBrowser WB_MAP)
        {
            //StringBuilder SB = new StringBuilder("https://maps.googleapis.com/maps/api/staticmap?center=Brooklyn+Bridge,New+York,NY&zoom=13&size=600x300&maptype=roadmap&markers=color:blue|label:S|40.702147,-74.015794&markers=color:green|label:G|40.711614,-74.012318");

            String miejsce = "Brooklyn+Bridge,New+York,NY";
            String znacznik1 = "&markers = color:green | label:G | 40.711614,-74.012318";
            String znacznik2 = "&markers = color:blue  | label:S | 40.702147,-74.015794";


            StringBuilder SB = new StringBuilder("https://maps.googleapis.com/maps/api/staticmap?center=" + miejsce + "&zoom=13&size=600x300" + znacznik1 + znacznik2);


            //String punkt1 ="Wroclaw";// miasto  poczatkowe
            //String punkt2 = "Opole"; // miasto docelowe na razie tylko na pokaz by zobaczyc czy w aplikacji wyswoetla sie trasa
            //String typpojazdu = "/data=!4m2!4m1!3e0"; //wyznacza trase dlasamochodow 
            //StringBuilder SB = new StringBuilder("https://www.google.pl/maps?q=");add.Append(punkt1);add.Append(punkt2);
            //StringBuilder SB = new StringBuilder("https://www.google.pl/maps/dir/" + punkt1 + "/" + punkt2 + "@51.1270779,16.9918639,9z" + typpojazdu);

            SetWebBrowserVersion(11001); // musi byc
            WB_MAP.Navigate(SB.ToString()); // wyswietla trase pomiedzy pierwszym i ostatnim miaste ma liscie reszte miast pomija

        }
        */
        private void SetRegistryDword(string key_name, string value_name, int value)
        {
            // Open the key.
            RegistryKey key =
                Registry.CurrentUser.OpenSubKey(key_name, true);
            if (key == null)
                key = Registry.CurrentUser.CreateSubKey(key_name,
                    RegistryKeyPermissionCheck.ReadWriteSubTree);

            // Set the desired value.
            key.SetValue(value_name, value, RegistryValueKind.DWord);

            key.Close();
        }
        private void SetWebBrowserVersion(int ie_version)
        {
            const string key64bit =
                @"SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\" +
                @"MAIN\FeatureControl\FEATURE_BROWSER_EMULATION";
            const string key32bit =
                @"SOFTWARE\Microsoft\Internet Explorer\MAIN\" +
                @"FeatureControl\FEATURE_BROWSER_EMULATION";
            string app_name = System.AppDomain.CurrentDomain.FriendlyName;

            // You can do both if you like.
            SetRegistryDword(key64bit, app_name, ie_version);
            SetRegistryDword(key32bit, app_name, ie_version);
        }
    }
}
