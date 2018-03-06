using Client.Command;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Client.Common;
using ProductTracking.Models.Core;
using System.Collections.Generic;
using Entities;
using Client.Logging;
using Client.Views;

namespace Client.ViewModels
{
    public class ConditionVM : ViewModelBase
    {
        private string selectTPText, selectRang2, selectPhRange1, selectRoomtempto,
           selecttoRoomTemp, selectDirectional1, selectDirectional2, selecttoReflux, selectRang1,
           selectRefluxto, selectPlessMinues1, selectPlessMinues2, selectPHRang2, selectPhText, selectTimeText,
            selectTimeRangetext1, selectTimeRangetext2, selectPressureText, selectPressureRangetext1,
            selectPressureRangetext2, selectPressureDirectional1, selectPressureDirectional2, rempresult, pHresult, timeresult, pressureresult;
        private TemperatureCodeVM selectValue, selectedComboValue, selectedComboRoomtempto, selectedComboRefluxto, selectedCombotoReflux, selectedComboPluessorMinnus, slectedCombotoRoomTemp, selectedComboDirectional, selectedCombotoRoomTemp, ResetTempComboValues,
            ResetTempComboRoomtempto, ResetTempCombotoRoomTemp, ResetTempComboDirectional, ResetTempComboRang, ResetTempComboRefluxto, ResetTempCombotoReflux, ResetTempComboPluessorMinnus;
        private TimeCombovalues timeCombovalues;
        private TimeComboCodevalues timeComboCodevalues;
        private PressureComboValues selectPressureValue, selectedPressureComboDirectional, selectedPressureCoboRange;

        private System.Windows.Visibility conditionVisible;
        private string TemperatureValue, TimeValue, pHVValue, PressureValue, Temp_TypeValue, Time_TypeValue, pH_TypeValue, Pressure_TypeValue;

        private string regExpNum = @"^[><\-.0-9\s]+";

        public ConditionVM()
        {
            selectValue = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
            selectedComboValue = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
            selectedCombotoRoomTemp = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
            selectedComboRoomtempto = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
            slectedCombotoRoomTemp = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
            selectedComboDirectional = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
            selectedComboRefluxto = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
            selectedCombotoReflux = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
            selectedComboPluessorMinnus = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
            timeCombovalues = StaticCollection.TimeCombovalues.ToList().Where(x => x.Name == "Select").SingleOrDefault();
            timeComboCodevalues = StaticCollection.TimeComboCodevalues.ToList().Where(x => x.Value == "h").SingleOrDefault();
            selectPressureValue = StaticCollection.PressureComboValuess.ToList().Where(x => x.Value == "a").SingleOrDefault();
            ConditonAdd = new Command.DelegateCommand(this.AddSelectedCondition);
            ConditonReset = new DelegateCommand(this.ResetConditon);
            ConditionVisible = System.Windows.Visibility.Visible;
        }

