﻿using sdmap.Macros.Implements;
using sdmap.Utils;
using System;
using System.Linq;
using Xunit;

namespace sdmap.unittest.MacroTest
{
    public class MacroUtilTest
    {
        [Fact]
        public void GetSimplePropOk()
        {
            var val = new { A = 3 };
            var prop = DynamicRuntimeMacros.GetProp(val, "A");
            Assert.NotNull(prop);
        }

        [Fact]
        public void GetNestedPropOk()
        {
            var val = new { A = new { A = 3 } };
            var prop = DynamicRuntimeMacros.GetProp(val, "A.A");
            Assert.NotNull(prop);
        }

        [Fact]
        public void GetNotExistPropWillReturnNull()
        {
            var val = new { A = 3 };
            var prop = DynamicRuntimeMacros.GetProp(val, "A.A");
            Assert.Null(prop);
        }

        [Fact]
        public void GetNotExistPropWillNotThrow()
        {
            var val = new { A = 3 };
            var prop = DynamicRuntimeMacros.GetProp(val, "B.C.D");
        }

        [Fact]
        public void CanGetNextedObjectValue()
        {
            var val = new { A = new { B = 4 } };
            var getted = DynamicRuntimeMacros.GetPropValue(val, "A.B");
            Assert.Equal(4, getted);
        }

        [Fact]
        public void CanDetectEmptyArray()
        {
            var arr = Enumerable.Range(1, 0);
            Assert.True(DynamicRuntimeMacros.ArrayEmpty(arr));
        }

        [Fact]
        public void CanDetectNotEmptyArray()
        {
            var arr = Enumerable.Range(1, 10);
            Assert.False(DynamicRuntimeMacros.ArrayEmpty(arr));
        }
    }
}
