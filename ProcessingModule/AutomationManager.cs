using Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ProcessingModule
{
    /// <summary>
    /// Class containing logic for automated work.
    /// </summary>
    public class AutomationManager : IAutomationManager, IDisposable
	{
		private Thread automationWorkerDigital;
		private Thread automationWorkerAnalog;
        private AutoResetEvent automationTrigger;
        private IStorage storage;
		private IProcessingManager processingManager;
		private int delayBetweenCommands;
        private IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationManager"/> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="processingManager">The processing manager.</param>
        /// <param name="automationTrigger">The automation trigger.</param>
        /// <param name="configuration">The configuration.</param>
        public AutomationManager(IStorage storage, IProcessingManager processingManager, AutoResetEvent automationTrigger, IConfiguration configuration)
		{
			this.storage = storage;
			this.processingManager = processingManager;
            this.configuration = configuration;
            this.automationTrigger = automationTrigger;
        }

        /// <summary>
        /// Initializes and starts the threads.
        /// </summary>
		private void InitializeAndStartThreads()
		{
			InitializeAutomationWorkerThread();
			StartAutomationWorkerThread();
		}

        /// <summary>
        /// Initializes the automation worker thread.
        /// </summary>
		private void InitializeAutomationWorkerThread()
		{
			automationWorkerDigital = new Thread(AutomationWorker_DoWork_Digital);
			automationWorkerDigital.Name = "Automation Thread for Digital input/output";
			automationWorkerAnalog = new Thread(AutomationWorker_DoWork_Analog);
			automationWorkerAnalog.Name = "Automation Thread for Analog input/output";
		}

        /// <summary>
        /// Starts the automation worker thread.
        /// </summary>
		private void StartAutomationWorkerThread()
		{
			automationWorkerDigital.Start();
			automationWorkerAnalog.Start();
		}


		private void AutomationWorker_DoWork_Digital()
		{
			PointIdentifier l1 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 4100);
			PointIdentifier l2 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 4101);
			PointIdentifier s4 = new PointIdentifier(PointType.DIGITAL_INPUT, 3100);
			List<PointIdentifier> pIdentifiers = new List<PointIdentifier> { l1, l2, s4 };
			while (!disposedValue)
			{
				List<IPoint> pnts = storage.GetPoints(pIdentifiers);
				Thread.Sleep(2000);
				for (int i = 0; i < pnts.Count; ++i)
				{
					processingManager.ExecuteReadCommand(pnts[i].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, pnts[i].ConfigItem.StartAddress, 1);
				}
			}
		}

        private void AutomationWorker_DoWork_Analog()
        {
			PointIdentifier v1 = new PointIdentifier(PointType.ANALOG_OUTPUT, 2400);
            PointIdentifier p1 = new PointIdentifier(PointType.ANALOG_OUTPUT, 2401);
			PointIdentifier s1 = new PointIdentifier(PointType.ANALOG_INPUT, 3800);
            PointIdentifier s2 = new PointIdentifier(PointType.ANALOG_INPUT, 3801);
            PointIdentifier s3= new PointIdentifier(PointType.ANALOG_INPUT, 3802);
			List<PointIdentifier> pIdentifiers = new List<PointIdentifier> { v1, p1, s1, s2, s3 };
			while (!disposedValue)
			{
				List<IPoint> pnts = storage.GetPoints(pIdentifiers);
				Thread.Sleep(4000);
				for(int i = 0; i < pnts.Count; ++i)
				{
					processingManager.ExecuteReadCommand(pnts[i].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, pnts[i].ConfigItem.StartAddress, 1);
				}
			}
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls


        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Indication if managed objects should be disposed.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
				}
				disposedValue = true;
			}
		}


		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// GC.SuppressFinalize(this);
		}

        /// <inheritdoc />
        public void Start(int delayBetweenCommands)
		{
			this.delayBetweenCommands = delayBetweenCommands*1000;
            InitializeAndStartThreads();
		}

        /// <inheritdoc />
        public void Stop()
		{
			Dispose();
		}
		#endregion
	}
}
