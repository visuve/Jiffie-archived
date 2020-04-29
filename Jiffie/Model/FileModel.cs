using System.IO;

namespace Jiffie
{
    internal class FileModel : PropertyNotifyBase
    {
        public FileInfo Info
        {
            get;
        }

        private bool isSelected = false;

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        internal FileModel(FileInfo fileInfo, bool isSelected)
        {
            Info = fileInfo;
            IsSelected = isSelected;
        }
    }
}