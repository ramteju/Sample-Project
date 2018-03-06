using Client.Logging;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Client.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (System.Exception ex)
            {
                Log.This(ex);
            }
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            try
            {
                if (EqualityComparer<T>.Default.Equals(storage, value))
                    return false;
                storage = value;
                this.OnPropertyChanged(propertyName);
                return true;
            }
            catch (System.Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }
    }
}
