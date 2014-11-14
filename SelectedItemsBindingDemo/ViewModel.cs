using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Input;

namespace SelectedItemsBindingDemo
{
    public class ViewModel : INotifyPropertyChanged
    {
        ObservableCollection<string> _selectedNames = new ObservableCollection<string>();
        string _summary;

        public ViewModel()
        {
        }

        public IEnumerable<string> AvailableNames
        {
            get
            {
                return new string[] 
                {
                    "Abraham", "George", "James", "Joel", "John", "Peter", "Samuel", "Zachariah"
                };
            }
        }

        public string Summary
        {
            get
            {
                return _summary;
            }
            private set
            {
                _summary = value;
                OnPropertyChanged("Summary");
            }
        }

        public ICommand SelectAll
        {
            get
            {
                return new RelayCommand(
                    parameter =>
                    {
                        SelectedNames.Clear();
                        foreach (var item in AvailableNames)
                        {
                            SelectedNames.Add(item);
                        }
                    });
            }
        }

        public ObservableCollection<string> SelectedNames 
        { 
            get
            {
                return _selectedNames;
            }
        }

        public ICommand SelectionChangedCommand
        {
            get
            {
                return new RelayCommand(
                    parameter =>
                    {
                        var items = (IList)parameter;
                        var currentSelectedItems = items.Cast<string>();

                        if (currentSelectedItems == null)
                        {
                            return;
                        }

                        UpdateSummary(currentSelectedItems);
                    });
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void UpdateSummary(IEnumerable<string> selectedNames)
        {
            Summary = string.Format("{0} names are selected.", selectedNames.Count());
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
