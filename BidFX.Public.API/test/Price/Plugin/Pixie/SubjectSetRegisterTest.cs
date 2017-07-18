using System;
using System.Collections.Generic;
using System.Linq;
using BidFX.Public.API.Price.Plugin.Pixie.Fields;
using BidFX.Public.API.Price.Plugin.Pixie.Messages;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    [TestFixture]
    public class SubjectSetRegisterTest
    {
        private static readonly Subject.Subject Eurchr2Mm = new Subject.Subject(
            "BuySideAccount=DYMON,AllocBuySideAccount=DYMONCITI,AllocQuantity=2000000,AssetClass=Fx,Currency=EUR,Level=1" +
            ",Symbol=EURCHF,Quantity=2000000.00,RequestFor=Stream,LiquidityProvider=JEFFX,DealType=Spot,Tenor=Spot");

        private static readonly Subject.Subject Eurgbp1Mm = new Subject.Subject(
            "BuySideAccount=CIC,AllocBuySideAccount=CIC,AllocQuantity=1000000,AssetClass=Fx,Currency=EUR,Level=1" +
            ",Symbol=EURGBP,Quantity=1000000.00,RequestFor=Stream,LiquidityProvider=MSFX,DealType=Spot,Tenor=Spot");

        private static readonly Subject.Subject Eurgbp5Mm = new Subject.Subject(
            "BuySideAccount=CIC,AllocBuySideAccount=CIC,AllocQuantity=5000000,AssetClass=Fx,Currency=EUR,Level=1" +
            ",Symbol=EURGBP,Quantity=5000000.00,RequestFor=Stream,LiquidityProvider=MSFX,DealType=Spot,Tenor=Spot");

        private static readonly Subject.Subject Gbpusd1Mm = new Subject.Subject(
            "BuySideAccount=AAA,AllocAccount=AAA,AllocQuantity=1000000,AssetClass=Fx,Currency=USD,Level=1" +
            ",Symbol=GBPUSD,Quantity=1000000.00,RequestFor=Quote,LiquidityProvider=MSFX,DealType=Spot,Tenor=Spot");

        private static readonly Subject.Subject Subject0 = Eurchr2Mm;
        private static readonly Subject.Subject Subject1 = Eurgbp1Mm;
        private static readonly Subject.Subject Subject2 = Eurgbp5Mm;
        private static readonly Subject.Subject Subject3 = Gbpusd1Mm;
        private static readonly Subject.Subject RfsSubject1 = Subject1;
        private static readonly Subject.Subject RfqSubject3 = Subject3;

        private static readonly FieldDef FieldDef1 = new FieldDef
        {
            Fid = 1,
            Type = FieldType.String,
            Encoding = FieldEncoding.VarintString,
            Name = "field1"
        };

        private static readonly FieldDef FieldDef2 = new FieldDef
        {
            Fid = 2,
            Type = FieldType.Integer,
            Encoding = FieldEncoding.Fixed4,
            Name = "field2"
        };

        private static readonly FieldDef[] FieldDefs = {FieldDef1, FieldDef2};

        private SubjectSetRegister _register;

        [SetUp]
        public void Before()
        {
            _register = new SubjectSetRegister();
        }

        [Test]
        public void InitiallyNoSubjectsAreCurrentlySubscribed()
        {
            Assert.AreEqual(false, _register.IsCurrentlySubscribed(Subject0));
            Assert.AreEqual(false, _register.IsCurrentlySubscribed(Subject1));
            Assert.AreEqual(false, _register.IsCurrentlySubscribed(Subject2));
        }

        [Test]
        public void RegisteringASubjectMakesItCurrentlySubscribed()
        {
            _register.Register(Subject0, false);
            Assert.AreEqual(true, _register.IsCurrentlySubscribed(Subject0));
        }

        [Test]
        public void RegisteringManySubjectsMakesThemCurrentlySubscribed()
        {
            _register.Register(Subject0, false);
            _register.Register(Subject1, false);
            _register.Register(Subject2, false);
            Assert.AreEqual(true, _register.IsCurrentlySubscribed(Subject0));
            Assert.AreEqual(true, _register.IsCurrentlySubscribed(Subject1));
            Assert.AreEqual(true, _register.IsCurrentlySubscribed(Subject2));
        }

        [Test]
        public void UnregisteringASubjectMakesItNotCurrentlySubscribed()
        {
            _register.Register(Subject0, false);
            _register.Register(Subject1, false);
            _register.Register(Subject2, false);

            _register.Unregister(Subject0);
            _register.Unregister(Subject1);
            Assert.AreEqual(false, _register.IsCurrentlySubscribed(Subject0));
            Assert.AreEqual(false, _register.IsCurrentlySubscribed(Subject1));
            Assert.AreEqual(true, _register.IsCurrentlySubscribed(Subject2));
            _register.Unregister(Subject2);
            Assert.AreEqual(false, _register.IsCurrentlySubscribed(Subject2));
        }

        [Test]
        public void NextSubscriptionSyncReturnsNullWhenThereHaveBeenNoRegistryOperations()
        {
            Assert.AreEqual(null, _register.NextSubscriptionSync());
        }

        [Test]
        public void NextSubscriptionSyncReturnsNullWhenThereHaveBeenNoRegistryOperationsSinceTheLastCall()
        {
            _register.Register(Subject0, false);
            _register.NextSubscriptionSync();
            Assert.AreEqual(null, _register.NextSubscriptionSync());
        }

        [Test]
        public void NextSubscriptionSyncAfterRegisteringSubjectsProvidesThoseSubjects()
        {
            _register.Register(Subject0, false);
            _register.Register(Subject1, false);
            Assert.AreEqual(new List<Subject.Subject> {Subject0, Subject1}, _register.NextSubscriptionSync().Subjects);
        }

        [Test]
        public void NextSubscriptionSyncAfterRegisteringSubjectsProvidesTheLatestCompleteSet()
        {
            _register.Register(Subject0, false);
            Assert.AreEqual(new List<Subject.Subject> {Subject0}, _register.NextSubscriptionSync().Subjects);
            _register.Register(Subject1, false);
            Assert.AreEqual(new List<Subject.Subject> {Subject0, Subject1}, _register.NextSubscriptionSync().Subjects);
            _register.Register(Subject2, false);
            Assert.AreEqual(new List<Subject.Subject> {Subject0, Subject1, Subject2},
                _register.NextSubscriptionSync().Subjects);
            _register.Unregister(Subject1);
            Assert.AreEqual(new List<Subject.Subject> {Subject0, Subject2}, _register.NextSubscriptionSync().Subjects);
        }

        [Test]
        public void NextSubscriptionSyncAfterRegisteringThenUnRegisteringWillBeNull()
        {
            _register.Register(Subject0, false);
            _register.Unregister(Subject0);
            Assert.AreEqual(null, _register.NextSubscriptionSync());
        }

        [Test]
        public void NextSubscriptionSyncReturnsAnUncompressedMessageThatWeCanCompressLater()
        {
            _register.Register(Subject0, false);
            SubscriptionSync subscriptionSync = _register.NextSubscriptionSync();
            Assert.AreEqual(false, subscriptionSync.IsCompressed());
        }

        [Test]
        public void NextSubscriptionSyncAfterFirstRegistrationHasEdition1()
        {
            _register.Register(Subject0, false);
            Assert.AreEqual(1, _register.NextSubscriptionSync().Edition);
        }

        [Test]
        public void NextSubscriptionSyncHasANewEditionNumberAfterEachChangeInvolvingANewlyRegisteredSubject()
        {
            _register.Register(Subject0, false);
            Assert.AreEqual(1, _register.NextSubscriptionSync().Edition);
            _register.Register(Subject1, false);
            Assert.AreEqual(2, _register.NextSubscriptionSync().Edition);
            _register.Register(Subject2, false);
            Assert.AreEqual(3, _register.NextSubscriptionSync().Edition);
            Assert.AreEqual(null, _register.NextSubscriptionSync());
        }

        [Test]
        public void NextSubscriptionSyncHasANewEditionNumberAfterEachChangeInvolvingAnUnRegisteredSubject()
        {
            _register.Register(Subject0, false);
            Assert.AreEqual(1, _register.NextSubscriptionSync().Edition);
            _register.Unregister(Subject0);
            Assert.AreEqual(2, _register.NextSubscriptionSync().Edition);
            Assert.AreEqual(null, _register.NextSubscriptionSync());
        }

        [Test]
        public void NextSubscriptionSyncProvidesTheRegisteredSubjectsSortedBySymbolAndQuantity()
        {
            _register.Register(Subject2, false);
            _register.Register(Subject0, false);
            _register.Register(Subject3, false);
            _register.Register(Subject1, false);
            var subscriptionSync = _register.NextSubscriptionSync();
            Assert.AreEqual(new List<Subject.Subject> {Subject0, Subject1, Subject2, Subject3},
                subscriptionSync.Subjects);
        }

        [Test]
        public void NextSubscriptionSyncAfterDuplicateNonRefreshRegistrationsReturnsNullAsNothingHasChanged()
        {
            _register.Register(Subject0, false);
            _register.Register(Subject1, false);
            Assert.AreEqual(new List<Subject.Subject> {Subject0, Subject1}, _register.NextSubscriptionSync().Subjects);

            _register.Register(Subject0, false);
            Assert.AreEqual(null, _register.NextSubscriptionSync());

            _register.Register(Subject1, false);
            Assert.AreEqual(null, _register.NextSubscriptionSync());

            _register.Register(Subject1, false);
            _register.Register(Subject1, false);
            _register.Register(Subject1, false);
            Assert.AreEqual(null, _register.NextSubscriptionSync());
        }

        [Test]
        public void NextSubscriptionSyncAfterUnRegisteringAllSubjectsGivesAnEmptySubjectList()
        {
            _register.Register(Subject0, false);
            _register.Register(Subject1, false);
            _register.Register(Subject2, false);
            Assert.AreEqual(new List<Subject.Subject> {Subject0, Subject1, Subject2},
                _register.NextSubscriptionSync().Subjects);

            _register.Unregister(Subject0);
            _register.Unregister(Subject2);
            _register.Unregister(Subject1);
            var subscriptionSync = _register.NextSubscriptionSync();
            Assert.AreEqual(new List<Subject.Subject> { }, subscriptionSync.Subjects);
            Assert.AreEqual(true, subscriptionSync.IsChangedEdition());
        }

        [Test]
        public void TestSubjectSetByEditionMayNotBeGivenANegativeEditionNumber()
        {
            try
            {
                _register.SubjectSetByEdition(-1);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void TestSubjectSetByEditionReturnsNullInitially()
        {
            try
            {
                Assert.AreEqual(null, _register.SubjectSetByEdition(1));
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void NextSubscriptionSyncUpdatesTheSubjectSetByEdition()
        {
            _register.Register(Subject0, false);
            _register.Register(Subject1, false);
            var subscriptionSync = _register.NextSubscriptionSync();
            List<Subject.Subject> subjectSet = _register.SubjectSetByEdition(subscriptionSync.Edition);
            Assert.AreEqual(new List<Subject.Subject> {Subject0, Subject1}, subjectSet);
            Assert.AreEqual(subscriptionSync.Subjects, subjectSet);

            _register.Register(Subject2, false);
            subscriptionSync = _register.NextSubscriptionSync();
            subjectSet = _register.SubjectSetByEdition(subscriptionSync.Edition);
            Assert.AreEqual(new List<Subject.Subject> {Subject0, Subject1, Subject2}, subjectSet);
            Assert.AreEqual(subscriptionSync.Subjects, subjectSet);
        }

        [Test]
        public void SubjectSetByEditionGivesAccessToPreviousSubjectSetsAreArchivedUseInPriceSyncHandling()
        {
            _register.Register(Subject0, false);
            SubscriptionSync subscriptionSync1 = _register.NextSubscriptionSync();
            _register.Register(Subject1, false);
            SubscriptionSync subscriptionSync2 = _register.NextSubscriptionSync();
            _register.Register(Subject2, false);
            SubscriptionSync subscriptionSync3 = _register.NextSubscriptionSync();

            List<Subject.Subject> subjectSet1 = _register.SubjectSetByEdition(1);
            List<Subject.Subject> subjectSet2 = _register.SubjectSetByEdition(2);
            List<Subject.Subject> subjectSet3 = _register.SubjectSetByEdition(3);

            Assert.AreEqual(subscriptionSync1.Subjects, subjectSet1);
            Assert.AreEqual(subscriptionSync2.Subjects, subjectSet2);
            Assert.AreEqual(subscriptionSync3.Subjects, subjectSet3);
        }

        [Test]
        public void SubjectSetByEditionRemovesAllOlderEditionSubjectSets()
        {
            _register.Register(Subject0, false);
            _register.NextSubscriptionSync();
            _register.Register(Subject1, false);
            _register.NextSubscriptionSync();
            _register.Register(Subject2, false);
            _register.NextSubscriptionSync();

            List<Subject.Subject> subjectSet3 = _register.SubjectSetByEdition(3);
            try
            {
                _register.SubjectSetByEdition(1);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
            }
            try
            {
                _register.SubjectSetByEdition(2);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
            }
            Assert.AreSame(subjectSet3, _register.SubjectSetByEdition(3));
            Assert.AreSame(subjectSet3, _register.SubjectSetByEdition(3));
        }

        [Test]
        public void GridHeaderRegisterAllowToStoreAndRetrieveHeadersAssociatedWithAGrid()
        {
            _register.Register(Subject0, false);
            _register.NextSubscriptionSync();
            _register.Register(Subject1, false);
            _register.NextSubscriptionSync();
            var headerRegisterEd2 = _register.GetGridHeaderRegistryByEdition(2);
            headerRegisterEd2.SetGridHeader(1, FieldDefs);
            _register.Unregister(Subject0);
            _register.NextSubscriptionSync();
            Assert.IsNull(_register.GetGridHeaderRegistryByEdition(1).GetGridHeader(0));
            Assert.AreEqual(FieldDefs, _register.GetGridHeaderRegistryByEdition(2).GetGridHeader(1));
            Assert.AreEqual(FieldDefs, _register.GetGridHeaderRegistryByEdition(3).GetGridHeader(0));
        }

        [Test]
        public void DeregisteringASubscriptionClearsItsAssociatedHeaders()
        {
            _register.Register(Subject0, false);
            _register.NextSubscriptionSync();
            _register.Register(Subject1, false);
            _register.NextSubscriptionSync();
            var headerRegisterEd2 = _register.GetGridHeaderRegistryByEdition(2);
            headerRegisterEd2.SetGridHeader(0, FieldDefs);
            _register.Unregister(Subject0);
            _register.NextSubscriptionSync();
            List<Subject.Subject> subjectSet3 = _register.SubjectSetByEdition(3);
            Assert.IsFalse(subjectSet3.Contains(Subject0));
            var headerRegisterEd3 = _register.GetGridHeaderRegistryByEdition(3);
            for (int i = 0; i < subjectSet3.Count; i++)
            {
                Assert.IsNull(headerRegisterEd3.GetGridHeader(i));
            }
            _register.NextSubscriptionSync();
            try
            {
                List<Subject.Subject> subjectSet4 = _register.SubjectSetByEdition(4);
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
            }
        }

        [Test]
        public void SubjectSetByEditionGivesMultipleAccessToOldSubjectSetEditions()
        {
            _register.Register(Subject0, false);
            _register.NextSubscriptionSync();
            _register.Register(Subject1, false);
            _register.NextSubscriptionSync();
            _register.Register(Subject2, false);
            _register.NextSubscriptionSync();

            var subjectSet1 = _register.SubjectSetByEdition(1);
            Assert.AreSame(subjectSet1, _register.SubjectSetByEdition(1));
            Assert.AreSame(subjectSet1, _register.SubjectSetByEdition(1));

            var subjectSet2 = _register.SubjectSetByEdition(2);
            Assert.AreSame(subjectSet2, _register.SubjectSetByEdition(2));
            Assert.AreSame(subjectSet2, _register.SubjectSetByEdition(2));
        }

        [Test]
        public void testSubscribeThenUnsubQuickly_thisShouldNeverResultInASubscriptionSync()
        {
            _register.Register(Subject0, false);
            _register.Register(Subject1, true);
            _register.Unregister(Subject0);
            _register.Unregister(Subject1);

            Assert.IsNull(_register.NextSubscriptionSync());

            _register.Register(Subject0, false);
            _register.Register(Subject1, true);
            _register.Unregister(Subject0);
            _register.Unregister(Subject1);

            Assert.IsNull(_register.NextSubscriptionSync());

            _register.Register(Subject2, false);
            _register.Unregister(Subject2);

            Assert.IsNull(_register.NextSubscriptionSync());

            Assert.IsFalse(_register.GetSubjectState().ContainsKey(Subject0));
            Assert.IsFalse(_register.GetSubjectState().ContainsKey(Subject1));
            Assert.IsFalse(_register.GetSubjectState().ContainsKey(Subject2));
        }

        [Test]
        public void test_comparator_with_messed_up_subjects()
        {
            Subject.Subject s1 = new Subject.Subject("Account=DYMONCITI,Currency=EUR,Exchange=OTC");
            Subject.Subject s2 = new Subject.Subject("AllocAccount=DYMONCITI,Currency=EUR");
            Subject.Subject s3 = new Subject.Subject("Account=DYMONCITI,Currency=EUR,Exchange=OTC,Quantity=2000000.00");
            Subject.Subject s4 = new Subject.Subject("Symbol=EURCHF,Quantity=500000.00");
            Subject.Subject s5 = new Subject.Subject("Currency=EUR,Symbol=EURCHF,Quantity=2000000.00");
            Subject.Subject s6 = new Subject.Subject("Symbol=EURCHF,Quantity=2000000.00");
            Subject.Subject s7 = new Subject.Subject("Quantity=1230000.00,Symbol=EURGBP");
            Subject.Subject s8 = new Subject.Subject("Symbol=EURGBP,Quantity=1230000.00");
            Subject.Subject s9 = new Subject.Subject("Symbol=EURGBP,Quantity=5000000.00");
            Subject.Subject s10 = new Subject.Subject("Symbol=USDJPY,Account=DYMONCITI");

            Assert.AreEqual(s7, s8);
            List<Subject.Subject> input = new List<Subject.Subject> {s1, s2, s3, s4, s5, s6, s7, s8, s9, s10};
            for (int i = 0; i < 100; i++)
            {
                SubjectSetRegister register = new SubjectSetRegister();
                var rnd = new Random();
                var orderedEnumerable = input.OrderBy(j => rnd.Next());
                foreach (Subject.Subject subject in
                    orderedEnumerable)
                {
                    register.Register(subject, false);
                }
                Assert.AreEqual(new List<Subject.Subject> {s1, s2, s3, s4, s5, s6, s7, s9, s10},
                    register.NextSubscriptionSync().Subjects);
            }
        }

        [Test]
        public void test_comparator_with_realistic_subjects()
        {
            List<Subject.Subject> sortedSubjects = new List<Subject.Subject>(RealSubscriptionsExample.SortedSubjects);
            for (int i = 0; i < 100; i++)
            {
                var rnd = new Random();
                var shuffledSubjects = sortedSubjects.OrderBy(j => rnd.Next());
                SubjectSetRegister subjectSetRegister = new SubjectSetRegister();
                foreach (Subject.Subject subject in
                    shuffledSubjects)
                {
                    subjectSetRegister.Register(subject, false);
                }
                Assert.AreEqual(sortedSubjects, subjectSetRegister.NextSubscriptionSync().Subjects);
            }
        }

        [Test]
        public void testRegisteringSomeSubjects_resultsInNoControls()
        {
            _register.Register(Subject0, false);
            _register.Register(Subject1, false);
            _register.Register(Subject2, false);
            Assert.AreEqual(new List<Subject.Subject> { }, _register.NextSubscriptionSync().Controls);
        }

        [Test]
        public void TestDuplicateNonRefreshRegistrationsDoNotAddControls()
        {
            _register.Register(Subject0, false);
            _register.Register(Subject0, false);

            _register.Register(Subject1, false);
            _register.Register(Subject1, false);
            _register.Register(Subject1, false);

            Assert.AreEqual(new List<Subject.Subject> { }, _register.NextSubscriptionSync().Controls);
        }

        [Test]
        public void NextSubscriptionSyncAfterRefreshRegistrationAddsARefreshControlToSync()
        {
            _register.Register(Subject0, false);
            _register.NextSubscriptionSync();
            _register.Register(Subject0, true);
            Assert.AreEqual(new Dictionary<int, ControlOperation> {{0, ControlOperation.Refresh}},
                _register.NextSubscriptionSync().Controls);
        }

        [Test]
        public void NextSubscriptionSyncAfterRefreshRegistrationAddsARefreshControlToSyncWithCorrectSid()
        {
            _register.Register(Subject0, false);
            _register.Register(Subject1, false);
            _register.Register(Subject2, false);
            _register.Register(Subject3, false);
            _register.NextSubscriptionSync();

            _register.Register(Subject2, true);
            Assert.AreEqual(new Dictionary<int, ControlOperation> {{2, ControlOperation.Refresh}},
                _register.NextSubscriptionSync().Controls);
        }

        [Test]
        public void NextSubscriptionSyncAfterRefreshRegistrationOnlyResultsInTheEditionBeingUnchanged()
        {
            _register.Register(Subject0, false);
            _register.NextSubscriptionSync();
            _register.Register(Subject0, true);
            Assert.AreEqual(false, _register.NextSubscriptionSync().IsChangedEdition());
        }

        [Test]
        public void NextSubscriptionSyncAfterRefreshRegistrationOnlyResultsInTheEditionNumberBeingReused()
        {
            _register.Register(Subject0, false);
            SubscriptionSync subscriptionSync1 = _register.NextSubscriptionSync();
            _register.Register(Subject0, true);
            SubscriptionSync subscriptionSync2 = _register.NextSubscriptionSync();
            Assert.AreEqual(subscriptionSync1.Edition, subscriptionSync2.Edition);
        }

        [Test]
        public void NextSubscriptionSyncRefreshControlsAreNotRemovedAsAResultsOfANonRefreshRegistration()
        {
            _register.Register(Subject0, false);
            _register.NextSubscriptionSync();

            _register.Register(Subject0, true); // causes refresh
            _register.Register(Subject0, false); // no refresh required by does no revert the refresh
            Assert.AreEqual(new Dictionary<int, ControlOperation> {{0, ControlOperation.Refresh}},
                _register.NextSubscriptionSync().Controls);
        }

        [Test]
        public void testRefreshingSubjectsWhichAreNotAlreadyRegistered_noControlsAndNewEdition()
        {
            _register.Register(Subject0, false);
            _register.Register(Subject0, true);
            _register.Register(Subject1, true);
            Assert.AreEqual(new List<Subject.Subject> { }, _register.NextSubscriptionSync().Controls);

            _register.Register(Subject2, true);
            Assert.AreEqual(new List<Subject.Subject> { }, _register.NextSubscriptionSync().Controls);
        }

        [Test]
        public void TestRetrievingAfterNoChangeReturnsNull()
        {
            Assert.IsNull(_register.NextSubscriptionSync());

            _register.Register(Subject0, false);
            _register.NextSubscriptionSync();

            Assert.IsNull(_register.NextSubscriptionSync());

            _register.Register(Subject0, false);
            Assert.IsNull(_register.NextSubscriptionSync());
        }

        [Test]
        public void testFirstSubscribeThenUnsubThenSubscribe_conflatedIntoOneNewSubscriptionWithNoControls()
        {
            _register.Register(Subject0, false);
            _register.Unregister(Subject0);
            _register.Register(Subject0, false);
            var subscriptionSync = _register.NextSubscriptionSync();
            Assert.AreEqual(new List<Subject.Subject> { }, subscriptionSync.Controls);
            Assert.AreEqual(new List<Subject.Subject> {Subject0}, subscriptionSync.Subjects);
        }

        [Test]
        public void testSubUnsubAndSubWithRefresh_conflatedIntoOneSubWithNoControls()
        {
            _register.Register(Subject0, false);
            _register.Unregister(Subject0);
            _register.Register(Subject0, true);

            SubscriptionSync subscriptionSync = _register.NextSubscriptionSync();
            Assert.AreEqual(new List<Subject.Subject> { }, subscriptionSync.Controls);
            Assert.AreEqual(new List<Subject.Subject> {Subject0}, subscriptionSync.Subjects);
        }

        [Test]
        public void TestRfsSubjectIsConfiguredRight()
        {
            Assert.AreEqual("Stream", RfsSubject1.LookupValue("RequestFor"));
        }

        [Test]
        public void TestRfqSubjectIsConfiguredRight()
        {
            Assert.AreEqual("Quote", RfqSubject3.LookupValue("RequestFor"));
        }

        [Test]
        public void testUnsubbingAndSubscribingAgainToAnRFS_resultInNotSubscriptionSyncRequired()
        {
            _register.Register(RfsSubject1, false);
            _register.NextSubscriptionSync();

            _register.Unregister(RfsSubject1);
            _register.Register(RfsSubject1, false);
            Assert.AreEqual(null, _register.NextSubscriptionSync());
        }

        [Test]
        public void NextSubscriptionSyncAfterRfqToggleOnlyAddsUnsubscribeAndRefreshControl()
        {
            _register.Register(RfqSubject3, false);
            _register.NextSubscriptionSync();

            _register.Unregister(RfqSubject3);
            _register.Register(RfqSubject3, false);
            Assert.AreEqual(new Dictionary<int, ControlOperation> {{0, ControlOperation.Toggle}},
                _register.NextSubscriptionSync().Controls);
        }

        [Test]
        public void NextSubscriptionSyncAfterRfqToggleOnlyAddsUnsubscribeAndRefreshControlEvenIfRefreshed()
        {
            _register.Register(RfqSubject3, false);
            _register.NextSubscriptionSync();

            _register.Unregister(RfqSubject3);
            _register.Register(RfqSubject3, false);
            _register.Register(RfqSubject3, true); // extra subscription refresh attempt
            Assert.AreEqual(new Dictionary<int, ControlOperation> {{0, ControlOperation.Toggle}},
                _register.NextSubscriptionSync().Controls);
        }

        [Test]
        public void NextSubscriptionSyncAfterRfqToggleOnlyAddsUnsubscribeAndRefreshControlWithCorrectSid()
        {
            _register.Register(Subject0, false);
            _register.Register(Subject1, false);
            _register.Register(RfqSubject3, false);
            _register.Register(Subject3, false);
            _register.NextSubscriptionSync();

            _register.Unregister(RfqSubject3);
            _register.Register(RfqSubject3, false);
            Assert.AreEqual(new Dictionary<int, ControlOperation> {{2, ControlOperation.Toggle}},
                _register.NextSubscriptionSync().Controls);
        }

        [Test]
        public void NextSubscriptionSyncAfterRfqToggleOnlyResultsInTheEditionBeingUnchanged()
        {
            _register.Register(RfqSubject3, false);
            _register.NextSubscriptionSync();

            _register.Unregister(RfqSubject3);
            _register.Register(RfqSubject3, false);
            Assert.AreEqual(false, _register.NextSubscriptionSync().IsChangedEdition());
        }

        [Test]
        public void NextSubscriptionSyncAfterRfqToggleOnlyResultsInTheEditionNumberBeingReused()
        {
            _register.Register(RfqSubject3, false);
            SubscriptionSync subscriptionSync1 = _register.NextSubscriptionSync();

            _register.Unregister(RfqSubject3);
            _register.Register(RfqSubject3, false);
            SubscriptionSync subscriptionSync2 = _register.NextSubscriptionSync();
            Assert.AreEqual(subscriptionSync1.Edition, subscriptionSync2.Edition);
        }

        [Test]
        public void NextSubscriptionSyncAfterUnsubAndSubMultipleInstruments_controlsAreInCorrectOrder()
        {
            _register.Register(Subject2, false);
            _register.Register(Subject0, false);
            _register.Register(RfqSubject3, false);
            _register.Register(RfsSubject1, false);
            Assert.AreEqual(new List<Subject.Subject> {Subject0, RfsSubject1, Subject2, RfqSubject3},
                _register.NextSubscriptionSync().Subjects);

            _register.Unregister(Subject0);
            _register.Unregister(RfsSubject1);
            _register.Unregister(Subject2);
            _register.Unregister(RfqSubject3);
            _register.Register(Subject0, false); // revert to subscribed
            _register.Register(RfsSubject1, true); // forces refresh
            _register.Register(Subject2, false); // revert to subscribed
            _register.Register(RfqSubject3, false); // forces toggle unsubscribe and subscribe
            SubscriptionSync subscriptionSync = _register.NextSubscriptionSync();

            Assert.AreEqual(false, subscriptionSync.IsChangedEdition());
            Assert.AreEqual(
                new Dictionary<int, ControlOperation> {{1, ControlOperation.Refresh}, {3, ControlOperation.Toggle}},
                subscriptionSync.Controls);
        }

        [Test]
        public void NextSubscriptionSyncAfterUnsubAndSubMultiples_correctIndicesUsedInControls()
        {
            _register.Register(Subject2, false);
            _register.Register(Subject0, false);
            _register.Register(RfqSubject3, false);
            _register.Register(RfsSubject1, false);
            List<Subject.Subject> subjects = new List<Subject.Subject> {Subject0, RfsSubject1, Subject2, RfqSubject3};
            Assert.AreEqual(subjects, _register.NextSubscriptionSync().Subjects);

            _register.Unregister(Subject0);
            _register.Unregister(Subject1);
            _register.Unregister(Subject2);
            _register.Unregister(RfqSubject3);
            _register.Register(Subject0, false); // revert to subscribed
            _register.Register(RfsSubject1, true); // forces refresh
            _register.Register(Subject2, false); // revert to subscribed
            _register.Register(RfqSubject3, false); // forces toggle unsubscribe and subscribe

            Dictionary<int, ControlOperation> controls = _register.NextSubscriptionSync().Controls;
            Assert.False(controls.ContainsKey(subjects.IndexOf(Subject0)));
            Assert.AreEqual(ControlOperation.Refresh, controls[subjects.IndexOf(RfsSubject1)]);
            Assert.False(controls.ContainsKey(subjects.IndexOf(Subject2)));
            Assert.AreEqual(ControlOperation.Toggle, controls[subjects.IndexOf(RfqSubject3)]);
        }

        [Test]
        public void NextSubscriptionSyncMayCombineSubscriptionSetAdditionsWithControls()
        {
            _register.Register(RfsSubject1, false);
            _register.Register(Subject2, false);
            _register.Register(RfqSubject3, false);
            Assert.AreEqual(new List<Subject.Subject> {RfsSubject1, Subject2, RfqSubject3},
                _register.NextSubscriptionSync().Subjects);

            _register.Register(Subject1, true); // forces refresh
            _register.Register(Subject0, false); // new addition to subscription set
            _register.Unregister(RfqSubject3);
            _register.Register(RfqSubject3, false); // forces toggle unsubscribe and subscribe
            var subscriptionSync = _register.NextSubscriptionSync();

            Assert.AreEqual(true, subscriptionSync.IsChangedEdition());
            Assert.AreEqual(new List<Subject.Subject> {Subject0, RfsSubject1, Subject2, RfqSubject3},
                subscriptionSync.Subjects);
            Assert.AreEqual(
                new Dictionary<int, ControlOperation> {{1, ControlOperation.Refresh}, {3, ControlOperation.Toggle}},
                subscriptionSync.Controls);
        }

        [Test]
        public void NextSubscriptionSyncMayCombineSubscriptionSetDeletionsWithControls()
        {
            _register.Register(Subject0, false);
            _register.Register(RfsSubject1, false);
            _register.Register(Subject2, false);
            _register.Register(RfqSubject3, false);
            Assert.AreEqual(new List<Subject.Subject> {Subject0, RfsSubject1, Subject2, RfqSubject3},
                _register.NextSubscriptionSync().Subjects);

            _register.Register(Subject1, true); // forces refresh
            _register.Unregister(Subject0); // removal from subscription set
            _register.Unregister(RfqSubject3);
            _register.Register(RfqSubject3, false); // forces toggle unsubscribe and subscribe
            var subscriptionSync = _register.NextSubscriptionSync();

            Assert.AreEqual(true, subscriptionSync.IsChangedEdition());
            Assert.AreEqual(new List<Subject.Subject> {RfsSubject1, Subject2, RfqSubject3}, subscriptionSync.Subjects);
            Assert.AreEqual(
                new Dictionary<int, ControlOperation> {{0, ControlOperation.Refresh}, {2, ControlOperation.Toggle}},
                subscriptionSync.Controls);
            Assert.IsFalse(_register.GetSubjectState().ContainsKey(Subject0));
        }

        [Test]
        public void NextSubscriptionSyncAfterNextCall_previousControlsAreClearedOut()
        {
            _register.Register(RfsSubject1, false);
            _register.Register(RfqSubject3, false);

            _register.NextSubscriptionSync();

            _register.Unregister(RfsSubject1);
            _register.Unregister(RfqSubject3);
            _register.Register(RfsSubject1, true);
            _register.Register(RfqSubject3, false);

            Assert.AreEqual(2, _register.NextSubscriptionSync().Controls.Count);

            _register.Register(Subject0, false);
            Assert.AreEqual(true, _register.NextSubscriptionSync().Controls.Count == 0);
        }

        [Test]
        public void TestSpuriousUnsubDoesNotAffectSubscriptions()
        {
            _register.Unregister(Subject0);
            Assert.IsNull(_register.NextSubscriptionSync());

            _register.Unregister(Subject0);
            Assert.IsNull(_register.NextSubscriptionSync());

            _register.Register(Subject0, false);
            Assert.IsNotNull(_register.NextSubscriptionSync());

            _register.Unregister(Subject1);
            Assert.IsNull(_register.NextSubscriptionSync());
        }

        [Test]
        public void TestUnsubAndSubSupercedesRefresh()
        {
            _register.Register(RfqSubject3, false);
            Assert.AreEqual(new List<Subject.Subject> {RfqSubject3}, _register.NextSubscriptionSync().Subjects);

            _register.Register(RfqSubject3, true);
            _register.Unregister(RfqSubject3);
            _register.Register(RfqSubject3, false);
            SubscriptionSync subscriptionSync = _register.NextSubscriptionSync();

            Assert.AreEqual(false, subscriptionSync.IsChangedEdition());
            Assert.AreEqual(new Dictionary<int, ControlOperation> {{0, ControlOperation.Toggle}},
                subscriptionSync.Controls);

            _register.Unregister(RfqSubject3);
            _register.Register(RfqSubject3, true);
            subscriptionSync = _register.NextSubscriptionSync();
            Assert.AreEqual(false, subscriptionSync.IsChangedEdition());
            Assert.AreEqual(new Dictionary<int, ControlOperation> {{0, ControlOperation.Toggle}},
                subscriptionSync.Controls);
        }

        [Test]
        public void testRefreshingThenUnsubscribing_noControlsAndOneLessSubject()
        {
            _register.Register(Subject0, false);
            _register.Register(Subject1, false);
            _register.Register(Subject2, false);
            _register.NextSubscriptionSync();

            _register.Register(Subject0, true);
            _register.Unregister(Subject0);
            SubscriptionSync subscriptionSync = _register.NextSubscriptionSync();
            Assert.AreEqual(new List<Subject.Subject> {Subject1, Subject2}, subscriptionSync.Subjects);
            Assert.AreEqual(new List<Subject.Subject> { }, subscriptionSync.Controls);
            Assert.AreEqual(true, subscriptionSync.IsChangedEdition());
        }

        [Test]
        public void testLotsOfUnsubbingAndSubscribing_allConflatedIntoOneUnsubAndSub()
        {
            _register.Register(RfsSubject1, false);
            _register.Register(Subject2, false);
            _register.Register(RfqSubject3, false);
            _register.NextSubscriptionSync();

            _register.Register(RfsSubject1, true);
            _register.Unregister(RfsSubject1);
            _register.Register(RfsSubject1, true);
            _register.Unregister(RfsSubject1);
            _register.Register(RfsSubject1, true);

            _register.Register(RfqSubject3, false);
            _register.Unregister(RfqSubject3);
            _register.Register(RfqSubject3, false);

            SubscriptionSync subscriptionSync = _register.NextSubscriptionSync();
            Assert.AreEqual(new List<Subject.Subject> {RfsSubject1, Subject2, RfqSubject3}, subscriptionSync.Subjects);
            Assert.AreEqual(
                new Dictionary<int, ControlOperation> {{0, ControlOperation.Refresh}, {2, ControlOperation.Toggle}},
                subscriptionSync.Controls);

            _register.Register(RfqSubject3, false);
            _register.Register(RfqSubject3, false);
            _register.Unregister(RfqSubject3);
            _register.Register(RfqSubject3, false);

            _register.Unregister(Subject2);

            subscriptionSync = _register.NextSubscriptionSync();
            Assert.AreEqual(new List<Subject.Subject> {RfsSubject1, RfqSubject3}, subscriptionSync.Subjects);
            Assert.AreEqual(new Dictionary<int, ControlOperation> {{1, ControlOperation.Toggle}},
                subscriptionSync.Controls);
        }
    }
}