using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_EOM_Utility
{
    public static class SD
    //Roles....
    {
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee User";
        public const string Role_Company = "Company User";
        public const string Role_Individual = "Individual User";

        //session cart count variable
        public const string ss_CartSessionCount = "Cart Count Session";

        //payment
        public static double GetPriceBasedOnQuantity(double quantity, double price, double price50, double
    price100)
        {
            if (quantity < 50)
                return price;
            else if (quantity < 100)
                return price50;
            else
                return price100;
        }

        //orderstatus and paymentstatus
        //Order Status
        public const string OrderStatusPending="Pending";
        public const string OrderStatusApproved = "Approved";
        public const string OrderStatusProcessing = "Processing";
        public const string OrderStatusShipped = "Shipped";
        public const string OrderStatusCancelled = "Cancelled";
        public const string OrderStatusRefunded = "Refunded";
        //Payment Status
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayPayment = "PaymentStatusDelay";
        public const string PaymentStatusRejected = "Rejected";

      

    }

}
