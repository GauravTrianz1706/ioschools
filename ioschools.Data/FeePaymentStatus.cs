using System.ComponentModel;

namespace ioschools.Data
{
    /// <summary>
    /// Represents the payment status of a school fee invoice.
    /// Migrated from legacy ASP.NET MVC enum — unnecessary using directives removed.
    /// </summary>
    public enum FeePaymentStatus
    {
        /// <summary>Payment status is unknown or not yet set.</summary>
        [Description("")]
        UNKNOWN = 0,

        /// <summary>The fee has not been paid.</summary>
        [Description("UNPAID")]
        UNPAID = 1,

        /// <summary>The fee has been paid in full.</summary>
        [Description("PAID")]
        PAID = 2
    }
}
