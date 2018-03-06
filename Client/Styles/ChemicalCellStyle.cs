using Client.Common;
using Client.Logging;
using Client.ViewModels;
using DTO;
using Entities;
using System.Windows;
using System.Windows.Controls;

namespace Client.Styles
{
    public class YieldCellStyle : StyleSelector
    {
        public Style EditableStyle { get; set; }
        public Style ReadOnlyStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            
            try
            {
                var value = item as ReactionParticipantVM;
                if (value.ParticipantType == ParticipantType.Product)
                    return EditableStyle;
                return ReadOnlyStyle;
            }
            catch (System.Exception ex)
            {
                Log.This(ex);
                return null;
            }
        }
    }
    public class ParticipantCellStyle : StyleSelector
    {
        public Style AllStyle { get; set; }
        public Style ProductStyle { get; set; }
        public Style ReactantStyle { get; set; }
        public Style SolventStyle { get; set; }
        public Style AgentStyle { get; set; }
        public Style CatalystStyle { get; set; }
        public override Style SelectStyle(object item, DependencyObject container)
        {
            
            try
            {
                var value = item as ReactionParticipantVM;
                if (value.ParticipantType == ParticipantType.Product)
                    return ProductStyle;
                else if (value.ParticipantType == ParticipantType.Reactant)
                    return ReactantStyle;
                else if (value.ParticipantType == ParticipantType.Solvent)
                    return SolventStyle;
                else if (value.ParticipantType == ParticipantType.Agent)
                    return AgentStyle;
                else if (value.ParticipantType == ParticipantType.Catalyst)
                    return CatalystStyle;
                return AllStyle;
            }
            catch (System.Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }

    }
}
