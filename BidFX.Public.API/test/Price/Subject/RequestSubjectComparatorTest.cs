using NUnit.Framework;

namespace BidFX.Public.API.Price.Subject
{
    public class RequestSubjectComparatorTest
    {
        private static readonly Subject Subject = CreateSubject("EURGBP", "1000000.00", "AAA");

        private RequestSubjectComparator _comparator;

        [SetUp]
        public void Before()
        {
            _comparator = new RequestSubjectComparator();
        }

        [Test]
        public void CompareNullWithNull()
        {
            Assert.AreEqual(0, _comparator.Compare(null, null));
        }

        [Test]
        public void CompareSubjectWithNull()
        {
            Assert.AreEqual(-1, _comparator.Compare(null, Subject));
            Assert.AreEqual(1, _comparator.Compare(Subject, null));
        }

        [Test]
        public void CompareIdenticalCompleteSubjects()
        {
            Assert.AreEqual(0, _comparator.Compare(Subject, Subject));
            Assert.AreEqual(0, _comparator.Compare(CreateSubject("EURGBP", "1000000.00", "AAA"), Subject));
            Assert.AreEqual(0, _comparator.Compare(Subject, CreateSubject("EURGBP", "1000000.00", "AAA")));
        }

        [Test]
        public void CompareIdenticalSubjectsWithEquivalentQuantities()
        {
            Subject subject1 = CreateSubject("EURGBP", "1000000.00", "AAA");
            Subject subject2 = CreateSubject("EURGBP", "1000000", "BBB");
            Assert.IsTrue(_comparator.Compare(subject1, subject2) < 0);
            Assert.IsTrue(_comparator.Compare(subject2, subject1) > 0);
        }

        [Test]
        public void CompareSortsFirstBySymbol()
        {
            Subject subject1 = CreateSubject("EURGBP", "3000000.00", "ZZZ");
            Subject subject2 = CreateSubject("GBPUSD", "2000000.00", "BBB");
            Assert.IsTrue(_comparator.Compare(subject1, subject2) < 0);
            Assert.IsTrue(_comparator.Compare(subject2, subject1) > 0);
        }

        [Test]
        public void CompareSortsSecondByQuantity()
        {
            Subject subject1 = CreateSubject("EURGBP", "1000000.00", "ZZZ");
            Subject subject2 = CreateSubject("EURGBP", "2000000.00", "BBB");
            Assert.IsTrue(_comparator.Compare(subject1, subject2) < 0);
            Assert.IsTrue(_comparator.Compare(subject2, subject1) > 0);
        }

        [Test]
        public void CompareSortsSecondByQuantityRightDownToThePenny()
        {
            Subject subject1 = CreateSubject("EURGBP", "1000000.01", "ZZZ");
            Subject subject2 = CreateSubject("EURGBP", "1000000.02", "BBB");
            Assert.IsTrue(_comparator.Compare(subject1, subject2) < 0);
            Assert.IsTrue(_comparator.Compare(subject2, subject1) > 0);
        }

        [Test]
        public void CompareSortsSecondByQuantityEvenWhenTheyDifferInLength()
        {
            Subject subject1 = CreateSubject("EURGBP", "20000.00", "ZZZ");
            Subject subject2 = CreateSubject("EURGBP", "1000000.00", "BBB");
            Assert.IsTrue(_comparator.Compare(subject1, subject2) < 0);
            Assert.IsTrue(_comparator.Compare(subject2, subject1) > 0);
        }

        [Test]
        public void CompareSortsSecondByQuantityEvenIfFirstIsNotInDoubleFormat()
        {
            Subject subject1 = CreateSubject("EURGBP", "1000000", "ZZZ");
            Subject subject2 = CreateSubject("EURGBP", "2000000.00", "BBB");
            Assert.IsTrue(_comparator.Compare(subject1, subject2) < 0);
            Assert.IsTrue(_comparator.Compare(subject2, subject1) > 0);
        }

        [Test]
        public void CompareSortsSecondByQuantityEvenIfSecondIsNotInDoubleFormat()
        {
            Subject subject1 = CreateSubject("EURGBP", "1000000.00", "ZZZ");
            Subject subject2 = CreateSubject("EURGBP", "2000000", "BBB");
            Assert.IsTrue(_comparator.Compare(subject1, subject2) < 0);
            Assert.IsTrue(_comparator.Compare(subject2, subject1) > 0);
        }

