// Copyright (c) Microsoft. All rights reserved.

using System;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;


namespace Blumenthalit.SocialUproar
{
    public sealed partial class MainPage : Page
    {
        private const int RED_PIN = 5;
        private const int GREEN_PIN = 6;
        private const int BLUE_PIN = 13;
        private const int LEFT_MOTOR_PIN = 19;
        private const int RIGHT_MOTOR_PIN = 26;
        private GpioPinValue RedPinState = GpioPinValue.High;
        private GpioPinValue GreenPinState = GpioPinValue.High;
        private GpioPinValue BluePinState = GpioPinValue.High;
        private GpioPinValue BlueMotorPinState = GpioPinValue.Low;
        private GpioPinValue RedMotorPinState = GpioPinValue.Low;
        private GpioPin RedPin;
        private GpioPin GreenPin;
        private GpioPin BluePin;
        private GpioPin BlueMotorPin;
        private GpioPin RedMotorPin;
        private SolidColorBrush VotingOpenBrush = new SolidColorBrush(Windows.UI.Colors.LimeGreen);
        private SolidColorBrush VotingClosedBrush = new SolidColorBrush(Windows.UI.Colors.DarkGreen);
        private SolidColorBrush RedOnBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush RedOffBrush = new SolidColorBrush(Windows.UI.Colors.DarkRed);
        private SolidColorBrush BlueOnBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
        private SolidColorBrush BlueOffBrush = new SolidColorBrush(Windows.UI.Colors.DarkBlue);

        private DispatcherTimer timer;

        int BlueVoteCount = 0;
        int RedVoteCount = 0;

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
            //SetState(RightMotorPin, GpioPinValue.Low);


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
            if (RedPinState == GpioPinValue.High)
            {
                SetState(RedPin, GpioPinValue.Low);
                RedPinState = GpioPinValue.Low;
                RedButton.Background = RedOnBrush;
            }
            else
            {
                SetState(RedPin, GpioPinValue.High);
                RedPinState = GpioPinValue.High;
                RedButton.Background = RedOffBrush;
            }
        }

        private void GreenButton_Click(object sender, RoutedEventArgs e)
        {
            if (GreenPinState == GpioPinValue.High)
            {
                SetState(GreenPin, GpioPinValue.Low);
                GreenPinState = GpioPinValue.Low;
                GreenButton.Background = VotingOpenBrush;
            }
            else
            {
                SetState(GreenPin, GpioPinValue.High);
                GreenPinState = GpioPinValue.High;
                GreenButton.Background = VotingClosedBrush;
            }
        }

