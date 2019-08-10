using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    public class ItemCodeDisbursement
    {
        //not for db
        public Products Product;
        public double Quantity;
        public double CollectedList;

        private List<ItemCodeDisbursement> ICDList;
        public IEnumerator<ItemCodeDisbursement> GetEnumerator()
        {
            return ICDList.GetEnumerator();
        }
    }
}