using System;
using System.Collections.Generic;
using System.Text;

namespace Startup
{
    public class DealManager
    {
        private static Dictionary<int, DealStatistic> DealCounter = new Dictionary<int, DealStatistic>();
        private static Dictionary<string, Hold> Holds = new Dictionary<string, Hold>();

        public DealStatistic GetDealStatistic(int id)
        {
            var dealStatistic = DealCounter.GetValueOrDefault(id);
            if (dealStatistic == null)
            {
                dealStatistic = new DealStatistic(100, 0, 100,0);
                DealCounter.Add(id, dealStatistic);
            }
            return dealStatistic;
        }
        public void StartHold(int id, int quantity)
        {
            var dealStatistic = GetDealStatistic(id);
            if (dealStatistic.Available > quantity)
                dealStatistic.Available -= quantity;
            DealCounter[id] = dealStatistic;

            var hold = new Hold(id, quantity);
            Holds.Add(hold.Id, hold);
        }
        public void EndHold(string holdId, bool success)
        {
            var hold = Holds[holdId];
            hold.FinishedAt = DateTime.Now;

            var dealStatistic = GetDealStatistic(hold.DealId);
            if (success)
            {
                hold.Success = true;

                dealStatistic.Onhand -= hold.Quantity;
                dealStatistic.Hold -= hold.Quantity;
                dealStatistic.Resolved += hold.Quantity;
            }
            else
            {
                hold.Success = false;

                dealStatistic.Onhand += hold.Quantity;
                dealStatistic.Available += hold.Quantity;
                dealStatistic.Hold -= hold.Quantity;
            }
        }
    }
    public class Hold
    {
        public string Id { get; set; }
        public int DealId { get; set; }
        public int Quantity { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public bool? Success { get; set; }
        public int LifetimeInSecond { get; set; }

        public Hold(int dealId, int quantity)
        {
            DealId = dealId;
            Quantity = quantity;
            StartedAt = DateTime.Now;
            Id = Guid.NewGuid().ToString();
            LifetimeInSecond = 120;
        }
    }
    public class DealStatistic
    {
        public int Onhand { get; set; }
        public int Hold { get; set; }
        public int Available { get; set; }
        public int Resolved { get; set; }

        public DealStatistic(int onhand, int hold, int available, int resolved)
        {
            Onhand = onhand;
            Hold = hold;
            Available = available;
            Resolved = resolved;
        }
    }
}
