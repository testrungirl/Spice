using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Utility
{
    public static class SD
    {
        public const string DefaultFoodImage = "DefaultImg.jpg";

        public const string ManagerUser = "Manager";
        public const string KitchenUser = "Kitchen";
        public const string FrontDeskUser = "FrontDesk";
        public const string CustomerEnduser = "Customer";

        public const string ssShoppingCartCount = "ssCartCount";
        public const string ssCouponCode = "ssCouponCode";

        public const string StatusSubmitted = "Submitted";
        public const string StatusInProcess = "Being prepared";
        public const string StatusReady = "ready for Pickup";
        public const string StatusCompleted = "Completed";
        public const string StatusCancled = "Canceled";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusRejected = "Rejected";

        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for(int i = 0; i<source.Length; i++)
            {
                char letter = source[i];
                if(letter == '<')
                {
                    inside = true;
                    continue;
                }
                if(letter == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = letter;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
        public static double DiscountedPrice(Coupon obj, double OriginalOrderTotal)
        {
            if (obj == null)
            {
                return OriginalOrderTotal;
            }
            else
            {
                if (obj.MinimumAmount > OriginalOrderTotal)
                {
                    return OriginalOrderTotal;
                }
                else
                {
                    //everything is valid
                    if (Convert.ToInt32(obj.CouponType) == (int)Coupon.ECouponType.Dollar)
                    {
                        //$10 off $100
                        return Math.Round(OriginalOrderTotal - obj.Discount, 2);
                    }
                    if (Convert.ToInt32(obj.CouponType) == (int)Coupon.ECouponType.percent)
                    {
                        //10% off $100
                        return Math.Round(OriginalOrderTotal - (OriginalOrderTotal * obj.Discount / 100), 2);
                    }
                }
            }
            return OriginalOrderTotal;
        }
    }
}
