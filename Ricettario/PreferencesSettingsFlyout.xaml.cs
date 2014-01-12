using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace Ricettario
{
    public sealed partial class PreferencesSettingsFlyout : SettingsFlyout
    {
        string localData="UseLocalData";
        string lightTheme = "IsLightTheme";

        public PreferencesSettingsFlyout()
        {
            this.InitializeComponent();

            // Initialize the ToggleSwitch control
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(localData))
                dataSwitch.IsOn = (bool)ApplicationData.Current.LocalSettings.Values[localData];

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(lightTheme))
                temaSwitch.IsOn = (bool)ApplicationData.Current.LocalSettings.Values[lightTheme];
        }

        private void temaSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values[lightTheme] = temaSwitch.IsOn;
        }

        private void dataSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values[localData] = dataSwitch.IsOn;
        }
    }
}
