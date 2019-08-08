using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogicUniversity.Models
{
    public class ItemCodeRequisition
    {//not for db
        public Products Product;
        public double Retrieved;
        //techncially this is product quantity
        public double QtyInInventory;
        public double TotalNeeded;
        public List<Department> DeptName;
        public List<double> NeededList;

        private List<ItemCodeRequisition> ICRList;
        public IEnumerator<ItemCodeRequisition> GetEnumerator()
        {
            return ICRList.GetEnumerator();
        }
    }
}