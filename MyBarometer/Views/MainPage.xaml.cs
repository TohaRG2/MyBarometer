using MyBarometer.Services;
using Preference.Models;
using Sensors.Model;
using System;
using Tizen.Sensor;
using Tizen.Wearable.CircularUI.Forms;

using Xamarin.Forms;

namespace MyBarometer.Views
{
    public partial class MainPage : ContentPage
    {
        private double ArrowValue = 0;

        public PressureSensor Pressure { get; private set; }
        public PressureModel Model { get; private set; }

        private readonly PreferenceModel _model = new PreferenceModel();

        public MainPage()
        {
            Model = new PressureModel
            {
                IsSupported = PressureSensor.IsSupported,
                SensorCount = PressureSensor.Count
            };
            InitializeComponent();

            if (Model.IsSupported)
            {
                double _savedTorr = _model.Get<double>("savedValue");
                Logger.Debug($"Восстановили данные - {_savedTorr}");
                if (_savedTorr.Equals(0)) _savedTorr = 760;
                ArrowValue = (_savedTorr - 710) * 3 - 120;
                arrowSaveImage.Rotation = ArrowValue;

                Pressure = new PressureSensor();
                Pressure.DataUpdated += Pressure_DataUpdated;
            }
        }

        // Called when the button is clicked.
        private void OnButtonClicked(object sender, EventArgs e)
        {
            double torr = Model.Pressure / 1.333223684;

            ArrowValue = ((float)torr - 710) * 3 - 120;
            arrowSaveImage.Rotation = ArrowValue;
            _model.Set("savedValue", torr);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Pressure?.Start();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Pressure?.Stop();
        }

        private void Pressure_DataUpdated(object sender, PressureSensorDataUpdatedEventArgs e)
        {
            var pressure = e.Pressure;
            Model.Pressure = pressure;
            var torr = Math.Round(Model.Pressure / 1.333223684, 2);
            ArrowValue = (torr - 710) * 3 - 120;

            arrowImage.Rotation = ArrowValue;
            textLabel.Text = torr.ToString();
            Logger.Debug($"Давление - {ArrowValue}");
        }
    }
}