using Microsoft.Band;
using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BarSpeed
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IBandClient bandClient;

        public MainPage()
        {
            this.InitializeComponent();

            ConnectToBand();
            PopulateSupportedIntervals();
        }

        private async void ConnectToBand()
        {
            IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();

            //connect to the Band to get a new BandClient object
            try
            {
                using (bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]))
                {
                    // check current sensor consent
                    if (bandClient.SensorManager.Accelerometer.GetCurrentUserConsent() != UserConsent.Granted)
                    {
                        // user hasn't consented, request consent
                        await bandClient.SensorManager.Accelerometer.RequestUserConsentAsync();
                    }
                }
            }
            catch (BandException ex)
            {
                // handle Band connection exception
            }

            // start the accelerometer sensor
            try
            {
                await bandClient.SensorManager.Accelerometer.StartReadingsAsync();
            }     
            catch(BandException ex)
            {
                // handle accelerometer exception
                throw ex;
            }
        }

        private void PopulateSupportedIntervals()
        {
            // get a list of available reporting intervals 
            IEnumerable<TimeSpan> supportedAccelerometerReportingIntervals = bandClient.SensorManager.Accelerometer.SupportedReportingIntervals;
            foreach (var ri in supportedAccelerometerReportingIntervals)
            {
                // do work with each reporting interval (i.e., add them to a list in the UI) 
                greetingOutput.Text = ri.Seconds.ToString();
            }

            // set the reporting interval 
            bandClient.SensorManager.Accelerometer.ReportingInterval = supportedAccelerometerReportingIntervals.GetEnumerator().Current;

            // hook up to the Heartrate sensor ReadingChanged event 
            bandClient.SensorManager.HeartRate.ReadingChanged += (sender, args) =>  
            {
                // do work when the reading changes (i.e., update a UI element) 
            }; 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            greetingOutput.Text = "Hi, " + nameInput.Text;
        }
    }
}
