using System;
using System.Collections.Generic;
using System.Linq;
using BidFX.Public.API.Price.Plugin.Pixie.Fields;
using BidFX.Public.API.Price.Plugin.Pixie.Messages;
using BidFX.Public.API.Price.Subject;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    public class SubjectSetRegister
    {
        private const int StateUnsubscribed = 0;
        private const int StateNewlySubscribed = 1;
        private const int StateSubscribed = 2;
        private const int StateRefresh = 3;
        private const int StateToggle = 4;

        private readonly object _lock = new object();

        private readonly SortedDictionary<int, List<Subject.Subject>> _subjectSetCache =
            new SortedDictionary<int, List<Subject.Subject>>();

        private readonly Dictionary<Subject.Subject, int> _subjectState = new Dictionary<Subject.Subject, int>();

        private readonly Dictionary<Subject.Subject, FieldDef[]> _subjectGridHeaders =
            new Dictionary<Subject.Subject, FieldDef[]>();

        private bool _modified = false;

        public void Register(Subject.Subject subject, bool refresh)
        {
            lock (_lock)
            {
                _modified = true;
                int state;
                var exists = _subjectState.TryGetValue(subject, out state);
                if (!exists)
                {
                    _subjectState[subject] = StateNewlySubscribed;
                }
                else
                {
                    if (state == StateUnsubscribed)
                    {
                        if ("RFQ".Equals(subject.LookupValue("QuoteStyle")))
                        {
                            _subjectState[subject] = StateToggle;
                        }
                        else
                        {
                            _subjectState[subject] = refresh ? StateRefresh : StateSubscribed;
                        }
                    }
                    else if (refresh && state == StateSubscribed)
                    {
                        _subjectState[subject] = StateRefresh;
                    }
                }
            }
        }

        public void Unregister(Subject.Subject subject)
        {
            lock (_lock)
            {
                _modified = true;
                int currentState;
                var exists = _subjectState.TryGetValue(subject, out currentState);
                if (!exists) return;
                if (currentState == StateNewlySubscribed)
                {
                    _subjectState.Remove(subject);
                }
                else
                {
                    ClearSubjectState(subject);
                    ClearHeaders(subject);
                }
            }
        }

        public IGridHeaderRegistry GetGridHeaderRegistryByEdition(List<Subject.Subject> subjectSetByEdition)
        {
            return new GridHeaderRegistry(_subjectGridHeaders, subjectSetByEdition);
        }

        private void ClearSubjectState(Subject.Subject subject)
        {
            _subjectState[subject] = StateUnsubscribed;
        }

        private void ClearHeaders(Subject.Subject subject)
        {
            _subjectGridHeaders.Remove(subject);
        }

        public bool IsCurrentlySubscribed(Subject.Subject subject)
        {
            lock (_lock)
            {
                int currentState;
                var exists = _subjectState.TryGetValue(subject, out currentState);
                return exists & currentState != StateUnsubscribed;
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
                return _subjectSetCache[edition];
            }
        }

        public SubscriptionSync NextSubscriptionSync()
        {
            lock (_lock)
            {
                if (!_modified) return null;
                _modified = false;
                var subjectSet = CurrentSubjectSetSorted();
                var edition = _subjectSetCache.Count == 0 ? 0 : _subjectSetCache.Keys.Last();
                if (_subjectSetCache.ContainsKey(edition) && subjectSet.Equals(_subjectSetCache[edition]))
                {
                    var subscriptionSync = CreateSubscriptionSync(edition, subjectSet);
                    if (!subscriptionSync.HasControls()) return null; //unchanged with no controls so send nothing
                    subscriptionSync.SetChangedEdition(false);
                    return subscriptionSync;
                }
                else
                {
                    _subjectSetCache[++edition] = subjectSet;
                    var subscriptionSync = CreateSubscriptionSync(edition, subjectSet);
                    CleanUnsubscribeState();
                    return subscriptionSync;
                }
            }
        }

        private void CleanUnsubscribeState()
        {
            var toRemove = (from subjectState in _subjectState
                where subjectState.Value == StateUnsubscribed
                select subjectState.Key).ToList();
            foreach (var subject in toRemove)
            {
                _subjectState.Remove(subject);
                ClearHeaders(subject);
            }
        }

        private List<Subject.Subject> CurrentSubjectSetSorted()
        {
            var subjectSet = new List<Subject.Subject>();
            foreach (var subjectState in _subjectState)
            {
                if (subjectState.Value != StateUnsubscribed)
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
                int currentState;
                var exists = _subjectState.TryGetValue(subject, out currentState);
                if (!exists) break;
                if (currentState == StateRefresh)
                {
                    subscriptionSync.AddControl(sid, ControlOperation.Refresh);
                }
                else if (currentState == StateToggle)
                {
                    subscriptionSync.AddControl(sid, ControlOperation.Toggle);
                }
                sid++;
            }
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