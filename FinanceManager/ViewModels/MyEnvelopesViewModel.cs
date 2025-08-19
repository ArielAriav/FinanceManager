using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    internal class MyEnvelopesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> Envelopes { get; } = new();

        private string _newEnvelopeName;
        public string NewEnvelopeName
        {
            get => _newEnvelopeName;
            set
            {
                if (_newEnvelopeName != value)
                {
                    _newEnvelopeName = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand AddEnvelopeCommand { get; }

        public MyEnvelopesViewModel()
        {
            AddEnvelopeCommand = new Command(AddEnvelope);
        }

        private void AddEnvelope()
        {
            if (!string.IsNullOrWhiteSpace(NewEnvelopeName))
            {
                Envelopes.Add(NewEnvelopeName);
                NewEnvelopeName = string.Empty;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
