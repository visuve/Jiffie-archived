using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Jiffie
{
    internal class MainViewModel : PropertyNotifyBase
    {
        private string junkDirectory;

        public string JunkDirectory
        {
            get
            {
                return junkDirectory;
            }
            set
            {
                if (value != junkDirectory)
                {
                    junkDirectory = value;
                    OnPropertyChanged(nameof(JunkDirectory));
                }
            }
        }

        private string junkExtension;

        public string JunkExtension
        {
            get
            {
                return junkExtension;
            }
            set
            {
                if (value != junkExtension)
                {
                    junkExtension = value;
                    OnPropertyChanged(nameof(JunkExtension));
                }
            }
        }

        private ICommand browseJunkDirectory;

        public ICommand BrowseJunkDirectory
        {
            get
            {
                if (browseJunkDirectory == null)
                {
                    browseJunkDirectory = new RelayCommand(x =>
                    {
                        Debug.WriteLine("No support for open file dialog...");
                    }, x => !isRunningSearch);
                }

                return browseJunkDirectory;
            }
        }

        private bool IsWildcardExtension(string extension)
        {
            if (!string.IsNullOrEmpty(extension))
            {
                return new Regex("\\*\\.[a-zA-Z0-9]+").IsMatch(extension);
            }

            return false;
        }

        private ICommand runSearch;

        public ICommand RunSearch
        {
            get
            {
                if (runSearch == null)
                {
                    runSearch = new RelayCommand(x =>
                    {
                        Debug.WriteLine("Running search...");
                        isRunningSearch = true;

                        JunkFiles = new ObservableCollection<FileModel>();
                        var directory = new DirectoryInfo(junkDirectory);

                        foreach (FileInfo file in directory.EnumerateFiles(junkExtension, SearchOption.AllDirectories))
                        {
                            JunkFiles.Add(new FileModel(file, false));
                        }

                        isRunningSearch = false;
                        string message = junkFiles.Count > 0 ? $"Search finished. Found {junkFiles.Count} files." : "Search finished. Nothing found.";
                        MessageBox.Show(message, "Jiffie");
                    }, x => !isRunningSearch && !string.IsNullOrEmpty(junkDirectory) && Directory.Exists(junkDirectory) && IsWildcardExtension(junkExtension));
                }

                return runSearch;
            }
        }

        private ObservableCollection<FileModel> junkFiles;

        public ObservableCollection<FileModel> JunkFiles
        {
            get
            {
                return junkFiles;
            }
            set
            {
                junkFiles = value;
                OnPropertyChanged(nameof(JunkFiles));
            }
        }

        private ICommand selectAllFiles;

        public ICommand SelectAllFiles
        {
            get
            {
                if (selectAllFiles == null)
                {
                    selectAllFiles = new RelayCommand(x =>
                    {
                        foreach (var junkFile in junkFiles)
                        {
                            junkFile.IsSelected = true;
                        }
                    }, x => !isRunningSearch && junkFiles?.Count > 0);
                }

                return selectAllFiles;
            }
        }

        private ICommand deselectAllFiles;

        public ICommand DeselectAllFiles
        {
            get
            {
                if (deselectAllFiles == null)
                {
                    deselectAllFiles = new RelayCommand(x =>
                    {
                        foreach (var junkFile in junkFiles)
                        {
                            junkFile.IsSelected = false;
                        }
                    }, x => !isRunningSearch && junkFiles?.Count > 0);
                }

                return deselectAllFiles;
            }
        }

        private ICommand deleteSelectedFiles;

        public ICommand DeleteSelectedFiles
        {
            get
            {
                if (deleteSelectedFiles == null)
                {
                    deleteSelectedFiles = new RelayCommand(x =>
                    {
                        var failed = new List<string>();
                        var updated = junkFiles.ToList();

                        long kibiBytesRemoved = 0;

                        int removedCount = updated.RemoveAll(file =>
                        {
                            if (file.IsSelected)
                            {
                                try
                                {
                                    file.Info.Delete();

                                    long fileSize = file.Info.Length;

                                    if (fileSize > 0)
                                    {
                                        kibiBytesRemoved += fileSize / 1024;
                                    }

                                    return true;
                                }
                                catch (Exception)
                                {
                                    failed.Add(file.Info.FullName);
                                }
                            }

                            return false;
                        });

                        JunkFiles = new ObservableCollection<FileModel>(updated);

                        if (failed.Count > 0)
                        {
                            MessageBox.Show("Exceptions occurred while trying to delete:\n" + string.Join('\n', failed), "Errors occurred");
                        }

                        string message = $"Removed {removedCount} files.";

                        if (kibiBytesRemoved > 0 && kibiBytesRemoved / 1024 > 1)
                        {
                            message += $" Space freed: {kibiBytesRemoved / 1024}MiB.";
                        }

                        MessageBox.Show(message, "Jiffie");
                    }, x => !isRunningSearch && junkFiles?.Count > 0);
                }

                return deleteSelectedFiles;
            }
        }

        private bool isRunningSearch = false;

        internal MainViewModel()
        {
        }
    }
}