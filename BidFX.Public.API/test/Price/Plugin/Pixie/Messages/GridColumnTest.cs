using System;
using NUnit.Framework;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    [TestFixture]
    public class GridColumnTest
    {
        private static readonly object[] Image = {1, 2, 3};

        private GridColumn _gridColumn;

        [Test]
        public void test_to_string()
        {
            _gridColumn.SetImage(new object[] {1, 2, 3}, 3);
            Assert.AreEqual("GridColumn(Size=3,[1,2,3])", _gridColumn.ToString());
        }

        [SetUp]
        public void Init()
        {
            _gridColumn = new GridColumn();
            Console.WriteLine("before");
        }

        [Test]
        public void by_default_is_empty()
        {
            Assert.AreEqual(0, _gridColumn.Size());
        }

        [Test]
        public void content_can_be_retrieved_by_index()
        {
            _gridColumn.SetImage(Image, 3);
            Assert.AreEqual(3, _gridColumn.Size());
            Assert.AreEqual(1, _gridColumn.Get(0));
            Assert.AreEqual(2, _gridColumn.Get(1));
            Assert.AreEqual(3, _gridColumn.Get(2));
        }

        [Test]
        public void an_image_replaces_the_existing_content()
        {
            _gridColumn.SetImage(new object[] {4, 5}, 2);
            _gridColumn.SetImage(Image, 3);
            Assert.AreEqual(3, _gridColumn.Size());
            Assert.AreEqual(1, _gridColumn.Get(0));
            Assert.AreEqual(2, _gridColumn.Get(1));
            Assert.AreEqual(3, _gridColumn.Get(2));
        }

        [Test]
        public void test_set_value()
        {
            _gridColumn.SetImage(Image, 3);
            _gridColumn.Set(1, 5);
            Assert.AreEqual(3, _gridColumn.Size());
            Assert.AreEqual(1, _gridColumn.Get(0));
            Assert.AreEqual(5, _gridColumn.Get(1));
            Assert.AreEqual(3, _gridColumn.Get(2));
        }

        [Test]
        public void Get_after_the_end_throws_a_array_index_out_of_bound_exception()
        {
            try
            {
                _gridColumn.Get(0);
                Assert.Fail();
            }
            catch (IndexOutOfRangeException e)
            {
            }
        }

        [Test]
        public void delete_from_reduces_the_Size()
        {
            _gridColumn.SetImage(Image, 3);
            _gridColumn.DeleteFrom(1);
            Assert.AreEqual(1, _gridColumn.Size());
        }

        [Test]
        public void a_Size_different_from_the_array_length_can_be_specified_when_setting_the_image()
        {
            _gridColumn.SetImage(Image, 1);
            Assert.AreEqual(1, _gridColumn.Size());
        }

        [Test]
        public void delete_from_leaves_the_rest_untouched()
        {
            _gridColumn.SetImage(Image, 3);
            _gridColumn.DeleteFrom(2);
            Assert.AreEqual(2, _gridColumn.Size());
            Assert.AreEqual(1, _gridColumn.Get(0));
            Assert.AreEqual(2, _gridColumn.Get(1));
        }

        [Test]
        public void delete_from_with_index_higher_than_Size_is_no_op()
        {
            _gridColumn.SetImage(Image, 3);
            _gridColumn.DeleteFrom(4);
            Assert.AreEqual(3, _gridColumn.Size());
        }

        [Test]
        public void delete_from_with_index_equals_to_Size_is_no_op()
        {
            _gridColumn.SetImage(Image, 3);
            _gridColumn.DeleteFrom(3);
            Assert.AreEqual(3, _gridColumn.Size());
        }

        [Test]
        public void Get_after_the_end_after_a_delete_from_throws_exception()
        {
            try
            {
                _gridColumn.SetImage(Image, 3);
                _gridColumn.DeleteFrom(1);
                _gridColumn.Get(1);
                Assert.Fail();
            }
            catch (IndexOutOfRangeException e)
            {
            }
        }

        [Test]
        public void setting_a_value_after_the_end_automatically_extends_the_column()
        {
            _gridColumn.Set(0, 8);
            Assert.AreEqual(1, _gridColumn.Size());
            Assert.AreEqual(8, _gridColumn.Get(0));
        }

        [Test]
        public void setting_a_value_long_after_the_end_automatically_extends_the_column()
        {
            _gridColumn.Set(1023, 8);
            Assert.AreEqual(1024, _gridColumn.Size());
            Assert.AreEqual(8, _gridColumn.Get(1023));
        }

        [Test]
        public void test_delete_followed_by_extend()
        {
            _gridColumn.SetImage(Image, 3);
            _gridColumn.DeleteFrom(2);
            _gridColumn.Set(2, 3);
            Assert.AreEqual(3, _gridColumn.Size());
            Assert.AreEqual(1, _gridColumn.Get(0));
            Assert.AreEqual(2, _gridColumn.Get(1));
            Assert.AreEqual(3, _gridColumn.Get(2));
        }
    }
}