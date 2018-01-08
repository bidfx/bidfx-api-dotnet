using System;
using System.Collections.Generic;
using System.Linq;
using BidFX.Public.API.Price.Plugin.Pixie.Fields;
using BidFX.Public.API.Price.Plugin.Pixie.Messages;
using BidFX.Public.API.Price.Subject;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    internal class SubjectSetRegister
    {
        private const int StateUnsubscribed = 0;
        private const int StateNewlySubscribed = 1;
        private const int StateSubscribed = 2;
        private const int StateRefresh = 3;
        private const int StateToggle = 4;

        internal class GridHeader
        {
            public FieldDef[] FieldDefs { get; set; }
        }

        internal class SubjectState
        {
            private readonly GridHeader _gridHeader = new GridHeader();
            public int State { get; set; }

            public SubjectState(int state)
            {
                State = state;
            }

            public GridHeader GridHeader
            {
                get { return _gridHeader; }
            }
        }

        private class EditionData : IGridHeaderRegistry
        {
            private readonly List<Subject.Subject> _subjects;
            private readonly List<GridHeader> _subjectGridHeaders;

            public EditionData(List<Subject.Subject> subjects, List<GridHeader> subjectGridHeaders)
            {
                _subjects = subjects;
                _subjectGridHeaders = subjectGridHeaders;
            }

            public List<Subject.Subject> Subjects
            {
                get { return _subjects; }
            }

            public void SetGridHeader(int sid, FieldDef[] headerDefs)
            {
                _subjectGridHeaders[sid].FieldDefs = headerDefs;
            }

            public FieldDef[] GetGridHeader(int sid)
            {
                return _subjectGridHeaders[sid].FieldDefs;
            }
        }

        private readonly object _lock = new object();

        private readonly SortedDictionary<int, EditionData> _subjectSetCache =
            new SortedDictionary<int, EditionData>()
            {
                {0, new EditionData(new List<Subject.Subject>(), new List<GridHeader>())}
            };

        private readonly Dictionary<Subject.Subject, SubjectState> _subjectState =
            new Dictionary<Subject.Subject, SubjectState>();

        private bool _modified = false;

        public void Register(Subject.Subject subject, bool refresh)
        {
            lock (_lock)
            {
                SubjectState state;
                var exists = _subjectState.TryGetValue(subject, out state);
                if (!exists)
                {
                    _modified = true;
                    _subjectState[subject] = new SubjectState(StateNewlySubscribed);
                }
                else
                {
                    if (state.State == StateUnsubscribed)
                    {
                        if ("Quote".Equals(subject.GetComponent(SubjectComponentName.RequestFor)))
                        {
                            _modified = true;
                            state.State = StateToggle;
                        }
                        else
                        {
                            _modified = true;
                            state.State = refresh ? StateRefresh : StateSubscribed;
                        }
                    }
                    else if (refresh && state.State == StateSubscribed)
                    {
                        _modified = true;
                        state.State = StateRefresh;
                    }
                }
            }
        }

        public void Unregister(Subject.Subject subject)
        {
            lock (_lock)
            {
                _modified = true;
                SubjectState state;
                var exists = _subjectState.TryGetValue(subject, out state);
                if (!exists || state.State == StateNewlySubscribed)
                {
                    ClearSubjectState(subject);
                }
                else
                {
                    state.State = StateUnsubscribed;
                }
            }
        }

        public IGridHeaderRegistry GetGridHeaderRegistryByEdition(int edition)
        {
            lock (_lock)
            {
                return _subjectSetCache[edition];
            }
        }

        private void ClearSubjectState(Subject.Subject subject)
        {
            _subjectState.Remove(subject);
        }

        public bool IsCurrentlySubscribed(Subject.Subject subject)
        {
            lock (_lock)
            {
                SubjectState currentState;
                var exists = _subjectState.TryGetValue(subject, out currentState);
                return !(!exists || currentState.State == StateUnsubscribed);
            }
        }

        public List<Subject.Subject> SubjectSetByEdition(int edition)
        {
            if (edition < 0)
            {
                throw new ArgumentException("Edition cannot be negative.");
            }
            if (!_subjectSetCache.ContainsKey(edition))
            {
                throw new ArgumentException("Edition not found in the cache.");
            }
            lock (_lock)
            {
                var keyCollection = _subjectSetCache.Keys.ToList();
                foreach (var subjectSet in keyCollection)
                {
                    if (subjectSet < edition)
                    {
                        _subjectSetCache.Remove(subjectSet);
                    }
                }
                EditionData editionData;
                var exists = _subjectSetCache.TryGetValue(edition, out editionData);
                return !exists ? null : editionData.Subjects;
            }
        }

        public SubscriptionSync NextSubscriptionSync()
        {
            lock (_lock)
            {
                if (!_modified) return null;
                _modified = false;
                var subjectSet = CurrentSubjectSetSorted();
                var subjectGridHeaders = CurrentGridHeaders(subjectSet);
                var edition = _subjectSetCache.Count == 0 ? 0 : _subjectSetCache.Keys.Last();
                EditionData editionData;
                var exists = _subjectSetCache.TryGetValue(edition, out editionData);
                if (exists && subjectSet.SequenceEqual(editionData.Subjects))
                {
                    var subscriptionSync = CreateSubscriptionSync(edition, subjectSet);
                    if (!subscriptionSync.HasControls()) return null; //unchanged with no controls so send nothing
                    subscriptionSync.SetChangedEdition(false);
                    return subscriptionSync;
                }
                else
                {
                    _subjectSetCache[++edition] = new EditionData(subjectSet, subjectGridHeaders);
                    var subscriptionSync = CreateSubscriptionSync(edition, subjectSet);
                    CleanUnsubscribeState();
                    return subscriptionSync;
                }
            }
        }

        private List<GridHeader> CurrentGridHeaders(List<Subject.Subject> subjectSet)
        {
            var gridHeaders = new List<GridHeader>(subjectSet.Count);
            foreach (var subject in subjectSet)
            {
                gridHeaders.Add(_subjectState[subject].GridHeader);
            }
            return gridHeaders;
        }

        private void CleanUnsubscribeState()
        {
            var toRemove = (from subjectState in _subjectState
                where subjectState.Value.State == StateUnsubscribed
                select subjectState.Key).ToList();
            foreach (var subject in toRemove)
            {
                _subjectState.Remove(subject);
            }
        }

        private List<Subject.Subject> CurrentSubjectSetSorted()
        {
            var subjectSet = new List<Subject.Subject>();
            foreach (var subjectState in _subjectState)
            {
                if (subjectState.Value.State != StateUnsubscribed)
                {
                    subjectSet.Add(subjectState.Key);
                }
            }
            subjectSet.Sort(new RequestSubjectComparator());
            return subjectSet;
        }

        private SubscriptionSync CreateSubscriptionSync(int edition, List<Subject.Subject> subjectSet)
        {
            SubscriptionSync subscriptionSync = new SubscriptionSync(edition, subjectSet);
            AddSubscriptionControls(subscriptionSync);
            return subscriptionSync;
        }

        private void AddSubscriptionControls(SubscriptionSync subscriptionSync)
        {
            var sid = 0;
            foreach (var subject in subscriptionSync.Subjects)
            {
                SubjectState currentState;
                var exists = _subjectState.TryGetValue(subject, out currentState);
                if (!exists) break;
                if (currentState.State == StateRefresh)
                {
                    subscriptionSync.AddControl(sid, ControlOperation.Refresh);
                }
                else if (currentState.State == StateToggle)
                {
                    subscriptionSync.AddControl(sid, ControlOperation.Toggle);
                }
                _subjectState[subject].State = StateSubscribed;
                sid++;
            }
        }

        //only for test
        internal Dictionary<Subject.Subject, SubjectState> GetSubjectState()
        {
            return _subjectState;
        }
    }

    internal class GridHeaderRegistry : IGridHeaderRegistry
    {
        private readonly Dictionary<Subject.Subject, FieldDef[]> _subjectGridHeaders;
        private readonly List<Subject.Subject> _subjectSetByEdition;

        public GridHeaderRegistry(Dictionary<Subject.Subject, FieldDef[]> subjectGridHeaders,
            List<Subject.Subject> subjectSetByEdition)
        {
            _subjectGridHeaders = subjectGridHeaders;
            _subjectSetByEdition = subjectSetByEdition;
        }

        public void SetGridHeader(int sid, FieldDef[] headerDefs)
        {
            _subjectGridHeaders[_subjectSetByEdition[sid]] = headerDefs;
        }

        public FieldDef[] GetGridHeader(int sid)
        {
            FieldDef[] gridHeader;
            var tryGetValue = _subjectGridHeaders.TryGetValue(_subjectSetByEdition[sid], out gridHeader);
            return tryGetValue ? gridHeader : null;
        }
    }
}