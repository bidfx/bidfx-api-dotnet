/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System.Collections.Generic;
using BidFX.Public.API.Price.Plugin.Pixie.Messages;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    internal class GridCache
    {
        private readonly Dictionary<Subject.Subject, Grid> _grids = new Dictionary<Subject.Subject, Grid>();

        private object _lock = new object();

        public Grid Get(Subject.Subject subject)
        {
            lock (_lock)
            {
                if (!_grids.ContainsKey(subject))
                {
                    return null;
                }

                return _grids[subject];
            }
        }

        public void Add(Subject.Subject subject)
        {
            lock (_lock)
            {
                if (!_grids.ContainsKey(subject))
                {
                    _grids[subject] = new Grid();
                }
            }
        }

        public void Remove(Subject.Subject subject)
        {
            lock (_lock)
            {
                if (_grids.ContainsKey(subject))
                {
                    _grids.Remove(subject);
                }
            }
        }

        public void Reset()
        {
            lock (_lock)
            {
                _grids.Clear();
            }
        }
    }
}