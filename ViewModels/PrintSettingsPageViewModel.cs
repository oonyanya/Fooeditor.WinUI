using System;
using System.Globalization;
using FooEditor.WinUI.Models;

namespace FooEditor.WinUI.ViewModels
{
    sealed class PrintSettingsPageViewModel : ViewModelBase
    {
        public bool IsMetric
        {
            get
            {
                return RegionInfo.CurrentRegion.IsMetric;
            }
        }
        public float TopMargin
        {
            get
            {
                return PrintModel.GetUnit(AppSettings.Current.TopMargin, this.IsMetric);
            }
            set
            {
                AppSettings.Current.TopMargin = PrintModel.GetPixel(value, this.IsMetric);
                this.OnPropertyChanged();
            }
        }
        public float RightMargin
        {
            get
            {
                return PrintModel.GetUnit(AppSettings.Current.RightMargin, this.IsMetric);
            }
            set
            {
                AppSettings.Current.RightMargin = PrintModel.GetPixel(value, this.IsMetric);
                this.OnPropertyChanged();
            }
        }
        public float BottomMargin
        {
            get
            {
                return PrintModel.GetUnit(AppSettings.Current.BottomMargin, this.IsMetric);
            }
            set
            {
                AppSettings.Current.BottomMargin = PrintModel.GetPixel(value, this.IsMetric);
                this.OnPropertyChanged();
            }
        }
        public float LeftMargin
        {
            get
            {
                return PrintModel.GetUnit(AppSettings.Current.LeftMargin, this.IsMetric);
            }
            set
            {
                AppSettings.Current.LeftMargin = PrintModel.GetPixel(value, this.IsMetric);
                this.OnPropertyChanged();
            }
        }
        public string Header
        {
            get
            {
                return AppSettings.Current.Header;
            }
            set
            {
                AppSettings.Current.Header = value;
                this.OnPropertyChanged();
            }
        }
        public string Footer
        {
            get
            {
                return AppSettings.Current.Footer;
            }
            set
            {
                AppSettings.Current.Footer = value;
                this.OnPropertyChanged();
            }
        }
        public string UnitNoticeText
        {
            get
            {
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                if (this.IsMetric)
                    return loader.GetString("MetricLabel");
                else
                    return loader.GetString("InchiLabel");
            }
        }
    }
    sealed class PrintModel
    {
        static public int GetPixel(float n, bool ismetric)
        {
            if (ismetric)
                return (int)Math.Round(n / 25.4f * AppSettings.Current.Dpi);
            else
                return (int)Math.Round(n * AppSettings.Current.Dpi + 0.5);
        }
        static public float GetUnit(float px, bool ismetric)
        {
            if (ismetric)
                return (float)Math.Round(px * 25.4f / AppSettings.Current.Dpi);
            else
                return (float)Math.Round(px / AppSettings.Current.Dpi);

        }
    }
}
