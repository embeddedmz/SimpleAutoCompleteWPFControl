using System.Collections;
using System.Collections.Generic;

namespace SimpleAutoCompleteWPFControl
{
    public interface ISuggestionProvider
    {
        IEnumerable<string> GetSuggestions(string filter);
        IEnumerable<string> GetFullCollection();
    }
}
