using System.Collections.ObjectModel;
using StoryWriter.Models;

namespace StoryWriter.ViewModels
{
    public class StoryViewModel : ViewModelBase
    {
        private string m_title;
        private Story? m_story;
        private ObservableCollection<StoryViewModel> m_children;

        public StoryViewModel()
        {
            m_title = "";
            m_story = null;
            m_children = new ObservableCollection<StoryViewModel>();
            Template = "";
        }

        public StoryViewModel(Story story)
        {
            m_title = "";
            m_story = story;
            m_children = new ObservableCollection<StoryViewModel>();
            Template = "Story";
        }

        public StoryViewModel(string name)
        {
            m_title = name;
            m_story = null;
            m_children = new ObservableCollection<StoryViewModel>();
            Template = "Folder";
        }

        public string Title
        {
            get
            {
                if (m_story != null)
                    return m_story.Title;

                return m_title;
            }
            set
            {
                if (m_story != null)
                    m_story.Title = value;
                else
                    m_title = value;

                OnPropertyChanged(nameof(Title));
                OnPropertyChanged(nameof(IsModified));
            }
        }

        public string Text
        {
            get => m_story != null ? m_story.Text : "";
            set
            {
                if (m_story == null)
                    return;

                m_story.Text = value;
                OnPropertyChanged(nameof(Text));
                OnPropertyChanged(nameof(IsModified));
                OnPropertyChanged(nameof(WordCount));
                OnPropertyChanged(nameof(CharacterCount));
            }
        }

        public string Tags
        {
            get => m_story != null ? m_story.Tags : "";
            set
            {
                if (m_story == null)
                    return;

                m_story.Tags = value;
                OnPropertyChanged(nameof(Tags));
                OnPropertyChanged(nameof(IsModified));
            }
        }

        public string Folder
        {
            get => m_story != null ? m_story.Folder : "";
            set
            {
                if (m_story == null)
                    return;

                m_story.Folder = value;
                OnPropertyChanged(nameof(Folder));
                OnPropertyChanged(nameof(IsModified));
            }
        }

        public ObservableCollection<StoryViewModel> Children => m_children;

        public int WordCount => m_story != null ? m_story.WordCount : 0;

        public int CharacterCount => m_story != null ? m_story.CharacterCount : 0;

        public bool IsModified => m_story != null ? m_story.State != StoryState.Normal : false;

        public string Template { get; set; }

        public void ResetProperties()
        {
            OnPropertyChanged(nameof(IsModified));
            OnPropertyChanged(nameof(WordCount));
            OnPropertyChanged(nameof(CharacterCount));
        }
    }
}
