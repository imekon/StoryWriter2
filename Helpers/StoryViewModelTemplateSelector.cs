using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using StoryWriter.ViewModels;

namespace StoryWriter.Helpers
{
    public class StoryViewModelTemplateSelector : IDataTemplate
    {
        public bool SupportsRecycling => false;

        [Content]
        public Dictionary<string, IDataTemplate> Templates { get; } = new Dictionary<string, IDataTemplate>();

        public IControl Build(object data)
        {
            return Templates[((StoryViewModel)data).Template].Build(data);
        }

        public bool Match(object data)
        {
            return data is StoryViewModel;
        }
    }
}
