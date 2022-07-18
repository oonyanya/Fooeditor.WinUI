using FooEditor.WinUI.Services;
using FooEditor.WinUI.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System;

namespace FooEditor.WinUI.Services
{
    public class NavigatedToEventArgs
    {
        public object Parameter;
    }

    public interface INavigationService
    {
        void GoBack();
        void ClearHistory();
        void Navigate(string name);
        void Navigate(string name, object parameters);
    }

    public class ActivatorViewModelArgs
    {
        public Type TargetViewMode;
    }

    public class NavigationService : INavigationService
    {
        Frame _frame;
        public Frame frame
        {
            get
            {
                return _frame;
            }
            set
            {
                if(_frame != value)
                {
                    _frame = value;
                    _frame.IsNavigationStackEnabled = false;
                    _frame.Navigated += (s, e) => {
                        Page page = e.Content as Page;
                        Type type = Type.GetType("FooEditor.WinUI.ViewModels." + page.GetType().Name + "ViewModel");

                        ViewModelBase vm = null;

                        if (ActivatorViewModel != null)
                            vm = ActivatorViewModel(new ActivatorViewModelArgs() {TargetViewMode = type});

                        if(vm == null)
                            vm = Activator.CreateInstance(type) as ViewModelBase;

                        if (vm != null)
                        {
                            vm.NavigationService = this;
                            vm.OnNavigatedTo(new NavigatedToEventArgs() {Parameter = e.Parameter }, null);
                            page.DataContext = vm;
                        }
                    };
                }
            }
        }

        public Func<ActivatorViewModelArgs, ViewModelBase> ActivatorViewModel;

        public NavigationService()
        {
        }

        public void GoBack()
        {
            if (this.frame == null)
                return;
            if (this.frame.IsNavigationStackEnabled)
                this.frame.GoBack();
        }

        public void ClearHistory()
        {
            _frame.CacheSize = 0;
        }

        public void Navigate(string name)
        {
            this.Navigate(name, null);
        }

        public void Navigate(string name, object parameters)
        {
            if (this.frame == null)
                return;
            Type type = Type.GetType("FooEditor.WinUI.Views." + name + "Page");
            this.frame.Navigate(type, parameters);
        }
    }
}
