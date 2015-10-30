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
        private GpioPin RedPin;
        private GpioPin GreenPin;
        private GpioPin BluePin;
        private GpioPinValue pinValue;


        public MainPage()
        {
            InitializeComponent();

            RedPin = initPin(RED_PIN);
            GreenPin = initPin(GREEN_PIN);
            BluePin = initPin(BLUE_PIN);

            SetState(RedPin, GpioPinValue.High);
            SetState(GreenPin, GpioPinValue.High);
            SetState(BluePin,GpioPinValue.High);

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
                    + " Sharing Mode is " + pin.SharingMode.ToString()+ ".";
            }

        }

   


        private void RedButton_Click(object sender, RoutedEventArgs e)
        {
            if (RedPinState == GpioPinValue.High)
            {
                SetState(RedPin, GpioPinValue.Low);
                RedPinState = GpioPinValue.Low;
            } else
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
    }
}
