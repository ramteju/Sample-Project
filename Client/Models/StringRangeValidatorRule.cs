using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Windows.Controls;
using Client.Common;
using Entities;
using Client.Logging;

namespace Client.Models
{
    public class StringRangeValidationRule : ValidationRule
    {
        private int _minimumLength = -1;
        private int _maximumLength = -1;
        private string _errorMessage;

        public int Minimum
        {
            get { return _minimumLength; }
            set { _minimumLength = value; }
        }

        public int Maximum
        {
            get { return _maximumLength; }
            set { _maximumLength = value; }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                ValidationResult result = new ValidationResult(true, null);
                string inputString = (value ?? string.Empty).ToString();

                if (inputString == null || inputString == "")
                {
                    inputString = string.Empty;
                    result = new ValidationResult(false, this.ErrorMessage);
                }

                else if (Convert.ToDouble(inputString) < this.Minimum ||
                       (this.Maximum > 0 &&
                        Convert.ToDouble(inputString) > this.Maximum))
                {
                    result = new ValidationResult(false, this.ErrorMessage);
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }
    }
}
