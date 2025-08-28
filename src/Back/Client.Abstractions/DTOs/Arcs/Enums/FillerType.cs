using System.ComponentModel.DataAnnotations;

namespace GrandLineQuotes.Client.Abstractions.DTOs.Arcs.Enums
{
    public enum FillerType
    {


        [Display(Name = "Select one")]
        UNDEFINED = 0,
        [Display(Name = "Canon")]
        CANON = 1,
        [Display(Name = "Partial")]
        PARTIAL = 2,
        [Display(Name = "Full")]
        FULL = 3
    }
}
