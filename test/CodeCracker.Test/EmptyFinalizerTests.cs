﻿using Microsoft.CodeAnalysis;
using TestHelper;
using Xunit;

namespace CodeCracker.Test
{
    public class EmptyFinalizerTests : CodeFixTest<EmptyFinalizerAnalyser, EmptyFinalizerCodeFixProvider>
    {
        [Fact]
        public void RemoveEmptyFinalizerWhenIsEmpty()
        {
            var test = @"public class MyClass { ~MyClass() { } }";

            var expected = new DiagnosticResult
            {
                Id = EmptyFinalizerAnalyser.DiagnosticId,
                Message = "Remove Empty Finalizers",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 0, 24) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void MaintainFinalizerWhenUsed()
        {
            var test = @"
                public class MyClass
                {
                    private System.IntPtr pointer;

                    ~MyClass() { pointer = System.IntPtr.Zero; }
                }";

            VerifyCSharpDiagnostic(test);
        }
    }
}