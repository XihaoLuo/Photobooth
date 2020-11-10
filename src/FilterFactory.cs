using System.Collections.Generic;

namespace Photobooth
{
    // Implement class FilterFactory
    class FilterFactory
    {
        // private variables
        Dictionary<string, Filter.CreateFn> _filters = 
                                    new Dictionary<string, Filter.CreateFn>();

        // default constructor
        public FilterFactory(){}

        // effects: associates the filter creator function with name 
        //   replaces the existing creator if one exists
        public void RegisterFilter(string name, Filter.CreateFn fn)
        {
            if (_filters.ContainsKey(name) == true)
            {
                _filters[name] = fn;
            }
            else
            {
                _filters.Add(name, fn);
            }
        }

        // effects: removes the CreatorFn corresponding to name, if one exists; 
        // otherwise, does nothing
        public void DeregisterFilter(string name)
        {
            if (_filters.ContainsKey(name) == true)
            {
                _filters.Remove(name);
            }
        }

        // effects: returns a new filter corresponding to name or null if none exists
        public Filter Create(string name)
        {
            if (_filters.ContainsKey(name) == false)
            {
                return null;
            }
            else
            {
                return _filters[name]();
            }
        }

        // effects: Returns the list of names corresponding to each filter
        public List<string> GetFilterNames()
        {
            List<string> filterNames = new List<string>();
            foreach(var item in _filters.Keys)
            {
                filterNames.Add(item);
            } 
            return filterNames;
        }
    }
}
