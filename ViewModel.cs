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
            _selectedNames.CollectionChanged += (sender, e) => UpdateSummary();
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

        public ObservableCollection<string> SelectedNames
        {
            get
            {
                return _selectedNames;
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

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void UpdateSummary()
        {
            Summary = string.Format("{0} names are selected.", SelectedNames.Count);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
