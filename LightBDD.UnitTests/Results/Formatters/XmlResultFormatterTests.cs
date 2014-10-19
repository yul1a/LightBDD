﻿using System;
using LightBDD.Results.Formatters;
using NUnit.Framework;

namespace LightBDD.UnitTests.Results.Formatters
{
    [TestFixture]
    public class XmlResultFormatterTests
    {
        private IResultFormatter _subject;
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _subject = new XmlResultFormatter();
        }

        #endregion

        [Test]
        public void Should_format_xml()
        {
            var result = ResultFormatterTestData.GetFeatureResultWithDescription();
            var text = _subject.Format(result);
            Console.WriteLine(text);

            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestResults>
  <Summary TestExecutionStart=""2014-09-23T19:21:58.055Z"" TestExecutionTime=""PT1M4.257S"">
    <Features Count=""1"" />
    <Scenarios Count=""2"" Passed=""0"" Failed=""1"" Ignored=""1"" />
    <Steps Count=""5"" Passed=""2"" Failed=""1"" Ignored=""1"" NotRun=""1"" />
  </Summary>
  <Feature Name=""My feature"" Label=""Label 1"">
    <Description>My feature
long description</Description>
    <Scenario Status=""Ignored"" Name=""name"" Label=""Label 2"" ExecutionStart=""2014-09-23T19:21:58.055Z"" ExecutionTime=""PT1M2.1S"">
      <Step Status=""Passed"" Number=""1"" Name=""step1"" ExecutionStart=""2014-09-23T19:21:59.055Z"" ExecutionTime=""PT1M1S"" />
      <Step Status=""Ignored"" Number=""2"" Name=""step2"" ExecutionStart=""2014-09-23T19:22:00.055Z"" ExecutionTime=""PT1.1S"" />
      <StatusDetails>Not implemented yet</StatusDetails>
    </Scenario>
    <Scenario Status=""Failed"" Name=""name2"" ExecutionStart=""2014-09-23T19:22:01.055Z"" ExecutionTime=""PT2.157S"">
      <Step Status=""Passed"" Number=""1"" Name=""step3"" ExecutionStart=""2014-09-23T19:22:02.055Z"" ExecutionTime=""PT2.107S"" />
      <Step Status=""Failed"" Number=""2"" Name=""step4"" ExecutionStart=""2014-09-23T19:22:03.055Z"" ExecutionTime=""PT0.05S"" />
      <Step Status=""NotRun"" Number=""3"" Name=""step5"" />
      <StatusDetails>  Expected: True
  But was: False</StatusDetails>
    </Scenario>
  </Feature>
</TestResults>";
            Assert.That(text, Is.EqualTo(expectedText));
        }

        [Test]
        public void Should_format_xml_without_description_nor_label_nor_details()
        {
            var result = ResultFormatterTestData.GetFeatureResultWithoutDescriptionNorLabelNorDetails();
            var text = _subject.Format(result);
            Console.WriteLine(text);

            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestResults>
  <Summary TestExecutionStart=""2014-09-23T19:21:58.055Z"" TestExecutionTime=""PT0.025S"">
    <Features Count=""1"" />
    <Scenarios Count=""1"" Passed=""0"" Failed=""0"" Ignored=""1"" />
    <Steps Count=""2"" Passed=""1"" Failed=""0"" Ignored=""1"" NotRun=""0"" />
  </Summary>
  <Feature Name=""My feature"">
    <Scenario Status=""Ignored"" Name=""name"" ExecutionStart=""2014-09-23T19:21:58.055Z"" ExecutionTime=""PT0.025S"">
      <Step Status=""Passed"" Number=""1"" Name=""step1"" ExecutionStart=""2014-09-23T19:21:59.055Z"" ExecutionTime=""PT0.02S"" />
      <Step Status=""Ignored"" Number=""2"" Name=""step2"" ExecutionStart=""2014-09-23T19:22:00.055Z"" ExecutionTime=""PT0.005S"" />
    </Scenario>
  </Feature>
</TestResults>";
            Assert.That(text, Is.EqualTo(expectedText));
        }

        [Test]
        public void Should_format_multiple_features()
        {
            var results = ResultFormatterTestData.GetMultipleFeatureResults();

            var text = _subject.Format(results);
            Console.WriteLine(text);
            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestResults>
  <Summary TestExecutionStart=""2014-09-23T19:21:58.055Z"" TestExecutionTime=""PT0.04S"">
    <Features Count=""2"" />
    <Scenarios Count=""2"" Passed=""2"" Failed=""0"" Ignored=""0"" />
    <Steps Count=""2"" Passed=""2"" Failed=""0"" Ignored=""0"" NotRun=""0"" />
  </Summary>
  <Feature Name=""My feature"">
    <Scenario Status=""Passed"" Name=""scenario1"" ExecutionStart=""2014-09-23T19:21:58.055Z"" ExecutionTime=""PT0.02S"">
      <Step Status=""Passed"" Number=""1"" Name=""step1"" ExecutionStart=""2014-09-23T19:21:59.055Z"" ExecutionTime=""PT0.02S"" />
    </Scenario>
  </Feature>
  <Feature Name=""My feature2"">
    <Scenario Status=""Passed"" Name=""scenario1"" ExecutionStart=""2014-09-23T19:22:01.055Z"" ExecutionTime=""PT0.02S"">
      <Step Status=""Passed"" Number=""1"" Name=""step1"" ExecutionStart=""2014-09-23T19:22:02.055Z"" ExecutionTime=""PT0.02S"" />
    </Scenario>
  </Feature>
</TestResults>";
            Assert.That(text, Is.EqualTo(expectedText));
        }
    }
}