using System; 
using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBCook : SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBCook() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{
                Add(new GenericBuyInfo("Skillet", typeof(Skillet), Skillet.GetSBPurchaseValue(), 50, 0x97F, 0));
                Add(new GenericBuyInfo("Rolling Pin", typeof(RollingPin), RollingPin.GetSBPurchaseValue(), 50, 0x1043, 0));
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{
                Add(typeof(Skillet), Skillet.GetSBSellValue());
                Add(typeof(RollingPin), Skillet.GetSBSellValue());
			} 
		} 
	} 
}