using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Common
{
    public static partial class S
    {
        #region Thermal
        public readonly static string THERMAL_STRING = "thermal";
        public readonly static string SOLVO_THERMAL_STRING = "solvothermal";
        public readonly static string HYDRO_THERMAL_STRING = "hydrothermal";
        public readonly static string MICROWAVE_STRING = "microwave";
        #endregion
        #region Pressure
        public static readonly float KP_HIGHVALUE = 1010f, K_LOWVALUE = 101.3f, S_HIGHVALUE = 147f, S_LOWVALUE = 14.7f, A_B_LOWVALUE = 1f, A_B_HIGHVALUE = 10f;
        public static readonly string EXCEEDS_KP_HIGH = $"exceeds {KP_HIGHVALUE}", BELOW_KP_VALUE = $"below {K_LOWVALUE}", EXCEEDS_S_HIGH = $"exceeds {S_HIGHVALUE}", BELOW_S_VALUE = $"below {S_LOWVALUE}", EXCEEDS_A_B_HIGH = $"exceeds {A_B_HIGHVALUE}", BELOW_A_B_VALUE = $"below {A_B_LOWVALUE}";
        public static readonly string HIGH_PRESSURE_CVT_TERM = "high pressure";
        public static readonly string LOW_PRESSURE_CVT_TERM = "low pressure";
        public static readonly string HIGH_PRESSURE_FREETEXT_TERM = "high pressure conditions used";
        public static readonly string LOW_PRESSURE_FREETEXT_TERM = "low pressure conditions used";
        #endregion
        #region SelfCheck
        public static readonly string NOT_LOADED_YET = "Data Not Loaded Yet . .";
        public static readonly string NOT_FOUND = "Not Found.";
        public static readonly string ACCE_DRAW = "Accelrys Draw"; 
        #endregion
        #region TempUnits
        public readonly static string FH_UNIT = "f";
        public readonly static string K_UNIT = "k";
        public readonly static string RANKINE_UNIT = "r";
        public readonly static string CENTI_UNIT = "c";
        #endregion
        public readonly static string SUBMIT_TEXT = "submit";
        public readonly static string APPROVE_TEXT = "approve";
        public readonly static string REJECT_TEXT = "f";
        public readonly static string JONES_TEXT = "Jones reagent used";
        public static readonly string STAGE_RANGE_ERR_MSG = "Invalid Stage Range. Please Check Stage range in Freetext: ";//"Stage Information values are must in ascending Order"
        public static readonly string STAGE_ASCE_ERR_MSG = "Stage Information values are must in ascending Order";
        public static readonly string STAGE_DUPLICATE_ERR_MSG = "Stage Information values contains Duplicates";
        public static readonly string RSNTitle = "Reaction Search Note";
        public readonly static string CAT_PRE_USED = "catalyst prepared & used";
        public readonly static string CAT_PRE_AND_USED = "catalyst prepared and used";
        public static readonly string NO_EXP_DET = "no experimental detail";
        public static readonly string MIC_IRR = "microwave irradiation";
        public static readonly string THERMAL = "thermal";
        public static readonly string FAILED_RXN = "failed reaction";
        public static readonly string FUMING_NIT_USED = "fuming nitric acid used";
        public static readonly string OLEM_USED = "oleum used";
        public static readonly string FUMING_SULF_USED = "fuming sulfuric acid used";
        public static readonly string BUFFER = "buffer";
        public static readonly string DES_MED = "described medium";
        public static readonly string KARS_CAT_USED = "Karstedt's catalyst used";
        public static readonly string NO_SOL = "no solvent";
        public static readonly string RN = "Raney nickel";
        public static readonly string PARA_FORM_USED = "paraformaldehyde used";
        public static readonly string PARA_FORM = "paraformaldehyde";
        public static readonly string LIN_CAT_USED = "Lindlar's catalyst used";
        public static readonly string NO_EXP_DET_CHRO = "no experimental details-chromatography";
        public static readonly string REA_AS_REA = "reactant used as reactant";
        public static readonly string CAT_AS_SOL = "catalyst used as solvent";
        public static readonly string AGE_AS_SOL = "reagent used as solvent";
        public static readonly string SOL_AS_CAT = "solvent used as catalyst";
        public static readonly string AGE_AS_CAT = "reagent used as catalyst";
        public static readonly string REA_AS_AGE = "reactant used as reagent";
        public static readonly string SOL_AS_AGE = "solvent used as reagent";
        public static readonly string CAT_AS_AGE = "catalyst used as reagent";
        public static readonly string STRERIOSELECTIVE = "stereoselective";
        public static readonly string REGIOSELECTICE = "regioselective";
        public static readonly string PARAFORMALDEHIDE_USED_CVT = "50000(paraformaldehyde) reactant/reagent used";
        public static readonly string ENZYMIC_CVT = "enzymic";
        public static readonly string ENZYMIC_FREETEXT = "enzymatic";
        public static readonly string BIOTRANSFORMATION_CVT = "biotransformation";
        public static readonly string BIOTRANSFORMATION_FREETEXT = "biotransformation process";
        public static readonly string UNSPECIFIEDREAGENT_BASIC_HYDROLYSIS = "unspecified reagent used for basic hydrolysis";
        public static readonly string UNSPECIFIEDREAGENT_BASIDIFICATION = "unspecified reagent used for basidification";
        public static readonly string UNSPECIFIEDREAGENT_ACIDIC_HYDROLYSIS = "unspecified reagent used for acidic hydrolysis";
        public static readonly string UNSPECIFIEDREAGENT_ACIDIFICATION = "unspecified reagent used for acidification";
        public static readonly string SOLID_SUPPORT_CATALYST_CVT = "solid-supported catalyst";
        public static readonly string SOLID_SUPPORT_CATALYST_NAME1 = "pd(oh)2/c";
        public static readonly string SOLID_SUPPORT_CATALYST_NAME2 = "pearlman's catalyst";
        public static readonly string ALTERNATIVE_PREPARATION_SHOWN = "alternative preparation shown";
        public static readonly string DUAL_ROLE_STRING = "dual role";
        public static readonly string PDF_GOOGLE_SEARCH_URL = "https://www.google.co.in/search?source=hp&q=";



        public static readonly string YIELD_REG_EXP = @"^[,0-9\s]+$";
        public static readonly string RXN_MARKUP_REG_EXP = @"rxn [0-9]+-[0-9]+";
        public static readonly string S8000_MARKUP_REG_EXP = @"(\b(8[0-3][0-9]{2}|84[0-8][0-9]|849[0-9])\b)|(\b(7[0-8][0-9]{2}|79[0-8][0-9]|799[0-9])\b)";
        public static readonly string HCL_GAS_USED = "hcl gas used";
        //@"\(stage \d+\)|\(stages \d+(-| and )\d+\)|\(stages (\d,\s)+\d\)|\(stages (\d+,\s)+\d+( and )\d+\)|\(stages \d+-\d+,\s\d+( and )\d+\)|\(stages \d+-\d+( and )\d+-\d+\)"
        public static readonly string STAGEINFO_SINGLE_STAGE = @"\(stage \d+\)";
        public static readonly string STAGEINFO_RANGE = @"\(stages \d+-\d+\)";
        public static readonly string STAGEINFO_AND = @"\(stages \d+ and \d+\)";
        public static readonly string STAGEINFO_RANGE_AND = @"\(stages \d+-\d+ and \d+\)";
        public static readonly string STAGEINFO_MULTIPLE_COMMA = @"\(stages (\d,\s)+\d\)";
        public static readonly string STAGEINFO_MULTIPLE_COMMA_AND = @"\(stages (\d+,\s)+\d+( and )\d+\)";
        public static readonly string STAGEINFO_RANGE_COMMA_AND = @"\(stages \d+-\d+,\s\d+( and )\d+\)";
        public static readonly string STAGEINFO_RANGE_AND_RANGE = @"\(stages \d+-\d+( and )\d+-\d+\)";
        public static readonly string STAGEINFO_RANGE_COMMA_RANGE_AND = @"\(stages \d+-\d+,\s\d+-\d+( and )\d+\)";
        public static readonly string STAGES_REGEXPR_ALLTYPES = $"{STAGEINFO_SINGLE_STAGE}|{STAGEINFO_RANGE}|{STAGEINFO_AND}|{STAGEINFO_MULTIPLE_COMMA}|{STAGEINFO_MULTIPLE_COMMA_AND}|{STAGEINFO_RANGE_COMMA_AND}|{STAGEINFO_RANGE_AND_RANGE}|{STAGEINFO_RANGE_COMMA_RANGE_AND}"; //@"\(stage \d+\)|\(stages \d+(-| and )\d+\)|\(stages (\d,\s)+\d\)|\(stages (\d+,\s)+\d+( and )\d+\)";
        public static readonly List<String> AllowedDuplicateChemicals = new List<string>() {
            "Sodium bicarbonate (Solid)",
            "Sodium carbonate (Solid)",
            "Ammonium hydroxide (Solid)",
            "Hydrochloric acid (Gas)",
            "H2SO4 (Conc.)",
            "Ammonium chloride (Solid)",
            "paraformaldehyde",
            "Sodium Hydroxide (Solid)",
            "Pd(OH)2/C", "Raney Nickel"
        };
        public static readonly string CORBON_REG_EXP = @"(c|C)\d";
        public static readonly List<string> InargonicAcidsList = new List<string> { "7647010", "497198", "144558", "1336216", "298146", "12125029", "7664939" };
        public static List<string> CommentParameters = new List<string>() { "num", "page", "line", "para", "column", "table", "figure", "scheme", "sheet", "footnote", "comment" };
        public static List<string> FreeTextListToRestrict = new List<string>() { "â€™", "â€“", "fï¬n", "[Î¼3", "Î³ : Î²", "&gt;", "Î³", " Î²", "Îµ", "Î´", "Î¶", "Î·", "Â®", "Â", "Ã", "Â°C", "â„¢", "Î»",
            "Â·", "â‰¥", " Î±,Î±â€²", "Î±", "Î,Î", "Îâ€™", "Â°", "â€²", "Â° C", "’", "–", "·", "âˆ’", "Î—", "ï¬‚", "Î•/Î•,Î–/Î–", "Î²", " ÎÎÎ'", "â„¢," };
        public static List<TanState> UnDelivarableTanStates = new List<TanState> { TanState.Curation_Assigned, TanState.Curation_Assigned_Rejected, TanState.Curation_InProgress, TanState.Curation_Progress_Rejected, TanState.Curation_ReAssigned, TanState.Not_Assigned };


        public static readonly List<string> WindowsToClose = new List<string>() { "PdfReaderWindow", "SelectChemical", "EditAnalogousWindow", "ConditionsWindow", "RSNWindow", "TanCommentsWindow", "PdfAnnotationsWindow", "PdfSearchWindow" }; 
    }
    public enum Action
    {
        SUBMIT = 0,
        APPROVE = 1,
        REJECT = 2,
        SHOW = 3
    }
}
