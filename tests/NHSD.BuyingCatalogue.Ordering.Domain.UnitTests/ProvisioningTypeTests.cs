﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Domain.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal sealed class ProvisioningTypeTests
    {
        [Test]
        public void List_ReturnsExpectedList()
        {
            var actual = ProvisioningType.List();

            var expected = new List<ProvisioningType>
            {
                ProvisioningType.Patient, 
                ProvisioningType.Declarative, 
                ProvisioningType.OnDemand
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void FromId_ProvisioningTypeId_ReturnsExpectedType()
        {
            var actual = ProvisioningType.FromId(1);

            actual.Should().Be(ProvisioningType.Patient);
        }

        [Test]
        public void FromId_UnknownProvisioningTypeId_ReturnsNull()
        {
            var actual = ProvisioningType.FromId(10);
            actual.Should().BeNull();
        }

        [TestCase("Declarative")]
        [TestCase("declarative")]
        [TestCase("dEclArative")]
        public void FromName_ProvisioningTypeId_ReturnsExpectedType(string provisioningTypeName)
        {
            var actual = ProvisioningType.FromName(provisioningTypeName);

            actual.Should().Be(ProvisioningType.Declarative);
        }

        [Test]
        public void FromName_UnknownProvisioningTypeId_ReturnsNull()
        {
            var actual = ProvisioningType.FromName("Unknown");
            actual.Should().BeNull();
        }

        [Test]
        public void FromName_NullName_ThrowsArgumentNullException()
        {
            static void Test()
            {
                ProvisioningType.FromName(null);
            }

            Assert.Throws<ArgumentNullException>(Test);
        }

        [TestCase(null)]
        [TestCase("InvalidType")]
        public void Equals_ComparisonObject_AreNotEqual(object comparisonObject)
        {
            ProvisioningType.Declarative.Equals(comparisonObject).Should().BeFalse();
        }

        [Test]
        public void Equals_Same_AreEqual()
        {
            var instance = ProvisioningType.Declarative;
            instance.Equals(instance).Should().BeTrue();
        }

        [Test]
        public void Equals_Different_AreNotEqual()
        {
            var instance = ProvisioningType.Declarative;
            var comparisonObject = ProvisioningType.Patient;

            instance.Equals(comparisonObject).Should().BeFalse();
        }
    }
}
