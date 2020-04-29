using System.ComponentModel;
using System.Diagnostics;

namespace Jiffie
{
    internal abstract class PropertyNotifyBase : INotifyPropertyChanged
    {
        protected PropertyNotifyBase()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        internal void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                Debug.Fail(string.Concat("Invalid property name: ", propertyName));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}