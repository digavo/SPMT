using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPMT
{
    class GA_View
    {
        private string DOTNET2HTML;

        void dynmap_init()
        {
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
"center: { lat: 41.85, lng: -87.65}" +
"});" +
"directionsDisplay.setMap(map);" +
"var onChangeHandler = function() {" +
"calculateAndDisplayRoute(directionsService, directionsDisplay);" +
"};"+





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
        */
    }
}
