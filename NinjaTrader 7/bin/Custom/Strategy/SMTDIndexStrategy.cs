#region Using declarations
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Strategy;
#endregion

// This namespace holds all strategies and is required. Do not change it.
namespace NinjaTrader.Strategy
{
    /// <summary>
    /// This strategy uses the SMTD indicator to go long on the bullish signals generated by the indicator and short on the bearish signals.
	///
	/// Requirements:
	/// https://www.bigmiketrading.com/elite-circle/32758-want-your-ninjatrader-strategy-created-free.html#post427520
	/// 
	/// * when private BoolSeries	upDeviceSeries; go long next bar at market
	/// * when private BoolSeries	downDeviceSeries; go short next bar at market
	/// * stop is 15 ticks
	/// * profit taking is 20 ticks
	///
	/// If an opposite signal is given before the profit taking is reached please reverse the position. 
	/// The stop and profit taking please coded as variables so they can be optimize.
    /// </summary>
    [Description("This strategy uses the SMTD indicator to go long on the bullish signals generated by the indicator and short on the bearish signals.")]
    public class SMTDIndexStrategy : Strategy
    {
        #region Variables
		private SMTDIndex smtdInd;
		
		private double 	dTargetTicks 	= 40;
		private int  	iStopLoss 		= 30;
		
		#endregion

        /// <summary>
        /// This method is used to configure the strategy and is called once before any strategy method is called.
        /// </summary>
        protected override void Initialize()
        {
            CalculateOnBarClose = true;
			
			smtdInd = SMTDIndex();
			Add(smtdInd);
			
			// Profit target and stop loss
			SetProfitTarget( CalculationMode.Ticks, ProfitTargetTicks );
			SetStopLoss( CalculationMode.Ticks, StopLoss );

        }

        /// <summary>
        /// Called on each bar update event (incoming tick)
        /// </summary>
        protected override void OnBarUpdate()
        {
			//if (Time[0].DayOfWeek == DayOfWeek.Friday)
			//	return;
			
			if( Position.MarketPosition == MarketPosition.Flat
				|| Position.MarketPosition == MarketPosition.Short )
			{
				if( smtdInd.UpDeviceSeries[0] == true ) {
					EnterLong();
				}
			}
			if( Position.MarketPosition == MarketPosition.Flat
				|| Position.MarketPosition == MarketPosition.Long )
			{
				if( smtdInd.DownDeviceSeries[0] == true ) {
					EnterShort();
				}
			}
        }

        #region Properties
		[Description("Profit target, in ticks.")]
		[Gui.Design.DisplayNameAttribute("Profit Target")]
		[Category("Parameters")]
		public double ProfitTargetTicks
		{
			get { return dTargetTicks; }
			set { dTargetTicks = value; }
		}
		[Description("Stop loss, in ticks")]
		[Gui.Design.DisplayNameAttribute("Stop loss")]
		[Category("Parameters")]
		public int StopLoss
		{
			get { return iStopLoss; }
			set { iStopLoss = Math.Max( 1, value); }
		}
        #endregion
    }
}