﻿using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Execution.Coordination
{
    /// <summary>
    /// Framework level feature coordinator.
    /// </summary>
    [DebuggerStepThrough]
    public abstract class FrameworkFeatureCoordinator : FeatureCoordinator
    {
        internal new static FeatureCoordinator GetInstance()
        {
            return FeatureCoordinator.GetInstance();
        }

        internal new static FeatureCoordinator TryGetInstance()
        {
            return FeatureCoordinator.TryGetInstance();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="runnerRepository">Runner factory instance that would be used for instantiating runners.</param>
        /// <param name="featureAggregator">Feature aggregator instance used for aggregating feature results on coordinator disposal.</param>
        /// <param name="configuration"><see cref="LightBddConfiguration"/> instance used to initialize LightBDD tests.</param>
        protected FrameworkFeatureCoordinator(FeatureRunnerRepository runnerRepository, IFeatureAggregator featureAggregator, LightBddConfiguration configuration)
            : base(runnerRepository, featureAggregator, configuration)
        {
        }
    }
}
