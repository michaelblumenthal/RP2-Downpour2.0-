// Copyright (c) Microsoft. All rights reserved.

using System;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Blinky
{
    public sealed partial class MainPage : Page
    {
        private const int RED_PIN = 5;
        private const int GREEN_PIN = 6;
        private const int BLUE_PIN = 13;
        private GpioPinValue RedPinState = GpioPinValue.High;
        private GpioPinValue GreenPinState = GpioPinValue.High;
        private GpioPinValue BluePinState = GpioPinValue.High;
        private GpioPin pin;
        private GpioPinValue pinValue;


        public MainPage()
        {
            InitializeComponent();

            SetState(RED_PIN, GpioPinValue.High);
            SetState(GREEN_PIN, GpioPinValue.High);
            SetState(BLUE_PIN,GpioPinValue.High);

        }

        private void SetState(int PinNum, GpioPinValue HiOrLo)
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                pin = null;
                GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }
            try
            {
                pin = gpio.OpenPin(PinNum);
                pinValue = HiOrLo;
                pin.Write(pinValue);
                pin.SetDriveMode(GpioPinDriveMode.Output);
                GpioStatus.Text = "GPIO pin " + PinNum.ToString() + " set correctly.";
            }
            catch (Exception ex)
            {

                GpioStatus.Text = "Exception when setting pin #" + PinNum.ToString() + ".  Error is " + ex.Message
                    + " Sharing Mode is " + pin.SharingMode.ToString()+ ".";
            }

        }

   





        private void RedButton_Click(object sender, RoutedEventArgs e)
        {
            if (RedPinState == GpioPinValue.High)
            {
                SetState(RED_PIN, GpioPinValue.Low);
                RedPinState = GpioPinValue.Low;
            } else
            {
                SetState(RED_PIN, GpioPinValue.High);
                RedPinState = GpioPinValue.High;
            }
        }

        private void GreenButton_Click(object sender, RoutedEventArgs e)
        {
            if (GreenPinState == GpioPinValue.High)
            {
                SetState(GREEN_PIN, GpioPinValue.Low);
                GreenPinState = GpioPinValue.Low;
            }
            else
            {
                SetState(GREEN_PIN, GpioPinValue.High);
                GreenPinState = GpioPinValue.High;
            }
        }

        private void BlueButton_Click(object sender, RoutedEventArgs e)
        {
            if (BluePinState == GpioPinValue.High)
            {
                SetState(BLUE_PIN, GpioPinValue.Low);
                BluePinState = GpioPinValue.Low;
            }
            else
            {
                SetState(BLUE_PIN, GpioPinValue.High);
                BluePinState = GpioPinValue.High;
            }
        }
    }
}
