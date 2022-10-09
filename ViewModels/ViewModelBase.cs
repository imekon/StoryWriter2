using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ReactiveUI;

namespace StoryWriter.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public ViewModelBase()
        {

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