        [Test]
        public void CompareSortsSecondByQuantityEvenIfNeitherAreInDoubleFormat()
        {
            Subject subject1 = CreateSubject("EURGBP", "1000000", "ZZZ");
            Subject subject2 = CreateSubject("EURGBP", "2000000", "BBB");
            Assert.IsTrue(_comparator.Compare(subject1, subject2) < 0);
            Assert.IsTrue(_comparator.Compare(subject2, subject1) > 0);
        }

        [Test]
        public void CompareSortsSecondByQuantityEvenIfNotANumber()
        {
            Subject subject1 = CreateSubject("EURGBP", "A1000000", "ZZZ");
            Subject subject2 = CreateSubject("EURGBP", "A2000000", "BBB");
            Assert.IsTrue(_comparator.Compare(subject1, subject2) < 0);
            Assert.IsTrue(_comparator.Compare(subject2, subject1) > 0);
        }

        [Test]
        public void CompareSortsThirdByOverallString()
        {
            Subject subject1 = CreateSubject("EURGBP", "2000000.00", "AAA");
            Subject subject2 = CreateSubject("EURGBP", "2000000.00", "BBB");
            Assert.IsTrue(_comparator.Compare(subject1, subject2) < 0);
            Assert.IsTrue(_comparator.Compare(subject2, subject1) > 0);
        }

        [Test]
        public void CompareIdenticalSubjectsWithMissingSymbol()
        {
            Subject subject = CreateSubject(null, "1000000.00", "AAA");
            Assert.AreEqual(0, _comparator.Compare(subject, subject));
        }

        [Test]
        public void CompareIdenticalSubjectsWithMissingSymbolAndAccount()
        {
            Subject subject = CreateSubject(null, "1000000.00", null);
            Assert.AreEqual(0, _comparator.Compare(subject, subject));
        }

        [Test]
        public void CompareIdenticalSubjectsWithMissingSymbolAndQuantity()
        {
            Subject subject = CreateSubject(null, null, "AAA");
            Assert.AreEqual(0, _comparator.Compare(subject, subject));
        }

        [Test]
        public void CompareIdenticalSubjectsWithMissingQuantity()
        {
            Subject subject = CreateSubject("EURGBP", null, "AAA");
            Assert.AreEqual(0, _comparator.Compare(subject, subject));
        }

        [Test]
        public void CompareIdenticalSubjectsWithMissingAccount()
        {
            Subject subject = CreateSubject("EURGBP", "1000000.00", null);
            Assert.AreEqual(0, _comparator.Compare(subject, subject));
        }

        [Test]
        public void CompareIdenticalSubjectsWithMissingQuantityAndAccount()
        {
            Subject subject = CreateSubject("EURGBP", null, null);
            Assert.AreEqual(0, _comparator.Compare(subject, subject));
        }

        [Test]
        public void CompareIdenticalSubjectsWithMissingSymbolQuantityAndAccount()
        {
            Subject subject = CreateSubject(null, null, null);
            Assert.AreEqual(0, _comparator.Compare(subject, subject));
        }

        private static Subject CreateSubject(string symbol, string quantity, string account)
        {
            SubjectBuilder subjectBuilder = CreateSubjectBuilder();
            if (account != null)
            {
                subjectBuilder.SetComponent("Account", account);
            }
            if (quantity != null)
            {
                subjectBuilder.SetComponent("Quantity", quantity);
                subjectBuilder.SetComponent("AllocQty", quantity.Replace("\\.\\d+", ""));
            }
            if (symbol != null)
            {
                subjectBuilder.SetComponent("Symbol", symbol);
                subjectBuilder.SetComponent("Currency", symbol.Substring(0, 3));
            }
            return subjectBuilder.CreateSubject();
        }

        private static SubjectBuilder CreateSubjectBuilder()
        {
            return new SubjectBuilder()
                .SetComponent("AssetClass", "Fx")
                .SetComponent("Customer", "C123")
                .SetComponent("Dealer", "ABC")
                .SetComponent("Exchange", "OTC")
                .SetComponent("Level", "1")
                .SetComponent("QuoteStyle", "RFS")
                .SetComponent("Source", "GSFX")
                .SetComponent("SubClass", "Spot")
                .SetComponent("Tenor", "Spot")
                .SetComponent("User", "psweeny")
                .SetComponent("ValueDate", "21050129");
        }
    }
}