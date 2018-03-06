using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class TemparatureVM : ViewModelBase
    {
        private string result, selectTPText, selectRang2,  selectRoomtempto,
            selecttoRoomTemp, selectDirectional1, selectDirectional2, selecttoReflux, selectRang1,
            selectRefluxto, selectPlessMinues1, selectPlessMinues2;
        private int selectedIndex;

        private TemperatureCodeVM selectValue, selectedComboValue;
        private TimeCombovalues timeCombovalues;
        private TimeComboCodevalues timeComboCodevalues;
        #region Temperature
        public string Result
        {
            get { return result; }
            set { SetProperty(ref result, value); }
        }



        #region Temperature 


        public string SelectTempTemperature
        {
            get { return selectTPText; }
            set
            {

                SetProperty(ref selectTPText, value);

            }
        }


        public string SelectRoomtempto
        {
            get { return selectRoomtempto; }
            set
            {

                SetProperty(ref selectRoomtempto, value);

            }
        }

        public string SelecttoRoomTemp
        {
            get { return selecttoRoomTemp; }
            set
            {

                SetProperty(ref selecttoRoomTemp, value);

            }
        }

        public string SelectDirectional1
        {
            get { return selectDirectional1; }
            set
            {

                SetProperty(ref selectDirectional1, value);

            }
        }


        public string SelectDirectional2
        {
            get { return selectDirectional2; }
            set
            {

                SetProperty(ref selectDirectional2, value);

            }
        }


        public string SelectRang1
        {
            get { return selectRang1; }
            set
            {
                SetProperty(ref selectRang1, value);

            }
        }

        public string SelectRang2
        {
            get { return selectRang2; }
            set
            {
                SetProperty(ref selectRang2, value);

            }
        }

        public string SelectRefluxto
        {
            get { return selectRefluxto; }
            set
            {
                SetProperty(ref selectRefluxto, value);

            }
        }

        public string SelectPlessMinues1
        {
            get { return selectPlessMinues1; }
            set
            {
                SetProperty(ref selectPlessMinues1, value);

            }
        }
        public string SelectPlessMinues2
        {
            get { return selectPlessMinues2; }
            set
            {
                SetProperty(ref selectPlessMinues2, value);

            }
        }
        public string SelecttoReflux
        {
            get { return selecttoReflux; }
            set
            {
                SetProperty(ref selecttoReflux, value);

            }
        }





        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                SetProperty(ref selectedIndex, value);

            }
        }
        public int SelectedIndex1
        {
            get { return selectedIndex; }
            set
            {
                SetProperty(ref selectedIndex, value);

            }
        }



        public TemperatureCodeVM SelectedComboValue
        {
            get { SelectedIndex1 = 0; return selectedComboValue; }
            set
            {
                SetProperty(ref selectedComboValue, value);

            }
        }


        public TemperatureCodeVM SelectedComboRoomtempto
        {
            get { return selectValue; }
            set
            {
                SetProperty(ref selectValue, value);

            }
        }

        public TemperatureCodeVM SelectedCombotoRoomTemp
        {
            get { return selectValue; }
            set
            {
                SetProperty(ref selectValue, value);

            }
        }

        public TemperatureCodeVM SelectedComboDirectional
        {
            get { return selectValue; }
            set
            {
                SetProperty(ref selectValue, value);

            }
        }

        public TemperatureCodeVM SelectedComboRang
        {
            get { return selectValue; }
            set
            {
                SetProperty(ref selectValue, value);

            }
        }

        public TemperatureCodeVM SelectedComboRefluxto
        {
            get { return selectValue; }
            set
            {
                SetProperty(ref selectValue, value);

            }
        }

        public TemperatureCodeVM SelectedComboPluessorMinnus
        {
            get { return selectValue; }
            set
            {
                SetProperty(ref selectValue, value);

            }
        }
        public TemperatureCodeVM SelectedCombotoReflux
        {
            get { return selectValue; }
            set
            {
                SetProperty(ref selectValue, value);

            }
        }








        #endregion
        #endregion
    }
}