        private void BlueButton_Click(object sender, RoutedEventArgs e)
        {
            if (BluePinState == GpioPinValue.High)
            {
                SetState(BluePin, GpioPinValue.Low);
                BluePinState = GpioPinValue.Low;
                BlueButton.Background = BlueOnBrush;
            }
            else
            {
                SetState(BluePin, GpioPinValue.High);
                BluePinState = GpioPinValue.High;
                BlueButton.Background = BlueOffBrush;
            }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {

            SetState(RedPin, GpioPinValue.High);
            RedButton.Background = RedOffBrush;
            SetState(GreenPin, GpioPinValue.Low);
            SetState(BluePin, GpioPinValue.High);
            BlueButton.Background = BlueOffBrush;

            RedTweetCountBox.Text = "0";
            BlueTweetCountBox.Text = "0";

            GreenButton.Content = "Voting Open";
            GreenButton.Background = VotingOpenBrush;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(double.Parse(VotingIntervalBox.Text));
            timer.Tick += Timer_Tick;
            timer.Start();
        }

   
        private void Timer_Tick(object sender, object e)
        {
            timer.Stop();
            SetState(GreenPin, GpioPinValue.High);
            GreenPinState = GpioPinValue.High;
            GreenButton.Content = "Voting Closed";
            GreenButton.Background = VotingClosedBrush;

            if (VoteCountMode.IsOn)
                {
                    GetCountsFromWeb();
                }
                else { 
                    RedVoteCount = (new Random(int.Parse(DateTime.Now.ToString("ss")))).Next(100);
                    BlueVoteCount = (new Random(int.Parse(DateTime.Now.ToString("ss"))+1)).Next(100);
                }

            

            if (RedVoteCount > BlueVoteCount)
            {
                SetState(RedPin, GpioPinValue.Low);
                RedPinState = GpioPinValue.Low;
                //              SetState(RightMotorPin, GpioPinValue.High);
                SetState(BlueMotorPin, GpioPinValue.Low);
                BlueMotorPinState = GpioPinValue.Low;
                RedButton.Background = RedOnBrush;
                BlueButton.Background = BlueOffBrush;
            }
            if (RedVoteCount < BlueVoteCount)
            {
                SetState(BluePin, GpioPinValue.Low);
                SetState(BlueMotorPin, GpioPinValue.High);
                BluePinState = GpioPinValue.Low;
                BlueMotorPinState = GpioPinValue.High;
                RedButton.Background = RedOffBrush;
                BlueButton.Background = BlueOnBrush;
            }

            if (RedVoteCount == BlueVoteCount)
            {
                SetState(RedPin, GpioPinValue.Low);
//                SetState(RightMotorPin, GpioPinValue.High);
                SetState(BluePin, GpioPinValue.Low);
                SetState(BlueMotorPin, GpioPinValue.High);
                BluePinState = GpioPinValue.Low;
                BlueMotorPinState = GpioPinValue.High;
                BlueButton.Background = BlueOnBrush;
                RedButton.Background = RedOnBrush;
            }

        }

        private async void GetCountsFromWeb()
        {
            //Create an HTTP client object
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();

            //Add a user-agent header to the GET request. 
            var headers = httpClient.DefaultRequestHeaders;

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
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                //Send the GET request
                httpResponse =  await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                debugText.Text += httpResponseBody;
                string rednum = httpResponseBody.Substring("[{\"Color\":\"Red\",\"Count\":".Length);
                string blunum = rednum;
                int curlyLoc = rednum.IndexOf("}");
                rednum = rednum.Substring(0, curlyLoc);
                debugText.Text = "Rednum=" + rednum;
                int.TryParse(rednum, out RedVoteCount);
                string bluenum = blunum.Substring(curlyLoc + 2 + "{\"Color\":\"Blue\",\"Count\":".Length);
                bluenum = bluenum.Substring(0, bluenum.Length - 2);
                debugText.Text += "\n Bluenum=" + bluenum;
                int.TryParse(bluenum, out BlueVoteCount);
                RedTweetCountBox.Text = RedVoteCount.ToString();
                BlueTweetCountBox.Text = BlueVoteCount.ToString();
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                debugText.Text = httpResponseBody;
            }

            
        }



        private void RightMotorButton_Click(object sender, RoutedEventArgs e)
        {
            if (RedMotorPinState == GpioPinValue.High)
            {
                SetState(RedMotorPin, GpioPinValue.Low);
                RedMotorPinState = GpioPinValue.Low;
                RedMotorButton.Background = RedOnBrush;
            }
            else
            {
                SetState(RedMotorPin, GpioPinValue.High);
                RedMotorPinState = GpioPinValue.High;
                RedMotorButton.Background = RedOffBrush;
            }
        }

        private void BlueMotorButton_Click(object sender, RoutedEventArgs e)
        {
            if (BlueMotorPinState == GpioPinValue.High)
            {
                SetState(BlueMotorPin, GpioPinValue.Low);
                BlueMotorPinState = GpioPinValue.Low;
                BlueMotorButton.Background = BlueOnBrush;
            }
            else
            {
                SetState(BlueMotorPin, GpioPinValue.High);
                BlueMotorPinState = GpioPinValue.High;
                BlueMotorButton.Background = BlueOffBrush;
            }
        }

        private void RedMotorButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
