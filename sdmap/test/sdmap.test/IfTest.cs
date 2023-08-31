﻿using sdmap.Compiler;
using Xunit;

namespace sdmap.IntegratedTest
{
    public class IfTest
    {
        [Fact]
        public void TrueWillEmit()
        {
            var code = "sql v1{#if(A){HelloWorld}}";
            var rt = new SdmapCompiler();
            rt.AddSourceCode(code);
            var result = rt.Emit("v1", new { A = true });
            Assert.Equal("HelloWorld", result);
        }

        [Fact]
        public void FalseWontEmit()
        {
            var code = "sql v1{#if(A){HelloWorld}}";
            var rt = new SdmapCompiler();
            rt.AddSourceCode(code);
            var result = rt.Emit("v1", new { A = false });
            Assert.Equal("", result);
        }

        [Theory]
        [InlineData(null, "== null", true)]
        [InlineData(null, "!= null", false)]
        [InlineData("nn", "== null", false)]
        [InlineData("nn", "!= null", true)]
        public void EqualNullTest(string aValue, string nullOperator, bool willEmit)
        {
            var code = $"sql v1{{#if(A {nullOperator}){{HelloWorld}}}}";
            var rt = new SdmapCompiler();
            rt.AddSourceCode(code);
            var result = rt.Emit("v1", new { A = aValue });
            if (willEmit)
            {
                Assert.Equal("HelloWorld", result);
            }
            else
            {
                Assert.Equal("", result);
            }
        }

        [Fact]
        public void AllTest()
        {
            var code = "sql v1{#if(A && !(false || isEmpty(B))){Emit}}";
            var rt = new SdmapCompiler();
            rt.AddSourceCode(code);
            var result = rt.Emit("v1", new
            {
                A = true,
                B = new[] { 1, 2, 3 }
            });
            Assert.Equal("Emit", result);
        }

        [Fact]
        public void NestedIfTest()
        {
            var code = "sql v1{#if(A){A#if(B){B}}T}";
            var rt = new SdmapCompiler();
            rt.AddSourceCode(code);
            var result = rt.Emit("v1", new
            {
                A = true,
                B = true
            });
            Assert.Equal("ABT", result);
        }

        [Fact]
        public void MixIfAndMacroTest()
        {
            var code = "sql v1{#if(A){A}#prop<A>}";
            var rt = new SdmapCompiler();
            rt.AddSourceCode(code);
            var result = rt.Emit("v1", new
            {
                A = true,
            });
            Assert.Equal("ATrue", result);
        }
        
        [Theory]
        [InlineData(true, true, true)]
        [InlineData(false, false, true)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        public void EqualBooleanTest(bool a, bool b, bool expected)
        {
            var code = @"sql v1{#if(A == " + a.ToString().ToLowerInvariant() + "){A}}";
            var rt = new SdmapCompiler();
            rt.AddSourceCode(code);
            var result = rt.Emit("v1", new
            {
                A = b
            });
            Assert.Equal(expected ? "A" : "", result);
        }

        [Theory]
        [InlineData(true, true, false)]
        [InlineData(false, false, false)]
        [InlineData(true, false, true)]
        [InlineData(false, true, true)]
        public void NotEqualBooleanTest(bool a, bool b, bool expected)
        {
            var code = @"sql v1{#if(A != " + a.ToString().ToLowerInvariant() + "){A}}";
            var rt = new SdmapCompiler();
            rt.AddSourceCode(code);
            var result = rt.Emit("v1", new
            {
                A = b
            });
            Assert.Equal(expected ? "A" : "", result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void BooleanCheckTest(bool flag)
        {
            var code = @"sql v1{#if(" + (flag ? "A" : "!A") + "){A}}";
            var rt = new SdmapCompiler();
            rt.AddSourceCode(code);
            var result = rt.Emit("v1", new
            {
                A = flag
            });
            Assert.Equal("A", result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void BooleanUnCheckTest(bool flag)
        {
            var code = @"sql v1{#if(" + (flag ? "A" : "!A") + "){A}}";
            var rt = new SdmapCompiler();
            rt.AddSourceCode(code);
            var result = rt.Emit("v1", new
            {
                A = !flag
            });
            Assert.Equal("", result);
        }

        [Fact]
        public void TwoIsEmptyWithOr()
        {
            var code = "sql v1{#if(!isEmpty(A) || !isEmpty(B))   {OK}}";
            var rt = new SdmapCompiler();
            rt.AddSourceCode(code);
            var result1 = rt.Emit("v1", new { A = "", B = "" });
            var result2 = rt.Emit("v1", new { A = new int[0], B = "abc" });
            var result3 = rt.Emit("v1", new { A = "b", B = "b" });
            Assert.Equal("", result1);
            Assert.Equal("OK", result2);
            Assert.Equal("OK", result3);
        }
    }
}
