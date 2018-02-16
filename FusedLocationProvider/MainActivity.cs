using System;
using Android.App;
using Android.OS;
using Android.Gms.Location;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Util;
using Android.Widget;
using Android.Locations;
using Android.Content;
using Android.Telephony;
using System.IO;
using System.Net;
using Android.Net;

namespace FusedLocationProvider
{
	[Activity (Label = "Captura Dinâmica de Posição", MainLauncher = true)]
	public class MainActivity : Activity, GoogleApiClient.IConnectionCallbacks,
	    GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener 
	{
		GoogleApiClient apiClient;
		LocationRequest locRequest;
		Button button;
		TextView latitude;
		TextView longitude;
		TextView provider;
        TextView txt_counter;
        TextView txt_lastupdate;
        TextView txt_error;


        //https://forums.xamarin.com/discussion/54272/how-to-get-the-imei-number


        int count = 0;

        //bool upload_active = false;

        bool _isGooglePlayServicesInstalled;


		////Lifecycle methods

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Log.Debug ("OnCreate", "OnCreate called, initializing views...");

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// UI to print last location
			button = FindViewById<Button> (Resource.Id.myButton);
			latitude = FindViewById<TextView> (Resource.Id.latitude);
			longitude = FindViewById<TextView> (Resource.Id.longitude);
			provider = FindViewById<TextView> (Resource.Id.provider);

            // UI to print status of system
            txt_counter = FindViewById<TextView>(Resource.Id.txt_contador);
            txt_lastupdate = FindViewById<TextView>(Resource.Id.txt_lastupdate);
            txt_error = FindViewById<TextView>(Resource.Id.txt_error);

            _isGooglePlayServicesInstalled = IsGooglePlayServicesInstalled ();

			if (_isGooglePlayServicesInstalled) {
				// pass in the Context, ConnectionListener and ConnectionFailedListener
				apiClient = new GoogleApiClient.Builder (this, this, this)
					.AddApi (LocationServices.API).Build ();

				// generate a location request that we will pass into a call for location updates
				locRequest = new LocationRequest ();
                apiClient.Connect();
                locRequest.SetFastestInterval(500);
                locRequest.SetInterval(1000);
                CountDown();



            } else {
				Log.Error ("OnCreate", "Google Play Services is not installed");
				//Toast.MakeText (this, "Google Play Services is not installed", ToastLength.Long).Show ();
				Finish ();
			}

		}

