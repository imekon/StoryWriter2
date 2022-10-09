using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Media;
using LiteDB;
using StoryWriter.Helpers;
using StoryWriter.Models;

namespace StoryWriter.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool m_modified;
        private string m_filename;
        private string m_password;
        private ObservableCollection<StoryViewModel> m_folders;
        private List<Story> m_stories;
        private string m_filter;
        private StoryViewModel? m_selectedStory;

        public MainWindowViewModel()
        {
            m_modified = false;
            m_filename = "";
            m_password = "";
            m_folders = new ObservableCollection<StoryViewModel>();
            m_stories = new List<Story>();
            m_filter = "";
            m_selectedStory = null;

            var appSettings = ConfigurationManager.AppSettings;
            m_filename = appSettings["StoryFile"]!;
            var passwordFile = appSettings["Password"]!;
            if (!string.IsNullOrEmpty(passwordFile))
                m_password = File.ReadAllText(passwordFile);
        }

        public ObservableCollection<StoryViewModel> Folders => m_folders;

        public StoryViewModel? SelectedStory
        {
            get => m_selectedStory;
            set
            {
                m_selectedStory = value;
                OnPropertyChanged(nameof(SelectedStory));
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(Text));
                OnPropertyChanged(nameof(Tags));
                OnPropertyChanged(nameof(StoryColour));
            }
        }

        public string Filter
        {
            get => m_filter;
            set
            {
                m_filter = value;
                OnPropertyChanged(nameof(Filter));
            }
        }

        public string Title
        {
            get
            {
                if (m_selectedStory == null)
                    return "None";

                return m_selectedStory.Title;
            }

            set
            {
                if (m_selectedStory == null)
                    return;

                m_selectedStory.Title = value;
                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(StoryColour));
            }
        }

        public string Text
        {
            get
            {
                if (m_selectedStory == null)
                    return "None";

                return m_selectedStory.Text;
            }

            set
            {
                if (m_selectedStory == null)
                    return;

                m_selectedStory.Text = value;
                OnPropertyChanged(nameof(Text));
                OnPropertyChanged(nameof(StoryColour));
            }
        }

        public string Tags
        {
            get
            {
                if (m_selectedStory == null)
                    return "None";

                return m_selectedStory.Tags;
            }

            set
            {
                if (m_selectedStory == null)
                    return;

                m_selectedStory.Tags = value;
                OnPropertyChanged(nameof(Tags));
                OnPropertyChanged(nameof(StoryColour));
            }
        }

        


        private StoryViewModel? FindFolder(string name)
        {
            foreach(var folder in m_folders)
            {
                if (folder.Title == name)
                    return folder;
            }

            return null;
        }

        private SolidColorBrush StoryColour
        {
            get
            {
                if (m_selectedStory != null && m_selectedStory.IsModified)
                    return new SolidColorBrush(Colors.AliceBlue);
                    
                return new SolidColorBrush(Colors.White);
            }
        }

        private void Build()
        {
            m_folders.Clear();

            foreach(var story in m_stories)
            {
                var folder = FindFolder(story.Folder);
                if (folder == null)
                {
                    folder = new StoryViewModel(story.Folder);
                    m_folders.Add(folder);
                }

                folder.Children.Add(new StoryViewModel(story));
            }
        }

        private void LoadStories()
        {
            using (var stories = new LiteDatabase($"Filename={m_filename};Password={m_password}"))
            {
                var col = stories.GetCollection<Story>("stories");
                if (col == null)
                    return;

                m_stories = col.Query()
                    .OrderBy(x => x.Folder + x.Title)
                    .ToList();

                foreach (var story in m_stories)
                    story.State = StoryState.Normal;
            }

            Build();
        }

        private void SaveStories()
        {
            if (File.Exists(m_filename))
                File.Delete(m_filename);

            using (var db = new LiteDatabase($"Filename={m_filename};Password={m_password}"))
            {
                var stories = db.GetCollection<Story>("stories");

                foreach (var story in m_stories)
                {
                    stories.Insert(story);
                }

                stories.EnsureIndex(t => t.Title);
            }
        }

        private void UpdateStories()
        {
            if (!m_modified)
                return;

            using (var db = new LiteDatabase($"Filename={m_filename};Password={m_password}"))
            {
                var stories = db.GetCollection<Story>("Stories");

                foreach (var story in m_stories)
                {
                    switch (story.State)
                    {
                        case StoryState.Normal:
                            break;

                        case StoryState.Modified:
                            stories.Update(story);
                            break;

                        case StoryState.Created:
                            stories.Insert(story);
                            break;

                        case StoryState.Deleted:
                            stories.Delete(story.Id);
                            break;
                    }

                    story.State = StoryState.Normal;
                }
            }

            foreach(var folder in m_folders)
            {
                foreach (var story in folder.Children)
                    story.ResetProperties();
            }
        }

        public ICommand LoadCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    LoadStories();
                });
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {

                });
            }
        }

        public ICommand UpdateCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {

                });
            }
        }

        public ICommand ExitCommand
        {
            get
            {
                return new DelegateCommand((o) =>
                {
                    if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        desktop.MainWindow.Close();                        
                    }
                });
            }
        }
    }
}
