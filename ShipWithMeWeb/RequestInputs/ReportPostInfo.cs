using System.ComponentModel.DataAnnotations;

namespace ShipWithMeWeb.RequestInputs
{
    public sealed class ReportPostInfo
    {
        [MinLength(1)]
        public string Message { get; set; }
    }
}
