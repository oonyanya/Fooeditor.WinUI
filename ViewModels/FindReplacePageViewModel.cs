using FooEditor.WinUI.Models;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Microsoft.Toolkit.Mvvm.Input;

namespace FooEditor.WinUI.ViewModels
{
    class FindReplacePageViewModel : ViewModelBase
    {
        #region FindAndReplace
        FindModel _FindModel = new FindModel();

        public FindReplacePageViewModel()
        {
            this._FindModel.DocumentCollection = DocumentCollection.Instance;
        }

        string _Result;
        public string Result
        {
            get
            {
                return this._Result;
            }
            set
            {
                SetProperty(ref this._Result, value);
            }
        }

        string _FindPattern;
        public string FindPattern
        {
            get
            {
                return this._FindPattern;
            }
            set
            {
                this._FindPattern = value;
                this.OnPropertyChanged();
            }
        }

        string _SelectedFindPattern;
        public string SelectedFindPattern
        {
            get
            {
                return this._SelectedFindPattern;
            }
            set
            {
                this._SelectedFindPattern = value;
                this.FindPattern = value;
                this.OnPropertyChanged();
            }
        }

        ObservableCollection<string> _FindHistroy;
        public ObservableCollection<string> FindHistroy
        {
            get
            {
                return this._FindHistroy;
            }
            set
            {
                this._FindHistroy = value;
                this.OnPropertyChanged();
            }
        }

        string _ReplacePattern;
        public string ReplacePattern
        {
            get
            {
                return this._ReplacePattern;
            }
            set
            {
                this._ReplacePattern = value;
                this.OnPropertyChanged();
            }
        }

        bool _UseRegEx;
        public bool UseRegEx
        {
            get
            {
                return this._UseRegEx;
            }
            set
            {
                this._UseRegEx = value;
                this._FindModel.Reset();
                this.OnPropertyChanged();
            }
        }

        bool _RestrictSearch;
        public bool RestrictSearch
        {
            get
            {
                return this._RestrictSearch;
            }
            set
            {
                this._RestrictSearch = value;
                this._FindModel.Reset();
                this.OnPropertyChanged();
            }
        }

        bool _UseGroup;
        public bool UseGroup
        {
            get
            {
                return this._UseGroup;
            }
            set
            {
                this._UseGroup = value;
                this.OnPropertyChanged();
            }
        }

        public bool AllDocuments
        {
            get
            {
                return this._FindModel.AllDocuments;
            }
            set
            {
                this._FindModel.AllDocuments = value;
                this.OnPropertyChanged();
            }
        }

        public RelayCommand<object> FindNextCommand
        {
            get
            {
                return new RelayCommand<object>((s) =>
                {
                    this.Result = string.Empty;
                    try
                    {
                        this.AddFindHistory(this.FindPattern);

                        RegexOptions opt = this.RestrictSearch ? RegexOptions.None : RegexOptions.IgnoreCase;
                        this._FindModel.FindNext(this.FindPattern, this.UseRegEx, opt);
                    }
                    catch (NotFoundExepction)
                    {
                        var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                        this.Result = loader.GetString("NotFoundInDocument");
                    }
#if !DEBUG
                    catch (Exception e)
                    {
                        this.Result = e.Message;
                    }
#endif
                });
            }
        }

        public RelayCommand<object> ReplaceNextCommand
        {
            get
            {
                return new RelayCommand<object>((s) =>
                {
                    this.Result = string.Empty;
                    try
                    {
                        this._FindModel.Replace(this.ReplacePattern, this.UseGroup);
                        RegexOptions opt = this.RestrictSearch ? RegexOptions.None : RegexOptions.IgnoreCase;
                        this._FindModel.FindNext(this.FindPattern, this.UseRegEx, opt);
                    }
                    catch (NotFoundExepction)
                    {
                        var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                        this.Result = loader.GetString("NotFoundInDocument");
                    }
#if !DEBUG
                    catch (Exception e)
                    {
                        this.Result = e.Message;
                    }
#endif
                });
            }
        }

        public RelayCommand<object> ReplaceAllCommand
        {
            get
            {
                return new RelayCommand<object>((s) => {
                    this.Result = string.Empty;
                    try
                    {
                        this.AddFindHistory(this.FindPattern);

                        RegexOptions opt = this.RestrictSearch ? RegexOptions.None : RegexOptions.IgnoreCase;
                        this._FindModel.ReplaceAll(this.FindPattern, this.ReplacePattern, this.UseGroup, this.UseRegEx, opt);
                    }
                    catch (Exception e)
                    {
                        this.Result = e.Message;
                    }
                });
            }
        }

        void AddFindHistory(string pattern)
        {
            if (this.FindHistroy == null)
                return;
            if (!this.FindHistroy.Contains(this.FindPattern))
                this.FindHistroy.Add(this.FindPattern);
        }
#endregion
    }
}