		bool IsGooglePlayServicesInstalled()
		{
			int queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable (this);
			if (queryResult == ConnectionResult.Success)
			{
				Log.Info ("MainActivity", "Google Play Services is installed on this device.");
				return true;
			}

			if (GoogleApiAvailability.Instance.IsUserResolvableError (queryResult))
			{
				string errorString = GoogleApiAvailability.Instance.GetErrorString (queryResult);
				Log.Error ("ManActivity", "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);

				// Show error dialog to let user debug google play services
			}
			return false;
		}

        /*
		protected override void OnResume()
		{
			base.OnResume ();
			Log.Debug ("OnResume", "OnResume called, connecting to client...");

			apiClient.Connect();

			// Clicking the first button will make a one-time call to get the user's last location
			button.Click += async delegate {
				if (apiClient.IsConnected)
				{
					button.Text = "Iniciar captura de dados";
                    // Setting location priority to PRIORITY_HIGH_ACCURACY (100)
                    locRequest.SetPriority(100);

                    // Setting interval between updates, in milliseconds
                    // NOTE: the default FastestInterval is 1 minute. If you want to receive location updates more than 
                    // once a minute, you _must_ also change the FastestInterval to be less than or equal to your Interval
                    locRequest.SetFastestInterval(500);
                    locRequest.SetInterval(1000);

                    Log.Debug("LocationRequest", "Request priority set to status code {0}, interval set to {1} ms",
                        locRequest.Priority.ToString(), locRequest.Interval.ToString());

                    // pass in a location request and LocationListener
                    await LocationServices.FusedLocationApi.RequestLocationUpdates(apiClient, locRequest, this);
                    // In OnLocationChanged (below), we will make calls to update the UI
                    // with the new location data



                    Location location = LocationServices.FusedLocationApi.GetLastLocation (apiClient);
					if (location != null)
					{
						//latitude.Text = "Latitude: " + location.Latitude.ToString();
						//longitude.Text = "Longitude: " + location.Longitude.ToString();
						//provider.Text = "Provider: " + location.Provider.ToString();
      //                  txt_counter.Text = "Number of observations: " + count.ToString();
      //                  txt_lastupdate.Text = "Last observation: " + DateTime.Now.ToString();
                        Log.Debug ("LocationClient", "Last location printed");
                        SavetoSd(location.Latitude.ToString(), location.Longitude.ToString());
                        CountDown();
                        
                    }
				}
				else
				{
					Log.Info ("LocationClient", "Please wait for client to connect");
				}
			};

			// Clicking the second button will send a request for continuous updates
		}
        */
        /*
		protected override void OnPause ()
		{
			base.OnPause ();
			Log.Debug ("OnPause", "OnPause called, stopping location updates");

            apiClient.Connect();

            // Clicking the first button will make a one-time call to get the user's last location
            button.Click += async delegate {
                if (apiClient.IsConnected)
                {
                    button.Text = "Iniciar captura de dados";
                    // Setting location priority to PRIORITY_HIGH_ACCURACY (100)
                    locRequest.SetPriority(100);

                    // Setting interval between updates, in milliseconds
                    // NOTE: the default FastestInterval is 1 minute. If you want to receive location updates more than 
                    // once a minute, you _must_ also change the FastestInterval to be less than or equal to your Interval
                    locRequest.SetFastestInterval(500);
                    locRequest.SetInterval(1000);

                    Log.Debug("LocationRequest", "Request priority set to status code {0}, interval set to {1} ms",
                        locRequest.Priority.ToString(), locRequest.Interval.ToString());

                    // pass in a location request and LocationListener
                    await LocationServices.FusedLocationApi.RequestLocationUpdates(apiClient, locRequest, this);
                    // In OnLocationChanged (below), we will make calls to update the UI
                    // with the new location data



                    Location location = LocationServices.FusedLocationApi.GetLastLocation(apiClient);
                    if (location != null)
                    {
                        //latitude.Text = "Latitude: " + location.Latitude.ToString();
                        //longitude.Text = "Longitude: " + location.Longitude.ToString();
                        //provider.Text = "Provider: " + location.Provider.ToString();
                        //txt_counter.Text = "Number of observations: " + count.ToString();
                        //txt_lastupdate.Text = "Last observation: " + DateTime.Now.ToString();
                        Log.Debug("LocationClient", "Last location printed");
                        SavetoSd(location.Latitude.ToString(), location.Longitude.ToString());
                        CountDown();

                    }
                }
                else
                {
                    Log.Info("LocationClient", "Please wait for client to connect");
                }
            };

        }
        */

		////Interface methods

		public void OnConnected (Bundle bundle)
		{
			// This method is called when we connect to the LocationClient. We can start location updated directly form
			// here if desired, or we can do it in a lifecycle method, as shown above 

			// You must implement this to implement the IGooglePlayServicesClientConnectionCallbacks Interface
			Log.Info("LocationClient", "Now connected to client");
		}

		public void OnDisconnected ()
		{
			// This method is called when we disconnect from the LocationClient.

			// You must implement this to implement the IGooglePlayServicesClientConnectionCallbacks Interface
			Log.Info("LocationClient", "Now disconnected from client");
            txt_error.Text = "Erro foi desconectado";
            //Toast.MakeText(ApplicationContext, "Now disconnected from client", ToastLength.Long).Show();
        }

		public void OnConnectionFailed (ConnectionResult bundle)
		{
			// This method is used to handle connection issues with the Google Play Services Client (LocationClient). 
			// You can check if the connection has a resolution (bundle.HasResolution) and attempt to resolve it

			// You must implement this to implement the IGooglePlayServicesClientOnConnectionFailedListener Interface
			Log.Info("LocationClient", "Connection failed, attempting to reach google play services");
            txt_error.Text = "Erro conexão Falhou";
            //Toast.MakeText(ApplicationContext, "Connection failed, attempting to reach google play services", ToastLength.Long).Show();
        }

		public void OnLocationChanged (Location location)
		{
			// This method returns changes in the user's location if they've been requested
			 
			// You must implement this to implement the Android.Gms.Locations.ILocationListener Interface
			Log.Debug ("LocationClient", "Location updated");
        }

		public void OnConnectionSuspended (int i)
		{
            txt_error.Text = "Erro conexão Suspensa";
        }

        private void CountDown ()
            {

                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Interval = 1000; 
                timer.Elapsed += OnTimedEvent;
                timer.Enabled = true;

            }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
            {
                if (apiClient.IsConnected)
                {
                    button.Text = "Iniciar captura de dados";


                    Location location = LocationServices.FusedLocationApi.GetLastLocation(apiClient);
                    if (location != null)
                    {
                        Log.Debug("LocationClient", "Last location printed");

                        //Bouding Box - America Latina
                        double[] bbox = new double[] { -86.4, -58.9, -24.1, 14.7 };
                        //Bouding Box - IMT
                        //double[] bbox = new double[] { -46.5786105034, -23.6509426581, -46.569802129, -23.6450262594 };
                        //Bouding Box - Casa
                        //double[] bbox = new double[] { -46.6534387705,-23.6000745207,-46.6446303961,-23.5941558242 };

                        if (location.Latitude >= bbox[1]) //Esquerda
                        {
                            if (location.Latitude <= bbox[3]) //Direita
                            {
                                if (location.Longitude >= bbox[0]) //Topo
                                {
                                    if (location.Longitude <= bbox[2]) //Baixo
                                    {
                                        Update_screen(location);
                                        SavetoSd(location.Latitude.ToString(), location.Longitude.ToString());
                                } else
                                    {
                                        Log.Debug("LocationClient", "Out of Bouding Box");
                                        //Toast.MakeText(ApplicationContext, "Out of Bouding Box", ToastLength.Long).Show();

                                }

                                }
                            }
                        }

                        
                    }
                }
                else
                {
                    // pass in the Context, ConnectionListener and ConnectionFailedListener
                    apiClient = new GoogleApiClient.Builder(this, this, this)
                        .AddApi(LocationServices.API).Build();

                    // generate a location request that we will pass into a call for location updates
                    locRequest = new LocationRequest();
                    apiClient.Connect();
                    locRequest.SetFastestInterval(500);
                    locRequest.SetInterval(1000);

                    Log.Info("LocationClient", "Please wait for client to connect");
                    txt_error.Text = "Erro conexão API Google";
                }

            }

        private void SavetoSd(String lat, String lon)
        {

            //Get IMEI
            var telephonyManager = (TelephonyManager)GetSystemService(TelephonyService);
            var id = telephonyManager.DeviceId;


            var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.Path;
            sdCardPath += "/Download";
            //var filePath = System.IO.Path.Combine(sdCardPath, DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + "_" + id.ToString() + "_monioring.txt");
            var filePath = System.IO.Path.Combine(sdCardPath, DateTime.Now.ToString("yyyy-MM-dd_HH") + "_" + id.ToString() + "_monioring.txt");
            //Log.Debug(filePath, "");

            var filter = new IntentFilter(Intent.ActionBatteryChanged);
            var battery = RegisterReceiver(null, filter);
            int level = battery.GetIntExtra(BatteryManager.ExtraLevel, -1);
            int scale = battery.GetIntExtra(BatteryManager.ExtraScale, -1);

            int level_0_to_100 = (int)Math.Floor(level*100D/ scale);
            //Log.Debug("Battery Level: "+level_0_to_100.ToString(), "");





            //           if (!System.IO.File.Exists(filePath))
            //           {
            using (System.IO.StreamWriter write = new System.IO.StreamWriter(filePath, true))
                {
                var list = Directory.GetFiles(sdCardPath, "*.txt");

                write.Write(DateTime.Now.ToString()+";"+id.ToString()+";"+level_0_to_100.ToString()+";"+lat+";"+lon+"\n");
                write.Dispose();
                count++;

                ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
                NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;

                if (networkInfo != null)
                {
                    bool isOnline = networkInfo.IsConnected;

                    bool isWifi = networkInfo.Type == ConnectivityType.Wifi;
                    if (isWifi)
                    {
                        Log.Debug("", "Wifi connected.");

                        if (list.Length > 1)
                        {
                            for (int i = 0; i < list.Length; i++)
                            {
                                if (list[i].ToString() != filePath)
                                {

                                    string source = list[i];
                                    string destination = @"/srv/shiny-server/bases de dados/Mobile";
                                    string host = "186.201.214.56";
                                    string username = "lcv";
                                    string password = "15u3@dsf";
                                    int port = 22;  //Port 22 is defaulted for SFTP upload

                                    try
                                    {
                                        SFTP.UploadSFTPFile(host, username, password, source, destination, port);
                                        File.Delete(list[i]);
                                    }
                                    catch
                                    {
                                        Log.Debug("Upload not complete!", "");
                                        //Toast.MakeText(ApplicationContext, "WiFi OK - Upload Failed!", ToastLength.Long).Show();
                                    }
                                }
                            }

                        }



                    }
                    else
                    {
                        Log.Debug("", "Wifi disconnected.");
                        txt_error.Text = "Erro WiFi desconectado";
                    }
                }



                



            }
 //           }

        }

        private void Update_screen(Location location)
        {
            latitude.Text = "Latitude: " + location.Latitude.ToString();
            longitude.Text = "Longitude: " + location.Longitude.ToString();
            provider.Text = "Provider: " + location.Provider.ToString();
            txt_counter.Text = "Number of observations: " + count.ToString();
            txt_lastupdate.Text = "Last observation: " + DateTime.Now.ToString();
            count++;
        }
      
    }
}


