using System;
using System.Collections.Generic;
using System.Linq;
using SecretSanta.Domain;
using Xunit;
using Xunit.Abstractions;

namespace SecretSanta.Tests
{
    public class SecretSantaServiceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly SecretSantaService _sut;

        public SecretSantaServiceTests (ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _sut = new SecretSantaService();
        }

        [Fact]
        public void AssignSantas_EmptyArray_ThrowsArgumentException ()
        {
            Assert.Throws<ArgumentException> (() => _sut.AssignSantas (Array.Empty<Santa>()));
        }

        [Fact]
        public void AssignSantas_OneElement_ThrowsArgumentException ()
        {
            Assert.Throws<ArgumentException> (() => _sut.AssignSantas (new[] { new Santa { Name = "Max", Email = "max@test.local" } }));
        }

        [Fact]
        public void AssignSantas_TwoElements_ReturnsCorrectAssignment ()
        {
            var max = new Santa { Name = "Max", Email = "max@test.local" };
            var john = new Santa() { Name = "John", Email = "john@test.local" };
            var santas = new[] { max, john };
            
            var assignments = AssignAndPrint(santas);

            Assert.All (assignments, tuple => Assert.NotEqual(tuple.Item1, tuple.Item2));
        }
        
        [Fact]
        public void AssignSantas_MoreElements_ReturnsCorrectAssignment ()
        {
            var max = new Santa { Name = "Max", Email = "max@test.local" };
            var john = new Santa() { Name = "John", Email = "john@test.local" };
            var alice = new Santa() { Name = "Alice", Email = "alice@test.local" };
            var erika = new Santa() { Name = "Erika", Email = "erika@test.local" };
            var santas = new[] { max, john, alice, erika };
            
            var assignments = AssignAndPrint(santas);

            Assert.All (assignments, tuple => Assert.NotEqual(tuple.Item1, tuple.Item2));
        }

        [Fact]
        public void AssignSantas_Twice_ReturnsDifferentAssignments ()
        {
            var santas = CreateSantas(100).ToArray();

            var assignments1 = _sut.AssignSantas (santas);
            var assignments2 = _sut.AssignSantas (santas);

            Assert.NotEqual (assignments1, assignments2);
        }

        private static IEnumerable<Santa> CreateSantas (int santaCount)
        {
            for (var i = 0; i < santaCount; i++)
                yield return new Santa { Name = $"Santa{i}", Email = $"santa{i}@test.local" };
        }

        private List<(Santa, Santa)> AssignAndPrint (Santa[] santas)
        {
            var assignments = _sut.AssignSantas (santas);
            PrintAssignments (assignments);
            return assignments;
        }

        private void PrintAssignments (List<(Santa, Santa)> assignments)
        {
            foreach (var assignment in assignments)
            {
                _testOutputHelper.WriteLine ($"{assignment.Item1} => {assignment.Item2}");
            }
        }
    }
}