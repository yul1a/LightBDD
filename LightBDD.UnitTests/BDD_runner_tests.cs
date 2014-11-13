﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests
{
    [TestFixture]
    [FeatureDescription("Runner tests description")]
    [Label("Ticket-1")]
    public class BDD_runner_tests : SomeSteps
    {
        private AbstractBDDRunner _subject;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _subject = new TestableBDDRunner(GetType(), MockRepository.GenerateMock<IProgressNotifier>());
        }

        #endregion

        [Test]
        public void Should_collect_scenario_result()
        {
            _subject.RunScenario(Step_one, Step_two);
            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));

            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "Step one",  ResultStatus.Passed),
                new StepResultExpectation(2, "Step two", ResultStatus.Passed)
            });
        }

        [Test]
        public void Should_collect_scenario_result_via_fluent_interfaces()
        {
            _subject.NewScenario().Run(Step_one, Step_two);

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result via fluent interfaces"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "Step one", ResultStatus.Passed),
                new StepResultExpectation(2, "Step two", ResultStatus.Passed)
            });
        }

        [Test]
        public void Should_collect_scenario_result_for_explicitly_named_scenario()
        {
            const string scenarioName = "my scenario";
            _subject.RunScenario(scenarioName, Step_one, Step_two);
            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo(scenarioName));
            Assert.That(result.Label, Is.Null);
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "Step one", ResultStatus.Passed),
                new StepResultExpectation(2, "Step two", ResultStatus.Passed)
            });
        }

        [Test]
        public void Should_collect_scenario_result_for_explicitly_named_scenario_with_label()
        {
            const string scenarioName = "my scenario";
            const string scenarioLabel = "label";
            _subject.RunScenario(scenarioName, scenarioLabel, Step_one, Step_two);
            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo(scenarioName));
            Assert.That(result.Label, Is.EqualTo(scenarioLabel));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Passed));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "Step one", ResultStatus.Passed),
                new StepResultExpectation(2, "Step two", ResultStatus.Passed)
            });
        }

        [Test]
        public void Should_collect_scenario_result_for_failing_scenario()
        {
            try
            {
                _subject.RunScenario(Step_one, Step_throwing_exception, Step_two);
            }
            catch
            {
            }
            const string expectedStatusDetails = "exception text";

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result for failing scenario"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Failed));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "Step one", ResultStatus.Passed),
                new StepResultExpectation(2, "Step throwing exception", ResultStatus.Failed, expectedStatusDetails),
                new StepResultExpectation(3, "Step two", ResultStatus.NotRun)
            });
            Assert.That(result.StatusDetails, Is.EqualTo(expectedStatusDetails));
        }

        [Test]
        public void Should_collect_scenario_result_for_ignored_scenario_steps()
        {
            try
            {
                _subject.RunScenario(Step_one, Step_with_ignore_assertion, Step_two);
            }
            catch
            {
            }
            const string expectedStatusDetails = "some reason";

            var result = _subject.Result.Scenarios.Single();
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result for ignored scenario steps"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Ignored));
            StepResultExpectation.Assert(result.Steps, new[]
            {
                new StepResultExpectation(1, "Step one", ResultStatus.Passed),
                new StepResultExpectation(2, "Step with ignore assertion", ResultStatus.Ignored, expectedStatusDetails),
                new StepResultExpectation(3, "Step two", ResultStatus.NotRun)
            });
            Assert.That(result.StatusDetails, Is.EqualTo(expectedStatusDetails));
        }

        [Test]
        [Label("Label 1")]
        public void Should_include_labels_in_result()
        {
            _subject.RunScenario(Step_one, Step_two);
            Assert.That(_subject.Result.Label, Is.EqualTo("Ticket-1"));
            Assert.That(_subject.Result.Scenarios.Single().Label, Is.EqualTo("Label 1"));
        }

        [Test]
        public void Should_pass_exception_to_runner_caller()
        {
            Assert.Throws<InvalidOperationException>(() => _subject.RunScenario(Step_throwing_exception));
        }

        [Test]
        public void Should_pass_ignore_exception_to_runner_caller()
        {
            Assert.Throws<IgnoreException>(() => _subject.RunScenario(Step_with_ignore_assertion));
        }

        [Test]
        public void Should_pass_inconclusive_exception_to_runner_caller()
        {
            Assert.Throws<InconclusiveException>(() => _subject.RunScenario(Step_with_inconclusive_assertion));
        }

        [Test]
        public void Should_run_scenario_be_thread_safe()
        {
            var scenarios = new List<string>();
            for (int i = 0; i < 3000; ++i)
                scenarios.Add(i.ToString(CultureInfo.InvariantCulture));

            scenarios.AsParallel().ForAll(scenario => _subject.RunScenario(scenario, Step_one, Step_two));

            Assert.That(_subject.Result.Scenarios.Select(s => s.Name).ToArray(), Is.EquivalentTo(scenarios));
        }

        [Test]
        public void Should_use_console_progress_notifier_by_default()
        {
            Assert.That(new TestableBDDRunner(GetType()).ProgressNotifier, Is.InstanceOf<ConsoleProgressNotifier>());
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\n\r")]
        public void Should_not_allow_to_run_scenarios_without_name(string name)
        {
            var exception = Assert.Throws<ArgumentException>(() => _subject.NewScenario(name));
            Assert.That(exception.Message, Is.EqualTo("Unable to create scenario without name"));
        }

        [Test]
        public void AbstractBDDRunner_should_throw_if_initialized_with_null_type_parameter()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new TestableBDDRunner(null, new TestableMetadataProvider(), new ConsoleProgressNotifier()));
            Assert.That(ex.Message, Is.StringContaining("featureTestClass"));
        }

        [Test]
        public void AbstractBDDRunner_should_throw_if_initialized_with_null_metadataProvider_parameter()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new TestableBDDRunner(typeof(BDD_runner_tests), null, new ConsoleProgressNotifier()));
            Assert.That(ex.Message, Is.StringContaining("metadataProvider"));
        }

        [Test]
        public void AbstractBDDRunner_should_throw_if_initialized_with_null_progressNotifier_parameter()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new TestableBDDRunner(typeof(BDD_runner_tests), new TestableMetadataProvider(), null));
            Assert.That(ex.Message, Is.StringContaining("progressNotifier"));
        }
    }
}
