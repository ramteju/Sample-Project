using Client.Logging;
using Client.ViewModels;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.Common
{
    public class ExtentionMethods
    {
        public string GetPressureAndUnitsFromString(string _pressure, out string _pres_units)
        {
            string strPres = string.Empty;
            string strPresUnits = "a";
            try
            {
                if (_pressure.Contains("-") || _pressure.Contains("]"))
                {
                    string[] pressureValues = _pressure.Split(new string[] { "-", "]" }, StringSplitOptions.RemoveEmptyEntries);
                    _pressure = pressureValues.Count() > 1 ? ((_pressure.Contains($"--{pressureValues[1]}") || _pressure.Contains($"]-{pressureValues[1]}")) ? $"-{pressureValues[1]}" : pressureValues[1]) : _pressure.Contains($"-{pressureValues[0]}") ? $"-{pressureValues[0]}" : pressureValues[0];
                }

                strPres = _pressure;
                
                string[] splitter = { "b", "kb", "mb", "j", "m", "p", "kp", "mp", "gp", "hp", "s", "t", "kc", "m", "s", "d", "w", "mo", "ms" };
                string[] strPresVals = strPres.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                if (strPresVals != null)
                {
                    if (strPresVals.Length > 0)
                    {
                        strPresUnits = Regex.Replace(strPres.ToString(), "[0-9.\\-\\](<|>)]", string.Empty);
                        if (strPresUnits == "")
                        {
                            strPresUnits = "a";
                        }
                        else
                        {
                        }
                        strPres = strPresVals[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            _pres_units = strPresUnits;
            return strPres;
        }
        public string GetTemperatureAndUnitsFromString(string _temp, out string _temp_units, StageConditionVM condition = null)
        {
            string strTemp = _temp;
            string strTempUnits = "c";
            try
            {
                if (condition != null)
                {
                    if (_temp.Contains("]") || _temp.Contains("-"))
                    {
                        string[] tempValues = _temp.Split(new string[] { "-", "]", "+/-" }, StringSplitOptions.RemoveEmptyEntries);
                        if (condition.TEMP_TYPE == TemperatureEnum.Refluxto.ToString())
                            _temp = tempValues[1];
                        else if (condition.TEMP_TYPE == TemperatureEnum.toReflux.ToString())
                            _temp = tempValues[0];
                        else if (condition.TEMP_TYPE == TemperatureEnum.Roomtempto.ToString())
                            _temp = tempValues[1];
                        else if (condition.TEMP_TYPE == TemperatureEnum.toRoomTemp.ToString())
                            _temp = tempValues[0];
                        else if (condition.TEMP_TYPE == TemperatureEnum.Directional.ToString())
                            _temp = tempValues[1];
                        else if (condition.TEMP_TYPE == TemperatureEnum.Rang.ToString())
                            _temp = tempValues[1];
                        else if (condition.TEMP_TYPE == TemperatureEnum.pluseDevminus.ToString())
                            _temp = tempValues[1];
                    }
                }
                string[] splitter = { "f", "k", "r" };
                string[] strTempVals = strTemp.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                if (strTempVals != null)
                {
                    if (strTempVals.Length > 0)
                    {
                        strTempUnits = Regex.Replace(_temp.ToString(), "[0-9.\\-\\](<|>)]", "");
                        if (strTempUnits == "")
                        {
                            strTempUnits = "c";
                        }
                        else
                        {
                        }
                        strTemp = strTempVals[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            _temp_units = strTempUnits;
            return strTemp;
        }
        public string GetTimeAndUnitsFromString(string _time, out string _time_units)
        {
            string strTime = _time;
            string strTimeUnits = "h";
            try
            {
                string[] splitter = { "m", "s", "d", "w", "mo", "ms" };
                string[] strTimeVals = strTime.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                if (strTimeVals != null)
                {
                    if (strTimeVals.Length > 0)
                    {
                        strTimeUnits = Regex.Replace(_time.ToString(), "[0-9.\\-\\]]", "");
                        if (strTimeUnits == "")
                        {
                            strTimeUnits = "h";
                        }
                        else
                        {
                        }
                        strTime = strTimeVals[0];
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            _time_units = strTimeUnits;
            return strTime;
        }




    }
}
