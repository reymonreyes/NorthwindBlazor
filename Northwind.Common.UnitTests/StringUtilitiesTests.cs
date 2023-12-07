using Northwind.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Northwind.Common.UnitTests
{
    public class StringUtilitiesTests
    {
        [Fact]
        public void IsEmptyOrLengthLessThan_ShouldReturnTrueIfNull()
        {
            string str = null;
            Assert.True(str.IsEmptyOrLengthLessThan(2));
        }

        [Fact]
        public void IsEmptyOrLengthLessThan_ShouldReturnTrueIfEmptyString()
        {
            string str = "";
            Assert.True(str.IsEmptyOrLengthLessThan(2));
        }

        [Fact]
        public void IsEmptyOrLengthLessThan_ShouldReturnTrueIfLengthLessThanRequired()
        {
            string str = "abcd";
            Assert.True(str.IsEmptyOrLengthLessThan(5));
        }

        [Fact]
        public void IsEmptyOrLengthLessThan_ShouldReturnFalseIfLengthIsGreaterOrEqualToRequiredLength()
        {
            string str = "ab";
            Assert.False(str.IsEmptyOrLengthLessThan(2));
        }
    }
}
