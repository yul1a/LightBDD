﻿using LightBDD;
using LightBDD.Coordination;
using NUnit.Framework;

namespace $rootnamespace$
{
	public partial class $safeitemname$
	{
		private readonly BDDRunner _runner;

		public $safeitemname$()
		{
			_runner = new BDDRunner(GetType());
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			FeatureCoordinator.Instance.AddFeature(_runner.Result);
		}

		private void Template_method()
		{
			Assert.Ignore("Not implemented yet");
		}
	}
}