﻿using System;
using FluentAssertions;
using NUnit.Framework;

namespace FluentOptionals.Tests
{
    [TestFixture]
    public class Optional1Tests
    {
        private Optional<int> _some;
        private Optional<int> _none;

        [SetUp]
        public void Setup()
        {
            _some = Optional.Some(1);
            _none = Optional.None<int>();
        }

        #region MatchSome

        [Test]
        public void MatchSome_HandleIsNotCalledOnNone()
        {
            var handleCalled = false;
            _none.MatchSome(_ => handleCalled = true);

            handleCalled.Should().Be(false);
        }

        [Test]
        public void MatchSome_HandleIsCalledOnSome()
        {
            var handleCalled = false;
            _some.MatchSome(_ => handleCalled = true);

            handleCalled.Should().Be(true);
        }

        [Test]
        public void MatchSome_HandleReturnsRightValue()
        {
            var returnedValue = 0;
            _some.MatchSome(i => returnedValue = i);

            returnedValue.Should().Be(1);
        }

        #endregion

        #region MatchNone

        [Test]
        public void MatchNone_HandleIsNotCalledOnSome()
        {
            var handleCalled = false;
            _some.MatchNone(() => handleCalled = true);

            handleCalled.Should().Be(false);
        }

        [Test]
        public void MatchNone_HandleIsCalledOnNone()
        {
            var handleCalled = false;
            _none.MatchNone(() => handleCalled = true);

            handleCalled.Should().Be(true);
        }

        #endregion

        #region ValueOr

        [Test]
        public void MatchNone_WhenOptionalIsNone_ThenValueFromHandleGetsReturned()
        {
            _none.ValueOr(() => 20).Should().Be(20);
        }

        [Test]
        public void MatchNone_WhenOptionalIsNone_ThenValueGetsReturned()
        {
            _none.ValueOr(20).Should().Be(20);
        }


        [Test]
        public void MatchNone_WhenOptionalIsSome_ThenValueFromHandleIsIgnored()
        {
            _some.ValueOr(() => 20).Should().NotBe(20);
        }

        [Test]
        public void MatchNone_WhenOptionalIsSome_ThenValueIsIgnored()
        {
            _some.ValueOr(20).Should().NotBe(20);
        }

        [Test]
        public void ValueOrThrow_WhenOptionalIsSome_ThenValueGetsReturned()
        {
            _some.ValueOrThrow(new TestException()).Should().NotBe(20);
        }

        [Test]
        public void ValueOrThrow_WhenOptionalIsValue_ThenValueGetsThrown()
        {
            Action noneThrowsException = () => _none.ValueOrThrow(new TestException());

            noneThrowsException.ShouldThrow<Exception>();
        }

        #endregion

        #region Match

        [Test]
        public void Match_WhenOptionalHasValue_SomeHandleGetsCalled()
        {
            bool someHandleCalled = false, noneHandleCalled = false;

            _some.Match(
                some: _ => someHandleCalled = true,
                none: () => noneHandleCalled = true);

            someHandleCalled.Should().BeTrue();
            noneHandleCalled.Should().BeFalse();
        }

        [Test]
        public void Match_WhenOptionalHasNoValue_NoneHandleGetsCalled()
        {
            bool someHandleCalled = false, noneHandleCalled = false;

            _none.Match(
                some: _ => someHandleCalled = true,
                none: () => noneHandleCalled = true);

            noneHandleCalled.Should().BeTrue();
            someHandleCalled.Should().BeFalse();
        }

        [Test]
        public void Match_WhenOptionalHasValue_SomeHandleGetsReturned()
        {
            _some.Match(
                some: _ => 20,
                none: () => 30).Should().Be(20);
        }

        [Test]
        public void Match_WhenOptionalHasNoValue_NoneHandleGetsReturned()
        {
            _none.Match(
                some: _ => 20,
                none: () => 30).Should().Be(30);
        }


        [Test]
        public void Match_CanReturNull()
        {
            _none.Match(
                some: _ => null as string,
                none: () => null as string).Should().Be(null);
        }

        #endregion

        #region Map

        [Test]
        public void Map_WhenOpationalIsNone_ThenNoneGetsReturnedAgain()
        {
            var resultOptional = 10.ToNone().Map(_ => "test");

            resultOptional.ShouldBeNone();
            typeof (Optional<string>).Should().Be(resultOptional.GetType());
        }

        [Test]
        public void Map_WhenOpationalIsSome_ThenSomeGetsReturnedAgain()
        {
            var resultOptional = 10.ToSome().Map(_ => "test");

            resultOptional.ShouldBeSome();
            typeof (Optional<string>).Should().Be(resultOptional.GetType());
        }

        [Test]
        public void Map_WhenMapOpationReturnsNull_ThenNoneGetsReturned()
        {
            var resultOptional = 10.ToSome().Map<string>(_ => null);

            resultOptional.ShouldBeNone();
            typeof (Optional<string>).Should().Be(resultOptional.GetType());
        }

        #endregion

        #region Shift

        [Test]
        public void Shift_WhenGivenPredicateIsTrue_ThenNoneGetsReturned()
        {
            10.ToSome().Shift(i => i > 5).ShouldBeNone();
        }

        [Test]
        public void Shift_WhenGivenPredicateIsFalse_ThenSomeGetsReturned()
        {
            10.ToSome().Shift(i => i > 15).ShouldBeSome();
        }

        [Test]
        public void Shift_WhenNoneIsShifted_ThenNoneGetsReturned()
        {
            10.ToNone().Shift(i => i > 15).ShouldBeNone();
        }

        #endregion 

        #region Implicit Operator

        [Test]
        public void ImplicitOperator_NullGetsNone()
        {
            Optional<object> optional = (object) null;
            optional.MatchSome(_ => Assert.Fail());
        }

        [Test]
        public void ImplicitOperator_ValueTypeGetsSome()
        {
            Optional<int> optional = 15;
            optional.MatchNone(() => Assert.Fail());
        }

        [Test]
        public void ImplicitOperator_ReferenceTypeGetsSome()
        {
            Optional<DateTime> optional = DateTime.Now;
            optional.MatchNone(() => Assert.Fail());
        }

        #endregion
    }

    public class TestException : Exception { }
}
    