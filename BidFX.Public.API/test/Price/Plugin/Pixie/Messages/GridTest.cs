using System;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class GridTest
    {
        public const string Bid = "Bid";
        public const string BidSize = "BidSize";
        public const string BidFirm = "BidFirm";
        public const string Ask = "Ask";
        public const string AskSize = "AskSize";
        public const string AskFirm = "AskiFirm";

        private Grid _grid;

        [SetUp]
        public void Before()
        {
            _grid = new Grid();
        }

        [Test]
        public void by_default_it_has_no_columns()
        {
            Assert.AreEqual(0, _grid.NumberOfColumns);
            Assert.AreEqual(0, _grid.ColumnNames.Length);
        }

        [Test]
        public void get_invalid_column_throws_out_of_bound_exception()
        {
            try
            {
                _grid.GetColumn(0);
                Assert.Fail();
            }
            catch (IndexOutOfRangeException e)
            {
            }
        }

        [Test]
        public void colums_are_set_on_grid_image()
        {
            _grid.StartGridImage(76, 6);
            _grid.ColumnImage(Bid, 3, new object[] {1.0, 2.0, 3.0});
            _grid.ColumnImage(BidSize, 3, new object[] {1000000.0, 2000000.0, 3000000.0});
            _grid.ColumnImage(BidFirm, 3, new object[] {"uno", "due", "tre"});
            _grid.ColumnImage(Ask, 2, new object[] {1.1, 2.1});
            _grid.ColumnImage(AskSize, 2, new object[] {1900000.0, 2900000.0});
            _grid.ColumnImage(AskFirm, 2, new object[] {"tre", "uno"});
            _grid.EndGridImage();

            Assert.AreEqual(6, _grid.NumberOfColumns);
            Assert.AreEqual(new object[] {Bid, BidSize, BidFirm, Ask, AskSize, AskFirm}, _grid.ColumnNames);
            int i = 0;
            assertColumnEquals(new object[] {1.0, 2.0, 3.0}, _grid.GetColumn(i++));
            assertColumnEquals(new object[] {1000000.0, 2000000.0, 3000000.0}, _grid.GetColumn(i++));
            assertColumnEquals(new object[] {"uno", "due", "tre"}, _grid.GetColumn(i++));
            assertColumnEquals(new object[] {1.1, 2.1}, _grid.GetColumn(i++));
            assertColumnEquals(new object[] {1900000.0, 2900000.0}, _grid.GetColumn(i++));
            assertColumnEquals(new object[] {"tre", "uno"}, _grid.GetColumn(i++));
        }

        [Test]
        public void a_new_image_replaces_the_previous_one()
        {
            _grid.StartGridImage(76, 6);
            _grid.ColumnImage(Bid, 3, new object[] {1.0, 2.0, 3.0});
            _grid.ColumnImage(BidSize, 3, new object[] {1000000.0, 2000000.0, 3000000.0});
            _grid.ColumnImage(BidFirm, 3, new object[] {"uno", "due", "tre"});
            _grid.ColumnImage(Ask, 2, new object[] {1.1, 2.1});
            _grid.ColumnImage(AskSize, 2, new object[] {1900000.0, 2900000.0});
            _grid.ColumnImage(AskFirm, 2, new object[] {"tre", "uno"});
            _grid.EndGridImage();

            _grid.StartGridImage(76, 4);
            _grid.ColumnImage(AskFirm, 2, new object[] {"a", "b"});
            _grid.ColumnImage(BidSize, 2, new object[] {333.0, 444.0});
            _grid.ColumnImage(AskSize, 2, new object[] {111.0, 999.0});
            _grid.ColumnImage(Bid, 4, new object[] {10.0, 11.0, 12.0, 13.0});
            _grid.EndGridImage();

            Assert.AreEqual(4, _grid.NumberOfColumns);
            Assert.AreEqual(new object[] {AskFirm, BidSize, AskSize, Bid}, _grid.ColumnNames);
            int i = 0;
            assertColumnEquals(new object[] {"a", "b"}, _grid.GetColumn(i++));
            assertColumnEquals(new object[] {333.0, 444.0}, _grid.GetColumn(i++));
            assertColumnEquals(new object[] {111.0, 999.0}, _grid.GetColumn(i++));
            assertColumnEquals(new object[] {10.0, 11.0, 12.0, 13.0}, _grid.GetColumn(i++));
        }


        [Test]
        public void test_image_with_lots_of_columns()
        {
            int numberOfColumns = 300;
            _grid.StartGridImage(76, numberOfColumns);
            for (int i = 0; i < numberOfColumns; i++)
            {
                _grid.ColumnImage("column-" + i, i, range(i));
            }
            _grid.EndGridImage();

            Assert.AreEqual(numberOfColumns, _grid.NumberOfColumns);
            object[] columnNames = _grid.ColumnNames;
            for (int i = 0; i < columnNames.Length; i++)
            {
                Assert.AreEqual("column-" + i, columnNames[i]);
            }

            for (int i = 0; i < numberOfColumns; i++)
            {
                assertColumnEquals(range(i), _grid.GetColumn(i++));
            }

            Assert.AreEqual(numberOfColumns, _grid.NumberOfColumns);
        }

        [Test]
        public void colums_are_updated_on_grid_update()
        {
            _grid.StartGridImage(76, 6);
            _grid.ColumnImage(Bid, 3, new object[] {1.0, 2.0, 3.0});
            _grid.ColumnImage(BidSize, 3, new object[] {1000000.0, 2000000.0, 3000000.0});
            _grid.ColumnImage(BidFirm, 3, new object[] {"uno", "due", "tre"});
            _grid.ColumnImage(Ask, 2, new object[] {1.1, 2.1});
            _grid.ColumnImage(AskSize, 3, new object[] {1900000.0, 2900000.0, 23345444.0});
            _grid.ColumnImage(AskFirm, 2, new object[] {"tre", "uno"});
            _grid.EndGridImage();

            _grid.StartGridUpdate(76, 3);
            _grid.FullColumnUpdate(BidSize, 1, 4, new object[] {1000000.0, 2000000.0, 3000000.0, 4000000.0});
            _grid.PartialColumnUpdate(Ask, 3, 2, new object[] {1.2, 3.1}, new int[] {0, 2});
            _grid.PartialTruncatedColumnUpdate(AskSize, 4, 1, new object[] {1800000.0}, new int[] {0}, 1);
            _grid.EndGridUpdate();

            Assert.AreEqual(6, _grid.NumberOfColumns);
            Assert.AreEqual(new object[] {Bid, BidSize, BidFirm, Ask, AskSize, AskFirm}, _grid.ColumnNames);
            int i = 0;
            assertColumnEquals(new object[] {1.0, 2.0, 3.0}, _grid.GetColumn(i++));
            assertColumnEquals(new object[] {1000000.0, 2000000.0, 3000000.0, 4000000.0}, _grid.GetColumn(i++));
            assertColumnEquals(new object[] {"uno", "due", "tre"}, _grid.GetColumn(i++));
            assertColumnEquals(new object[] {1.2, 2.1, 3.1}, _grid.GetColumn(i++));
            assertColumnEquals(new object[] {1800000.0}, _grid.GetColumn(i++));
            assertColumnEquals(new object[] {"tre", "uno"}, _grid.GetColumn(i++));
        }

        [Test]
        public void full_update_on_a_column_that_was_not_in_the_image_is_illegal()
        {
            try
            {
                _grid.StartGridImage(76, 6);
                _grid.ColumnImage(Bid, 3, new object[] {1.0, 2.0, 3.0});
                _grid.ColumnImage(BidSize, 3, new object[] {1000000.0, 2000000.0, 3000000.0});
                _grid.ColumnImage(BidFirm, 3, new object[] {"uno", "due", "tre"});
                _grid.ColumnImage(Ask, 2, new object[] {1.1, 2.1});
                _grid.ColumnImage(AskSize, 3, new object[] {1900000.0, 2900000.0, 23345444.0});
                _grid.ColumnImage(AskFirm, 2, new object[] {"tre", "uno"});
                _grid.EndGridImage();

                _grid.StartGridUpdate(76, 3);
                _grid.FullColumnUpdate(BidSize, 6, 4, new object[] {1000000.0, 2000000.0, 3000000.0, 4000000.0});
                Assert.Fail();
            }
            catch (IllegalStateException e)
            {
            }
        }

        [Test]
        public void partial_update_on_a_column_that_was_not_in_the_image_is_illegal()
        {
            try
            {
                _grid.StartGridImage(76, 6);
                _grid.ColumnImage(Bid, 3, new object[] {1.0, 2.0, 3.0});
                _grid.ColumnImage(BidSize, 3, new object[] {1000000.0, 2000000.0, 3000000.0});
                _grid.ColumnImage(BidFirm, 3, new object[] {"uno", "due", "tre"});
                _grid.ColumnImage(Ask, 2, new object[] {1.1, 2.1});
                _grid.ColumnImage(AskSize, 3, new object[] {1900000.0, 2900000.0, 23345444.0});
                _grid.ColumnImage(AskFirm, 2, new object[] {"tre", "uno"});
                _grid.EndGridImage();

                _grid.StartGridUpdate(76, 3);
                _grid.PartialColumnUpdate(Ask, 7, 2, new object[] {1.2, 3.1}, new int[] {0, 2});
                Assert.Fail();
            }
            catch (IllegalStateException e)
            {
            }
        }

        private object[] range(int i)
        {
            var values = new Object[i];
            for (int j = 0; j < values.Length; j++)
            {
                values[j] = j * i;
            }
            return values;
        }

        public static void assertColumnEquals(Object[] expectedValues, IColumn column)
        {
            Assert.AreEqual(expectedValues.Length, column.Size());
            for (int i = 0; i < expectedValues.Length; i++)
            {
                Assert.AreEqual(expectedValues[i], column.Get(i));
            }
        }
    }
}