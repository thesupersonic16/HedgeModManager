using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HedgeModManager.UI
{
    public class RangeValidator : ValidationRule
    {
        public double Min { get; set; }
        public double Max { get; set; }
        
        public RangeValidator()
        {

        }

        public RangeValidator(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(!double.TryParse((string)value, out double result))
                return new ValidationResult(false, "Invalid Characters.");

            if(result < Min || result > Max)
                return new ValidationResult(false, $"The value must be in {Min}-{Max} range.");

            return ValidationResult.ValidResult;
        }
    }
}
