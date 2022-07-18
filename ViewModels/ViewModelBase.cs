using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FooEditor.WinUI.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace FooEditor.WinUI
{
    public class ViewModelBase : ObservableObject, INotifyPropertyChanged
    {
        public INavigationService NavigationService { get; set; }


        public virtual void OnNavigatedFrom(NavigatedToEventArgs parameters, Dictionary<string, object> viewModelState)
        {

        }

        public virtual void OnNavigatedTo(NavigatedToEventArgs parameters, Dictionary<string, object> viewModelState)
        {

        }

        public virtual void OnNavigatingTo(NavigatedToEventArgs parameters, Dictionary<string, object> viewModelState)
        {

        }
    }
}