        public DelegateCommand DisplayConditionView { get; private set; }
        public DelegateCommand ConditonAdd { get; private set; }
        public DelegateCommand ConditonReset { get; private set; }
        private void ResetConditon(object s)
        {
            UnselectCondition();
            //SelectradioNoon = "True";
            //SelectedradioPressureNone = "True";
            //SelectedradiopHNone = "True";
            //SelectedradioTimeNone = "True";
        }
        #region Add Condition
        private void AddSelectedCondition(object s)
        {

            try
            {

                bool blTemp;
                #region Add Temperature
                if (TemperatureEnum.Temp.ToString() == TempResult)
                {
                    if (SelectTempTemperature.Trim() == "")
                    {
                        AppInfoBox.ShowInfoMessage("Please Enter Temp Value");
                        return;
                    }
                    else
                    {
                        blTemp = IsValidNumber(SelectTempTemperature.Trim());
                        if (blTemp == false)
                        {
                            AppInfoBox.ShowInfoMessage("In-Valid Temperature value");
                            return;
                        }
                        else
                        {
                            if (SelectedComboValue.Value == "c")
                            {
                                TemperatureValue = SelectTempTemperature;
                                Temp_TypeValue = TemperatureEnum.Temp.ToString();

                            }

                            else
                            {
                                TemperatureValue = SelectTempTemperature + SelectedComboValue.Value;
                                Temp_TypeValue = TemperatureEnum.Temp.ToString();
                            }
                        }
                    }
                }
                else if (TemperatureEnum.toReflux.ToString() == TempResult)
                {
                    if (selecttoReflux.Trim() == "")
                    {
                        AppInfoBox.ShowInfoMessage("Please Enter To Reflux Value");
                        return;
                    }
                    else
                    {
                        blTemp = IsValidNumber(selecttoReflux.Trim());
                        if (blTemp == false)
                        {
                            AppInfoBox.ShowInfoMessage("In-Valid Temperature value");
                            return;
                        }
                        else
                        {
                            if (selecttoReflux.Contains(">"))
                            {
                                AppInfoBox.ShowInfoMessage("> and < Not Allowled in toReflux Values");
                                return;
                            }
                            else
                            {
                                if (SelectedCombotoReflux.Value == "c")
                                {

                                    TemperatureValue = selecttoReflux + "]x";
                                    Temp_TypeValue = TemperatureEnum.toReflux.ToString();

                                }
                                else
                                {

                                    TemperatureValue = selecttoReflux + SelectedCombotoReflux.Value + "]x";
                                    Temp_TypeValue = TemperatureEnum.toReflux.ToString();
                                }
                            }
                        }
                    }
                }
                else if (TemperatureEnum.Roomtempto.ToString() == TempResult)
                {
                    if (SelectRoomtempto.Trim() == "")
                    {
                        AppInfoBox.ShowInfoMessage("Please Enter Room temp to Value");
                        return;
                    }
                    else
                    {
                        blTemp = IsValidNumber(SelectRoomtempto.Trim());
                        if (blTemp == false)
                        {
                            AppInfoBox.ShowInfoMessage("In-Valid Temperature value");
                            return;
                        }
                        else
                        {
                            if (SelectRoomtempto.Contains(">"))
                            {
                                AppInfoBox.ShowInfoMessage("> and < Not Allowled in Roomtempto Values");
                                return;
                            }
                            else
                            {
                                if (SelectedComboRoomtempto.Value == "c")
                                {

                                    TemperatureValue = "a]" + SelectRoomtempto;
                                    Temp_TypeValue = TemperatureEnum.Roomtempto.ToString();

                                }
                                else
                                {

                                    TemperatureValue = "a]" + SelectRoomtempto + SelectedComboRoomtempto.Value;
                                    Temp_TypeValue = TemperatureEnum.Roomtempto.ToString();

                                }
                            }
                        }
                    }
                }
                else if (TemperatureEnum.toRoomTemp.ToString() == TempResult)
                {
                    if (SelecttoRoomTemp.Trim() == "")
                    {
                        AppInfoBox.ShowInfoMessage("Please Enter to Room Temp Value");
                        return;
                    }
                    else
                    {
                        blTemp = IsValidNumber(SelecttoRoomTemp.Trim());
                        if (blTemp == false)
                        {
                            AppInfoBox.ShowInfoMessage("In-Valid Temperature value");
                            return;
                        }
                        else
                        {
                            if (SelecttoRoomTemp.Contains(">"))
                            {
                                AppInfoBox.ShowInfoMessage("> and < Not Allowled in toRoomTemp Values");
                                return;
                            }
                            else
                            {
                                if (SelectedCombotoRoomTemp.Value == "c")
                                {

                                    TemperatureValue = SelecttoRoomTemp + "]a";
                                    Temp_TypeValue = TemperatureEnum.toRoomTemp.ToString();

                                }
                                else
                                {
                                    TemperatureValue = SelecttoRoomTemp + SelectedCombotoRoomTemp.Value + "]a";
                                    Temp_TypeValue = TemperatureEnum.toRoomTemp.ToString();
                                }
                            }
                        }
                    }
                }
                else if (TemperatureEnum.Directional.ToString() == TempResult)
                {
                    if (SelectDirectional1.Trim() == "" || SelectDirectional2.Trim() == "")
                    {
                        AppInfoBox.ShowInfoMessage("Please Enter Directional Value");
                        return;
                    }
                    else
                    {
                        blTemp = IsValidNumber(SelectDirectional1.Trim());
                        bool blTempsirectional2 = IsValidNumber(SelectDirectional2.Trim());
                        if (blTemp == false || blTempsirectional2 == false)
                        {
                            AppInfoBox.ShowInfoMessage("In-Valid Temperature value");
                            return;
                        }
                        else
                        {
                            if (SelectDirectional1.Contains(">") || SelectDirectional2.Contains(">"))
                            {
                                AppInfoBox.ShowInfoMessage("> and < Not Allowled in Directional Values");
                                return;
                            }
                            else
                            {
                                if (Convert.ToDouble(SelectDirectional1) == Convert.ToDouble(SelectDirectional2))
                                {
                                    AppInfoBox.ShowInfoMessage("Directional Value  can't be same");
                                    return;
                                }
                                else
                                {
                                    if (SelectedComboDirectional.Value == "c")
                                    {

                                        TemperatureValue = SelectDirectional1 + "]" + SelectDirectional2;
                                        Temp_TypeValue = TemperatureEnum.Directional.ToString();
                                    }
                                    else
                                    {
                                        if (Convert.ToDouble(SelectDirectional1) <= Convert.ToDouble(SelectDirectional2))
                                        {
                                            TemperatureValue = SelectDirectional1 + SelectedComboDirectional.Value + "]" + SelectDirectional2 + SelectedComboDirectional.Value;
                                            Temp_TypeValue = TemperatureEnum.Directional.ToString();
                                        }

                                    }
                                }
                            }
                        }
                    }

                }

                else if (TemperatureEnum.Rang.ToString() == TempResult)
                {
                    if (SelectRang1.Trim() == "" || SelectRang2.Trim() == "")
                    {
                        AppInfoBox.ShowInfoMessage("Please Enter Range Value");
                        return;
                    }
                    else
                    {
                        blTemp = IsValidNumber(SelectRang1.Trim());
                        bool blTempPlessMinus = IsValidNumber(SelectRang2.Trim());
                        if (blTemp == false || blTempPlessMinus == false)
                        {
                            AppInfoBox.ShowInfoMessage("In-Valid Temperature value");
                            return;
                        }
                        else
                        {


                            if (SelectRang1.Contains(">") || SelectRang2.Contains(">"))
                            {
                                AppInfoBox.ShowInfoMessage("> and < Not Allowled in Range Values");
                                return;
                            }
                            else
                            {
                                if (Convert.ToDouble(SelectRang1) >= Convert.ToDouble(SelectRang2))
                                {
                                    AppInfoBox.ShowInfoMessage("left value should be less than right value");
                                    return;
                                }
                                else
                                {
                                    if (SelectedComboRang.Value == "c")
                                    {
                                        TemperatureValue = SelectRang1 + "-" + SelectRang2;
                                        Temp_TypeValue = TemperatureEnum.Rang.ToString();
                                    }
                                    else
                                    {
                                        TemperatureValue = SelectRang1 + SelectedComboRang.Value + "-" + SelectRang2 + SelectedComboRang.Value;
                                        Temp_TypeValue = TemperatureEnum.Rang.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
                else if (TemperatureEnum.Refluxto.ToString() == TempResult)
                {
                    if (SelectRefluxto.Trim() == "")
                    {
                        AppInfoBox.ShowInfoMessage("Please Enter Reflux to Value");
                        return;
                    }
                    else
                    {
                        blTemp = IsValidNumber(SelectRefluxto.Trim());
                        if (blTemp == false)
                        {
                            AppInfoBox.ShowInfoMessage("In-Valid Temperature value");
                            return;
                        }
                        else
                        {
                            if (SelectRefluxto.Contains(">"))
                            {
                                AppInfoBox.ShowInfoMessage("> and < Not Allowled in Refluxto Values");
                                return;
                            }
                            else
                            {
                                if (SelectedComboRefluxto.Value == "c")
                                {

                                    TemperatureValue = "x]" + SelectRefluxto;
                                    Temp_TypeValue = TemperatureEnum.Refluxto.ToString();

                                }
                                else
                                {

                                    TemperatureValue = "x]" + SelectRefluxto + SelectedComboRefluxto.Value;
                                    Temp_TypeValue = TemperatureEnum.Refluxto.ToString();

                                }
                            }
                        }
                    }
                }
                else if (TemperatureEnum.pluseDevminus.ToString() == TempResult)
                {
                    if (selectPlessMinues1.Trim() == "" || selectPlessMinues2.Trim() == "")
                    {
                        AppInfoBox.ShowInfoMessage("Please Enter +/- Value");
                        return;
                    }
                    else
                    {
                        blTemp = IsValidNumber(selectPlessMinues1.Trim());
                        bool blTempPlessMinus = IsValidNumber(selectPlessMinues2.Trim());
                        if (blTemp == false || blTempPlessMinus == false)
                        {
                            AppInfoBox.ShowInfoMessage("In-Valid Temperature value");
                            return;
                        }
                        else
                        {
                            if (selectPlessMinues1 == selectPlessMinues2)
                            {
                                AppInfoBox.ShowInfoMessage("+/- Value  can't be same");
                                return;
                            }
                            else
                            {
                                if (SelectedComboPluessorMinnus.Value == "c")
                                {

                                    TemperatureValue = selectPlessMinues1 + "+/-" + selectPlessMinues2;
                                    Temp_TypeValue = TemperatureEnum.pluseDevminus.ToString();

                                }
                                else
                                {

                                    TemperatureValue = selectPlessMinues1 + SelectedComboPluessorMinnus.Value + "+/-" + selectPlessMinues2 + SelectedComboPluessorMinnus.Value;
                                    Temp_TypeValue = TemperatureEnum.pluseDevminus.ToString();

                                }
                            }
                        }
                    }
                }
                else if (TemperatureEnum.RoomTemptoReflux.ToString() == TempResult)
                {
                    TemperatureValue = "a]x";
                    Temp_TypeValue = TemperatureEnum.RoomTemptoReflux.ToString();
                }
                else if (TemperatureEnum.Roomtemp.ToString() == TempResult)
                {
                    TemperatureValue = "a";
                    Temp_TypeValue = TemperatureEnum.Roomtemp.ToString();
                }
                else if (TemperatureEnum.RefluxBoiled.ToString() == TempResult)
                {
                    TemperatureValue = "x";
                    Temp_TypeValue = TemperatureEnum.RefluxBoiled.ToString();
                }
                else if (TemperatureEnum.RefluxtoRoomTemp.ToString() == TempResult)
                {
                    TemperatureValue = "x]a";
                    Temp_TypeValue = TemperatureEnum.RefluxtoRoomTemp.ToString();
                }
                else if (TemperatureEnum.Cool.ToString() == TempResult)
                {
                    TemperatureValue = "c";
                    Temp_TypeValue = TemperatureEnum.Cool.ToString();
                }
                else if (TemperatureEnum.LessthanRoomTemp.ToString() == TempResult)
                {
                    TemperatureValue = "<a";
                    Temp_TypeValue = TemperatureEnum.LessthanRoomTemp.ToString();
                }
                else if (TemperatureEnum.equels.ToString() == TempResult)
                {

                    TemperatureValue = "=";
                    Temp_TypeValue = TemperatureEnum.equels.ToString();
                }
                else if (TemperatureEnum.Heated.ToString() == TempResult)
                {
                    TemperatureValue = "h";
                    Temp_TypeValue = TemperatureEnum.Heated.ToString();
                }
                else if (TemperatureEnum.RoomTempgraterthan.ToString() == TempResult)
                {
                    TemperatureValue = ">a";
                    Temp_TypeValue = TemperatureEnum.RoomTempgraterthan.ToString();
                }
                else
                {
                    TemperatureValue = "";
                    Temp_TypeValue = TemperatureEnum.None.ToString();
                }
                #endregion
                #region Maintain Time
                if (TimeEnum.Time.ToString() == TimeResult)
                {
                    if (SelectTimeValue.Trim() == "")
                    {
                        AppInfoBox.ShowInfoMessage("Please Enter Time Value");
                        return;
                    }
                    else
                    {
                        blTemp = IsValidNumber(SelectTimeValue.Trim());
                        if (blTemp == false)
                        {
                            AppInfoBox.ShowInfoMessage("In-Valid Time value");
                            return;
                        }
                        else
                        {
                            if (SelectTimeValue.Contains("<") || SelectTimeValue.Contains(">"))
                            {
                                if (SelectedTimeComboValue.Value == "h")
                                {
                                    TimeValue = SelectTimeValue;
                                    Time_TypeValue = TimeEnum.Time.ToString();
                                }
                                else
                                {
                                    TimeValue = SelectTimeValue + SelectedTimeComboValue.Value;
                                    Time_TypeValue = TimeEnum.Time.ToString();
                                }
                            }
                            else if (Convert.ToDouble(SelectTimeValue) <= 0)
                            {
                                AppInfoBox.ShowInfoMessage("In-Valid Time value");
                                return;
                            }
                            else
                            {
                                if (SelectTimeValue.StartsWith("0"))
                                {
                                    if (SelectTimeValue.Contains("."))
                                    {
                                        int t = SelectTimeValue.IndexOf('.');
                                        if (Convert.ToDouble(SelectTimeValue.IndexOf('.')) > 1)
                                        {
                                            AppInfoBox.ShowInfoMessage("In-Valid Time value");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        AppInfoBox.ShowInfoMessage("In-Valid Time value");
                                        return;
                                    }
                                }

                                if (SelectedTimeComboValue.Value == "h")
                                {
                                    TimeValue = SelectTimeValue;
                                    Time_TypeValue = TimeEnum.Time.ToString();
                                }
                                else
                                {
                                    TimeValue = SelectTimeValue + SelectedTimeComboValue.Value;
                                    Time_TypeValue = TimeEnum.Time.ToString();
                                }
                            }
                        }
                    }
                }
                else if (TimeEnum.Rang.ToString() == TimeResult)
                {
                    if (SelectTimeRangetext1.Trim() == "" || SelectTimeRangetext2.Trim() == "")
                    {
                        AppInfoBox.ShowInfoMessage("Please Enter Time Range Value");
                        return;
                    }
                    else
                    {
                        blTemp = IsValidNumber(SelectTimeRangetext1.Trim());
                        bool blTempPlessMinus = IsValidNumber(SelectTimeRangetext2.Trim());
                        if (blTemp == false || blTempPlessMinus == false)
                        {
                            AppInfoBox.ShowInfoMessage("In-Valid Time value");
                            return;
                        }
                        else
                        {

                            string[] splitter = { "<", ">" };
                            string[] strPresValsRange1 = SelectTimeRangetext1.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                            string[] strPresValsRange2 = SelectTimeRangetext2.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                            if (Convert.ToDouble(strPresValsRange1[0]) >= Convert.ToDouble(strPresValsRange2[0]))
                            {
                                AppInfoBox.ShowInfoMessage("left value should be less than right value");
                                return;
                            }
                            else
                            {
                                if (SelectedCoboTimeRange.Value == "h")
                                {
                                    TimeValue = SelectTimeRangetext1 + "-" + SelectTimeRangetext2;
                                    Time_TypeValue = TimeEnum.Rang.ToString();
                                }
                                else
                                {
                                    TimeValue = SelectTimeRangetext1 + SelectedCoboTimeRange.Value + "-" + SelectTimeRangetext2 + SelectedCoboTimeRange.Value;
                                    Time_TypeValue = TimeEnum.Rang.ToString();
                                }
                            }
                            //}
                        }
                    }
                }
                else if (TimeEnum.InExactTime.ToString() == TimeResult)
                {
                    if (SelectedCoboTimeInExact.Value != null)
                    {
                        string getCode = GetSelectedINExactTimeCode(SelectedCoboTimeInExact.Name);
                        TimeValue = SelectedCoboTimeInExact.Value;
                        Time_TypeValue = TimeEnum.InExactTime.ToString() + getCode;
                    }
                }
                else if (TimeEnum.Overnight.ToString() == TimeResult)
                {
                    TimeValue = "o";
                    Time_TypeValue = TimeEnum.Overnight.ToString();
                }
                else if (TimeEnum.equals.ToString() == TimeResult)
                {
                    TimeValue = "=";
                    Time_TypeValue = TimeEnum.equals.ToString();
                }
                else
                {
                    TimeValue = "";
                    Time_TypeValue = TimeEnum.None.ToString();
                }
                #endregion
                #region Maintain pH



                if (PHEnum.pH.ToString() == pHResult)
                {
                    string[] splitterPlus = { "<", ">" };
                    string[] strValsPlus = SelectpH.Split(splitterPlus, StringSplitOptions.RemoveEmptyEntries);
                    //if (SelectpH.Contains(">"))
                    //{
                    //    AppInfoBox.ShowInfoMessage("> and < Not Allowled in Ph Values");
                    //    return;
                    //}
                    //else
                    //{
                    blTemp = IsValidNumber(SelectpH.Trim());
                    if (blTemp == false)
                    {
                        AppInfoBox.ShowInfoMessage("In-Valid PH value");
                        return;
                    }
                    else
                    {
                        if (SelectpH.Trim() == "" || SelectpH.Trim() == "14." || Convert.ToDouble(strValsPlus[0]) > 14)
                        {
                            AppInfoBox.ShowInfoMessage("Please Enter pH Value Below 14");
                            return;
                        }
                        else
                        {

                            pHVValue = SelectpH;
                            pH_TypeValue = PHEnum.pH.ToString();
                        }
                    }
                    //}
                }
                else if (PHEnum.Range.ToString() == pHResult)
                {
                    //if (SelectPHRange1.Contains(">") || SelectPHRange2.Contains(">"))
                    //{
                    //    AppInfoBox.ShowInfoMessage("> and < Not Allowled in Range Values");
                    //    return;
                    //}
                    //else
                    //{

                    blTemp = IsValidNumber(SelectPHRange1.Trim());
                    bool blTempPlessMinus = IsValidNumber(SelectPHRange2.Trim());
                    if (blTemp == false || blTempPlessMinus == false)
                    {
                        AppInfoBox.ShowInfoMessage("In-Valid PH value");
                        return;
                    }
                    else
                    {

                        string[] splitterPlus = { "<", ">" };
                        string[] strValsPlus = SelectPHRange1.Split(splitterPlus, StringSplitOptions.RemoveEmptyEntries);

                        string[] strValsPlus1 = SelectPHRange2.Split(splitterPlus, StringSplitOptions.RemoveEmptyEntries);
                        if (SelectPHRange1.Trim() == "" || SelectPHRange2.Trim() == "" || SelectPHRange1 == "14." || SelectPHRange2 == "14." || Convert.ToDouble(strValsPlus[0]) > 14 || Convert.ToDouble(strValsPlus1[0]) > 14)
                        {
                            AppInfoBox.ShowInfoMessage("Please Enter pH Range Value Below 14");
                            return;
                        }
                        else
                        {

                            if (Convert.ToDouble(strValsPlus[0]) >= Convert.ToDouble(strValsPlus1[0]))
                            {
                                AppInfoBox.ShowInfoMessage("left value should be less than right value");
                                return;
                            }
                            else
                            {
                                pHVValue = SelectPHRange1 + '-' + SelectPHRange2;
                                pH_TypeValue = PHEnum.Range.ToString();
                            }
                        }
                    }
                    //}
                }
                else if (PHEnum.AcidA.ToString() == pHResult)
                {
                    pHVValue = "a";
                    pH_TypeValue = PHEnum.AcidA.ToString();
                }
                else if (PHEnum.Base.ToString() == pHResult)
                {
                    pHVValue = "b";
                    pH_TypeValue = PHEnum.Base.ToString();
                }
                else if (PHEnum.Neutral.ToString() == pHResult)
                {
                    pHVValue = "n";
                    pH_TypeValue = PHEnum.Neutral.ToString();
                }
                else if (PHEnum.equal.ToString() == pHResult)
                {
                    pHVValue = "=";
                    pH_TypeValue = PHEnum.equal.ToString();
                }
                else
                {
                    pHVValue = "";
                    pH_TypeValue = PHEnum.None.ToString();
                }
                #endregion
                #region Maintain Pressure
                if (PressureEnum.Pressure.ToString() == PressureResult)
                {
                    if (SelectPressure.Trim() != "")
                    {
                        blTemp = IsValidNumber(SelectPressure.Trim());
                        if (blTemp == false)
                        {
                            AppInfoBox.ShowInfoMessage("In-Valid Pressure value");
                            return;
                        }
                        else
                        {
                            if (SelectPressure.Contains('-'))
                            {
                                AppInfoBox.ShowInfoMessage("Pressure value not contain negative values");
                                return;
                            }
                            else
                            {

                                if (SelectedPresureComboPressure.Value == "a")
                                {
                                    PressureValue = SelectPressure;
                                    Pressure_TypeValue = PressureEnum.Pressure.ToString();
                                }
                                else
                                {
                                    PressureValue = SelectPressure + SelectedPresureComboPressure.Value;
                                    Pressure_TypeValue = PressureEnum.Pressure.ToString();
                                }
                                selectPressureValue = SelectedPresureComboPressure;
                            }
                        }
                    }
                    else
                    {
                        AppInfoBox.ShowInfoMessage("Please Enter Presure Value");
                        return;
                    }
                }
                else if (PressureEnum.Range.ToString() == PressureResult)
                {

                    if (SelectPressureRangetext1.Trim() == "" || SelectPressureRangetext2.Trim() == "")
                    {
                        AppInfoBox.ShowInfoMessage("Please Enter Presure Range Value");
                        return;
                    }
                    else
                    {
                        blTemp = IsValidNumber(SelectPressureRangetext1.Trim());
                        bool blTempPlessMinus = IsValidNumber(SelectPressureRangetext2.Trim());
                        if (blTemp == false || blTempPlessMinus == false)
                        {
                            AppInfoBox.ShowInfoMessage("In-Valid PH value");
                            return;
                        }
                        else
                        {
                            if (SelectPressureRangetext1.Contains('-') || SelectPressureRangetext2.Contains('-'))
                            {
                                AppInfoBox.ShowInfoMessage("Pressure Range value not contain negative values");
                                return;
                            }
                            else
                            {
                                if (Convert.ToDouble(SelectPressureRangetext1) >= Convert.ToDouble(SelectPressureRangetext2))
                                {
                                    AppInfoBox.ShowInfoMessage("left value should be less than right value");
                                    return;
                                }
                                else
                                {
                                    if (SelectedPressureCoboRange.Value == "a")
                                    {
                                        PressureValue = SelectPressureRangetext1 + '-' + SelectPressureRangetext2;
                                        Pressure_TypeValue = PressureEnum.Range.ToString();
                                    }
                                    else
                                    {
                                        PressureValue = SelectPressureRangetext1 + SelectedPressureCoboRange.Value + '-' + SelectPressureRangetext2 + SelectedPressureCoboRange.Value;
                                        Pressure_TypeValue = PressureEnum.Range.ToString();
                                    }
                                    selectPressureValue = SelectedPressureCoboRange;
                                }
                            }
                        }

                    }



                }
                else if (PressureEnum.Directional.ToString() == PressureResult)
                {
                    if (SelectPressureDirectional1.Trim() == "" || SelectPressureDirectional2.Trim() == "")
                    {
                        AppInfoBox.ShowInfoMessage("Please Enter Pressure Directional Value");
                        return;
                    }
                    else
                    {
                        if (SelectPressureDirectional1.Contains(">") || SelectPressureDirectional2.Contains(">"))
                        {
                            AppInfoBox.ShowInfoMessage("> and < Not Allowled in Directional Values");
                            return;
                        }
                        else
                        {
                            blTemp = IsValidNumber(SelectPressureDirectional1.Trim());
                            bool blTempPlessMinus = IsValidNumber(SelectPressureDirectional2.Trim());
                            if (blTemp == false || blTempPlessMinus == false)
                            {
                                AppInfoBox.ShowInfoMessage("In-Valid Pressure value");
                                return;
                            }
                            else
                            {

                                if (SelectPressureDirectional1.Contains('-') || SelectPressureDirectional2.Contains('-'))
                                {
                                    AppInfoBox.ShowInfoMessage("Pressure Directional value not contain negative values");
                                    return;
                                }
                                else
                                {
                                    if (Convert.ToDouble(SelectPressureDirectional1) == Convert.ToDouble(SelectPressureDirectional2))
                                    {
                                        AppInfoBox.ShowInfoMessage("Directional Value can't be same");
                                        return;
                                    }
                                    else
                                    {
                                        if (SelectedPressureComboDirectional.Value == "a")
                                        {
                                            PressureValue = SelectPressureDirectional1 + ']' + SelectPressureDirectional2;
                                            Pressure_TypeValue = PressureEnum.Directional.ToString();
                                        }
                                        else
                                        {
                                            PressureValue = SelectPressureDirectional1 + SelectedPressureComboDirectional.Value + ']' + SelectPressureDirectional2 + SelectedPressureComboDirectional.Value;
                                            Pressure_TypeValue = PressureEnum.Directional.ToString();
                                        }
                                        selectPressureValue = SelectedPressureComboDirectional;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (PressureEnum.equal.ToString() == PressureResult)
                {
                    PressureValue = "=";
                    Pressure_TypeValue = PressureEnum.equal.ToString();
                }
                else if (PressureEnum.None.ToString() == PressureResult)
                {
                    PressureValue = "";
                    Pressure_TypeValue = PressureEnum.None.ToString();
                }
                #endregion

                StageConditionVM StageCondition;
                var mainViewModel = ((MainWindow)(App.Current.MainWindow)).DataContext as MainVM;
                if (!string.IsNullOrEmpty(TimeValue) && string.IsNullOrEmpty(TemperatureValue))
                {
                    DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Time corresponding Temperature is not there. Do you want to continue?", "Reactions", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dialogResult == DialogResult.No)
                    {
                        return;
                    }

                }



                Guid ConditionID;
                #region  Specific Validations               
                if (Temp_TypeValue == TemperatureEnum.None.ToString() && Time_TypeValue == TimeEnum.None.ToString() && pH_TypeValue == PHEnum.None.ToString() && Pressure_TypeValue == PressureEnum.None.ToString())
                {
                    AppInfoBox.ShowInfoMessage("Empty condition can't be added");
                    return;
                }
                if (mainViewModel.TanVM.SelectedReaction.SelectedStage.Conditions.Count == 0)
                {
                    var temp12 = ((mainViewModel.TanVM.SelectedReaction.SelectedStage.DisplayOrder) - 1);
                    var stages = mainViewModel.TanVM.SelectedReaction.Stages.Where(x => x.DisplayOrder == temp12).ToList();
                    foreach (var stage in stages)
                    {
                        StageCondition = stage.Conditions.LastOrDefault() as StageConditionVM;
                        if (StageCondition != null)
                        {
                            if (StageCondition.TEMP_TYPE.ToString() == TemperatureEnum.Cool.ToString() && Temp_TypeValue == TemperatureEnum.equels.ToString())
                            {
                                AppInfoBox.ShowInfoMessage("If Temperature is 'c' in the last sub-stage of " + stage.Name + " = is not allowed in the " + mainViewModel.TanVM.SelectedReaction.SelectedStage.Name.ToString());
                                return;
                            }
                            if (string.IsNullOrEmpty(StageCondition.Time) && Time_TypeValue == TimeEnum.equals.ToString())
                            {
                                AppInfoBox.ShowInfoMessage("If Time is empty in the last sub-stage of " + stage.Name + " = is not allowed in the " + mainViewModel.TanVM.SelectedReaction.SelectedStage.Name.ToString());
                                return;
                            }
                            if (string.IsNullOrEmpty(StageCondition.PH) && PHEnum.equal.ToString() == pH_TypeValue)
                            {
                                AppInfoBox.ShowInfoMessage("If pH is empty in the last sub-stage of " + stage.Name + " = is not allowed in the " + mainViewModel.TanVM.SelectedReaction.SelectedStage.Name.ToString());
                                return;
                            }
                            if (string.IsNullOrEmpty(StageCondition.Pressure) && PressureEnum.equal.ToString() == Pressure_TypeValue)
                            {
                                AppInfoBox.ShowInfoMessage("If Pressure is empty in the last sub-stage of " + stage.Name + " = is not allowed in the " + mainViewModel.TanVM.SelectedReaction.SelectedStage.Name.ToString());
                                return;
                            }
                            break;
                        }
                    }
                    if (Temp_TypeValue == TemperatureEnum.equels.ToString())
                    {
                        AppInfoBox.ShowInfoMessage("'=' is not allowed in the 1st sub-stage of the reaction");
                        return;
                    }
                }
                else if (mainViewModel.TanVM.SelectedReaction.SelectedStage.Conditions.Count > 0)
                {

                    var conditions = new List<StageConditionVM>();
                    conditions = mainViewModel.TanVM.SelectedReaction.SelectedStage.Conditions.Where(x => x.Id != Guid.Empty).ToList();
                    if (conditions.Count > 0)
                    {
                        var Substage = mainViewModel.TanVM.SelectedReaction.SelectedStage.Conditions.Where(x => x.TEMP_TYPE == TemperatureEnum.Cool.ToString() || x.TEMP_TYPE == TemperatureEnum.Heated.ToString() || x.Temperature.Contains(']')).ToList();
                        if (Substage.Count > 0 && Temp_TypeValue == TemperatureEnum.equels.ToString())
                        {
                            AppInfoBox.ShowInfoMessage("If Temperature is 'c' / 'h' / ']' in the previous sub-stage, = is not allowed in the current sub-stage");
                            return;
                        }
                        if (SelectedConditionId != Guid.Empty)
                        {
                            if (Temp_TypeValue == TemperatureEnum.equels.ToString())
                            {
                                AppInfoBox.ShowInfoMessage("If Temperature is empty in the previous sub-stage, = is not allowed in the current sub-stage");
                                return;
                            }
                            if (Time_TypeValue == TimeEnum.equals.ToString())
                            {
                                AppInfoBox.ShowInfoMessage("If Time is empty in the previous sub-stage, = is not allowed in the current sub-stage");
                                return;
                            }
                            if (pH_TypeValue == PHEnum.equal.ToString())
                            {
                                AppInfoBox.ShowInfoMessage("If pH is empty in the previous sub-stage, = is not allowed in the current sub-stage");
                                return;
                            }
                            if (Pressure_TypeValue == PressureEnum.equal.ToString())
                            {
                                AppInfoBox.ShowInfoMessage("If Pressure is empty in the previous sub-stage, = is not allowed in the current sub-stage");
                                return;
                            }
                            var Substage_Pressure = mainViewModel.TanVM.SelectedReaction.SelectedStage.Conditions.Where(x => x.PRESSURE_TYPE.Contains(']')).ToList();
                            if (Substage_Pressure.Count > 0 && Pressure_TypeValue == PressureEnum.equal.ToString())
                            {
                                AppInfoBox.ShowInfoMessage("If Pressure is empty in the previous sub-stage, = is not allowed in the current sub-stage");
                                return;
                            }
                        }
                    }
                }
                #endregion
                if (SelectedConditionId != Guid.Empty)
                {
                    StageCondition = mainViewModel.TanVM.SelectedReaction.SelectedStage.Conditions.ToList().Where(x => x.Id == SelectedConditionId).FirstOrDefault() as StageConditionVM;
                    StageCondition.Temperature = TemperatureValue;
                    StageCondition.Time = TimeValue;
                    StageCondition.PH = pHVValue;
                    StageCondition.Pressure = PressureValue;
                    StageCondition.TEMP_TYPE = Temp_TypeValue;
                    StageCondition.TIME_TYPE = Time_TypeValue;
                    StageCondition.PH_TYPE = pH_TypeValue;
                    StageCondition.PRESSURE_TYPE = Pressure_TypeValue;
                    mainViewModel.TanVM.PerformAutoSave("Condition Updated");
                    mainViewModel.TanVM.UpdateReactionPreview();
                    if (U.RoleId == 1 && mainViewModel.TanVM.SelectedReaction.IsCurationCompleted)
                        mainViewModel.TanVM.SelectedReaction.IsCurationCompleted = false;
                    if (U.RoleId == 2 && mainViewModel.TanVM.SelectedReaction.IsReviewCompleted)
                        mainViewModel.TanVM.SelectedReaction.IsReviewCompleted = false;
                    ConditionVisible = System.Windows.Visibility.Hidden;
                }
                else
                {
                    StageCondition = new StageConditionVM
                    {
                        Id = Guid.NewGuid(),
                        Temperature = TemperatureValue,
                        Time = TimeValue,
                        PH = pHVValue,
                        Pressure = PressureValue,
                        TEMP_TYPE = Temp_TypeValue,
                        TIME_TYPE = Time_TypeValue,
                        PH_TYPE = pH_TypeValue,
                        PRESSURE_TYPE = Pressure_TypeValue,
                        StageId = mainViewModel.TanVM.SelectedReaction.SelectedStage.Id
                    };
                    if (mainViewModel.TanVM.SelectedReaction.SelectedStage.SelectedCondition != null && (mainViewModel.TanVM.SelectedReaction.SelectedStage.BeforeInsert || mainViewModel.TanVM.SelectedReaction.SelectedStage.AfterInsert))
                    {
                        var conditionList = mainViewModel.TanVM.SelectedReaction.SelectedStage.Conditions.ToList();
                        var index = conditionList.Count() >= 1 ? conditionList.FindIndex(x => x.Id == mainViewModel.TanVM.SelectedReaction.SelectedStage.SelectedCondition.Id) : 0;
                        if (mainViewModel.TanVM.SelectedReaction.SelectedStage.BeforeInsert)
                            mainViewModel.TanVM.SelectedReaction.SelectedStage.Conditions.Insert(index, StageCondition);
                        else
                            mainViewModel.TanVM.SelectedReaction.SelectedStage.Conditions.Insert(index + 1, StageCondition);
                    }
                    else
                        mainViewModel.TanVM.SelectedReaction.SelectedStage.Conditions.Add(StageCondition);
                    mainViewModel.TanVM.UpdateReactionPreview();
                    mainViewModel.TanVM.PerformAutoSave("Condition Added");
                    mainViewModel.Validate(true);
                    UnselectCondition();
                    // ResetConditon(null);
                    if (U.RoleId == 1)
                    {
                        mainViewModel.TanVM.SelectedReaction.LastupdatedDate = DateTime.Now;
                        mainViewModel.TanVM.SelectedReaction.IsCurationCompleted = false;
                    }
                    else if (U.RoleId == 2)
                    {
                        mainViewModel.TanVM.SelectedReaction.ReviewLastupdatedDate = DateTime.Now;
                        mainViewModel.TanVM.SelectedReaction.IsReviewCompleted = false;
                    }
                    else if (U.RoleId == 3)
                        mainViewModel.TanVM.SelectedReaction.QCLastupdatedDate = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                string msg = "AddSelectedCondition" + Environment.NewLine + ex.Message;
                Log.This(ex);
                AppErrorBox.ShowErrorMessage("Some Error Occured", ex.ToString());
            }
        }






        private bool IsValidNumber(string _numval)
        {

            bool blStatus = false;
            try
            {
                Regex objregex = new Regex(@"^[><]?-?[0-9]{1,4}(\.[0-9]{1,4})?$");
                blStatus = objregex.IsMatch(_numval);
                return blStatus;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return blStatus;
        }

        #endregion
        #region
        public Guid selectedId;
        public Guid SelectedConditionId { get { return selectedId; } set { SetProperty(ref selectedId, value); } }
        #endregion
        #region Temperature 
        public string TempResult { get { return rempresult; } set { SetProperty(ref rempresult, value); } }
        public string SelectTempTemperature
        {
            get { return selectTPText; }
            set
            {
                SetProperty(ref selectTPText, value);
                if (value != null && value != "")
                {
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";
                    SelectradioRoomTemperature = "false";
                    SelectradioLessthanRoomTemp = "false";
                    SelectradioHeated = "false";
                    Selectradioequals = "false";
                    SelectradioCool = "false";
                    SelectradioReflextoRoomTemp = "false";
                    SelectradioRefluxBoiled = "false";
                    SelectradioRoomTemptoReflux = "false";
                    SelectradioGreaterthanRoomTemp = "false";
                    SelectradioNoon = "false";
                    ResetTempComboValues = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
                    SelectedCombotoReflux = ResetTempComboValues;
                    SelectedComboRoomtempto = ResetTempComboValues;
                    SelectedCombotoRoomTemp = ResetTempComboValues;
                    SelectedComboDirectional = ResetTempComboValues;
                    SelectedComboRang = ResetTempComboValues;
                    SelectedComboRefluxto = ResetTempComboValues;
                    SelectedComboPluessorMinnus = ResetTempComboValues;
                    TempResult = TemperatureEnum.Temp.ToString();
                }
            }
        }
        public string SelectRoomtempto
        {
            get { return selectRoomtempto; }
            set
            {
                SetProperty(ref selectRoomtempto, value);
                if (value != null && value != "")
                {
                    SelectTempTemperature = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";
                    SelectradioRoomTemperature = "false";
                    SelectradioLessthanRoomTemp = "false";
                    SelectradioHeated = "false";
                    Selectradioequals = "false";
                    SelectradioCool = "false";
                    SelectradioReflextoRoomTemp = "false";
                    SelectradioRoomTemptoReflux = "false";
                    SelectradioGreaterthanRoomTemp = "false";
                    SelectradioRefluxBoiled = "false";
                    SelectradioNoon = "false";
                    ResetTempComboRoomtempto = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
                    SelectedComboRang = ResetTempComboRoomtempto;
                    SelectedComboPluessorMinnus = ResetTempComboRoomtempto;
                    SelectedCombotoReflux = ResetTempComboRoomtempto;
                    SelectedComboValue = ResetTempComboRoomtempto;
                    SelectedCombotoRoomTemp = ResetTempComboRoomtempto;
                    SelectedComboDirectional = ResetTempComboRoomtempto;
                    SelectedComboRefluxto = ResetTempComboRoomtempto;
                    TempResult = TemperatureEnum.Roomtempto.ToString();
                }
            }
        }
        public string SelecttoRoomTemp
        {
            get { return selecttoRoomTemp; }
            set
            {
                SetProperty(ref selecttoRoomTemp, value);
                if (value != null && value != "")
                {
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";
                    SelectradioRoomTemperature = "false";
                    SelectradioLessthanRoomTemp = "false";
                    SelectradioHeated = "false";
                    Selectradioequals = "false";
                    SelectradioCool = "false";
                    SelectradioReflextoRoomTemp = "false";
                    SelectradioRoomTemptoReflux = "false";
                    SelectradioGreaterthanRoomTemp = "false";
                    SelectradioRefluxBoiled = "false";
                    SelectradioNoon = "false";
                    ResetTempComboValues = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
                    SelectedCombotoReflux = ResetTempComboValues;
                    SelectedComboValue = ResetTempComboValues;
                    SelectedComboRoomtempto = ResetTempComboValues;
                    SelectedComboDirectional = ResetTempComboValues;
                    SelectedComboRang = ResetTempComboValues;
                    SelectedComboRefluxto = ResetTempComboValues;
                    SelectedComboPluessorMinnus = ResetTempComboValues;
                    TempResult = TemperatureEnum.toRoomTemp.ToString();
                }
            }
        }
        public string SelectDirectional1
        {
            get { return selectDirectional1; }
            set
            {
                SetProperty(ref selectDirectional1, value);
                if (value != null && value != "")
                {
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";
                    SelectradioRoomTemperature = "false";
                    SelectradioLessthanRoomTemp = "false";
                    SelectradioHeated = "false";
                    Selectradioequals = "false";
                    SelectradioCool = "false";
                    SelectradioReflextoRoomTemp = "false";
                    SelectradioRoomTemptoReflux = "false";
                    SelectradioGreaterthanRoomTemp = "false";
                    SelectradioRefluxBoiled = "false";
                    SelectradioNoon = "false";
                    ResetTempComboValues = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
                    SelectedCombotoReflux = ResetTempComboValues;
                    SelectedComboValue = ResetTempComboValues;
                    SelectedComboRoomtempto = ResetTempComboValues;
                    SelectedCombotoRoomTemp = ResetTempComboValues;
                    SelectedComboRang = ResetTempComboValues;
                    SelectedComboRefluxto = ResetTempComboValues;
                    SelectedComboPluessorMinnus = ResetTempComboValues;
                    TempResult = TemperatureEnum.Directional.ToString();
                }
            }
        }
        public string SelectDirectional2
        {
            get { return selectDirectional2; }
            set
            {
                SetProperty(ref selectDirectional2, value);
                if (value != null && value != "")
                {
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";
                    SelectradioRoomTemperature = "false";
                    SelectradioLessthanRoomTemp = "false";
                    SelectradioHeated = "false";
                    Selectradioequals = "false";
                    SelectradioCool = "false";
                    SelectradioReflextoRoomTemp = "false";
                    SelectradioRoomTemptoReflux = "false";
                    SelectradioGreaterthanRoomTemp = "false";
                    SelectradioNoon = "false";
                    SelectradioRefluxBoiled = "false";
                    ResetTempComboValues = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
                    SelectedCombotoReflux = ResetTempComboValues;
                    SelectedComboValue = ResetTempComboValues;
                    SelectedComboRoomtempto = ResetTempComboValues;
                    SelectedCombotoRoomTemp = ResetTempComboValues;
                    SelectedComboRang = ResetTempComboValues;
                    SelectedComboRefluxto = ResetTempComboValues;
                    SelectedComboPluessorMinnus = ResetTempComboValues;
                    TempResult = TemperatureEnum.Directional.ToString();
                }
            }
        }
        public string SelectRang1
        {
            get { return selectRang1; }
            set
            {
                SetProperty(ref selectRang1, value);
                if (value != null && value != "")
                {
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";
                    SelectradioRoomTemperature = "false";
                    SelectradioLessthanRoomTemp = "false";
                    SelectradioHeated = "false";
                    Selectradioequals = "false";
                    SelectradioCool = "false";
                    SelectradioReflextoRoomTemp = "false";
                    SelectradioRoomTemptoReflux = "false";
                    SelectradioGreaterthanRoomTemp = "false";
                    SelectradioRefluxBoiled = "false";
                    SelectradioNoon = "false";
                    ResetTempComboValues = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
                    SelectedComboValue = ResetTempComboValues;
                    SelectedComboRoomtempto = ResetTempComboValues;
                    SelectedCombotoRoomTemp = ResetTempComboValues;
                    SelectedComboDirectional = ResetTempComboValues;
                    SelectedComboRefluxto = ResetTempComboValues;
                    SelectedCombotoReflux = ResetTempComboValues;
                    SelectedComboPluessorMinnus = ResetTempComboValues;
                    TempResult = TemperatureEnum.Rang.ToString();
                }
            }
        }
        public string SelectRang2
        {
            get { return selectRang2; }
            set
            {
                SetProperty(ref selectRang2, value);
                if (value != null && value != "")
                {
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";
                    SelectradioRoomTemperature = "false";
                    SelectradioLessthanRoomTemp = "false";
                    SelectradioHeated = "false";
                    Selectradioequals = "false";
                    SelectradioCool = "false";
                    SelectradioReflextoRoomTemp = "false";
                    SelectradioRoomTemptoReflux = "false";
                    SelectradioGreaterthanRoomTemp = "false";
                    SelectradioRefluxBoiled = "false";
                    SelectradioNoon = "false";
                    ResetTempComboValues = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
                    SelectedCombotoReflux = ResetTempComboValues;
                    SelectedComboValue = ResetTempComboValues;
                    SelectedComboRoomtempto = ResetTempComboValues;
                    SelectedCombotoRoomTemp = ResetTempComboValues;
                    SelectedComboDirectional = ResetTempComboValues;
                    SelectedComboRefluxto = ResetTempComboValues;
                    SelectedComboPluessorMinnus = ResetTempComboValues;
                    TempResult = TemperatureEnum.Rang.ToString();
                }
            }
        }
        public string SelectRefluxto
        {
            get { return selectRefluxto; }
            set
            {
                SetProperty(ref selectRefluxto, value);
                if (value != null && value != "")
                {
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";

                    SelectradioRoomTemperature = "false";
                    SelectradioLessthanRoomTemp = "false";
                    SelectradioHeated = "false";
                    Selectradioequals = "false";
                    SelectradioCool = "false";
                    SelectradioReflextoRoomTemp = "false";
                    SelectradioRoomTemptoReflux = "false";
                    SelectradioGreaterthanRoomTemp = "false";
                    SelectradioRefluxBoiled = "false";
                    SelectradioNoon = "false";
                    ResetTempComboValues = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
                    SelectedCombotoReflux = ResetTempComboValues;
                    SelectedComboValue = ResetTempComboValues;
                    SelectedComboRoomtempto = ResetTempComboValues;
                    SelectedCombotoRoomTemp = ResetTempComboValues;
                    SelectedComboDirectional = ResetTempComboValues;
                    SelectedComboRang = ResetTempComboValues;
                    SelectedComboPluessorMinnus = ResetTempComboValues;
                    TempResult = TemperatureEnum.Refluxto.ToString();
                }
            }
        }
        public string SelectPlessMinues1
        {
            get { return selectPlessMinues1; }
            set
            {
                SetProperty(ref selectPlessMinues1, value);
                if (value != null && value != "")
                {
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelecttoReflux = "";
                    SelectradioRoomTemperature = "false";
                    SelectradioLessthanRoomTemp = "false";
                    SelectradioHeated = "false";
                    Selectradioequals = "false";
                    SelectradioCool = "false";
                    SelectradioReflextoRoomTemp = "false";
                    SelectradioRoomTemptoReflux = "false";
                    SelectradioGreaterthanRoomTemp = "false";
                    SelectradioRefluxBoiled = "false";
                    SelectradioNoon = "false";
                    ResetTempComboValues = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
                    SelectedComboValue = ResetTempComboValues;
                    SelectedComboRoomtempto = ResetTempComboValues;
                    SelectedCombotoRoomTemp = ResetTempComboValues;
                    SelectedComboDirectional = ResetTempComboValues;
                    SelectedComboRang = ResetTempComboValues;
                    SelectedComboRefluxto = ResetTempComboValues;
                    SelectedCombotoReflux = ResetTempComboValues;
                    TempResult = TemperatureEnum.pluseDevminus.ToString();
                }
            }
        }
        public string SelectPlessMinues2
        {
            get { return selectPlessMinues2; }
            set
            {
                SetProperty(ref selectPlessMinues2, value);
                if (value != null && value != "")
                {
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelecttoReflux = "";
                    SelectradioRoomTemperature = "false";
                    SelectradioLessthanRoomTemp = "false";
                    SelectradioHeated = "false";
                    Selectradioequals = "false";
                    SelectradioCool = "false";
                    SelectradioReflextoRoomTemp = "false";
                    SelectradioRoomTemptoReflux = "false";
                    SelectradioGreaterthanRoomTemp = "false";
                    SelectradioRefluxBoiled = "false";
                    SelectradioNoon = "false";
                    ResetTempComboValues = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
                    SelectedCombotoReflux = ResetTempComboValues;
                    SelectedComboValue = ResetTempComboValues;
                    SelectedComboRoomtempto = ResetTempComboValues;
                    SelectedCombotoRoomTemp = ResetTempComboValues;
                    SelectedComboDirectional = ResetTempComboValues;
                    SelectedComboRang = ResetTempComboValues;
                    SelectedComboRefluxto = ResetTempComboValues;
                    SelectedCombotoReflux = ResetTempComboValues;
                    TempResult = TemperatureEnum.pluseDevminus.ToString();
                }
            }
        }
        public string SelecttoReflux
        {
            get { return selecttoReflux; }
            set
            {
                SetProperty(ref selecttoReflux, value);
                if (value != null && value != "")
                {
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelectradioRoomTemperature = "false";
                    SelectradioLessthanRoomTemp = "false";
                    SelectradioHeated = "false";
                    Selectradioequals = "false";
                    SelectradioCool = "false";
                    SelectradioReflextoRoomTemp = "false";
                    SelectradioRoomTemptoReflux = "false";
                    SelectradioGreaterthanRoomTemp = "false";
                    SelectradioRefluxBoiled = "false";
                    SelectradioNoon = "false";
                    ResetTempComboValues = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
                    SelectedCombotoReflux = ResetTempComboValues;
                    SelectedComboValue = ResetTempComboValues;
                    SelectedComboRoomtempto = ResetTempComboValues;
                    SelectedCombotoRoomTemp = ResetTempComboValues;
                    SelectedComboDirectional = ResetTempComboValues;
                    SelectedComboRang = ResetTempComboValues;
                    SelectedComboRefluxto = ResetTempComboValues;
                    SelectedComboPluessorMinnus = ResetTempComboValues;
                    TempResult = TemperatureEnum.toReflux.ToString();
                }
            }
        }
        public TemperatureCodeVM SelectedComboValue { get { return selectedComboValue; } set { SetProperty(ref selectedComboValue, value); TempResult = TemperatureEnum.Temp.ToString(); } }
        public TemperatureCodeVM SelectedComboRoomtempto
        {
            get { return selectedComboRoomtempto; }
            set
            {
                SetProperty(ref selectedComboRoomtempto, value);
                TempResult = TemperatureEnum.Roomtempto.ToString();
            }
        }
        public TemperatureCodeVM SelectedCombotoRoomTemp
        {
            get { return selectedCombotoRoomTemp; }
            set
            {
                SetProperty(ref selectedCombotoRoomTemp, value);
                TempResult = TemperatureEnum.toRoomTemp.ToString();
            }
        }
        public TemperatureCodeVM SelectedComboDirectional
        {
            get { return selectedComboDirectional; }
            set
            {
                SetProperty(ref selectedComboDirectional, value);
                TempResult = TemperatureEnum.Directional.ToString();
            }
        }
        public TemperatureCodeVM SelectedComboRang
        {
            get { return selectValue; }
            set
            {
                SetProperty(ref selectValue, value);
                TempResult = TemperatureEnum.Rang.ToString();
            }
        }
        public TemperatureCodeVM SelectedComboRefluxto
        {
            get { return selectedComboRefluxto; }
            set
            {
                SetProperty(ref selectedComboRefluxto, value);
                TempResult = TemperatureEnum.Refluxto.ToString();
            }
        }
        public TemperatureCodeVM SelectedComboPluessorMinnus
        {
            get { return selectedComboPluessorMinnus; }
            set
            {
                SetProperty(ref selectedComboPluessorMinnus, value);
                TempResult = TemperatureEnum.pluseDevminus.ToString();
            }
        }
        public TemperatureCodeVM SelectedCombotoReflux
        {
            get { return selectedCombotoReflux; }
            set
            {
                SetProperty(ref selectedCombotoReflux, value);
                TempResult = TemperatureEnum.toReflux.ToString();
            }
        }
        private string selectradioRoomTemperature, selectradioRefluxBoiled, selectradioCool, selectradioHeated, selectradioRoomTemptoReflux, selectradioReflextoRoomTemp, selectradioLessthanRoomTemp,
            selectradioGreaterthanRoomTemp, selectradioNoon, selectradioequals;
        public string SelectradioReflextoRoomTemp
        {
            get { return selectradioReflextoRoomTemp; }
            set
            {
                SetProperty(ref selectradioReflextoRoomTemp, value);
                if (value == "True")
                {
                    TempResult = TemperatureEnum.RefluxtoRoomTemp.ToString();
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";
                }
            }
        }
        public string SelectradioLessthanRoomTemp
        {
            get { return selectradioLessthanRoomTemp; }
            set
            {
                SetProperty(ref selectradioLessthanRoomTemp, value);
                if (value == "True")
                {
                    TempResult = TemperatureEnum.LessthanRoomTemp.ToString();
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";
                }
            }
        }
        public string SelectradioGreaterthanRoomTemp
        {
            get { return selectradioGreaterthanRoomTemp; }
            set
            {
                SetProperty(ref selectradioGreaterthanRoomTemp, value);
                if (value == "True")
                {
                    TempResult = TemperatureEnum.RoomTempgraterthan.ToString();
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";
                }
            }
        }
        public string SelectradioNoon
        {
            get { return selectradioNoon; }
            set
            {
                SetProperty(ref selectradioNoon, value);
                if (value == "True")
                {
                    TempResult = TemperatureEnum.None.ToString();
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";
                }
            }
        }
        public string Selectradioequals
        {
            get { return selectradioequals; }
            set
            {
                SetProperty(ref selectradioequals, value);
                if (value == "True")
                {
                    TempResult = TemperatureEnum.equels.ToString();
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";
                }
            }
        }
        public string SelectradioRoomTemperature
        {
            get { return selectradioRoomTemperature; }
            set
            {
                SetProperty(ref selectradioRoomTemperature, value);
                if (value == "True")
                {
                    TempResult = TemperatureEnum.Roomtemp.ToString();
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";
                }
            }
        }
        public string SelectradioRefluxBoiled
        {
            get { return selectradioRefluxBoiled; }
            set
            {
                SetProperty(ref selectradioRefluxBoiled, value);
                if (value == "True")
                {
                    TempResult = TemperatureEnum.RefluxBoiled.ToString();
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";
                }
            }
        }
        public string SelectradioCool
        {
            get { return selectradioCool; }
            set
            {
                SetProperty(ref selectradioCool, value);
                if (value == "True")
                {
                    TempResult = TemperatureEnum.Cool.ToString();
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";
                }
            }
        }
        public string SelectradioHeated
        {
            get { return selectradioHeated; }
            set
            {
                SetProperty(ref selectradioHeated, value);
                if (value == "True")
                {
                    TempResult = TemperatureEnum.Heated.ToString();
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";
                }
            }
        }
        public string SelectradioRoomTemptoReflux
        {
            get { return selectradioRoomTemptoReflux; }
            set
            {
                SetProperty(ref selectradioRoomTemptoReflux, value);
                if (value == "True")
                {
                    TempResult = TemperatureEnum.RoomTemptoReflux.ToString();
                    SelectTempTemperature = "";
                    SelectRoomtempto = "";
                    SelecttoRoomTemp = "";
                    SelectDirectional1 = "";
                    SelectDirectional2 = "";
                    SelectRang1 = "";
                    SelectRang2 = "";
                    SelectRefluxto = "";
                    SelectPlessMinues1 = "";
                    SelectPlessMinues2 = "";
                    SelecttoReflux = "";
                }
            }
        }
        public System.Windows.Visibility ConditionVisible { get { return conditionVisible; } set { SetProperty(ref conditionVisible, value); } }
        #endregion
        #region Start PH UserControl
        public string pHResult
        {
            get { return pHresult; }
            set { SetProperty(ref pHresult, value); }
        }
        public string SelectpH
        {
            get { return selectPhText; }
            set
            {
                SetProperty(ref selectPhText, value);
                if (value != null && value != "")
                {
                    SelectPHRange1 = "";
                    SelectPHRange2 = "";
                    SelectedradiopHAcid = "false";
                    SelectedradiopHBase = "false";
                    SelectedradiopHNeutral = "false";
                    SelectedradiopHequal = "false";
                    SelectedradiopHNone = "false";
                    pHResult = PHEnum.pH.ToString();
                }
            }
        }
        public string SelectPHRange1
        {
            get { return selectPhRange1; }
            set
            {
                SetProperty(ref selectPhRange1, value);
                if (value != null && value != "")
                {
                    SelectpH = "";
                    SelectedradiopHAcid = "false";
                    SelectedradiopHBase = "false";
                    SelectedradiopHNeutral = "false";
                    SelectedradiopHequal = "false";
                    SelectedradiopHNone = "false";
                    pHResult = PHEnum.Range.ToString();
                }
            }
        }
        public string SelectPHRange2
        {
            get { return selectPHRang2; }
            set
            {
                SetProperty(ref selectPHRang2, value);
                if (value != null && value != "")
                {
                    SelectpH = "";
                    SelectedradiopHAcid = "false";
                    SelectedradiopHBase = "false";
                    SelectedradiopHNeutral = "false";
                    SelectedradiopHequal = "false";
                    SelectedradiopHNone = "false";
                    pHResult = PHEnum.Range.ToString();
                }
            }
        }
        private string selectedradiopHAcid, selectedradiopHBase, selectedradiopHNeutral, selectedradiopHequal, selectedradiopHNone;
        public string SelectedradiopHAcid
        {
            get { return selectedradiopHAcid; }
            set
            {
                SetProperty(ref selectedradiopHAcid, value);
                if (value == "True")
                {
                    SelectpH = "";
                    SelectPHRange1 = "";
                    SelectPHRange2 = "";
                    pHResult = PHEnum.AcidA.ToString();
                }
            }
        }
        public string SelectedradiopHBase
        {
            get { return selectedradiopHBase; }
            set
            {
                SetProperty(ref selectedradiopHBase, value);
                if (value == "True")
                {
                    SelectpH = "";
                    SelectPHRange1 = "";
                    SelectPHRange2 = "";
                    pHResult = PHEnum.Base.ToString();
                }
            }
        }
        public string SelectedradiopHNeutral
        {
            get { return selectedradiopHNeutral; }
            set
            {
                SetProperty(ref selectedradiopHNeutral, value);
                if (value == "True")
                {
                    SelectpH = "";
                    SelectPHRange1 = "";
                    SelectPHRange2 = "";
                    pHResult = PHEnum.Neutral.ToString();
                }
            }
        }
        public string SelectedradiopHequal
        {
            get { return selectedradiopHequal; }
            set
            {
                SetProperty(ref selectedradiopHequal, value);
                if (value == "True")
                {
                    SelectpH = "";
                    SelectPHRange1 = "";
                    SelectPHRange2 = "";
                    pHResult = PHEnum.equal.ToString();
                }
            }
        }
        public string SelectedradiopHNone
        {
            get { return selectedradiopHNone; }
            set
            {
                SetProperty(ref selectedradiopHNone, value);
                if (value == "True")
                {
                    SelectpH = "";
                    SelectPHRange1 = "";
                    SelectPHRange2 = "";
                    pHResult = PHEnum.None.ToString();
                }
            }
        }
        #endregion
        #region Time
        public string TimeResult { get { return timeresult; } set { SetProperty(ref timeresult, value); } }
        public string SelectTimeValue
        {
            get { return selectTimeText; }
            set
            {
                SetProperty(ref selectTimeText, value);
                if (value != null && value != "")
                {
                    SelectTimeRangetext1 = "";
                    SelectTimeRangetext2 = "";
                    SelectedradioTimeOvernight = "false";
                    SelectedradioTimeEquals = "false";
                    SelectedradioTimeNone = "false";
                    TimeResult = TimeEnum.Time.ToString();
                }
            }
        }
        public string SelectTimeRangetext1
        {
            get { return selectTimeRangetext1; }
            set
            {
                SetProperty(ref selectTimeRangetext1, value);
                if (value != null && value != "")
                {
                    SelectTimeValue = "";
                    SelectedradioTimeOvernight = "false";
                    SelectedradioTimeEquals = "false";
                    SelectedradioTimeNone = "false";
                    TimeResult = TimeEnum.Rang.ToString();
                }
            }
        }
        public string SelectTimeRangetext2
        {
            get { return selectTimeRangetext2; }
            set
            {
                SetProperty(ref selectTimeRangetext2, value);
                if (value != null && value != "")
                {
                    SelectTimeValue = "";
                    SelectedradioTimeOvernight = "false";
                    SelectedradioTimeEquals = "false";
                    SelectedradioTimeNone = "false";
                    TimeResult = TimeEnum.Rang.ToString();
                }
            }
        }
        public TimeComboCodevalues SelectedTimeComboValue { get { return timeComboCodevalues; } set { SetProperty(ref timeComboCodevalues, value); TimeResult = TimeEnum.Time.ToString(); } }
        private TimeComboCodevalues timeComboCodevaluesRange;
        public TimeComboCodevalues SelectedCoboTimeRange { get { return timeComboCodevaluesRange; } set { SetProperty(ref timeComboCodevaluesRange, value); TimeResult = TimeEnum.Rang.ToString(); } }
        public TimeCombovalues SelectedCoboTimeInExact { get { return timeCombovalues; } set { SetProperty(ref timeCombovalues, value); TimeResult = TimeEnum.InExactTime.ToString(); } }
        private string selectedradioTimeOvernight, selectedradioTimeEquals, selectedradioTimeNone;
        public string SelectedradioTimeOvernight
        {
            get { return selectedradioTimeOvernight; }
            set
            {
                SetProperty(ref selectedradioTimeOvernight, value);
                if (value == "True")
                {
                    TimeResult = TimeEnum.Overnight.ToString();
                    SelectTimeValue = "";
                    SelectTimeRangetext1 = "";
                    SelectTimeRangetext2 = "";
                }
            }
        }
        public string SelectedradioTimeEquals
        {
            get { return selectedradioTimeEquals; }
            set
            {
                SetProperty(ref selectedradioTimeEquals, value);
                if (value == "True")
                {
                    TimeResult = TimeEnum.equals.ToString();
                    SelectTimeValue = "";
                    SelectTimeRangetext1 = "";
                    SelectTimeRangetext2 = "";
                }
            }
        }
        public string SelectedradioTimeNone
        {
            get { return selectedradioTimeNone; }
            set
            {
                SetProperty(ref selectedradioTimeNone, value);
                if (value == "True")
                {
                    TimeResult = TimeEnum.None.ToString();
                    SelectTimeValue = "";
                    SelectTimeRangetext1 = "";
                    SelectTimeRangetext2 = "";
                }
            }
        }
        #endregion
        #region Pressure
        public string PressureResult { get { return pressureresult; } set { SetProperty(ref pressureresult, value); } }
        public string SelectPressure
        {
            get { return selectPressureText; }
            set
            {
                SetProperty(ref selectPressureText, value);
                if (value != null && value != "")
                {
                    SelectPressureRangetext1 = "";
                    SelectPressureRangetext2 = "";
                    SelectPressureDirectional1 = "";
                    SelectPressureDirectional2 = "";
                    SelectedradioPressureequal = "false";
                    SelectedradioPressureNone = "false";
                    PressureResult = PressureEnum.Pressure.ToString();
                }
            }
        }
        public string SelectPressureRangetext1
        {
            get { return selectPressureRangetext1; }
            set
            {
                SetProperty(ref selectPressureRangetext1, value);
                if (value != null && value != "")
                {
                    SelectPressure = "";
                    SelectPressureDirectional1 = "";
                    SelectPressureDirectional2 = "";
                    SelectedradioPressureequal = "false";
                    SelectedradioPressureNone = "false";
                    PressureResult = PressureEnum.Range.ToString();
                }
            }
        }
        public string SelectPressureRangetext2
        {
            get { return selectPressureRangetext2; }
            set
            {
                SetProperty(ref selectPressureRangetext2, value);
                if (value != null && value != "")
                {
                    SelectPressure = "";
                    SelectPressureDirectional1 = "";
                    SelectPressureDirectional2 = "";
                    SelectedradioPressureequal = "false";
                    SelectedradioPressureNone = "false";
                    PressureResult = PressureEnum.Range.ToString();
                }
            }
        }
        public string SelectPressureDirectional1
        {
            get { return selectPressureDirectional1; }
            set
            {
                SetProperty(ref selectPressureDirectional1, value);
                if (value != null && value != "")
                {
                    SelectPressure = "";
                    SelectPressureRangetext1 = "";
                    SelectPressureRangetext2 = "";
                    SelectedradioPressureequal = "false";
                    SelectedradioPressureNone = "false";
                    PressureResult = PressureEnum.Directional.ToString();
                }
            }
        }
        public string SelectPressureDirectional2
        {
            get { return selectPressureDirectional2; }
            set
            {
                SetProperty(ref selectPressureDirectional2, value);
                if (value != null && value != "")
                {
                    SelectPressure = "";
                    SelectPressureRangetext1 = "";
                    SelectPressureRangetext2 = "";
                    SelectedradioPressureequal = "false";
                    SelectedradioPressureNone = "false";
                    PressureResult = PressureEnum.Directional.ToString();
                }
            }
        }
        public PressureComboValues SelectedPresureComboPressure
        {
            get { return selectPressureValue; }
            set
            {
                SetProperty(ref selectPressureValue, value);
                PressureResult = PressureEnum.Pressure.ToString();
            }
        }
        public PressureComboValues SelectedPressureComboDirectional
        {
            get { return selectedPressureComboDirectional; }
            set
            {
                SetProperty(ref selectedPressureComboDirectional, value);
                PressureResult = PressureEnum.Directional.ToString();
            }
        }
        public PressureComboValues SelectedPressureCoboRange
        {
            get { return selectedPressureCoboRange; }
            set
            {
                SetProperty(ref selectedPressureCoboRange, value);
                PressureResult = PressureEnum.Range.ToString();
            }
        }
        private string selectedradioPressureequal, selectedradioPressureNone;
        public string SelectedradioPressureequal
        {
            get { return selectedradioPressureequal; }
            set
            {
                SetProperty(ref selectedradioPressureequal, value);
                if (value == "True")
                {
                    PressureResult = PressureEnum.equal.ToString();
                    SelectPressure = "";
                    SelectPressureRangetext1 = "";
                    SelectPressureRangetext2 = "";
                    SelectPressureDirectional1 = "";
                    SelectPressureDirectional2 = "";
                }
            }
        }
        public string SelectedradioPressureNone
        {
            get { return selectedradioPressureNone; }
            set
            {
                SetProperty(ref selectedradioPressureNone, value);
                if (value == "True")
                {
                    PressureResult = PressureEnum.None.ToString();
                    SelectPressure = "";
                    SelectPressureRangetext1 = "";
                    SelectPressureRangetext2 = "";
                    SelectPressureDirectional1 = "";
                    SelectPressureDirectional2 = "";
                }
            }
        }
        #endregion
        #region Splited Conditions Code
        public string GetSelectedINExactTimeFromString(string _inexacttime)
        {

            string strInExact = "";
            try
            {
                ;
                if (_inexacttime.Trim() != "")
                {
                    switch (_inexacttime.Trim())
                    {
                        case "FMIN":
                            strInExact = "few minutes";
                            break;
                        case "FHOUR":
                            strInExact = "few hours";
                            break;
                        case "FDAY":
                            strInExact = "few days";
                            break;
                        case "CMIN":
                            strInExact = "couple of minutes";
                            break;
                        case "CHOUR":
                            strInExact = "couple of hours";
                            break;
                        case "CDAY":
                            strInExact = "couple of days";
                            break;
                        case "OWEEK":
                            strInExact = "over the weekend";
                            break;
                        default:
                            strInExact = "few hours";
                            break;
                    }
                     ;
                }
            }
            catch (Exception ex)
            {
                string msg = "GetSelectedINExactTimeFromString" + Environment.NewLine + ex.Message;
                Log.This(ex);
                AppInfoBox.ShowInfoMessage("Please Report to SME");
            }
            return strInExact;
        }
        public string GetSelectedINExactTimeCode(string _inexactTime)
        {

            string strInExact = "";
            try
            {
                ;
                if (_inexactTime.Trim() != "")
                {
                    switch (_inexactTime.Trim())
                    {
                        case "few minutes":
                            strInExact = "_FMIN";
                            break;
                        case "few hours":
                            strInExact = "_FHOUR";
                            break;
                        case "few days":
                            strInExact = "_FDAY";
                            break;
                        case "couple of minutes":
                            strInExact = "_CMIN";
                            break;
                        case "couple of hours":
                            strInExact = "_CHOUR";
                            break;
                        case "couple of days":
                            strInExact = "_CDAY";
                            break;
                        case "over the weekend":
                            strInExact = "_OWEEK";
                            break;
                        default:
                            strInExact = "";
                            break;
                    }
                     ;
                }
            }
            catch (Exception ex)
            {
                string msg = "GetSelectedINExactTimeCode" + Environment.NewLine + ex.Message;
                Log.This(ex);
                AppInfoBox.ShowInfoMessage("Please Report to SME");
            }
            return strInExact;
        }
        #endregion

        private void UnselectCondition()
        {
            SelectedConditionId = Guid.Empty;
            UnselectRadiobuttion();
            UnselectTime();
            UnselectpHControls();
            UnselectPressure();
            SelectradioNoon = "True";
            SelectedradioPressureNone = "True";
            SelectedradiopHNone = "True";
            SelectedradioTimeNone = "True";
        }
        private void UnselectRadiobuttion()
        {

            try
            {
                TemperatureCodeVM UnselecttemperatureVM = new TemperatureCodeVM();

                SelectTempTemperature = "";
                UnselecttemperatureVM = StaticCollection.TemperatureCodes.ToList().Where(x => x.Value == "c").SingleOrDefault();
                SelectedComboValue = UnselecttemperatureVM;

                SelectRoomtempto = "";
                SelectedComboRoomtempto = UnselecttemperatureVM;
                SelectedCombotoRoomTemp = UnselecttemperatureVM;
                SelecttoRoomTemp = "";
                SelectDirectional1 = "";
                SelectDirectional2 = "";
                SelectedComboDirectional = UnselecttemperatureVM;
                SelectRang1 = "";
                SelectRang2 = "";
                SelecttoReflux = "";
                SelectedComboRang = UnselecttemperatureVM;
                SelectedCombotoReflux = UnselecttemperatureVM;
                SelectRefluxto = "";
                SelectedComboRefluxto = UnselecttemperatureVM;
                SelectPlessMinues1 = "";
                SelectPlessMinues2 = "";
                SelectedComboPluessorMinnus = UnselecttemperatureVM;
                SelectradioLessthanRoomTemp = "false";
                SelectradioHeated = "false";
                Selectradioequals = "false";
                SelectradioCool = "false";
                SelectradioReflextoRoomTemp = "false";
                SelectradioRoomTemptoReflux = "false";
                SelectradioRefluxBoiled = "false";
                //conditionVM.SelectradioRoomTemptoReflux = "false";
                SelectradioGreaterthanRoomTemp = "false";
                SelectradioNoon = "false";
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }


        private void UnselectTime()
        {

            try
            {
                TimeComboCodevalues UnselecttimeComboCodevalues;
                TimeCombovalues UnSelecttimeCombovalues;
                UnSelecttimeCombovalues = StaticCollection.TimeCombovalues.ToList().Where(x => x.Name == "Select").SingleOrDefault();
                UnselecttimeComboCodevalues = StaticCollection.TimeComboCodevalues.ToList().Where(x => x.Value == "h").SingleOrDefault();
                SelectTimeValue = "";
                SelectedTimeComboValue = UnselecttimeComboCodevalues;
                SelectTimeRangetext1 = "";
                SelectTimeRangetext2 = "";
                SelectedCoboTimeRange = UnselecttimeComboCodevalues;
                SelectedCoboTimeInExact = UnSelecttimeCombovalues;
                SelectedradioTimeEquals = "false";
                SelectedradioTimeOvernight = "false";
                SelectedradioTimeNone = "false";
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }

        }


        private void UnselectpHControls()
        {

            try
            {
                SelectpH = "";
                SelectPHRange1 = "";
                SelectPHRange2 = "";
                SelectedradiopHAcid = "false";
                SelectedradiopHBase = "false";
                SelectedradiopHNeutral = "false";
                SelectedradiopHequal = "false";
                SelectedradiopHNone = "false";
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }


        private void UnselectPressure()
        {

            try
            {
                PressureComboValues UnselectpressureComboValues;
                UnselectpressureComboValues = StaticCollection.PressureComboValuess.ToList().Where(x => x.Value == "a").SingleOrDefault();
                SelectPressure = "";
                SelectedPresureComboPressure = UnselectpressureComboValues;
                SelectPressureDirectional1 = "";
                SelectPressureDirectional2 = "";
                SelectedPressureComboDirectional = UnselectpressureComboValues;
                SelectPressureRangetext1 = "";
                SelectPressureRangetext2 = "";
                SelectedPressureCoboRange = UnselectpressureComboValues;
                SelectedradioPressureequal = "false";
                SelectedradioPressureNone = "false";
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }

        }


        #region Edit Conditon Splited Combo Values
        public string GetPressureAndUnitsFromString(string _pressure, out string _pres_units)
        {

            string strPres = _pressure;
            string strPresUnits = "a";
            try
            {
                ;
                string[] splitter = { "b", "kb", "mb", "j", "m", "p", "kp", "mp", "gp", "hp", "s", "t", "kc" };
                string[] strPresVals = strPres.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                if (strPresVals != null)
                {
                    if (strPresVals.Length > 0)
                    {
                        strPresUnits = Regex.Replace(strPres, regExpNum, "");
                        if (strPresUnits == "")
                        {
                            strPresUnits = "a";
                        }
                        else
                        {
                        }
                        strPres = strPresVals[0];
                        ;
                    }
                }

            }
            catch (Exception ex)
            {
                string msg = "GetPressureAndUnitsFromString" + Environment.NewLine + ex.Message;
                Log.This(ex);
                AppInfoBox.ShowInfoMessage("Please Report to SME");
            }
            _pres_units = strPresUnits;
            return strPres;
        }
        public string GetTemperatureAndUnitsFromString(string _temp, out string _temp_units)
        {

            string strTemp = _temp;
            string strTempUnits = "c";
            try
            {
                ;
                string[] splitter = { "f", "k", "r" };
                string[] strTempVals = strTemp.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                if (strTempVals != null)
                {
                    if (strTempVals.Length > 0)
                    {
                        Regex reg = new Regex(regExpNum);
                        strTempUnits = reg.Replace(_temp, "");
                        if (strTempUnits == "")
                        {
                            strTempUnits = "c";
                        }
                        else
                        {
                        }
                        strTemp = strTempVals[0];
                        ;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "GetTemperatureAndUnitsFromString" + Environment.NewLine + ex.Message;
                Log.This(ex);
                AppInfoBox.ShowInfoMessage("Please Report to SME");
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
                ;
                string[] splitter = { "m", "s", "d", "w", "mo", "ms" };
                string[] strTimeVals = strTime.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                if (strTimeVals != null)
                {
                    if (strTimeVals.Length > 0)
                    {
                        strTimeUnits = Regex.Replace(_time, regExpNum, "");
                        if (strTimeUnits == "")
                        {
                            strTimeUnits = "h";
                        }
                        else
                        {
                        }
                        strTime = strTimeVals[0];
                        ;
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
        #endregion

    }

    #region Enums Temperature,PH,Pressure,Time
    public enum TemperatureEnum
    {
        Temp = 0,
        RoomTemptoReflux = 1,
        Roomtempto = 2,
        Roomtemp = 3,
        RefluxBoiled = 4,
        RefluxtoRoomTemp = 5,
        toRoomTemp = 6,
        Directional = 7,
        Rang = 8,
        Cool = 9,
        Refluxto = 10,
        LessthanRoomTemp = 11,
        pluseDevminus = 12,
        equels = 13,
        Heated = 14,
        toReflux = 15,
        RoomTempgraterthan = 16,
        None = 17
    }
    public enum PHEnum
    {
        pH = 0,
        AcidA = 1,
        Base = 2,
        Neutral = 3,
        Range = 4,
        equal = 5,
        None = 6
    }
    public enum PressureEnum
    {
        Pressure = 0,
        Directional = 1,
        Range = 2,
        equal = 3,
        None = 4
    }
    public enum TimeEnum
    {
        Time = 0,
        InExactTime = 1,
        Overnight = 2,
        Rang = 3,
        equals = 4,
        None = 5
    }
    #endregion
    public class TemperatureVM : ViewModelBase
    {
        public string Value { get; set; }
        public string Name { get; set; }
    }
    public class TemperatureCodeVM : ViewModelBase
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
    }
    public class TimeVM : ViewModelBase
    {
        public string Value { get; set; }
        public string Name { get; set; }
    }
    public class PHVM : ViewModelBase
    {
        public string Value { get; set; }
        public string Name { get; set; }
    }
    public class PressureVM : ViewModelBase
    {
        public string Value { get; set; }
        public string Name { get; set; }
    }
    public class TimeCombovalues : ViewModelBase
    {
        public string Value { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
    }
    public class TimeComboCodevalues : ViewModelBase
    {
        public string Value { get; set; }
        public string Name { get; set; }
    }
    public class PressureComboValues : ViewModelBase
    {
        public string Value { get; set; }
        public string Name { get; set; }
    }
}
