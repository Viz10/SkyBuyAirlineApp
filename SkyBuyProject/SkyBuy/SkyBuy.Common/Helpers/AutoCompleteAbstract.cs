using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SkyBuy.Common.Helpers
{
    public abstract partial class AutoCompleteAbstract<T>
    {
        public List<T> Options { get; set; } /// objects
        public abstract Task PopulateOptions();

        public abstract List<T> FilterOprions(string text); /// based on typed text filter the correct objects
            
        protected void TextChanged(string text,ObservableCollection<T> suggestions) /// update suggestions
        {

            if (string.IsNullOrEmpty(text)) return;

            suggestions.Clear();

            List<T> PotentialCandidates = FilterOprions(text);

            foreach (var item in PotentialCandidates)
                suggestions.Add(item);
        }
        
        protected abstract void SuggestionChosen(T data); /// let inheritor decide what to do with obj rtrieved
       
    }
}
