using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleAutoCompleteWPFControl
{
    public class MySuggestionProvider : ISuggestionProvider
    {
        public static readonly List<string> s_test = new List<string>
        {
            "The badger knows something",
            "Your head looks something like a pineapple",
            "Crazy like a box of green frogs",
            "The billiard table has green cloth",
            "The sky is blue",
            "We're going to need some golf shoes",
            "This is going straight to the pool room",
            "We're going to  Bonnie Doon",
            "Spring forward - Fall back",
            "Gerry had a plan which involved telling all",
            "When is the summer coming",
            "Take you time and tell me what you saw",
            "All hands on deck"
        };

        IEnumerable<string> ISuggestionProvider.GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return null;

            //item.ToUpperInvariant().Contains(filter.ToUpperInvariant())
            //s_test.Where(p => p.ToLower().Contains(filter.ToLower())).ToList();
            return s_test.Where(state => state.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) > -1).ToList();
        }

        IEnumerable<string> ISuggestionProvider.GetFullCollection()
        {
            return s_test;
        }
    }
}
