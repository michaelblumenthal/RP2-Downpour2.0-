// Copyright (c) Microsoft. All rights reserved.

using System;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace Blumenthalit.SocialUproar
{
    public sealed partial class MainPage : Page
    {
        class webResults
        {
            public webResults() { }
            public int RedVotes;
            public int BlueVotes;
        }

        webResults baselineResults = new webResults();
        webResults votingResults = new webResults();


        private const int RED_PIN = 5;
        private const int GREEN_PIN = 6;
        private const int BLUE_PIN = 13;
        private const int LEFT_MOTOR_PIN = 19;
        private const int RIGHT_MOTOR_PIN = 20;
        private GpioPin RedPin;
        private GpioPin GreenPin;
        private GpioPin BluePin;
        private GpioPin BlueMotorPin;
        private GpioPin RedMotorPin;
        private SolidColorBrush VotingOpenBrush = new SolidColorBrush(Windows.UI.Colors.LimeGreen);
        private SolidColorBrush VotingClosedBrush = new SolidColorBrush(Windows.UI.Colors.DarkGreen);
        private SolidColorBrush RedOnBrush = new SolidColorBrush(Windows.UI.Colors.OrangeRed);
        private SolidColorBrush RedOffBrush = new SolidColorBrush(Windows.UI.Colors.DarkRed);
        private SolidColorBrush BlueOnBrush = new SolidColorBrush(Windows.UI.Colors.LightBlue);
        private SolidColorBrush BlueOffBrush = new SolidColorBrush(Windows.UI.Colors.DarkBlue);

        private DispatcherTimer timer;
        private int SecondsRemaining;

        private bool isInternetVote = false;



        public MainPage()
        {
            InitializeComponent();

            RedPin = initPin(RED_PIN);
            GreenPin = initPin(GREEN_PIN);
            BluePin = initPin(BLUE_PIN);
            BlueMotorPin = initPin(LEFT_MOTOR_PIN);
            RedMotorPin = initPin(RIGHT_MOTOR_PIN);

            SetState(RedPin, GpioPinValue.High);
            SetState(GreenPin, GpioPinValue.High);
            SetState(BluePin, GpioPinValue.High);
            SetState(BlueMotorPin, GpioPinValue.Low);
            SetState(RedMotorPin, GpioPinValue.Low);


        }

        private GpioPin initPin(int PinNum)
        {
            var gpio = GpioController.GetDefault();
            if (gpio == null)
            {
                // Show an error if there is no GPIO controller
                GpioStatus.Text = "There is no GPIO controller on this device.";
                throw new Exception(GpioStatus.Text);

            }
            return gpio.OpenPin(PinNum);
        }

        private void SetState(GpioPin pin, GpioPinValue HiOrLo)
        {

            try
            {
                pin.Write(HiOrLo);
                pin.SetDriveMode(GpioPinDriveMode.Output);
                GpioStatus.Text = "GPIO pin " + pin.PinNumber.ToString() + " set correctly.";
            }
            catch (Exception ex)
            {

                GpioStatus.Text = "Exception when setting pin #" + pin.PinNumber.ToString() + ".  Error is " + ex.Message
                    + " Sharing Mode is " + pin.SharingMode.ToString() + ".";
            }

        }




        private void RedButton_Click(object sender, RoutedEventArgs e)
        {
            if (RedPin.Read() == GpioPinValue.High)
            {
                SetState(RedPin, GpioPinValue.Low);
                RedButton.Background = RedOnBrush;
            }
            else
            {
                SetState(RedPin, GpioPinValue.High);
                RedButton.Background = RedOffBrush;
            }
        }

        private void GreenButton_Click(object sender, RoutedEventArgs e)
        {
            if (GreenPin.Read() == GpioPinValue.High)
            {
                SetState(GreenPin, GpioPinValue.Low);
                GreenButton.Background = VotingOpenBrush;
            }
            else
            {
                SetState(GreenPin, GpioPinValue.High);
                GreenButton.Background = VotingClosedBrush;
            }
        }

        private void BlueButton_Click(object sender, RoutedEventArgs e)
        {
            if (BluePin.Read() == GpioPinValue.High)
            {
                SetState(BluePin, GpioPinValue.Low);
                BlueButton.Background = BlueOnBrush;
            }
            else
            {
                SetState(BluePin, GpioPinValue.High);
                BlueButton.Background = BlueOffBrush;
            }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {

            SetState(RedPin, GpioPinValue.High);
            RedButton.Background = RedOffBrush;
            SetState(BluePin, GpioPinValue.High);
            BlueButton.Background = BlueOffBrush;
           
            SetState(RedMotorPin, GpioPinValue.Low);
            SetState(BlueMotorPin, GpioPinValue.Low);

            GoButton.Background = VotingOpenBrush;
            SetState(GreenPin, GpioPinValue.Low);
            votingResults.RedVotes = -1;
            votingResults.BlueVotes = -1;

            RedPrevCount.Text = "";
            BluePrevCount.Text = "";
            RedCurrentCount.Text = "";
            BlueCurrentCount.Text = "";
            RedVoteCount.Text = "";
            BlueVoteCount.Text = "";

            getBaselineVotes();

            SecondsRemaining = int.Parse(VotingIntervalBox.Text);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void getBaselineVotes()
        {
            if (isInternetVote)
            {
                GetCountsFromWeb(ref baselineResults);

                RedPrevCount.Text = baselineResults.RedVotes.ToString();
                BluePrevCount.Text = baselineResults.BlueVotes.ToString();
            }
            else
            {
                baselineResults.RedVotes = 0;
                baselineResults.BlueVotes = 0;

                RedPrevCount.Text = baselineResults.RedVotes.ToString();
                BluePrevCount.Text = baselineResults.BlueVotes.ToString();
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            SecondsRemaining = SecondsRemaining - 1;
            CountdownTimer.Text = (TimeSpan.FromSeconds(SecondsRemaining)).ToString("c");
            if (SecondsRemaining == 0)
            {
                timer.Stop();
                SetState(GreenPin, GpioPinValue.High);
                GoButton.Background = VotingClosedBrush;
                getNewVotes();
            }
        }


        private void getNewVotes()
        {
            int RedVotes = 0;
            int BlueVotes = 0;

            if (isInternetVote)
            {
                GetCountsFromWeb(ref votingResults);//this sets VotingResults
                if (votingResults.BlueVotes == -1)
                {
                    debugText.Text += "Web call returned, but without data.";
                }
                RedCurrentCount.Text = votingResults.RedVotes.ToString();
                BlueCurrentCount.Text = votingResults.BlueVotes.ToString();
                if (votingResults.RedVotes == baselineResults.RedVotes && votingResults.BlueVotes == baselineResults.BlueVotes)
                {
                    RedVotes = votingResults.RedVotes;
                    BlueVotes = votingResults.BlueVotes;
                }
                else
                {
                    RedVotes = votingResults.RedVotes - baselineResults.RedVotes;
                    BlueVotes = votingResults.BlueVotes - baselineResults.BlueVotes;
                }
                RedVoteCount.Text = RedVotes.ToString();
                BlueVoteCount.Text = BlueVotes.ToString();
            }
            else
            {

                RedVotes = (new Random(int.Parse(DateTime.Now.ToString("ss")))).Next(5,50);
                BlueVotes = (new Random(int.Parse(DateTime.Now.ToString("ss")))).Next(50,100);
                RedCurrentCount.Text = RedVotes.ToString();
                BlueCurrentCount.Text = BlueVotes.ToString();
                RedVoteCount.Text = RedCurrentCount.Text;
                BlueVoteCount.Text = BlueCurrentCount.Text;

            }

            if (RedVotes > BlueVotes)
            {
                SetState(RedPin, GpioPinValue.Low);
                SetState(RedMotorPin, GpioPinValue.High);
                SetState(BlueMotorPin, GpioPinValue.Low);
                RedButton.Background = RedOnBrush;
                BlueButton.Background = BlueOffBrush;
            }
            if (RedVotes < BlueVotes)
            {
                SetState(BluePin, GpioPinValue.Low);
                SetState(BlueMotorPin, GpioPinValue.High);
                RedButton.Background = RedOffBrush;
                BlueButton.Background = BlueOnBrush;
            }

            if (RedVotes == BlueVotes)
            {
                SetState(RedPin, GpioPinValue.Low);
                SetState(RedMotorPin, GpioPinValue.High);
                SetState(BluePin, GpioPinValue.Low);
                SetState(BlueMotorPin, GpioPinValue.High);
                BlueButton.Background = BlueOnBrush;
                RedButton.Background = RedOnBrush;
            }

        }

        private  void GetCountsFromWeb(ref webResults VoteColorPair)
        {
            //Create an HTTP client object
            // Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            System.Net.Http.HttpClient snhClient = new System.Net.Http.HttpClient();
            //Add a user-agent header to the GET request. 
            var headers = snhClient.DefaultRequestHeaders;

            //The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            //especially if the header value is coming from user input.
            string header = "ie";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            Uri requestUri = new Uri(RestUrlBox.Text);

            //Send the GET request asynchronously and retrieve the response as a string.
            System.Net.Http.HttpResponseMessage snhResponse = new System.Net.Http.HttpResponseMessage();
   
           // Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                int Reds = -1;
                int Blues = -1;

                //Send the GET request
                snhResponse = snhClient.GetAsync(requestUri).Result;
                //httpResponse =  await httpClient.GetAsync(requestUri,Windows.Web.Http.HttpCompletionOption.ResponseContentRead).Re;
                snhResponse.EnsureSuccessStatusCode();
                //httpResponse.EnsureSuccessStatusCode();
                //httpResponseBody = httpResponse.Content.ToString();
                httpResponseBody = snhResponse.Content.ReadAsStringAsync().Result;
                debugText.Text += httpResponseBody;
                int blueCloseCurlyLoc = httpResponseBody.IndexOf("}");
                string bluestring = httpResponseBody.Substring(0, blueCloseCurlyLoc);
                int lastColonLoc = bluestring.LastIndexOf(":");
                bluestring = bluestring.Substring(lastColonLoc+1, blueCloseCurlyLoc - lastColonLoc-1);
                Blues = int.Parse(bluestring);

                int redCloseCurlyLoc = httpResponseBody.LastIndexOf("}");
                string redstring = httpResponseBody.Substring(blueCloseCurlyLoc+1, redCloseCurlyLoc - blueCloseCurlyLoc);
                lastColonLoc = redstring.LastIndexOf(":");
                redCloseCurlyLoc = redstring.LastIndexOf("}");
                redstring = redstring.Substring(lastColonLoc + 1, redCloseCurlyLoc - lastColonLoc - 1);
                Reds = int.Parse(redstring);

                /*                JObject voteJson = JObject.Parse(httpResponseBody);
                if (voteJson[0]["Color"].ToString() == "Blue")
                {
                    Blues = int.Parse(voteJson[0]["Count"].ToString());
                }
                if (voteJson[1]["Color"].ToString() == "Red")
                {
                    Reds = int.Parse(voteJson[1]["Count"].ToString());
                }
*/

                VoteColorPair.RedVotes = Reds;
                VoteColorPair.BlueVotes = Blues;
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                debugText.Text = httpResponseBody;
            }


        }



        private void RedMotorButton_Click(object sender, RoutedEventArgs e)
        {
            if (RedMotorPin.Read() == GpioPinValue.High )
            {
                SetState(RedMotorPin, GpioPinValue.Low);
                RedMotorButton.Background = RedOnBrush;
            }
            else
            {
                SetState(RedMotorPin, GpioPinValue.High);
                RedMotorButton.Background = RedOffBrush;
            }
        }

        private void BlueMotorButton_Click(object sender, RoutedEventArgs e)
        {
            if (BlueMotorPin.Read() == GpioPinValue.High)
            {
                SetState(BlueMotorPin, GpioPinValue.Low);
                BlueMotorButton.Background = BlueOffBrush;
            }
            else
            {
                SetState(BlueMotorPin, GpioPinValue.High);
                BlueMotorButton.Background = BlueOnBrush;
            }
        }

        private void VoteCountMode_Toggled(object sender, RoutedEventArgs e)
        {
            if (VoteCountMode.IsOn)
            {
                isInternetVote = true;

            }
            else
            {
                isInternetVote = false;

            }
        }


        private void rootPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(rootPivot.SelectedIndex + ":" + ((PivotItem)(rootPivot.SelectedItem)).Name);
                if (((PivotItem)(rootPivot.SelectedItem)).Name == "Vote")
                {

                    if (isInternetVote)
                    {
                        VoteModeTextbox.Text = "Internet";
                    }
                    else
                    {
                        VoteModeTextbox.Text = "local";
                    }
                    SecondsRemaining = int.Parse(VotingIntervalBox.Text);
                    CountdownTimer.Text = (TimeSpan.FromSeconds(SecondsRemaining)).ToString("c");
                }

                if (((PivotItem)(rootPivot.SelectedItem)).Name == "Test")
                {

                    if (BlueMotorPin.Read() == GpioPinValue.High)
                    {
                       BlueMotorButton.Background = BlueOnBrush;
                    }
                    else
                    {
                       BlueMotorButton.Background = BlueOffBrush;
                    }


                    if (RedMotorPin.Read() == GpioPinValue.High)
                    {
                        RedMotorButton.Background = RedOnBrush;
                    }
                    else
                    {
                        RedMotorButton.Background = RedOffBrush;
                    }


                    if (RedPin.Read() == GpioPinValue.High)
                    {
                        RedButton.Background = RedOffBrush;
                    }
                    else
                    {
                        RedButton.Background = RedOnBrush;
                    }

                    if (BluePin.Read() == GpioPinValue.High)
                    {
                        BlueButton.Background = BlueOffBrush;
                    }
                    else
                    {
                        BlueButton.Background = BlueOnBrush;
                    }

                    if (GreenPin.Read() == GpioPinValue.High)
                    {
                        GreenButton.Background = VotingClosedBrush;
                    }
                    else
                    {
                        GreenButton.Background = VotingClosedBrush;
                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
                debugText.Text += ex.Message;
            }
        }

        private void HurryUp_Click(object sender, RoutedEventArgs e)
        {
            if (SecondsRemaining > 5) { SecondsRemaining = 5; }
        }
    }
}

