using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace Client.ViewModels
{
  public static  class StaticCollection
    {
        static private ObservableCollection<UserManualVM> userManual = new ObservableCollection<UserManualVM>();
        static public ObservableCollection<UserManualVM> UserManual
        {
            get { return userManual; }
            set { if (userManual != value) userManual = UserManual; }
        }
        static private ObservableCollection<TemperatureVM> temperatures = new ObservableCollection<TemperatureVM>();
        static public ObservableCollection<TemperatureVM> Temperatures
        {
            get { return temperatures; }
            set { if (temperatures != value) temperatures = Temperatures; }
        }
        static private ObservableCollection<TemperatureCodeVM> temperatureCodeVMs = new ObservableCollection<TemperatureCodeVM>();
        static public ObservableCollection<TemperatureCodeVM> TemperatureCodes
        {
            get { return temperatureCodeVMs; }
            set { if (temperatureCodeVMs != value) temperatureCodeVMs = TemperatureCodes; }
        }


        static private ObservableCollection<TimeVM> timeVMs = new ObservableCollection<TimeVM>();
        static public ObservableCollection<TimeVM> TimeVMs
        {
            get { return timeVMs; }
            set { if (timeVMs != value) timeVMs = TimeVMs; }
        }
        static private ObservableCollection<TimeCombovalues> timeCombovalues = new ObservableCollection<TimeCombovalues>();
        static public ObservableCollection<TimeCombovalues> TimeCombovalues
        {
            get { return timeCombovalues; }
            set { if (timeCombovalues != value) timeCombovalues = TimeCombovalues; }
        }


        static private ObservableCollection<TimeComboCodevalues> timeComboCodevalues = new ObservableCollection<TimeComboCodevalues>();
        static public ObservableCollection<TimeComboCodevalues> TimeComboCodevalues
        {
            get { return timeComboCodevalues; }
            set { if (timeComboCodevalues != value) timeComboCodevalues = TimeComboCodevalues; }
        }

        static private ObservableCollection<PHVM> pHVMs = new ObservableCollection<PHVM>();
        static public ObservableCollection<PHVM> PHVMs
        {
            get { return pHVMs; }
            set { if (pHVMs != value) pHVMs = PHVMs; }
        }


        static private ObservableCollection<PressureVM> pPressureVMs = new ObservableCollection<PressureVM>();
        static public ObservableCollection<PressureVM> PressureVMs
        {
            get { return pPressureVMs; }
            set { if (pPressureVMs != value) pPressureVMs = PressureVMs; }
        }

        static private ObservableCollection<PressureComboValues> pressureComboValues = new ObservableCollection<PressureComboValues>();
        static public ObservableCollection<PressureComboValues> PressureComboValuess
        {
            get { return pressureComboValues; }
            set { if (pressureComboValues != value) pressureComboValues = PressureComboValuess; }
        }

        static StaticCollection()
        {
            #region Temperature

            Temperatures = new ObservableCollection<TemperatureVM> { };
            Temperatures.Add(new TemperatureVM {Value="1",Name="Temp" });
            Temperatures.Add(new TemperatureVM { Value = "a]x", Name = "Room Temp to Reflux (a]x)" });
            Temperatures.Add(new TemperatureVM { Value = "a]", Name = "Room temp to" });
            Temperatures.Add(new TemperatureVM { Value = "a", Name = "Room temp(a)" });
            Temperatures.Add(new TemperatureVM { Value = "x", Name = "Reflux /Boiled(x)" });
            Temperatures.Add(new TemperatureVM { Value = "x]a", Name = "Reflux to Room Temp (x]a)" });
            Temperatures.Add(new TemperatureVM { Value = "]a", Name = "to Room Temp" });
            Temperatures.Add(new TemperatureVM { Value = "]", Name = "Directional (])" });
            Temperatures.Add(new TemperatureVM { Value = "-", Name = "Rang(-)" });
            Temperatures.Add(new TemperatureVM { Value = "c", Name = "Cool(c)" });

            Temperatures.Add(new TemperatureVM { Value = "]x", Name = "Reflux to" });
            Temperatures.Add(new TemperatureVM { Value = "<a", Name = "<Room Temp" });
            Temperatures.Add(new TemperatureVM { Value = "+/-", Name = "+/-" });
            Temperatures.Add(new TemperatureVM { Value = "=", Name = "=" });
            Temperatures.Add(new TemperatureVM { Value = "h", Name = "Heated (h or Δ)" });
            Temperatures.Add(new TemperatureVM { Value = "]x", Name = "to Reflux" });
            Temperatures.Add(new TemperatureVM { Value = ">", Name = ">Room Temp" });

            TemperatureCodes = new ObservableCollection<TemperatureCodeVM> { };
            TemperatureCodes.Add(new TemperatureCodeVM { Value = "c", Name = "c" });
            TemperatureCodes.Add(new TemperatureCodeVM { Value = "f", Name = "f" });
            TemperatureCodes.Add(new TemperatureCodeVM { Value = "k", Name = "k" });
            TemperatureCodes.Add(new TemperatureCodeVM { Value = "r", Name = "r" });

            #endregion 

            #region Time


            TimeCombovalues = new ObservableCollection<TimeCombovalues> { };
            TimeCombovalues.Add(new TimeCombovalues { Value = "", Name = "Select" });
            TimeCombovalues.Add(new TimeCombovalues { Value = ">1m", Name = "few minutes"});
            TimeCombovalues.Add(new TimeCombovalues { Value = ">1", Name = "few hours" });
            TimeCombovalues.Add(new TimeCombovalues { Value = ">1d", Name = "few days" });
            TimeCombovalues.Add(new TimeCombovalues { Value = ">1m", Name = "couple of minutes" });
            TimeCombovalues.Add(new TimeCombovalues { Value = ">1", Name = "couple of hours" });
            TimeCombovalues.Add(new TimeCombovalues { Value = ">1d", Name = "couple of days" });
            TimeCombovalues.Add(new TimeCombovalues { Value = ">1d", Name = "over the weekend" });

            TimeVMs = new ObservableCollection<TimeVM> { };
            TimeVMs.Add(new TimeVM { Value = "1", Name = "Time" });
            TimeVMs.Add(new TimeVM { Value = "In", Name = "InExact Time" });
            TimeVMs.Add(new TimeVM { Value = "(o)", Name = "Over night (o)" });
            TimeVMs.Add(new TimeVM { Value = "-", Name = "Rang (-)" });
            TimeVMs.Add(new TimeVM { Value = "=", Name = "=" });
            TimeVMs.Add(new TimeVM { Value = "None", Name = "None" });
            TimeComboCodevalues = new ObservableCollection<TimeComboCodevalues> { };
            TimeComboCodevalues.Add(new TimeComboCodevalues { Value = "h", Name = "h" });
            TimeComboCodevalues.Add(new TimeComboCodevalues { Value = "m", Name = "m" });
            TimeComboCodevalues.Add(new TimeComboCodevalues { Value = "s", Name = "s" });
            TimeComboCodevalues.Add(new TimeComboCodevalues { Value = "d", Name = "d" });
            TimeComboCodevalues.Add(new TimeComboCodevalues { Value = "w", Name = "w" });
            TimeComboCodevalues.Add(new TimeComboCodevalues { Value = "mo", Name = "mo" });
            TimeComboCodevalues.Add(new TimeComboCodevalues { Value = "ms", Name = "ms" });

            #endregion



            #region pH
            PHVMs = new ObservableCollection<PHVM> { };
            PHVMs.Add(new PHVM { Value = "1", Name = "pH" });
            PHVMs.Add(new PHVM { Value = "a", Name = "Acid (a)" });
            PHVMs.Add(new PHVM { Value = "b", Name = "Base (b)" });
            PHVMs.Add(new PHVM { Value = "n", Name = "Neutral (n)" });
            PHVMs.Add(new PHVM { Value = "-", Name = "Range (-)" });
            PHVMs.Add(new PHVM { Value = "=", Name = "=" });

            #endregion

            #region Pressure

            PressureVMs = new ObservableCollection<PressureVM> { };
            PressureVMs.Add(new PressureVM { Value = "1", Name = "Pressure" });
            PressureVMs.Add(new PressureVM { Value = "]", Name = "Directional (])" });
            PressureVMs.Add(new PressureVM { Value = "-", Name = "Range (-)" });
            PressureVMs.Add(new PressureVM { Value = "=", Name = "=" });


            PressureComboValuess = new ObservableCollection<PressureComboValues> { };
            PressureComboValuess.Add(new PressureComboValues { Value = "a", Name = "a" });
            PressureComboValuess.Add(new PressureComboValues { Value = "b", Name = "b" });
            PressureComboValuess.Add(new PressureComboValues { Value = "kb", Name = "kb" });
            PressureComboValuess.Add(new PressureComboValues { Value = "mb", Name = "mb" });
            PressureComboValuess.Add(new PressureComboValues { Value = "j", Name = "j" });
            PressureComboValuess.Add(new PressureComboValues { Value = "m", Name = "m" });
            PressureComboValuess.Add(new PressureComboValues { Value = "p", Name = "p" });
            PressureComboValuess.Add(new PressureComboValues { Value = "kp", Name = "kp" });
            PressureComboValuess.Add(new PressureComboValues { Value = "mp", Name = "mp" });
            PressureComboValuess.Add(new PressureComboValues { Value = "gp", Name = "gp" });
            PressureComboValuess.Add(new PressureComboValues { Value = "hp", Name = "hp" });
            PressureComboValuess.Add(new PressureComboValues { Value = "s", Name = "s" });
            PressureComboValuess.Add(new PressureComboValues { Value = "t", Name = "t" });
            PressureComboValuess.Add(new PressureComboValues { Value = "kc", Name = "kc" });


            #endregion

            #region  Usermanul Data
            UserManual = new ObservableCollection<UserManualVM> { };
            UserManual.Add(new UserManualVM { Value = @"Amino Acids.pdf", Name = "Amino Acids" });
            UserManual.Add(new UserManualVM { Value = @"Assumed Reactants_External.pdf", Name = "Reactants External" });
            UserManual.Add(new UserManualVM { Value = @"CAS Roles_STN_Quick Reference Card.pdf", Name = "STN Quick Reference Card" });

            UserManual.Add(new UserManualVM { Value = @"CASREACT Coding Hints And Reminders.pdf", Name = "Coding Hints And Reminders" });
            UserManual.Add(new UserManualVM { Value = @"Comparison Studies_External.pdf", Name = "Studies External" });
            UserManual.Add(new UserManualVM { Value = @"External Introduction to Writing Reactions Section.pdf", Name = "Writing Reactions Section" });

            UserManual.Add(new UserManualVM { Value = @"Important Note from Client Feedback.pdf", Name = "Client Feedback" });
            UserManual.Add(new UserManualVM { Value = @"Named Reactions.pdf", Name = "Named Reactions" });
            UserManual.Add(new UserManualVM { Value = @"Point on Temp and Time.pdf", Name = "Temp and Time" });

            UserManual.Add(new UserManualVM { Value = @"Polymer and AMD.pdf", Name = "Polymer and AMD" });
            UserManual.Add(new UserManualVM { Value = @"Protocol For Oligonucleotides.pdf", Name = "Oligonucleotides" });
            UserManual.Add(new UserManualVM { Value = @"Reaction Analysis_Input_Manual.pdf", Name = "Analysis Input Manual" });

            UserManual.Add(new UserManualVM { Value = @"Reaction Analysis_RSN_Manual.pdf", Name = "Analysis RSN Manual" });
            UserManual.Add(new UserManualVM { Value = @"Salt Formation.pdf", Name = "Salt Formation" });




            #endregion
        }
    }
}
