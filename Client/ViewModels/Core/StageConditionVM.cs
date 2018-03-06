using Client.ViewModel;
using System;

namespace Client.ViewModels
{
    public class StageConditionVM : OrderableVM
    {

        private Guid id, stageId;
        private int displayOrder;
        private string temperature, pressure, ph, time, temp_type, time_type, ph_type, pressure_type;

        public Guid Id { get { return id; } set { SetProperty(ref id, value); } }
        public String Temperature { get { return temperature; } set { SetProperty(ref temperature, value); } }
       // public TemperatureCodeVM TemperatureUnit { get { return temperatureUnit; } set { SetProperty(ref temperatureUnit, value); } }
        public String Pressure { get { return pressure; } set { SetProperty(ref pressure, value); } }
        //public PressureComboValues PressureUnit { get { return pressureUnit; } set { SetProperty(ref pressureUnit, value); } }
        public String PH { get { return ph; } set { SetProperty(ref ph, value); } }
        public String Time { get { return time; } set { SetProperty(ref time, value); } }
        public String TEMP_TYPE { get { return temp_type; } set { SetProperty(ref temp_type, value); } }
        public String TIME_TYPE { get { return time_type; } set { SetProperty(ref time_type, value); } }
        public String PH_TYPE { get { return ph_type; } set { SetProperty(ref ph_type, value); } }
        public String PRESSURE_TYPE { get { return pressure_type; } set { SetProperty(ref pressure_type, value); } }
        public Guid StageId { get { return stageId; } set { SetProperty(ref stageId, value); } }
        public override int DisplayOrder
        {
            get { return displayOrder; }
            set
            {
                SetProperty(ref displayOrder, value);
            }
        }

    }
}
