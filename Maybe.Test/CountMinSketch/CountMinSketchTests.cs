﻿using System;
using Maybe.CountMinSketch;
using Xunit;

namespace Maybe.Tests.CountMinSketch
{
    public class CountMinSketchTests
    {
        [Fact]
        public void Constructor_WithNegativeDepth_ShouldThrowArgumentException() => Assert.Throws<ArgumentException>(() => new CountMinSketch<int>(-2, 5, 42));

        [Fact]
        public void Constructor_WithNegativeWidth_ShouldThrowArgumentException() => Assert.Throws<ArgumentException>(() => new CountMinSketch<int>(5, -5, 42));

        [Fact]
        public void Constructor_WithNegativeEpsilon_ShouldThrowArgumentException() => Assert.Throws<ArgumentException>(() => new CountMinSketch<int>(-5d, 5d, 42));

        [Fact]
        public void Constructor_WithNegativeConfidence_ShouldThrowArgumentException() => Assert.Throws<ArgumentException>(() => new CountMinSketch<int>(5d, -5d, 42));

        [Fact]
        public void Constructor_WithConfidenceOverOne_ShouldThrowArgumentException() => Assert.Throws<ArgumentException>(() => new CountMinSketch<int>(5d, 2, 42));

        [Fact]
        public void TotalCount_ShouldIncrement_WhenItemIsAdded()
        {
            var sketch = new CountMinSketch<int>(5d, 0.95d, 42);
            sketch.Add(31337);
            Assert.Equal(1, sketch.TotalCount);
        }

        [Fact]
        public void EstimateCount_ShouldBeWithinConfidenceInterval_ForItemThatHasBeenAdded()
        {
            const string input = "Testing!!";
            var sketch = new CountMinSketch<string>(5d, 0.95, 42);
            for (var i = 0; i < 1000; i++)
            {
                sketch.Add(input);
            }
            var estimate = sketch.EstimateCount(input);
            Assert.InRange(estimate, 1000, 1050);
        }

        [Fact]
        public void MergeInPlace_WithNullOther_ShouldThrowIncompatibleMergeException()
        {
            var sketch = new CountMinSketch<string>(5d, 0.95d, 42);
            Assert.Throws<IncompatibleMergeException>(() => sketch.MergeInPlace(null));
        }

        [Fact]
        public void MergeInPlace_WithDifferentDepths_ShouldThrowIncompatibleMergeException()
        {
            var sketch = new CountMinSketch<int>(20, 20, 42);
            var sketch2 = new CountMinSketch<int>(10, 20, 42);
            Assert.Throws<IncompatibleMergeException>(() => sketch.MergeInPlace(sketch2));
        }

        [Fact]
        public void MergeInPlace_WithDifferentWidths_ShouldThrowIncompatibleMergeException()
        {
            var sketch = new CountMinSketch<int>(20, 20, 42);
            var sketch2 = new CountMinSketch<int>(20, 10, 42);
            Assert.Throws<IncompatibleMergeException>(() => sketch.MergeInPlace(sketch2));
        }

        [Fact]
        public void MergeInPlace_WithDifferentSeeds_ShouldThrowIncompatibleMergeException()
        {
            var sketch = new CountMinSketch<int>(20, 20, 42);
            var sketch2 = new CountMinSketch<int>(20, 20, 22);
            Assert.Throws<IncompatibleMergeException>(() => sketch.MergeInPlace(sketch2));
        }

        [Fact]
        public void TotalCount_AfterMergeInPlace_ShouldBeSumOfMergedTotals()
        {
            var sketch = new CountMinSketch<int>(5d, 0.95d, 42);
            var sketch2 = new CountMinSketch<int>(5d, 0.95d, 42);
            for (var i = 0; i < 100; i++)
            {
                sketch.Add(42);
                sketch2.Add(42);
            }
            sketch.MergeInPlace(sketch2);

            Assert.Equal(200, sketch.TotalCount);
        }

        [Fact]
        public void EstimateCount_AfterMergeInPlace_ShouldBeWithinConfidenceInterval()
        {
            const string input = "Testing!!";
            var sketch = new CountMinSketch<string>(5d, 0.95, 42);
            var sketch2 = new CountMinSketch<string>(5d, 0.95, 42);
            for (var i = 0; i < 1000; i++)
            {
                sketch.Add(input);
                sketch2.Add(input);
            }
            sketch.MergeInPlace(sketch2);
            var estimate = sketch.EstimateCount(input);
            Assert.InRange(estimate, 2000, 2100);
        }
    }
}
