using System.Collections;
using System.Collections.Generic;

namespace SimpleAutoCompleteWPFControl
{
    public interface ISuggestionProvider
    {
        public IEnumerable<string> GetSuggestions(string filter);
        public IEnumerable<string> GetFullCollection();
    }
}
