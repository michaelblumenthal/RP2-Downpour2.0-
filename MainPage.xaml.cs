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
        private string TwitterSinceID = "";
        private GpioPinValue RedPinState = GpioPinValue.High;
        private GpioPinValue GreenPinState = GpioPinValue.High;
        private GpioPinValue BluePinState = GpioPinValue.High;
        private GpioPin RedPin;
        private GpioPin GreenPin;
        private GpioPin BluePin;
        private SolidColorBrush VotingOpenBrush = new SolidColorBrush(Windows.UI.Colors.LimeGreen);
        private SolidColorBrush VotingClosedBrush = new SolidColorBrush(Windows.UI.Colors.DarkGreen);
        private DispatcherTimer timer;


        public MainPage()
        {
            InitializeComponent();

            RedPin = initPin(RED_PIN);
            GreenPin = initPin(GREEN_PIN);
            BluePin = initPin(BLUE_PIN);

            SetState(RedPin, GpioPinValue.High);
            SetState(GreenPin, GpioPinValue.High);
            SetState(BluePin, GpioPinValue.High);

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
            }
            else
            {
                SetState(RedPin, GpioPinValue.High);
                RedPinState = GpioPinValue.High;
            }
        }

        private void GreenButton_Click(object sender, RoutedEventArgs e)
        {
            if (GreenPinState == GpioPinValue.High)
            {
                SetState(GreenPin, GpioPinValue.Low);
                GreenPinState = GpioPinValue.Low;
            }
            else
            {
                SetState(GreenPin, GpioPinValue.High);
                GreenPinState = GpioPinValue.High;
            }
        }

        private void BlueButton_Click(object sender, RoutedEventArgs e)
        {
            if (BluePinState == GpioPinValue.High)
            {
                SetState(BluePin, GpioPinValue.Low);
                BluePinState = GpioPinValue.Low;
            }
            else
            {
                SetState(BluePin, GpioPinValue.High);
                BluePinState = GpioPinValue.High;
            }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {

            SetState(RedPin, GpioPinValue.High);
            SetState(GreenPin, GpioPinValue.Low);
            SetState(BluePin, GpioPinValue.High);
            GreenButton.Content = "Voting Open";
            GreenButton.Background = VotingOpenBrush;
           TwitterSinceID =  getBaselineTweetsForToday();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(double.Parse(VotingIntervalBox.Text));
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private string getBaselineTweetsForToday()
        {
            //TODO
            return "";
        }

        private void Timer_Tick(object sender, object e)
        {
            timer.Stop();
            SetState(GreenPin, GpioPinValue.High);
            GreenButton.Content = "Voting Closed";
            GreenButton.Background = VotingClosedBrush;

            int RedCount = 0;
            int BlueCount = 0;

            //Blumenthalit.SocialUproar.TwitterQueryer.QueryTwitter.

            if (RedCount > BlueCount)
            {
                SetState(RedPin, GpioPinValue.Low);
            }
            if (RedCount > BlueCount)
            {
                SetState(BluePin, GpioPinValue.Low);
            }

            if (RedCount == BlueCount)
            {
                SetState(RedPin, GpioPinValue.Low);
                SetState(BluePin, GpioPinValue.Low);
            }

        }

        private void GetCountsFromTwitter(ref int redCount, ref int blueCount)
        {
            //TODO Actually talk to twitter
            redCount = 1;
            blueCount = 0;
        }
    }
}
